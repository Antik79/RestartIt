using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Win32;

namespace RestartIt
{
    /// <summary>
    /// Represents a program that is being monitored by RestartIt.
    /// Implements INotifyPropertyChanged for data binding support in the UI.
    /// </summary>
    public class MonitoredProgram : INotifyPropertyChanged
    {
        private string _programName;
        private string _executablePath;
        private string _arguments;
        private string _workingDirectory;
        private int _checkIntervalSeconds;
        private int _restartDelaySeconds;
        private bool _enabled;
        private string _status;
        private DateTime? _lastRestartTime;

        /// <summary>
        /// Gets or sets the display name of the program.
        /// </summary>
        public string ProgramName
        {
            get => _programName;
            set { _programName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the full path to the executable file.
        /// </summary>
        public string ExecutablePath
        {
            get => _executablePath;
            set { _executablePath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the command-line arguments to pass to the program.
        /// </summary>
        public string Arguments
        {
            get => _arguments;
            set { _arguments = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the working directory for the program.
        /// If empty, uses the directory containing the executable.
        /// </summary>
        public string WorkingDirectory
        {
            get => _workingDirectory;
            set { _workingDirectory = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the interval in seconds between checks to see if the program is running.
        /// </summary>
        public int CheckIntervalSeconds
        {
            get => _checkIntervalSeconds;
            set { _checkIntervalSeconds = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the delay in seconds before restarting the program after it stops.
        /// </summary>
        public int RestartDelaySeconds
        {
            get => _restartDelaySeconds;
            set { _restartDelaySeconds = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether monitoring is enabled for this program.
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set { _enabled = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the current status of the program (e.g., "Running", "Stopped", "Restarting", "Failed").
        /// </summary>
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the timestamp of the last successful restart.
        /// </summary>
        public DateTime? LastRestartTime
        {
            get => _lastRestartTime;
            set { _lastRestartTime = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. If null, inferred from caller.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Settings for file-based logging functionality.
    /// </summary>
    public class LogSettings
    {
        public string LogFilePath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RestartIt", "logs");

        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Info;
        public bool EnableFileLogging { get; set; } = true;
        public int MaxLogFileSizeMB { get; set; } = 10;
        public int KeepLogFilesForDays { get; set; } = 30;
    }

    /// <summary>
    /// Settings for email notification functionality.
    /// Password is stored encrypted using DPAPI and should never be stored as plain text.
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>Gets or sets whether email notifications are enabled.</summary>
        public bool EnableEmailNotifications { get; set; } = false;
        /// <summary>Gets or sets the SMTP server address (e.g., "smtp.gmail.com").</summary>
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        /// <summary>Gets or sets the SMTP server port (typically 587 for TLS, 465 for SSL).</summary>
        public int SmtpPort { get; set; } = 587;
        /// <summary>Gets or sets whether to use SSL/TLS encryption for SMTP connection.</summary>
        public bool UseSSL { get; set; } = true;
        /// <summary>Gets or sets the sender email address.</summary>
        public string SenderEmail { get; set; } = "";
        /// <summary>Gets or sets the display name for the sender.</summary>
        public string SenderName { get; set; } = "RestartIt Monitor";
        /// <summary>Gets or sets the sender password (encrypted with DPAPI).</summary>
        public string SenderPassword { get; set; } = "";
        /// <summary>Gets or sets the recipient email address for notifications.</summary>
        public string RecipientEmail { get; set; } = "";
        /// <summary>Gets or sets whether to send email notifications on successful restart.</summary>
        public bool NotifyOnRestart { get; set; } = true;
        /// <summary>Gets or sets whether to send email notifications on restart failure.</summary>
        public bool NotifyOnFailure { get; set; } = true;
    }

    /// <summary>
    /// Application-wide settings for behavior and preferences.
    /// </summary>
    public class AppSettings
    {
        /// <summary>Gets or sets whether to start RestartIt automatically when Windows starts.</summary>
        public bool StartWithWindows { get; set; } = false;
        /// <summary>Gets or sets whether to minimize to system tray instead of taskbar.</summary>
        public bool MinimizeToTray { get; set; } = true;
        /// <summary>Gets or sets whether to start the application minimized to tray.</summary>
        public bool StartMinimized { get; set; } = false;
        /// <summary>Gets or sets the language code (e.g., "en", "de", "fr").</summary>
        public string Language { get; set; } = "en";
    }

    /// <summary>
    /// Complete application configuration containing all settings and monitored programs.
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>Gets or sets the list of programs being monitored.</summary>
        public List<MonitoredProgram> Programs { get; set; } = new List<MonitoredProgram>();
        /// <summary>Gets or sets the logging settings.</summary>
        public LogSettings LogSettings { get; set; } = new LogSettings();
        /// <summary>Gets or sets the email notification settings.</summary>
        public NotificationSettings NotificationSettings { get; set; } = new NotificationSettings();
        /// <summary>Gets or sets the application settings.</summary>
        public AppSettings AppSettings { get; set; } = new AppSettings();
    }

    /// <summary>
    /// Logging severity levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>Debug messages for development and troubleshooting.</summary>
        Debug = 0,
        /// <summary>Informational messages about normal operation.</summary>
        Info = 1,
        /// <summary>Warning messages for potential issues.</summary>
        Warning = 2,
        /// <summary>Error messages for failures and exceptions.</summary>
        Error = 3
    }

    /// <summary>
    /// Event arguments for log message events.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>Gets or sets the log message text.</summary>
        public string Message { get; set; }
        /// <summary>Gets or sets the log level.</summary>
        public LogLevel Level { get; set; }
        /// <summary>Gets or sets the timestamp when the log message was created.</summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Manages loading and saving application configuration to/from JSON file.
    /// Handles password encryption/decryption using DPAPI.
    /// Configuration is stored in %APPDATA%\RestartIt\config.json
    /// </summary>
    public class ConfigurationManager
    {
        private readonly string _configPath;
        public LogSettings LogSettings { get; set; }
        public NotificationSettings NotificationSettings { get; set; }
        public AppSettings AppSettings { get; set; }

        /// <summary>
        /// Initializes a new instance of ConfigurationManager.
        /// Creates the configuration directory if it doesn't exist.
        /// </summary>
        public ConfigurationManager()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RestartIt");

            Directory.CreateDirectory(appDataPath);
            _configPath = Path.Combine(appDataPath, "config.json");
            LogSettings = new LogSettings();
            NotificationSettings = new NotificationSettings();
            AppSettings = new AppSettings();
        }

        /// <summary>
        /// Loads the application configuration from the JSON file.
        /// Returns default configuration if file doesn't exist or loading fails.
        /// </summary>
        /// <returns>The loaded configuration, or default configuration if loading fails</returns>
        public AppConfiguration LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    var config = JsonSerializer.Deserialize<AppConfiguration>(json);

                    if (config != null)
                    {
                        LogSettings = config.LogSettings ?? new LogSettings();
                        NotificationSettings = config.NotificationSettings ?? new NotificationSettings();
                        AppSettings = config.AppSettings ?? new AppSettings();

                        // Keep password encrypted in memory - only decrypt to SecureString when actually using it
                        // If password is plain text (legacy), it will be encrypted on next save
                        // EmailNotificationService will handle both encrypted and plain text passwords

                        return config;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");
            }

            return new AppConfiguration
            {
                LogSettings = LogSettings,
                NotificationSettings = NotificationSettings,
                AppSettings = AppSettings
            };
        }

        /// <summary>
        /// Saves the application configuration to the JSON file.
        /// Encrypts the password if it's not already encrypted (handles legacy plain text passwords).
        /// </summary>
        /// <param name="config">The configuration to save</param>
        public void SaveConfiguration(AppConfiguration config)
        {
            try
            {
                // Create a copy of the config to avoid modifying the original
                var notificationSettings = new NotificationSettings
                {
                    EnableEmailNotifications = config.NotificationSettings.EnableEmailNotifications,
                    SmtpServer = config.NotificationSettings.SmtpServer,
                    SmtpPort = config.NotificationSettings.SmtpPort,
                    UseSSL = config.NotificationSettings.UseSSL,
                    SenderEmail = config.NotificationSettings.SenderEmail,
                    SenderName = config.NotificationSettings.SenderName,
                    RecipientEmail = config.NotificationSettings.RecipientEmail,
                    NotifyOnRestart = config.NotificationSettings.NotifyOnRestart,
                    NotifyOnFailure = config.NotificationSettings.NotifyOnFailure
                };

                // Encrypt the password before saving
                // If password is already encrypted, keep it as-is
                // If password is plain text (legacy), encrypt it now
                if (string.IsNullOrEmpty(config.NotificationSettings.SenderPassword))
                {
                    notificationSettings.SenderPassword = string.Empty;
                }
                else if (CredentialManager.IsEncrypted(config.NotificationSettings.SenderPassword))
                {
                    // Already encrypted - keep as-is
                    notificationSettings.SenderPassword = config.NotificationSettings.SenderPassword;
                }
                else
                {
                    // Plain text (legacy) - encrypt it now
                    notificationSettings.SenderPassword = CredentialManager.Encrypt(config.NotificationSettings.SenderPassword);
                }

                var configToSave = new AppConfiguration
                {
                    Programs = config.Programs,
                    LogSettings = config.LogSettings,
                    AppSettings = config.AppSettings,
                    NotificationSettings = notificationSettings
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(configToSave, options);
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Manages Windows startup registry entries to enable/disable automatic startup with Windows.
    /// </summary>
    public static class StartupManager
    {
        private const string APP_NAME = "RestartIt";
        private static readonly string STARTUP_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Sets whether RestartIt should start automatically with Windows.
        /// </summary>
        /// <param name="enable">True to enable startup with Windows, false to disable</param>
        public static void SetStartup(bool enable)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(STARTUP_KEY, true))
                {
                    if (enable)
                    {
                        string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        // For .NET Core/5+, use the actual executable path
                        if (appPath.EndsWith(".dll"))
                        {
                            appPath = appPath.Replace(".dll", ".exe");
                        }
                        key?.SetValue(APP_NAME, $"\"{appPath}\"");
                    }
                    else
                    {
                        key?.DeleteValue(APP_NAME, false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting startup: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if RestartIt is configured to start automatically with Windows.
        /// </summary>
        /// <returns>True if startup is enabled, false otherwise</returns>
        public static bool IsStartupEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(STARTUP_KEY, false))
                {
                    return key?.GetValue(APP_NAME) != null;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}