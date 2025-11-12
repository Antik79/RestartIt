using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;

namespace RestartIt
{
    /// <summary>
    /// Service for sending email notifications via SMTP.
    /// Handles secure password management using SecureString and DPAPI encryption.
    /// </summary>
    public class EmailNotificationService
    {
        private readonly NotificationSettings _settings;
        private readonly object _emailLock = new object();

        /// <summary>
        /// Initializes a new instance of the EmailNotificationService with the specified settings.
        /// </summary>
        /// <param name="settings">The notification settings containing SMTP configuration</param>
        public EmailNotificationService(NotificationSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Sends an email notification asynchronously if email notifications are enabled.
        /// </summary>
        /// <param name="subject">The email subject line</param>
        /// <param name="body">The email body text</param>
        public void SendNotification(string subject, string body)
        {
            if (!_settings.EnableEmailNotifications)
                return;

            Task.Run(() => SendEmailAsync(subject, body));
        }

        /// <summary>
        /// Sends an email asynchronously using SMTP.
        /// Decrypts password from DPAPI to SecureString for secure handling.
        /// </summary>
        /// <param name="subject">The email subject line</param>
        /// <param name="body">The email body text</param>
        private async Task SendEmailAsync(string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpServer) ||
                string.IsNullOrWhiteSpace(_settings.SenderEmail) ||
                string.IsNullOrWhiteSpace(_settings.RecipientEmail))
            {
                Debug.WriteLine("Email settings incomplete. Skipping notification.");
                return;
            }

            SecureString securePassword = null;
            try
            {
                // Decrypt password to SecureString for secure handling
                if (!string.IsNullOrEmpty(_settings.SenderPassword))
                {
                    // Check if password is encrypted (Base64 format)
                    if (CredentialManager.IsEncrypted(_settings.SenderPassword))
                    {
                        securePassword = CredentialManager.DecryptToSecureString(_settings.SenderPassword);
                    }
                    else
                    {
                        // Legacy plain text password - convert to SecureString
                        securePassword = new SecureString();
                        foreach (char c in _settings.SenderPassword)
                        {
                            securePassword.AppendChar(c);
                        }
                    }
                }
                else
                {
                    securePassword = new SecureString();
                }

                using (var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort))
                {
                    client.EnableSsl = _settings.UseSSL;
                    client.Timeout = 10000; // 10 seconds timeout
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    
                    // Use SecureString with NetworkCredential for secure password handling
                    client.Credentials = new NetworkCredential(_settings.SenderEmail, securePassword);

                    var message = new MailMessage
                    {
                        From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                        Subject = $"[RestartIt] {subject}",
                        Body = $"{body}\n\nTimestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\nSent by RestartIt Application Monitor",
                        IsBodyHtml = false
                    };

                    message.To.Add(_settings.RecipientEmail);

                    await Task.Run(() => client.Send(message));
                    Debug.WriteLine($"Email sent successfully: {subject}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending email notification: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Always dispose SecureString to clear it from memory
                securePassword?.Dispose();
            }
        }
    }

    /// <summary>
    /// Service for logging application events with file-based logging support.
    /// Provides automatic log rotation, cleanup of old logs, and event-based UI updates.
    /// </summary>
    public class LoggerService : IDisposable
    {
        private LogSettings _settings;
        private readonly object _logLock = new object();
        private StreamWriter _logWriter;
        private string _currentLogFile;
        private string _programName; // For per-program logging

        /// <summary>
        /// Event raised when a log message is received. Subscribers can update UI or handle logging.
        /// </summary>
        public event EventHandler<LogEventArgs> LogMessageReceived;

        /// <summary>
        /// Initializes a new instance of the LoggerService with the specified settings.
        /// </summary>
        /// <param name="settings">The log settings containing file path, level, and rotation settings</param>
        /// <param name="programName">Optional program name for per-program logging. If provided, log files will be named {ProgramName}_{yyyy-MM-dd}.log</param>
        public LoggerService(LogSettings settings, string programName = null)
        {
            _settings = settings;
            _programName = programName;
            InitializeLogFile();
        }

        /// <summary>
        /// Updates the logging settings and reinitializes the log file.
        /// </summary>
        /// <param name="settings">The new log settings</param>
        public void UpdateSettings(LogSettings settings)
        {
            _settings = settings;
            CloseLogFile();
            InitializeLogFile();
        }

