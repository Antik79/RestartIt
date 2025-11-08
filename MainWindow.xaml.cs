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

        public MainWindow()
        {
            InitializeComponent();

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

            ProgramsDataGrid.ItemsSource = _programs;

            _logger.LogMessageReceived += Logger_LogMessageReceived;
            _monitorService.StatusChanged += MonitorService_StatusChanged;

            InitializeSystemTray();
            LoadConfiguration();
            _monitorService.Start();

            LogMessage("RestartIt started successfully", LogLevel.Info);

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

            var showItem = new Forms.ToolStripMenuItem("Show Window");
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

        private void LoadConfiguration()
        {
            var config = _configManager.LoadConfiguration();
            foreach (var program in config.Programs)
            {
                _programs.Add(program);
            }
        }

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

        private void AddProgram_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                Title = "Select Program to Monitor"
            };

            if (dialog.ShowDialog() == true)
            {
                var editDialog = new ProgramEditDialog(dialog.FileName);
                if (editDialog.ShowDialog() == true)
                {
                    _programs.Add(editDialog.Program);
                    SaveConfiguration();
                    LogMessage($"Added program: {editDialog.Program.ProgramName}", LogLevel.Info);
                }
            }
        }

        private void RemoveProgram_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramsDataGrid.SelectedItem is MonitoredProgram selected)
            {
                var result = System.Windows.MessageBox.Show(
                    $"Are you sure you want to remove '{selected.ProgramName}' from monitoring?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _programs.Remove(selected);
                    SaveConfiguration();
                    LogMessage($"Removed program: {selected.ProgramName}", LogLevel.Info);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select a program to remove.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditProgram_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.Button)?.DataContext is MonitoredProgram program)
            {
                var editDialog = new ProgramEditDialog(program);
                if (editDialog.ShowDialog() == true)
                {
                    SaveConfiguration();
                    LogMessage($"Updated settings for: {program.ProgramName}", LogLevel.Info);
                }
            }
        }

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

                SaveConfiguration();
                LogMessage("Settings updated", LogLevel.Info);
            }
        }

        private void ExportLogs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|Log Files (*.log)|*.log|All Files (*.*)|*.*",
                Title = "Export Logs",
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
                            "Logs exported successfully!",
                            "Export Complete",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        LogMessage($"Logs exported to: {dialog.FileName}", LogLevel.Info);
                    }
                    else
                    {
                        // Export UI log if no file exists
                        File.WriteAllText(dialog.FileName, LogTextBlock.Text);
                        System.Windows.MessageBox.Show(
                            "UI logs exported successfully!",
                            "Export Complete",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"Error exporting logs: {ex.Message}",
                        "Export Error",
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
                string status = program.Enabled ? "enabled" : "disabled";
                LogMessage($"Monitoring {status} for: {program.ProgramName}", LogLevel.Info);
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
                StatusText.Text = isRunning ? "Monitoring Active" : "Monitoring Stopped";
            });
        }

        private void LogMessage(string message, LogLevel level)
        {
            _logger.Log(message, level);
        }

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
            LogMessage("RestartIt shutting down", LogLevel.Info);
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