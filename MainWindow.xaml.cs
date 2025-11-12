using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using Forms = System.Windows.Forms;
using Drawing = System.Drawing;
using System.Text;

namespace RestartIt
{
    /// <summary>
    /// Main window for the RestartIt application.
    /// Manages the UI, system tray integration, and coordinates all services.
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<MonitoredProgram> _programs;
        private ProcessMonitorService _monitorService;
        private LoggerService _logger;
        private ConfigurationManager _configManager;
        private Forms.NotifyIcon _notifyIcon;
        private Drawing.Icon _trayIcon;
        private bool _isClosing = false;
        private StringBuilder _logBuffer = new StringBuilder();
        private const int MAX_LOG_LINES = 1000;

        /// <summary>
        /// Initializes a new instance of the MainWindow.
        /// Loads configuration, initializes services, and sets up the UI.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Set window title with version
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Title = $"RestartIt - Application Monitor v{version.Major}.{version.Minor}.{version.Build}";

            // Set window icon from App.ico
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.ico");
                if (File.Exists(iconPath))
                {
                    this.Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri(iconPath, UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting window icon: {ex.Message}");
            }

            _programs = new ObservableCollection<MonitoredProgram>();
            _configManager = new ConfigurationManager();
            _logger = new LoggerService(_configManager.LogSettings);
            _monitorService = new ProcessMonitorService(_programs, _logger, _configManager.NotificationSettings);

            // Initialize localization
            LoadConfiguration();
            LocalizationService.Instance.LoadLanguage(_configManager.AppSettings.Language);
            LocalizationService.Instance.LanguageChanged += OnLanguageChanged;

            // Apply theme
            ThemeService.Instance.ApplyTheme(_configManager.AppSettings);

            ProgramsDataGrid.ItemsSource = _programs;

            _logger.LogMessageReceived += Logger_LogMessageReceived;
            _monitorService.StatusChanged += MonitorService_StatusChanged;

            InitializeSystemTray();
            UpdateUIText();
            _monitorService.Start();

            LogMessage(LocalizationService.Instance.GetString("Log.Started", "RestartIt started successfully"), LogLevel.Info);

            // Handle start minimized
            if (_configManager.AppSettings.StartMinimized)
            {
                WindowState = WindowState.Minimized;
                if (_configManager.AppSettings.MinimizeToTray)
                {
                    Hide();
                }
            }
        }

        /// <summary>
        /// Initializes the system tray icon and context menu.
        /// Loads the icon from App.ico or generates a fallback icon.
        /// </summary>
        private void InitializeSystemTray()
        {
            // Load tray icon from App.ico
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.ico");
            if (File.Exists(iconPath))
            {
                _trayIcon = new Drawing.Icon(iconPath);
            }
            else
            {
                // Fallback to generated icon if App.ico not found
                _trayIcon = IconHelper.CreateTrayIcon();
            }

            _notifyIcon = new Forms.NotifyIcon
            {
                Icon = _trayIcon,
                Visible = true,
                Text = "RestartIt - Application Monitor"
            };

            _notifyIcon.DoubleClick += (s, e) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            };

            // Create context menu
            var contextMenu = new Forms.ContextMenuStrip();