        /// <summary>
        /// Initializes the log file based on current settings.
        /// Creates the log directory if it doesn't exist and cleans up old log files.
        /// </summary>
        private void InitializeLogFile()
        {
            if (!_settings.EnableFileLogging)
                return;

            try
            {
                Directory.CreateDirectory(_settings.LogFilePath);
                CleanupOldLogFiles();

                string fileName = string.IsNullOrWhiteSpace(_programName) 
                    ? $"RestartIt_{DateTime.Now:yyyy-MM-dd}.log"
                    : $"{_programName}_{DateTime.Now:yyyy-MM-dd}.log";
                _currentLogFile = Path.Combine(_settings.LogFilePath, fileName);

                _logWriter = new StreamWriter(_currentLogFile, append: true)
                {
                    AutoFlush = true
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing log file: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes log files older than the configured retention period.
        /// </summary>
        private void CleanupOldLogFiles()
        {
            try
            {
                var directory = new DirectoryInfo(_settings.LogFilePath);
                if (!directory.Exists)
                    return;

                var cutoffDate = DateTime.Now.AddDays(-_settings.KeepLogFilesForDays);
                string pattern = string.IsNullOrWhiteSpace(_programName)
                    ? "RestartIt_*.log"
                    : $"{_programName}_*.log";
                var oldFiles = directory.GetFiles(pattern)
                    .Where(f => f.CreationTime < cutoffDate);

                foreach (var file in oldFiles)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                        // Ignore deletion errors
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cleaning up old log files: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if log rotation is needed (date changed or file size exceeded).
        /// Rotates to a new log file if necessary.
        /// </summary>
        private void CheckLogRotation()
        {
            if (_logWriter == null || _currentLogFile == null)
                return;

            try
            {
                var fileInfo = new FileInfo(_currentLogFile);
                string expectedFileName = string.IsNullOrWhiteSpace(_programName)
                    ? $"RestartIt_{DateTime.Now:yyyy-MM-dd}.log"
                    : $"{_programName}_{DateTime.Now:yyyy-MM-dd}.log";
                string expectedPath = Path.Combine(_settings.LogFilePath, expectedFileName);

                // Rotate if date changed or file too large
                if (_currentLogFile != expectedPath ||
                    fileInfo.Length > _settings.MaxLogFileSizeMB * 1024 * 1024)
                {
                    CloseLogFile();
                    try
                    {
                        InitializeLogFile();
                    }
                    catch (Exception initEx)
                    {
                        Debug.WriteLine($"Error reinitializing log file after rotation: {initEx.Message}");
                        // Ensure writer is null if initialization fails
                        _logWriter = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking log rotation: {ex.Message}");
                // Ensure log writer is properly closed on error
                try
                {
                    CloseLogFile();
                }
                catch
                {
                    // Ignore errors during cleanup
                }
            }
        }

        /// <summary>
        /// Logs a message with the specified log level.
        /// Only logs if the level meets the minimum log level threshold.
        /// Raises LogMessageReceived event and writes to file if enabled.
        /// </summary>
        /// <param name="message">The log message</param>
        /// <param name="level">The log level (Debug, Info, Warning, Error)</param>
        public void Log(string message, LogLevel level)
        {
            if (level < _settings.MinimumLogLevel)
                return;

            var logEvent = new LogEventArgs
            {
                Message = message,
                Level = level,
                Timestamp = DateTime.Now
            };

            // Raise UI event
            LogMessageReceived?.Invoke(this, logEvent);

            // Write to file
            if (_settings.EnableFileLogging)
            {
                lock (_logLock)
                {
                    try
                    {
                        CheckLogRotation();

                        if (_logWriter != null)
                        {
                            string logLine = $"[{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                            _logWriter.WriteLine(logLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error writing to log file: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Closes and disposes the current log file writer.
        /// Thread-safe operation.
        /// </summary>
        private void CloseLogFile()
        {
            lock (_logLock)
            {
                _logWriter?.Close();
                _logWriter?.Dispose();
                _logWriter = null;
            }
        }

        /// <summary>
        /// Disposes the logger service and closes any open log files.
        /// </summary>
        public void Dispose()
        {
            CloseLogFile();
        }
    }

    /// <summary>
    /// Service that monitors programs and automatically restarts them if they stop running.
    /// Uses background tasks to monitor each program independently with configurable check intervals.
    /// </summary>
    public class ProcessMonitorService
    {
        private readonly ObservableCollection<MonitoredProgram> _programs;
        private readonly LoggerService _logger;
        private readonly LogSettings _globalLogSettings;
        private readonly Dictionary<MonitoredProgram, CancellationTokenSource> _monitorTasks;
        private readonly Dictionary<MonitoredProgram, LoggerService> _programLoggers;
        private EmailNotificationService _emailService;
        private NotificationSettings _notificationSettings;
        private Forms.NotifyIcon _notifyIcon;
        private bool _isRunning;

        /// <summary>
        /// Event raised when the monitoring service starts or stops.
        /// </summary>
        public event EventHandler StatusChanged;
        
        /// <summary>
        /// Gets a value indicating whether the monitoring service is currently running.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Initializes a new instance of the ProcessMonitorService.
        /// </summary>
        /// <param name="programs">The collection of programs to monitor</param>
        /// <param name="logger">The logger service for recording monitoring events</param>
        /// <param name="notificationSettings">Settings for email notifications</param>
        /// <param name="notifyIcon">Optional NotifyIcon for taskbar notifications</param>
        public ProcessMonitorService(ObservableCollection<MonitoredProgram> programs,
            LoggerService logger, NotificationSettings notificationSettings, Forms.NotifyIcon notifyIcon = null, LogSettings globalLogSettings = null)
        {
            _programs = programs;
            _logger = logger;
            _globalLogSettings = globalLogSettings ?? new LogSettings
            {
                EnableFileLogging = true,
                LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RestartIt", "logs"),
                MinimumLogLevel = LogLevel.Info,
                MaxLogFileSizeMB = 10,
                KeepLogFilesForDays = 30
            };
            _notificationSettings = notificationSettings;
            _emailService = new EmailNotificationService(notificationSettings);
            _notifyIcon = notifyIcon;
            _monitorTasks = new Dictionary<MonitoredProgram, CancellationTokenSource>();
            _programLoggers = new Dictionary<MonitoredProgram, LoggerService>();
        }

        /// <summary>
        /// Updates the notification settings and recreates the email service.
        /// </summary>
        /// <param name="settings">The new notification settings</param>
        public void UpdateNotificationSettings(NotificationSettings settings)
        {
            _notificationSettings = settings;
            _emailService = new EmailNotificationService(settings);
        }

        /// <summary>
        /// Updates the NotifyIcon reference (in case it's recreated).
        /// </summary>
        /// <param name="notifyIcon">The new NotifyIcon instance</param>
        public void UpdateNotifyIcon(Forms.NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
        }

        /// <summary>
        /// Gets or creates a logger for the specified program.
        /// Returns the global logger if per-program logging is disabled or not configured.
        /// </summary>
        private LoggerService GetLoggerForProgram(MonitoredProgram program)
        {
            // If per-program logging is disabled, use global logger
            if (!program.EnableFileLogging)
            {
                return _logger;
            }

            // If logger already exists, return it
            if (_programLoggers.TryGetValue(program, out var existingLogger))
            {
                return existingLogger;
            }

            // Create per-program logger
            var logSettings = new LogSettings
            {
                EnableFileLogging = program.EnableFileLogging,
                LogFilePath = string.IsNullOrWhiteSpace(program.LogFilePath) ? _globalLogSettings.LogFilePath : program.LogFilePath,
                MinimumLogLevel = program.MinimumLogLevel,
                MaxLogFileSizeMB = program.MaxLogFileSizeMB,
                KeepLogFilesForDays = program.KeepLogFilesForDays
            };

            var programLogger = new LoggerService(logSettings, program.ProgramName);
            
            // Forward log events to the global logger so UI receives them
            // Per-program loggers write to their own files, but UI shows all logs
            programLogger.LogMessageReceived += (s, e) =>
            {
                // Forward to global logger for UI display
                _logger?.Log(e.Message, e.Level);
            };

            _programLoggers[program] = programLogger;
            return programLogger;
        }

        /// <summary>
        /// Shows a taskbar notification balloon tip if enabled.
        /// Respects both global and per-program notification settings.
        /// </summary>
        /// <param name="program">The program related to the notification</param>
        /// <param name="title">The notification title</param>
        /// <param name="message">The notification message</param>
        /// <param name="icon">The notification icon type</param>
        private void ShowTaskbarNotification(MonitoredProgram program, string title, string message, Forms.ToolTipIcon icon)
        {
            // Check if taskbar notifications are enabled globally
            if (!_notificationSettings.EnableTaskbarNotifications)
            {
                Debug.WriteLine($"Taskbar notification skipped: Global notifications disabled");
                return;
            }

            // Check if taskbar notifications are enabled for this program
            if (!program.EnableTaskbarNotifications)
            {
                Debug.WriteLine($"Taskbar notification skipped: Program '{program.ProgramName}' notifications disabled");
                return;
            }

            // Show notification on UI thread
            if (_notifyIcon == null)
            {
                Debug.WriteLine($"Taskbar notification skipped: NotifyIcon is null");
                return;
            }

            try
            {
                Debug.WriteLine($"Attempting to show taskbar notification: {title} - {message}");
                var app = System.Windows.Application.Current;
                if (app == null)
                {
                    Debug.WriteLine($"Taskbar notification failed: Application.Current is null");
                    return;
                }

                var dispatcher = app.Dispatcher;
                if (dispatcher == null)
                {
                    Debug.WriteLine($"Taskbar notification failed: Application dispatcher is null");
                    return;
                }

                // Use BeginInvoke for async execution - Windows Forms NotifyIcon works better this way
                dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        // Double-check NotifyIcon is still valid on UI thread
                        if (_notifyIcon == null)
                        {
                            Debug.WriteLine($"Taskbar notification failed: NotifyIcon is null on UI thread");
                            return;
                        }

                        if (!_notifyIcon.Visible)
                        {
                            Debug.WriteLine($"Taskbar notification failed: NotifyIcon is not visible on UI thread");
                            return;
                        }

                        // Show the balloon tip
                        _notifyIcon.ShowBalloonTip(5000, title, message, icon);
                        Debug.WriteLine($"Taskbar notification shown successfully: {title} - {message}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error showing taskbar notification: {ex.Message}");
                        Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error invoking taskbar notification: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Starts the monitoring service.
        /// Subscribes to program collection changes and property changes.
        /// Begins monitoring all enabled programs.
        /// </summary>
        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            StatusChanged?.Invoke(this, EventArgs.Empty);

            // Subscribe to PropertyChanged for all existing programs
            foreach (var program in _programs)
            {
                program.PropertyChanged += Program_PropertyChanged;
                if (program.Enabled)
                {
                    StartMonitoring(program);
                }
            }

            _programs.CollectionChanged += Programs_CollectionChanged;
        }

        /// <summary>
        /// Stops the monitoring service.
        /// Cancels all monitoring tasks and unsubscribes from events.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _programs.CollectionChanged -= Programs_CollectionChanged;

            // Unsubscribe from PropertyChanged for all programs
            foreach (var program in _programs)
            {
                program.PropertyChanged -= Program_PropertyChanged;
            }

            foreach (var cts in _monitorTasks.Values)
            {
                cts.Cancel();
            }
            _monitorTasks.Clear();

            StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Programs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MonitoredProgram program in e.NewItems)
                {
                    program.PropertyChanged += Program_PropertyChanged;
                    if (program.Enabled)
                    {
                        StartMonitoring(program);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (MonitoredProgram program in e.OldItems)
                {
                    program.PropertyChanged -= Program_PropertyChanged;
                    StopMonitoring(program);
                }
            }
        }

        private void Program_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MonitoredProgram.Enabled))
            {
                var program = sender as MonitoredProgram;
                if (program.Enabled)
                {
                    StartMonitoring(program);
                }
                else
                {
                    StopMonitoring(program);
                }
            }
        }

        private void StartMonitoring(MonitoredProgram program)
        {
            if (_monitorTasks.ContainsKey(program))
                return;

            var cts = new CancellationTokenSource();
            _monitorTasks[program] = cts;

            Task.Run(() => MonitorProgramAsync(program, cts.Token), cts.Token);
        }

        private void StopMonitoring(MonitoredProgram program)
        {
            if (_monitorTasks.TryGetValue(program, out var cts))
            {
                cts.Cancel();
                _monitorTasks.Remove(program);
                program.Status = "Disabled";
            }

            // Dispose per-program logger if it exists
            if (_programLoggers.TryGetValue(program, out var logger))
            {
                logger.Dispose();
                _programLoggers.Remove(program);
            }
        }

        private async Task MonitorProgramAsync(MonitoredProgram program, CancellationToken cancellationToken)
        {
            string previousStatus = program.Status;
            bool isFirstCheck = true; // Track if this is the first check after enabling
            
            while (!cancellationToken.IsCancellationRequested && program.Enabled)
            {
                try
                {
                    bool isRunning = IsProcessRunning(program);

                    if (isRunning)
                    {
                        // Only notify if status changed from stopped/failed to running (not from Restarting, as RestartProgram already sent notification)
                        // Don't notify if it was disabled (just enabled) or if it's the first check
                        if (previousStatus != "Running" && previousStatus != "Disabled" && previousStatus != "Restarting" && !isFirstCheck)
                        {
                            program.Status = "Running";
                            
                            // Send notification when program is detected as running again (came back online on its own, not via restart)
                            if (_notificationSettings.NotifyOnRestart)
                            {
                                _emailService.SendNotification(
                                    $"Program Running: {program.ProgramName}",
                                    $"The program '{program.ProgramName}' is now running.\n\n" +
                                    $"Executable: {program.ExecutablePath}\n" +
                                    $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                            }

                            // Show taskbar notification when program is detected as running
                            if (_notificationSettings.NotifyOnRestartTaskbar)
                            {
                                string notificationMessage = string.Format(
                                    LocalizationService.Instance.GetString("Notification.ProgramRunning", "{0} is now running"),
                                    program.ProgramName);
                                ShowTaskbarNotification(program, "RestartIt", notificationMessage, Forms.ToolTipIcon.Info);
                            }
                        }
                        else
                        {
                            program.Status = "Running";
                        }
                        previousStatus = "Running";
                        isFirstCheck = false;
                    }
                    else
                    {
                        // Only notify if status changed from running to stopped
                        // Don't notify on first check if program was disabled (just enabled)
                        if ((previousStatus == "Running" || (previousStatus == null && !isFirstCheck)) && !isFirstCheck)
                        {
                            program.Status = "Stopped";
                            string message = string.Format(
                                LocalizationService.Instance.GetString("Log.ProgramNotRunning", "{0} is not running. Restarting in {1} seconds..."),
                                program.ProgramName,
                                program.RestartDelaySeconds);
                            GetLoggerForProgram(program).Log(message, LogLevel.Warning);

                            // Send notification when program is detected as stopped
                            if (_notificationSettings.NotifyOnStop)
                            {
                                _emailService.SendNotification(
                                    $"Program Stopped: {program.ProgramName}",
                                    $"The program '{program.ProgramName}' has stopped running and will be restarted.\n\n" +
                                    $"Executable: {program.ExecutablePath}\n" +
                                    $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                            }

                            // Show taskbar notification when program is detected as stopped
                            if (_notificationSettings.NotifyOnStopTaskbar)
                            {
                                string notificationMessage = string.Format(
                                    LocalizationService.Instance.GetString("Notification.ProgramStopped", "{0} has stopped"),
                                    program.ProgramName);
                                ShowTaskbarNotification(program, "RestartIt", notificationMessage, Forms.ToolTipIcon.Warning);
                            }
                        }
                        else
                        {
                            program.Status = "Stopped";
                        }

                        isFirstCheck = false;

                        await Task.Delay(program.RestartDelaySeconds * 1000, cancellationToken);

                        program.Status = "Restarting";
                        RestartProgram(program);
                        previousStatus = "Restarting";
                    }

                    await Task.Delay(program.CheckIntervalSeconds * 1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    string message = string.Format(
                        LocalizationService.Instance.GetString("Log.ErrorMonitoring", "Error monitoring {0}: {1}"),
                        program.ProgramName,
                        ex.Message);
                    GetLoggerForProgram(program).Log(message, LogLevel.Error);
                }
            }
        }

        /// <summary>
        /// Checks if the specified program is currently running.
        /// Compares both process name and full executable path for accurate matching.
        /// </summary>
        /// <param name="program">The program to check</param>
        /// <returns>True if the program is running, false otherwise</returns>
        /// <remarks>
        /// This method properly disposes all Process objects to prevent resource leaks.
        /// Handles access denied exceptions gracefully when checking process details.
        /// </remarks>
        private bool IsProcessRunning(MonitoredProgram program)
        {
            Process[] processes = null;
            try
            {
                string processName = Path.GetFileNameWithoutExtension(program.ExecutablePath);
                processes = Process.GetProcessesByName(processName);

                // Check all processes and find matches
                bool isRunning = false;
                foreach (var process in processes)
                {
                    try
                    {
                        if (process.MainModule?.FileName?.Equals(program.ExecutablePath, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            isRunning = true;
                            break; // Found a match, no need to check further
                        }
                    }
                    catch
                    {
                        // Access denied or process exited - continue checking other processes
                        continue;
                    }
                }

                return isRunning;
            }
            catch (Exception ex)
            {
                string message = string.Format(
                    LocalizationService.Instance.GetString("Log.ErrorCheckingProcess", "Error checking if {0} is running: {1}"),
                    program.ProgramName,
                    ex.Message);
                _logger.Log(message, LogLevel.Error);
                return false;
            }
            finally
            {
                // Always dispose all process handles to prevent resource leak
                if (processes != null)
                {
                    foreach (var process in processes)
                    {
                        try
                        {
                            process?.Dispose();
                        }
                        catch
                        {
                            // Ignore disposal errors - process may have already exited
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restarts a program that has stopped running.
        /// Validates paths and arguments before starting, and sends notifications on success/failure.
        /// </summary>
        /// <param name="program">The program to restart</param>
        /// <remarks>
        /// Performs comprehensive validation of executable path, working directory, and arguments.
        /// Updates program status and sends email notifications based on notification settings.
        /// </remarks>
        private void RestartProgram(MonitoredProgram program)
        {
            try
            {
                // Validate executable path before attempting to start
                if (!PathValidator.ValidateExecutablePath(program.ExecutablePath, out string pathError))
                {
                    program.Status = "Failed";
                    string message = string.Format(
                        LocalizationService.Instance.GetString("Log.ValidationFailed", "Cannot restart {0}: {1}"),
                        program.ProgramName,
                        pathError);
                    GetLoggerForProgram(program).Log(message, LogLevel.Error);

                    // Send email notification on validation failure
                    if (_notificationSettings.NotifyOnFailure)
                    {
                        _emailService.SendNotification(
                            $"Restart Failed: {program.ProgramName}",
                            $"Failed to restart the program '{program.ProgramName}' due to validation error.\n\n" +
                            $"Executable: {program.ExecutablePath}\n" +
                            $"Error: {pathError}\n" +
                            $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    }

                    // Show taskbar notification on failure
                    if (_notificationSettings.NotifyOnFailureTaskbar)
                    {
                        string notificationMessage = string.Format(
                            LocalizationService.Instance.GetString("Notification.RestartFailure", "Failed to restart {0}"),
                            program.ProgramName);
                        ShowTaskbarNotification(program, "RestartIt", notificationMessage, Forms.ToolTipIcon.Error);
                    }
                    return;
                }

                // Validate working directory if provided
                if (!string.IsNullOrWhiteSpace(program.WorkingDirectory))
                {
                    if (!PathValidator.ValidateWorkingDirectory(program.WorkingDirectory, out string dirError))
                    {
                        program.Status = "Failed";
                        string message = string.Format(
                            LocalizationService.Instance.GetString("Log.WorkingDirectoryInvalid", "Cannot restart {0}: Working directory invalid - {1}"),
                            program.ProgramName,
                            dirError);
                        GetLoggerForProgram(program).Log(message, LogLevel.Error);

                        if (_notificationSettings.NotifyOnFailure)
                        {
                            _emailService.SendNotification(
                                $"Restart Failed: {program.ProgramName}",
                                $"Failed to restart the program '{program.ProgramName}' due to invalid working directory.\n\n" +
                                $"Executable: {program.ExecutablePath}\n" +
                                $"Working Directory: {program.WorkingDirectory}\n" +
                                $"Error: {dirError}\n" +
                                $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        }

                        // Show taskbar notification on failure
                        if (_notificationSettings.NotifyOnFailureTaskbar)
                        {
                            string notificationMessage = string.Format(
                                LocalizationService.Instance.GetString("Notification.RestartFailure", "Failed to restart {0}"),
                                program.ProgramName);
                            ShowTaskbarNotification(program, "RestartIt", notificationMessage, Forms.ToolTipIcon.Error);
                        }
                        return;
                    }
                }

                // Validate and sanitize arguments
                if (!PathValidator.ValidateAndSanitizeArguments(program.Arguments, out string sanitizedArguments, out string argsError))
                {
                    program.Status = "Failed";
                    string message = string.Format(
                        LocalizationService.Instance.GetString("Log.ArgumentsInvalid", "Cannot restart {0}: Arguments invalid - {1}"),
                        program.ProgramName,
                        argsError);
                    GetLoggerForProgram(program).Log(message, LogLevel.Error);

                    if (_notificationSettings.NotifyOnFailure)
                    {
                        _emailService.SendNotification(
                            $"Restart Failed: {program.ProgramName}",
                            $"Failed to restart the program '{program.ProgramName}' due to invalid arguments.\n\n" +
                            $"Executable: {program.ExecutablePath}\n" +
                            $"Error: {argsError}\n" +
                            $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    }

                    // Show taskbar notification on failure
                    if (_notificationSettings.NotifyOnFailureTaskbar)
                    {
                        string notificationMessage = string.Format(
                            LocalizationService.Instance.GetString("Notification.RestartFailure", "Failed to restart {0}"),
                            program.ProgramName);
                        ShowTaskbarNotification(program, "RestartIt", notificationMessage, Forms.ToolTipIcon.Error);
                    }
                    return;
                }

                // Determine working directory
                string workingDir = string.IsNullOrEmpty(program.WorkingDirectory)
                    ? Path.GetDirectoryName(program.ExecutablePath)
                    : program.WorkingDirectory;

                var startInfo = new ProcessStartInfo
                {
                    FileName = program.ExecutablePath,
                    Arguments = sanitizedArguments,
                    WorkingDirectory = workingDir,
                    UseShellExecute = false
                };

                Process.Start(startInfo);
                program.LastRestartTime = DateTime.Now;
                program.Status = "Running";

                string successMessage = string.Format(
                    LocalizationService.Instance.GetString("Log.SuccessfullyRestarted", "Successfully restarted {0}"),
                    program.ProgramName);
                GetLoggerForProgram(program).Log(successMessage, LogLevel.Info);

                // Send email notification on successful restart
                if (_notificationSettings.NotifyOnRestart)
                {
                    _emailService.SendNotification(
                        $"Program Restarted: {program.ProgramName}",
                        $"The program '{program.ProgramName}' has been automatically restarted.\n\n" +
                        $"Executable: {program.ExecutablePath}\n" +
                        $"Restart Time: {program.LastRestartTime:yyyy-MM-dd HH:mm:ss}");
                }

                // Show taskbar notification on successful restart
                if (_notificationSettings.NotifyOnRestartTaskbar)
                {
                    string notificationMessage = string.Format(
                        LocalizationService.Instance.GetString("Notification.RestartSuccess", "{0} restarted successfully"),
                        program.ProgramName);
                    ShowTaskbarNotification(program, "RestartIt", notificationMessage, Forms.ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {
                program.Status = "Failed";
                string message = string.Format(
                    LocalizationService.Instance.GetString("Log.FailedToRestart", "Failed to restart {0}: {1}"),
                    program.ProgramName,
                    ex.Message);
                _logger.Log(message, LogLevel.Error);

                // Send email notification on failure
                if (_notificationSettings.NotifyOnFailure)
                {
                    _emailService.SendNotification(
                        $"Restart Failed: {program.ProgramName}",
                        $"Failed to restart the program '{program.ProgramName}'.\n\n" +
                        $"Executable: {program.ExecutablePath}\n" +
                        $"Error: {ex.Message}\n" +
                        $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                }

                // Show taskbar notification on failure
                if (_notificationSettings.NotifyOnFailureTaskbar)
                {
                    string notificationMessage = string.Format(
                        LocalizationService.Instance.GetString("Notification.RestartFailure", "Failed to restart {0}"),
                        program.ProgramName);
                    ShowTaskbarNotification(program, "RestartIt", notificationMessage, Forms.ToolTipIcon.Error);
                }
            }
        }
    }
}