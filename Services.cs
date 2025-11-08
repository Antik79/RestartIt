using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace RestartIt
{
    // Email Notification Service
    public class EmailNotificationService
    {
        private readonly NotificationSettings _settings;
        private readonly object _emailLock = new object();

        public EmailNotificationService(NotificationSettings settings)
        {
            _settings = settings;
        }

        public void SendNotification(string subject, string body)
        {
            if (!_settings.EnableEmailNotifications)
                return;

            Task.Run(() => SendEmailAsync(subject, body));
        }

        private async Task SendEmailAsync(string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(_settings.SmtpServer) ||
                string.IsNullOrWhiteSpace(_settings.SenderEmail) ||
                string.IsNullOrWhiteSpace(_settings.RecipientEmail))
            {
                Debug.WriteLine("Email settings incomplete. Skipping notification.");
                return;
            }

            try
            {
                using (var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort))
                {
                    client.EnableSsl = _settings.UseSSL;
                    client.Timeout = 10000; // 10 seconds timeout
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword);

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
        }
    }

    // Logger Service with File Logging
    public class LoggerService : IDisposable
    {
        private LogSettings _settings;
        private readonly object _logLock = new object();
        private StreamWriter _logWriter;
        private string _currentLogFile;

        public event EventHandler<LogEventArgs> LogMessageReceived;

        public LoggerService(LogSettings settings)
        {
            _settings = settings;
            InitializeLogFile();
        }

        public void UpdateSettings(LogSettings settings)
        {
            _settings = settings;
            CloseLogFile();
            InitializeLogFile();
        }

        private void InitializeLogFile()
        {
            if (!_settings.EnableFileLogging)
                return;

            try
            {
                Directory.CreateDirectory(_settings.LogFilePath);
                CleanupOldLogFiles();

                string fileName = $"RestartIt_{DateTime.Now:yyyy-MM-dd}.log";
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

        private void CleanupOldLogFiles()
        {
            try
            {
                var directory = new DirectoryInfo(_settings.LogFilePath);
                if (!directory.Exists)
                    return;

                var cutoffDate = DateTime.Now.AddDays(-_settings.KeepLogFilesForDays);
                var oldFiles = directory.GetFiles("RestartIt_*.log")
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

        private void CheckLogRotation()
        {
            if (_logWriter == null || _currentLogFile == null)
                return;

            try
            {
                var fileInfo = new FileInfo(_currentLogFile);
                string expectedFileName = $"RestartIt_{DateTime.Now:yyyy-MM-dd}.log";
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

        private void CloseLogFile()
        {
            lock (_logLock)
            {
                _logWriter?.Close();
                _logWriter?.Dispose();
                _logWriter = null;
            }
        }

        public void Dispose()
        {
            CloseLogFile();
        }
    }

    // Process Monitor Service
    public class ProcessMonitorService
    {
        private readonly ObservableCollection<MonitoredProgram> _programs;
        private readonly LoggerService _logger;
        private readonly Dictionary<MonitoredProgram, CancellationTokenSource> _monitorTasks;
        private EmailNotificationService _emailService;
        private NotificationSettings _notificationSettings;
        private bool _isRunning;

        public event EventHandler StatusChanged;
        public bool IsRunning => _isRunning;

        public ProcessMonitorService(ObservableCollection<MonitoredProgram> programs,
            LoggerService logger, NotificationSettings notificationSettings)
        {
            _programs = programs;
            _logger = logger;
            _notificationSettings = notificationSettings;
            _emailService = new EmailNotificationService(notificationSettings);
            _monitorTasks = new Dictionary<MonitoredProgram, CancellationTokenSource>();
        }

        public void UpdateNotificationSettings(NotificationSettings settings)
        {
            _notificationSettings = settings;
            _emailService = new EmailNotificationService(settings);
        }

        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            StatusChanged?.Invoke(this, EventArgs.Empty);

            foreach (var program in _programs)
            {
                if (program.Enabled)
                {
                    StartMonitoring(program);
                }
            }

            _programs.CollectionChanged += Programs_CollectionChanged;
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _programs.CollectionChanged -= Programs_CollectionChanged;

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
        }

        private async Task MonitorProgramAsync(MonitoredProgram program, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && program.Enabled)
            {
                try
                {
                    bool isRunning = IsProcessRunning(program);

                    if (isRunning)
                    {
                        program.Status = "Running";
                    }
                    else
                    {
                        program.Status = "Stopped";
                        _logger.Log($"{program.ProgramName} is not running. Restarting in {program.RestartDelaySeconds} seconds...", LogLevel.Warning);

                        await Task.Delay(program.RestartDelaySeconds * 1000, cancellationToken);

                        program.Status = "Restarting";
                        RestartProgram(program);
                    }

                    await Task.Delay(program.CheckIntervalSeconds * 1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error monitoring {program.ProgramName}: {ex.Message}", LogLevel.Error);
                }
            }
        }

        private bool IsProcessRunning(MonitoredProgram program)
        {
            try
            {
                string processName = Path.GetFileNameWithoutExtension(program.ExecutablePath);
                var processes = Process.GetProcessesByName(processName);

                try
                {
                    return processes.Any(p =>
                    {
                        try
                        {
                            return p.MainModule?.FileName?.Equals(program.ExecutablePath, StringComparison.OrdinalIgnoreCase) ?? false;
                        }
                        catch
                        {
                            return false;
                        }
                    });
                }
                finally
                {
                    // Properly dispose all process handles to prevent resource leak
                    foreach (var process in processes)
                    {
                        process?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error checking if {program.ProgramName} is running: {ex.Message}", LogLevel.Error);
                return false;
            }
        }

        private void RestartProgram(MonitoredProgram program)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = program.ExecutablePath,
                    Arguments = program.Arguments ?? string.Empty,
                    WorkingDirectory = string.IsNullOrEmpty(program.WorkingDirectory)
                        ? Path.GetDirectoryName(program.ExecutablePath)
                        : program.WorkingDirectory,
                    UseShellExecute = false
                };

                Process.Start(startInfo);
                program.LastRestartTime = DateTime.Now;
                program.Status = "Running";

                _logger.Log($"Successfully restarted {program.ProgramName}", LogLevel.Info);

                // Send email notification on successful restart
                if (_notificationSettings.NotifyOnRestart)
                {
                    _emailService.SendNotification(
                        $"Program Restarted: {program.ProgramName}",
                        $"The program '{program.ProgramName}' has been automatically restarted.\n\n" +
                        $"Executable: {program.ExecutablePath}\n" +
                        $"Restart Time: {program.LastRestartTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
            catch (Exception ex)
            {
                program.Status = "Failed";
                _logger.Log($"Failed to restart {program.ProgramName}: {ex.Message}", LogLevel.Error);

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
            }
        }
    }
}