            var showItem = new Forms.ToolStripMenuItem(LocalizationService.Instance.GetString("Tray.ShowWindow", "Show Window"));
            showItem.Click += (s, e) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            };
            contextMenu.Items.Add(showItem);

            contextMenu.Items.Add(new Forms.ToolStripSeparator());

            var exitItem = new Forms.ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) =>
            {
                _isClosing = true;
                Close();
            };
            contextMenu.Items.Add(exitItem);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && _configManager.AppSettings.MinimizeToTray)
            {
                Hide();
                _notifyIcon.ShowBalloonTip(2000, "RestartIt",
                    "Application minimized to system tray", Forms.ToolTipIcon.Info);
            }
        }

        /// <summary>
        /// Loads the application configuration from disk and populates the programs collection.
        /// </summary>
        private void LoadConfiguration()
        {
            var config = _configManager.LoadConfiguration();
            foreach (var program in config.Programs)
            {
                _programs.Add(program);
            }
        }

        /// <summary>
        /// Saves the current application configuration to disk.
        /// Includes all monitored programs and settings.
        /// </summary>
        private void SaveConfiguration()
        {
            var config = new AppConfiguration
            {
                Programs = _programs.ToList(),
                LogSettings = _configManager.LogSettings,
                NotificationSettings = _configManager.NotificationSettings,
                AppSettings = _configManager.AppSettings
            };
            _configManager.SaveConfiguration(config);
        }

        /// <summary>
        /// Handles the Add Program button click event.
        /// Opens a file dialog to select an executable, then shows the edit dialog.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void AddProgram_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = LocalizationService.Instance.GetString("Dialog.ExecutableFilesFilter", "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*"),
                Title = LocalizationService.Instance.GetString("Dialog.SelectProgram", "Select Program to Monitor")
            };

            if (dialog.ShowDialog() == true)
            {
                var editDialog = new ProgramEditDialog(dialog.FileName);
                if (editDialog.ShowDialog() == true)
                {
                    _programs.Add(editDialog.Program);
                    SaveConfiguration();
                    LogMessage(string.Format(LocalizationService.Instance.GetString("Log.ProgramAdded", "Added program: {0}"), editDialog.Program.ProgramName), LogLevel.Info);
                }
            }
        }

        private void RemoveProgram_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramsDataGrid.SelectedItem is MonitoredProgram selected)
            {
                var result = System.Windows.MessageBox.Show(
                    string.Format(LocalizationService.Instance.GetString("Dialog.RemoveQuestion", "Are you sure you want to remove '{0}' from monitoring?"), selected.ProgramName),
                    LocalizationService.Instance.GetString("Dialog.ConfirmRemoval", "Confirm Removal"),
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _programs.Remove(selected);
                    SaveConfiguration();
                    LogMessage(string.Format(LocalizationService.Instance.GetString("Log.ProgramRemoved", "Removed program: {0}"), selected.ProgramName), LogLevel.Info);
                }
            }
            else
            {
                System.Windows.MessageBox.Show(
                    LocalizationService.Instance.GetString("Dialog.SelectProgramFirst", "Please select a program to remove."),
                    LocalizationService.Instance.GetString("Dialog.NoSelection", "No Selection"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Handles the Edit Program button click event.
        /// Opens the edit dialog for the selected program.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void EditProgram_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.Button)?.DataContext is MonitoredProgram program)
            {
                var editDialog = new ProgramEditDialog(program);
                if (editDialog.ShowDialog() == true)
                {
                    SaveConfiguration();
                    LogMessage(string.Format(LocalizationService.Instance.GetString("Log.ProgramUpdated", "Updated settings for: {0}"), program.ProgramName), LogLevel.Info);
                }
            }
        }

        /// <summary>
        /// Handles the Settings button click event.
        /// Opens the settings dialog and applies changes when closed.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsDialog = new SettingsDialog(
                _configManager.LogSettings,
                _configManager.NotificationSettings,
                _configManager.AppSettings);

            if (settingsDialog.ShowDialog() == true)
            {
                _configManager.LogSettings = settingsDialog.LogSettings;
                _configManager.NotificationSettings = settingsDialog.NotificationSettings;
                _configManager.AppSettings = settingsDialog.AppSettings;

                _logger.UpdateSettings(settingsDialog.LogSettings);
                _monitorService.UpdateNotificationSettings(settingsDialog.NotificationSettings);

                // Update startup registry
                StartupManager.SetStartup(settingsDialog.AppSettings.StartWithWindows);

                // Apply theme changes
                ThemeService.Instance.ApplyTheme(settingsDialog.AppSettings);

                SaveConfiguration();
                LogMessage(LocalizationService.Instance.GetString("Log.SettingsUpdated", "Settings updated"), LogLevel.Info);
            }
        }

        /// <summary>
        /// Handles the Export Logs button click event.
        /// Exports both UI logs and file logs to a text file.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void ExportLogs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = LocalizationService.Instance.GetString("Export.Filter", "Text Files (*.txt)|*.txt|Log Files (*.log)|*.log|All Files (*.*)|*.*"),
                Title = LocalizationService.Instance.GetString("Export.Title", "Export Logs"),
                FileName = $"RestartIt_Export_{DateTime.Now:yyyy-MM-dd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Get current log file content
                    string currentLogPath = Path.Combine(
                        _configManager.LogSettings.LogFilePath,
                        $"RestartIt_{DateTime.Now:yyyy-MM-dd}.log");

                    if (File.Exists(currentLogPath))
                    {
                        File.Copy(currentLogPath, dialog.FileName, true);
                        System.Windows.MessageBox.Show(
                            LocalizationService.Instance.GetString("Export.LogsExported", "Logs exported successfully!"),
                            LocalizationService.Instance.GetString("Export.Success", "Export Complete"),
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        LogMessage(string.Format(LocalizationService.Instance.GetString("Log.LogsExported", "Logs exported to: {0}"), dialog.FileName), LogLevel.Info);
                    }
                    else
                    {
                        // Export UI log if no file exists
                        File.WriteAllText(dialog.FileName, LogTextBlock.Text);
                        System.Windows.MessageBox.Show(
                            LocalizationService.Instance.GetString("Export.UILogsExported", "UI logs exported successfully!"),
                            LocalizationService.Instance.GetString("Export.Success", "Export Complete"),
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        string.Format(LocalizationService.Instance.GetString("Export.ErrorMessage", "Error exporting logs: {0}"), ex.Message),
                        LocalizationService.Instance.GetString("Export.Error", "Export Error"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void EnabledCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            SaveConfiguration();
            if ((sender as System.Windows.Controls.CheckBox)?.DataContext is MonitoredProgram program)
            {
                string logKey = program.Enabled ? "Log.MonitoringEnabled" : "Log.MonitoringDisabled";
                string defaultMsg = program.Enabled ? "Monitoring enabled for: {0}" : "Monitoring disabled for: {0}";
                LogMessage(string.Format(LocalizationService.Instance.GetString(logKey, defaultMsg), program.ProgramName), LogLevel.Info);
            }
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            _logBuffer.Clear();
            LogTextBlock.Text = string.Empty;
        }

        private void Logger_LogMessageReceived(object sender, LogEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                string timestamp = e.Timestamp.ToString("HH:mm:ss");
                string levelIndicator = e.Level switch
                {
                    LogLevel.Error => "❌",
                    LogLevel.Warning => "⚠️",
                    LogLevel.Info => "ℹ️",
                    _ => "•"
                };

                // Append new log entry to buffer
                _logBuffer.AppendLine($"[{timestamp}] {levelIndicator} {e.Message}");

                // Keep only the last MAX_LOG_LINES lines to prevent memory leak
                var lines = _logBuffer.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                if (lines.Length > MAX_LOG_LINES)
                {
                    _logBuffer.Clear();
                    // Keep only the last MAX_LOG_LINES lines
                    var linesToKeep = lines.Skip(lines.Length - MAX_LOG_LINES);
                    foreach (var line in linesToKeep)
                    {
                        _logBuffer.AppendLine(line);
                    }
                }

                // Update UI with bounded log content
                LogTextBlock.Text = _logBuffer.ToString();

                // Auto-scroll to bottom
                if (LogTextBlock.Parent is ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollToEnd();
                }
            });
        }

        private void MonitorService_StatusChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                bool isRunning = _monitorService.IsRunning;
                StatusIndicator.Fill = isRunning ?
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(40, 167, 69)) :
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 53, 69));
                StatusText.Text = isRunning ?
                    LocalizationService.Instance.GetString("MainWindow.MonitoringActive", "Monitoring Active") :
                    LocalizationService.Instance.GetString("MainWindow.MonitoringStopped", "Monitoring Stopped");
            });
        }

        /// <summary>
        /// Handles the LanguageChanged event from LocalizationService.
        /// Updates all UI text elements to reflect the new language.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateUIText();
            });
        }

        /// <summary>
        /// Updates all UI text elements with localized strings.
        /// Called on initialization and when the language changes.
        /// </summary>
        private void UpdateUIText()
        {
            // Update window title
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Title = $"{LocalizationService.Instance.GetString("App.Title", "RestartIt - Application Monitor")} v{version.Major}.{version.Minor}.{version.Build}";

            // Update toolbar buttons
            AddProgramButton.Content = LocalizationService.Instance.GetString("MainWindow.AddProgram", "➕ Add Program");
            RemoveButton.Content = LocalizationService.Instance.GetString("MainWindow.Remove", "Remove");
            ExportLogsButton.Content = LocalizationService.Instance.GetString("MainWindow.ExportLogs", "Export Logs");
            SettingsButton.Content = LocalizationService.Instance.GetString("MainWindow.Settings", "Settings");

            // Update status text
            bool isRunning = _monitorService.IsRunning;
            StatusText.Text = isRunning ?
                LocalizationService.Instance.GetString("MainWindow.MonitoringActive", "Monitoring Active") :
                LocalizationService.Instance.GetString("MainWindow.MonitoringStopped", "Monitoring Stopped");

            // Update DataGrid column headers
            ProgramsDataGrid.Columns[0].Header = LocalizationService.Instance.GetString("DataGrid.Enabled", "Enabled");
            ProgramsDataGrid.Columns[1].Header = LocalizationService.Instance.GetString("DataGrid.ProgramName", "Program Name");
            ProgramsDataGrid.Columns[2].Header = LocalizationService.Instance.GetString("DataGrid.Path", "Path");
            ProgramsDataGrid.Columns[3].Header = LocalizationService.Instance.GetString("DataGrid.CheckInterval", "Check Interval (sec)");
            ProgramsDataGrid.Columns[4].Header = LocalizationService.Instance.GetString("DataGrid.RestartDelay", "Restart Delay (sec)");
            ProgramsDataGrid.Columns[5].Header = LocalizationService.Instance.GetString("DataGrid.Status", "Status");
            ProgramsDataGrid.Columns[6].Header = LocalizationService.Instance.GetString("DataGrid.Actions", "Actions");

            // Update the Edit button text in the Actions column
            if (ProgramsDataGrid.Columns[6] is DataGridTemplateColumn actionsColumn)
            {
                // The button text will be updated via binding when the row is rendered
                ProgramsDataGrid.Items.Refresh();
            }

            // Update log panel
            ActivityLogText.Text = LocalizationService.Instance.GetString("MainWindow.ActivityLog", "Activity Log");
            ClearLogButton.Content = LocalizationService.Instance.GetString("MainWindow.ClearLog", "Clear Log");

            // Update system tray
            UpdateSystemTrayText();
        }

        private void UpdateSystemTrayText()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Text = LocalizationService.Instance.GetString("App.TrayTitle", "RestartIt - Application Monitor");

                if (_notifyIcon.ContextMenuStrip != null && _notifyIcon.ContextMenuStrip.Items.Count > 0)
                {
                    _notifyIcon.ContextMenuStrip.Items[0].Text = LocalizationService.Instance.GetString("Tray.ShowWindow", "Show Window");
                    _notifyIcon.ContextMenuStrip.Items[2].Text = LocalizationService.Instance.GetString("Tray.Exit", "Exit");
                }
            }
        }

        /// <summary>
        /// Logs a message using the logger service.
        /// The message will be displayed in the UI log and written to file if enabled.
        /// </summary>
        /// <param name="message">The log message</param>
        /// <param name="level">The log level</param>
        private void LogMessage(string message, LogLevel level)
        {
            _logger.Log(message, level);
        }

        /// <summary>
        /// Handles the window closing event.
        /// Saves configuration and properly disposes resources.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The cancel event arguments</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_isClosing && _configManager.AppSettings.MinimizeToTray)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
                return;
            }

            _monitorService.Stop();
            SaveConfiguration();
            LogMessage(LocalizationService.Instance.GetString("Log.ShuttingDown", "RestartIt shutting down"), LogLevel.Info);
            _logger.Dispose();

            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }

            if (_trayIcon != null)
            {
                _trayIcon.Dispose();
            }
        }
    }
}