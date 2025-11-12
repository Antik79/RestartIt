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
        private bool _enableTaskbarNotifications = true;
        private bool _enableFileLogging = true;
        private string _logFilePath;
        private LogLevel _minimumLogLevel = LogLevel.Info;
        private int _maxLogFileSizeMB = 10;
        private int _keepLogFilesForDays = 30;
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
        /// Gets or sets a value indicating whether taskbar notifications are enabled for this program.
        /// </summary>
        public bool EnableTaskbarNotifications
        {
            get => _enableTaskbarNotifications;
            set { _enableTaskbarNotifications = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether file logging is enabled for this program.
        /// If false, uses global logging settings.
        /// </summary>
        public bool EnableFileLogging
        {
            get => _enableFileLogging;
            set { _enableFileLogging = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the log file directory path for this program.
        /// If empty or null, uses the global log directory.
        /// </summary>
        public string LogFilePath
        {
            get => _logFilePath;
            set { _logFilePath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the minimum log level for this program's logging.
        /// </summary>
        public LogLevel MinimumLogLevel
        {
            get => _minimumLogLevel;
            set { _minimumLogLevel = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the maximum log file size in MB for this program.
        /// </summary>
        public int MaxLogFileSizeMB
        {
            get => _maxLogFileSizeMB;
            set { _maxLogFileSizeMB = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the number of days to keep log files for this program.
        /// </summary>
        public int KeepLogFilesForDays
        {
            get => _keepLogFilesForDays;
            set { _keepLogFilesForDays = value; OnPropertyChanged(); }
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
        /// <summary>Gets or sets whether to send email notifications when a program stops/crashes.</summary>
        public bool NotifyOnStop { get; set; } = true;
        /// <summary>Gets or sets whether taskbar notifications are enabled.</summary>
        public bool EnableTaskbarNotifications { get; set; } = true;
        /// <summary>Gets or sets whether to show taskbar notifications on successful restart.</summary>
        public bool NotifyOnRestartTaskbar { get; set; } = true;
        /// <summary>Gets or sets whether to show taskbar notifications on restart failure.</summary>
        public bool NotifyOnFailureTaskbar { get; set; } = true;
        /// <summary>Gets or sets whether to show taskbar notifications when a program stops/crashes.</summary>
        public bool NotifyOnStopTaskbar { get; set; } = true;
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
        /// <summary>Gets or sets whether to minimize to system tray when closing the window instead of exiting.</summary>
        public bool MinimizeOnClose { get; set; } = false;
        /// <summary>Gets or sets the language code (e.g., "en", "de", "fr").</summary>
        public string Language { get; set; } = "en";

        // Appearance/Theming Settings
        /// <summary>Gets or sets the font family name (e.g., "Segoe UI", "Arial").</summary>
        public string FontFamily { get; set; } = "Segoe UI";
        /// <summary>Gets or sets the base font size in points.</summary>
        public double FontSize { get; set; } = 12.0;
        /// <summary>Gets or sets the background color as a hex string (e.g., "#F5F5F5").</summary>
        public string BackgroundColor { get; set; } = "#F5F5F5";
        /// <summary>Gets or sets the primary text color as a hex string (e.g., "#212121").</summary>
        public string TextColor { get; set; } = "#212121";
        /// <summary>Gets or sets the highlight/accent color as a hex string (e.g., "#0078D4").</summary>
        public string HighlightColor { get; set; } = "#0078D4";
        /// <summary>Gets or sets the border color as a hex string (e.g., "#E0E0E0").</summary>
        public string BorderColor { get; set; } = "#E0E0E0";
        /// <summary>Gets or sets the surface/card background color as a hex string (e.g., "#FFFFFF").</summary>
        public string SurfaceColor { get; set; } = "#FFFFFF";
        /// <summary>Gets or sets the secondary text color as a hex string (e.g., "#757575").</summary>
        public string SecondaryTextColor { get; set; } = "#757575";
        /// <summary>Gets or sets the button text color as a hex string (e.g., "#FFFFFF" or "#000000"). If not set, will be calculated automatically based on highlight brightness.</summary>
        public string ButtonTextColor { get; set; }
        /// <summary>Gets or sets the header/toolbar background color as a hex string (e.g., "#FFFFFF"). If not set, defaults to SurfaceColor.</summary>
        public string HeaderColor { get; set; }
        /// <summary>Gets or sets the theme preset name (e.g., "Custom", "Latte", "Frappe", "Macchiato", "Mocha").</summary>
        public string ThemePreset { get; set; } = "Custom";
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
    /// Represents a theme definition loaded from a JSON file.
    /// Contains all color and font settings for a theme.
    /// </summary>
    public class ThemeDefinition
    {
        /// <summary>Gets or sets the internal theme name (used for file matching).</summary>
        public string Name { get; set; }
        
        /// <summary>Gets or sets the display name shown to users.</summary>
        public string DisplayName { get; set; }
        
        /// <summary>Gets or sets the theme description.</summary>
        public string Description { get; set; }
        
        /// <summary>Gets or sets the theme author.</summary>
        public string Author { get; set; }
        
        /// <summary>Gets or sets the theme version.</summary>
        public string Version { get; set; }
        
        /// <summary>Gets or sets the theme color palette.</summary>
        public ThemeColors Colors { get; set; }
        
        /// <summary>Gets or sets the default font family for this theme.</summary>
        public string FontFamily { get; set; }
        
        /// <summary>Gets or sets the default font size for this theme.</summary>
        public double FontSize { get; set; }
        
        /// <summary>
        /// Validates the theme definition.
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(DisplayName))
                return false;
            
            if (Colors == null)
                return false;
            
            // Validate all required colors are present
            return !string.IsNullOrWhiteSpace(Colors.BackgroundColor) &&
                   !string.IsNullOrWhiteSpace(Colors.TextColor) &&
                   !string.IsNullOrWhiteSpace(Colors.HighlightColor) &&
                   !string.IsNullOrWhiteSpace(Colors.BorderColor) &&
                   !string.IsNullOrWhiteSpace(Colors.SurfaceColor) &&
                   !string.IsNullOrWhiteSpace(Colors.SecondaryTextColor);
        }
        
        /// <summary>
        /// Applies this theme to AppSettings.
        /// </summary>
        public void ApplyToAppSettings(AppSettings settings)
        {
            if (settings == null || Colors == null)
                return;
            
            settings.BackgroundColor = Colors.BackgroundColor;
            settings.TextColor = Colors.TextColor;
            settings.HighlightColor = Colors.HighlightColor;
            settings.BorderColor = Colors.BorderColor;
            settings.SurfaceColor = Colors.SurfaceColor;
            settings.SecondaryTextColor = Colors.SecondaryTextColor;
            settings.ButtonTextColor = Colors.ButtonTextColor;
            settings.HeaderColor = Colors.HeaderColor;
            
            if (!string.IsNullOrWhiteSpace(FontFamily))
                settings.FontFamily = FontFamily;
            
            if (FontSize > 0)
                settings.FontSize = FontSize;
        }
    }
    
    /// <summary>
    /// Represents the color palette for a theme.
    /// </summary>
    public class ThemeColors
    {
        /// <summary>Gets or sets the background color as a hex string.</summary>
        [System.Text.Json.Serialization.JsonPropertyName("BackgroundColor")]
        public string BackgroundColor { get; set; }
        
        /// <summary>Gets or sets the text color as a hex string.</summary>
        [System.Text.Json.Serialization.JsonPropertyName("TextColor")]
        public string TextColor { get; set; }
        
        /// <summary>Gets or sets the highlight/accent color as a hex string.</summary>
        [System.Text.Json.Serialization.JsonPropertyName("HighlightColor")]
        public string HighlightColor { get; set; }
        
        /// <summary>Gets or sets the border color as a hex string.</summary>
        [System.Text.Json.Serialization.JsonPropertyName("BorderColor")]
        public string BorderColor { get; set; }
        
        /// <summary>Gets or sets the surface/card background color as a hex string.</summary>
        [System.Text.Json.Serialization.JsonPropertyName("SurfaceColor")]
        public string SurfaceColor { get; set; }
        
        /// <summary>Gets or sets the secondary text color as a hex string.</summary>
        [System.Text.Json.Serialization.JsonPropertyName("SecondaryTextColor")]
        public string SecondaryTextColor { get; set; }
        
        /// <summary>Gets or sets the button text color as a hex string.</summary>
        [System.Text.Json.Serialization.JsonPropertyName("ButtonTextColor")]
        public string ButtonTextColor { get; set; }
        
        /// <summary>Gets or sets the header/toolbar background color as a hex string.</summary>
        [System.Text.Json.Serialization.JsonPropertyName("HeaderColor")]
        public string HeaderColor { get; set; }
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

                        // Migrate old Catppuccin theme names to new theme system
                        MigrateThemePreset();

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
        /// Migrates old Catppuccin theme preset names to new theme system.
        /// Latte/Frappe → Light, Macchiato/Mocha → Dark
        /// </summary>
        private void MigrateThemePreset()
        {
            if (AppSettings == null || string.IsNullOrWhiteSpace(AppSettings.ThemePreset))
                return;

            var oldPreset = AppSettings.ThemePreset;
            string newPreset = null;

            // Map old Catppuccin themes to new themes
            if (oldPreset == "Latte" || oldPreset == "Frappe")
            {
                newPreset = "Light";
            }
            else if (oldPreset == "Macchiato" || oldPreset == "Mocha")
            {
                newPreset = "Dark";
            }

            // If migration is needed, update the preset
            if (newPreset != null && newPreset != oldPreset)
            {
                AppSettings.ThemePreset = newPreset;
                System.Diagnostics.Debug.WriteLine($"Migrated theme preset from '{oldPreset}' to '{newPreset}'");
            }
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
                    NotifyOnFailure = config.NotificationSettings.NotifyOnFailure,
                    NotifyOnStop = config.NotificationSettings.NotifyOnStop,
                    EnableTaskbarNotifications = config.NotificationSettings.EnableTaskbarNotifications,
                    NotifyOnRestartTaskbar = config.NotificationSettings.NotifyOnRestartTaskbar,
                    NotifyOnFailureTaskbar = config.NotificationSettings.NotifyOnFailureTaskbar,
                    NotifyOnStopTaskbar = config.NotificationSettings.NotifyOnStopTaskbar
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