using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Win32;

namespace RestartIt
{
    // Monitored Program Model
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

        public string ProgramName
        {
            get => _programName;
            set { _programName = value; OnPropertyChanged(); }
        }

        public string ExecutablePath
        {
            get => _executablePath;
            set { _executablePath = value; OnPropertyChanged(); }
        }

        public string Arguments
        {
            get => _arguments;
            set { _arguments = value; OnPropertyChanged(); }
        }

        public string WorkingDirectory
        {
            get => _workingDirectory;
            set { _workingDirectory = value; OnPropertyChanged(); }
        }

        public int CheckIntervalSeconds
        {
            get => _checkIntervalSeconds;
            set { _checkIntervalSeconds = value; OnPropertyChanged(); }
        }

        public int RestartDelaySeconds
        {
            get => _restartDelaySeconds;
            set { _restartDelaySeconds = value; OnPropertyChanged(); }
        }

        public bool Enabled
        {
            get => _enabled;
            set { _enabled = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public DateTime? LastRestartTime
        {
            get => _lastRestartTime;
            set { _lastRestartTime = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Log Settings
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

    // Email Notification Settings
    public class NotificationSettings
    {
        public bool EnableEmailNotifications { get; set; } = false;
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public bool UseSSL { get; set; } = true;
        public string SenderEmail { get; set; } = "";
        public string SenderName { get; set; } = "RestartIt Monitor";
        public string SenderPassword { get; set; } = "";
        public string RecipientEmail { get; set; } = "";
        public bool NotifyOnRestart { get; set; } = true;
        public bool NotifyOnFailure { get; set; } = true;
    }

    // Application Settings
    public class AppSettings
    {
        public bool StartWithWindows { get; set; } = false;
        public bool MinimizeToTray { get; set; } = true;
        public bool StartMinimized { get; set; } = false;
        public string Language { get; set; } = "en";
    }

    // Application Configuration
    public class AppConfiguration
    {
        public List<MonitoredProgram> Programs { get; set; } = new List<MonitoredProgram>();
        public LogSettings LogSettings { get; set; } = new LogSettings();
        public NotificationSettings NotificationSettings { get; set; } = new NotificationSettings();
        public AppSettings AppSettings { get; set; } = new AppSettings();
    }

    // Log Level Enum
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    // Log Event Args
    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // Configuration Manager
    public class ConfigurationManager
    {
        private readonly string _configPath;
        public LogSettings LogSettings { get; set; }
        public NotificationSettings NotificationSettings { get; set; }
        public AppSettings AppSettings { get; set; }

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

                        // Decrypt the email password if it's encrypted
                        if (!string.IsNullOrEmpty(NotificationSettings.SenderPassword))
                        {
                            // Check if password appears to be encrypted (Base64 format)
                            if (CredentialManager.IsEncrypted(NotificationSettings.SenderPassword))
                            {
                                NotificationSettings.SenderPassword = CredentialManager.Decrypt(NotificationSettings.SenderPassword);
                            }
                            // If not encrypted (legacy plain text), it will remain as-is and will be encrypted on next save
                        }

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

        public void SaveConfiguration(AppConfiguration config)
        {
            try
            {
                // Create a copy of the config to avoid modifying the original
                var configToSave = new AppConfiguration
                {
                    Programs = config.Programs,
                    LogSettings = config.LogSettings,
                    AppSettings = config.AppSettings,
                    NotificationSettings = new NotificationSettings
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
                        // Encrypt the password before saving
                        SenderPassword = string.IsNullOrEmpty(config.NotificationSettings.SenderPassword)
                            ? string.Empty
                            : CredentialManager.Encrypt(config.NotificationSettings.SenderPassword)
                    }
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

    // Startup Manager - Handles Windows Startup Registry
    public static class StartupManager
    {
        private const string APP_NAME = "RestartIt";
        private static readonly string STARTUP_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

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