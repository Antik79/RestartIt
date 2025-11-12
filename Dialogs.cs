using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace RestartIt
{
    /// <summary>
    /// Dialog window for adding or editing a monitored program.
    /// Provides validation for executable paths, working directories, and arguments.
    /// </summary>
    public class ProgramEditDialog : Window
    {
        /// <summary>
        /// Gets the MonitoredProgram object with the user's input.
        /// Only valid if DialogResult is true.
        /// </summary>
        public MonitoredProgram Program { get; private set; }
        private bool _isNewProgram;

        private TextBox _nameTextBox;
        private TextBox _pathTextBox;
        private Button _browseExecutableButton;
        private TextBox _argsTextBox;
        private TextBox _workingDirTextBox;
        private Button _browseWorkingDirButton;
        private TextBox _checkIntervalTextBox;
        private TextBox _restartDelayTextBox;
        private CheckBox _enableTaskbarNotificationsCheckBox;
        private CheckBox _enableFileLoggingCheckBox;
        private TextBox _logFilePathTextBox;
        private Button _browseLogPathButton;
        private ComboBox _minimumLogLevelComboBox;
        private TextBox _maxLogFileSizeTextBox;
        private TextBox _keepLogFilesDaysTextBox;

        /// <summary>
        /// Initializes a new instance for adding a new program.
        /// </summary>
        /// <param name="executablePath">The path to the executable file selected by the user</param>
        public ProgramEditDialog(string executablePath)
        {
            _isNewProgram = true;
            Program = new MonitoredProgram
            {
                ExecutablePath = executablePath,
                ProgramName = Path.GetFileNameWithoutExtension(executablePath),
                CheckIntervalSeconds = 30,
                RestartDelaySeconds = 5,
                Enabled = true,
                EnableTaskbarNotifications = true,
                Status = "Disabled",
                WorkingDirectory = Path.GetDirectoryName(executablePath)
            };

            InitializeDialog();
        }

        /// <summary>
        /// Initializes a new instance for editing an existing program.
        /// </summary>
        /// <param name="program">The existing MonitoredProgram to edit</param>
        public ProgramEditDialog(MonitoredProgram program)
        {
            _isNewProgram = false;
            Program = program;
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            Title = _isNewProgram ? 
                LocalizationService.Instance.GetString("ProgramEdit.AddTitle", "Add Program") : 
                LocalizationService.Instance.GetString("ProgramEdit.Title", "Edit Program");
            Width = 500;
            Height = 600;
            MinHeight = 400;
            MaxHeight = 800;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.CanResize;

            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Program Name
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.ProgramName", "Program Name:"), 0);
            _nameTextBox = AddTextBox(grid, Program.ProgramName, 0);

            // Executable Path
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.ExecutablePath", "Executable Path:"), 1);
            var executablePathPanel = new DockPanel { Margin = new Thickness(0, 35, 0, 0) };
            Grid.SetRow(executablePathPanel, 1);
            
            _browseExecutableButton = new Button
            {
                Content = LocalizationService.Instance.GetString("ProgramEdit.Browse", "Browse..."),
                Width = 80,
                Margin = new Thickness(5, 0, 0, 0)
            };
            _browseExecutableButton.Click += BrowseExecutable_Click;
            DockPanel.SetDock(_browseExecutableButton, Dock.Right);
            
            _pathTextBox = new TextBox
            {
                Text = Program.ExecutablePath ?? string.Empty,
                IsReadOnly = _isNewProgram,
                Padding = new Thickness(5)
            };
            
            executablePathPanel.Children.Add(_browseExecutableButton);
            executablePathPanel.Children.Add(_pathTextBox);
            grid.Children.Add(executablePathPanel);

            // Arguments
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.ArgumentsOptional", "Arguments (optional):"), 2);
            _argsTextBox = AddTextBox(grid, Program.Arguments, 2);

            // Working Directory
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.WorkingDirectoryOptional", "Working Directory (optional):"), 3);
            var workingDirPanel = new DockPanel { Margin = new Thickness(0, 35, 0, 0) };
            Grid.SetRow(workingDirPanel, 3);
            
            _browseWorkingDirButton = new Button
            {
                Content = LocalizationService.Instance.GetString("ProgramEdit.Browse", "Browse..."),
                Width = 80,
                Margin = new Thickness(5, 0, 0, 0)
            };
            _browseWorkingDirButton.Click += BrowseWorkingDirectory_Click;
            DockPanel.SetDock(_browseWorkingDirButton, Dock.Right);
            
            _workingDirTextBox = new TextBox
            {
                Text = Program.WorkingDirectory ?? string.Empty,
                Padding = new Thickness(5)
            };
            
            workingDirPanel.Children.Add(_browseWorkingDirButton);
            workingDirPanel.Children.Add(_workingDirTextBox);
            grid.Children.Add(workingDirPanel);

            // Check Interval
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.CheckInterval", "Check Interval (seconds):"), 4);
            _checkIntervalTextBox = AddTextBox(grid, Program.CheckIntervalSeconds.ToString(), 4);

            // Restart Delay
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.RestartDelay", "Restart Delay (seconds):"), 5);
            _restartDelayTextBox = AddTextBox(grid, Program.RestartDelaySeconds.ToString(), 5);

            // Enable Taskbar Notifications
            _enableTaskbarNotificationsCheckBox = new CheckBox
            {
                Content = LocalizationService.Instance.GetString("ProgramEdit.EnableTaskbarNotifications", "Enable Taskbar Notifications"),
                IsChecked = Program.EnableTaskbarNotifications,
                Margin = new Thickness(0, 20, 0, 15)
            };
            Grid.SetRow(_enableTaskbarNotificationsCheckBox, 6);
            grid.Children.Add(_enableTaskbarNotificationsCheckBox);

            // Logging Section
            var loggingHeader = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("ProgramEdit.Logging", "Logging"),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(0, 20, 0, 10)
            };
            Grid.SetRow(loggingHeader, 7);
            grid.Children.Add(loggingHeader);

            // Enable File Logging
            _enableFileLoggingCheckBox = new CheckBox
            {
                Content = LocalizationService.Instance.GetString("ProgramEdit.EnableFileLogging", "Enable File Logging"),
                IsChecked = Program.EnableFileLogging,
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(_enableFileLoggingCheckBox, 8);
            grid.Children.Add(_enableFileLoggingCheckBox);

            // Log File Path
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.LogFilePath", "Log File Directory (optional):"), 9);
            var logPathPanel = new DockPanel { Margin = new Thickness(0, 35, 0, 0) };
            Grid.SetRow(logPathPanel, 9);
            
            _browseLogPathButton = new Button
            {
                Content = LocalizationService.Instance.GetString("ProgramEdit.Browse", "Browse..."),
                Width = 80,
                Margin = new Thickness(5, 0, 0, 0)
            };
            _browseLogPathButton.Click += BrowseLogPath_Click;
            DockPanel.SetDock(_browseLogPathButton, Dock.Right);
            
            _logFilePathTextBox = new TextBox
            {
                Text = Program.LogFilePath ?? string.Empty,
                Padding = new Thickness(5)
            };
            
            logPathPanel.Children.Add(_browseLogPathButton);
            logPathPanel.Children.Add(_logFilePathTextBox);
            grid.Children.Add(logPathPanel);

            // Minimum Log Level
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.MinimumLogLevel", "Minimum Log Level:"), 10);
            _minimumLogLevelComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            _minimumLogLevelComboBox.Items.Add(LogLevel.Debug);
            _minimumLogLevelComboBox.Items.Add(LogLevel.Info);
            _minimumLogLevelComboBox.Items.Add(LogLevel.Warning);
            _minimumLogLevelComboBox.Items.Add(LogLevel.Error);
            _minimumLogLevelComboBox.SelectedItem = Program.MinimumLogLevel;
            Grid.SetRow(_minimumLogLevelComboBox, 10);
            grid.Children.Add(_minimumLogLevelComboBox);

            // Max Log File Size
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.MaxLogFileSize", "Max Log File Size (MB):"), 11);
            _maxLogFileSizeTextBox = new TextBox
            {
                Text = Program.MaxLogFileSizeMB.ToString(),
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_maxLogFileSizeTextBox, 11);
            grid.Children.Add(_maxLogFileSizeTextBox);

            // Keep Log Files For Days
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.KeepLogFilesForDays", "Keep Log Files For (days):"), 12);
            _keepLogFilesDaysTextBox = new TextBox
            {
                Text = Program.KeepLogFilesForDays.ToString(),
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_keepLogFilesDaysTextBox, 12);
            grid.Children.Add(_keepLogFilesDaysTextBox);

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };
            Grid.SetRow(buttonPanel, 13);

            var okButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Dialog.OK", "OK"),
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            okButton.Click += OkButton_Click;

            var cancelButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Dialog.Cancel", "Cancel"),
                Width = 80,
                Height = 30,
                IsCancel = true
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            grid.Children.Add(buttonPanel);

            // Wrap grid in ScrollViewer for scrolling
            var scrollViewer = new ScrollViewer
            {
                Content = grid,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
            
            Content = scrollViewer;
        }

        private void AddLabel(Grid grid, string text, int row)
        {
            var label = new TextBlock
            {
                Text = text,
                Margin = new Thickness(0, row == 0 ? 0 : 15, 0, 5),
                FontWeight = FontWeights.SemiBold
            };
            Grid.SetRow(label, row);
            grid.Children.Add(label);
        }

        private TextBox AddTextBox(Grid grid, string text, int row)
        {
            var textBox = new TextBox
            {
                Text = text ?? string.Empty,
                Margin = new Thickness(0, row == 0 ? 20 : 35, 0, 0),
                Padding = new Thickness(5)
            };
            Grid.SetRow(textBox, row);
            grid.Children.Add(textBox);
            return textBox;
        }

        /// <summary>
        /// Handles the OK button click event.
        /// Validates all input fields and saves the program configuration if valid.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate program name
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("ProgramEdit.ProgramNameRequired", "Please enter a program name."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate executable path
            string executablePath = _pathTextBox.Text.Trim();
            if (!PathValidator.ValidateExecutablePath(executablePath, out string pathError))
            {
                MessageBox.Show(
                    pathError,
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Normalize the executable path
            string normalizedPath = PathValidator.NormalizePath(executablePath);
            if (normalizedPath == null)
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Validation.InvalidPathFormat", "The path format is invalid."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate working directory
            string workingDirectory = _workingDirTextBox.Text.Trim();
            if (!PathValidator.ValidateWorkingDirectory(workingDirectory, out string dirError))
            {
                MessageBox.Show(
                    dirError,
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Normalize working directory if provided
            string normalizedWorkingDir = null;
            if (!string.IsNullOrWhiteSpace(workingDirectory))
            {
                normalizedWorkingDir = PathValidator.NormalizePath(workingDirectory);
                if (normalizedWorkingDir == null)
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Validation.InvalidPathFormat", "The path format is invalid."),
                        LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Validate and sanitize arguments
            if (!PathValidator.ValidateAndSanitizeArguments(_argsTextBox.Text, out string sanitizedArguments, out string argsError))
            {
                MessageBox.Show(
                    argsError,
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate check interval
            if (!int.TryParse(_checkIntervalTextBox.Text, out int checkInterval) || checkInterval < 1)
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("ProgramEdit.InvalidInterval", "Check interval must be a positive number."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate restart delay
            if (!int.TryParse(_restartDelayTextBox.Text, out int restartDelay) || restartDelay < 0)
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("ProgramEdit.InvalidDelay", "Restart delay must be zero or a positive number."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // All validation passed - assign values
            Program.ProgramName = _nameTextBox.Text.Trim();
            Program.ExecutablePath = normalizedPath;
            Program.Arguments = sanitizedArguments;
            Program.WorkingDirectory = normalizedWorkingDir ?? string.Empty;
            Program.CheckIntervalSeconds = checkInterval;
            Program.RestartDelaySeconds = restartDelay;
            Program.EnableTaskbarNotifications = _enableTaskbarNotificationsCheckBox.IsChecked ?? true;

            // Validate and save logging settings
            if (!int.TryParse(_maxLogFileSizeTextBox.Text, out int maxLogSize) || maxLogSize < 1)
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("ProgramEdit.InvalidMaxLogSize", "Max log file size must be a positive number."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(_keepLogFilesDaysTextBox.Text, out int keepLogsDays) || keepLogsDays < 1)
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("ProgramEdit.InvalidKeepLogsDays", "Keep log files days must be a positive number."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Program.EnableFileLogging = _enableFileLoggingCheckBox.IsChecked ?? true;
            Program.LogFilePath = _logFilePathTextBox.Text.Trim();
            Program.MinimumLogLevel = _minimumLogLevelComboBox.SelectedItem as LogLevel? ?? LogLevel.Info;
            Program.MaxLogFileSizeMB = maxLogSize;
            Program.KeepLogFilesForDays = keepLogsDays;

            DialogResult = true;
            Close();
        }

        private void BrowseWorkingDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Forms.FolderBrowserDialog
            {
                SelectedPath = _workingDirTextBox.Text
            };

            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                _workingDirTextBox.Text = dialog.SelectedPath;
            }
        }

        private void BrowseLogPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Forms.FolderBrowserDialog
            {
                SelectedPath = _logFilePathTextBox.Text
            };

            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                _logFilePathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void BrowseExecutable_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = LocalizationService.Instance.GetString("Dialog.ExecutableFilesFilter", "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*"),
                Title = LocalizationService.Instance.GetString("Dialog.SelectProgram", "Select Program to Monitor"),
                FileName = _pathTextBox.Text
            };

            if (dialog.ShowDialog() == true)
            {
                _pathTextBox.Text = dialog.FileName;
            }
        }
    }

    /// <summary>
    /// Dialog window for configuring application settings.
    /// Contains tabs for Logging, Email Notifications, Application settings, and About information.
    /// Handles secure password management and real-time UI localization updates.
    /// </summary>
    public class SettingsDialog : Window
    {
        /// <summary>
        /// Gets the log settings configured by the user.
        /// Only valid if DialogResult is true.
        /// </summary>
        public LogSettings LogSettings { get; private set; }
        
        /// <summary>
        /// Gets the notification settings configured by the user.
        /// Only valid if DialogResult is true.
        /// Password is encrypted using DPAPI before being stored.
        /// </summary>
        public NotificationSettings NotificationSettings { get; private set; }
        
        /// <summary>
        /// Gets the application settings configured by the user.
        /// Only valid if DialogResult is true.
        /// </summary>
        public AppSettings AppSettings { get; private set; }

        private TextBox _logPathTextBox;
        private ComboBox _logLevelComboBox;
        private CheckBox _enableFileLoggingCheckBox;
        private TextBox _maxLogSizeTextBox;
        private TextBox _keepLogsDaysTextBox;

        private CheckBox _enableEmailCheckBox;
        private TextBox _smtpServerTextBox;
        private TextBox _smtpPortTextBox;
        private CheckBox _useSslCheckBox;
        private TextBox _senderEmailTextBox;
        private TextBox _senderNameTextBox;
        private PasswordBox _senderPasswordBox;
        private TextBox _recipientEmailTextBox;
        private CheckBox _notifyOnRestartCheckBox;
        private CheckBox _notifyOnFailureCheckBox;
        private CheckBox _notifyOnStopCheckBox;
        private CheckBox _enableTaskbarCheckBox;
        private CheckBox _notifyOnRestartTaskbarCheckBox;
        private CheckBox _notifyOnFailureTaskbarCheckBox;
        private CheckBox _notifyOnStopTaskbarCheckBox;

        private CheckBox _startWithWindowsCheckBox;
        private CheckBox _minimizeToTrayCheckBox;
        private CheckBox _minimizeOnCloseCheckBox;
        private CheckBox _startMinimizedCheckBox;
        private ComboBox _languageComboBox;
        private string _originalEncryptedPassword; // Store original encrypted password

        // UI elements for localization
        private TabItem _loggingTab, _notificationsTab, _appTab, _appearanceTab, _aboutTab;
        private Button _okButton, _cancelButton, _applyButton, _browseFolderButton, _testEmailButton;
        private TextBlock _logPathLabel, _minLogLevelLabel, _maxLogSizeLabel, _keepLogsDaysLabel;
        private TextBlock _smtpServerLabel, _smtpPortLabel, _senderEmailLabel, _senderNameLabel;
        private TextBlock _senderPasswordLabel, _recipientEmailLabel, _languageLabel;
        private TextBlock _startWithWindowsDesc, _minimizeToTrayDesc, _startMinimizedDesc;
        private TextBlock _aboutDescription, _aboutGithubHeader, _aboutCopyright, _aboutTechStack, _aboutVersionText;
        
        // Appearance tab UI elements
        private ComboBox _themePresetComboBox;
        private ComboBox _fontFamilyComboBox;
        private Slider _fontSizeSlider;
        private TextBlock _fontSizeValueText;
        private Button _backgroundColorButton, _textColorButton, _highlightColorButton;
        private Button _borderColorButton, _surfaceColorButton, _secondaryTextColorButton;
        private Button _buttonTextColorButton, _headerColorButton;

        /// <summary>
        /// Initializes a new instance of the SettingsDialog with the current settings.
        /// </summary>
        /// <param name="logSettings">Current logging settings</param>
        /// <param name="notificationSettings">Current notification settings (password should be encrypted)</param>
        /// <param name="appSettings">Current application settings</param>
        public SettingsDialog(LogSettings logSettings, NotificationSettings notificationSettings, AppSettings appSettings)
        {
            LogSettings = new LogSettings
            {
                LogFilePath = logSettings.LogFilePath,
                MinimumLogLevel = logSettings.MinimumLogLevel,
                EnableFileLogging = logSettings.EnableFileLogging,
                MaxLogFileSizeMB = logSettings.MaxLogFileSizeMB,
                KeepLogFilesForDays = logSettings.KeepLogFilesForDays
            };

            NotificationSettings = new NotificationSettings
            {
                EnableEmailNotifications = notificationSettings.EnableEmailNotifications,
                SmtpServer = notificationSettings.SmtpServer,
                SmtpPort = notificationSettings.SmtpPort,
                UseSSL = notificationSettings.UseSSL,
                SenderEmail = notificationSettings.SenderEmail,
                SenderName = notificationSettings.SenderName,
                SenderPassword = notificationSettings.SenderPassword,
                RecipientEmail = notificationSettings.RecipientEmail,
                NotifyOnRestart = notificationSettings.NotifyOnRestart,
                NotifyOnFailure = notificationSettings.NotifyOnFailure
            };
            
            // Store original encrypted password (don't display it for security)
            _originalEncryptedPassword = notificationSettings.SenderPassword;

            AppSettings = new AppSettings
            {
                StartWithWindows = appSettings.StartWithWindows,
                MinimizeToTray = appSettings.MinimizeToTray,
                StartMinimized = appSettings.StartMinimized,
                MinimizeOnClose = appSettings.MinimizeOnClose,
                Language = appSettings.Language,
                FontFamily = appSettings.FontFamily ?? "Segoe UI",
                FontSize = appSettings.FontSize > 0 ? appSettings.FontSize : 12.0,
                BackgroundColor = appSettings.BackgroundColor ?? "#F5F5F5",
                TextColor = appSettings.TextColor ?? "#212121",
                HighlightColor = appSettings.HighlightColor ?? "#0078D4",
                BorderColor = appSettings.BorderColor ?? "#E0E0E0",
                SurfaceColor = appSettings.SurfaceColor ?? "#FFFFFF",
                SecondaryTextColor = appSettings.SecondaryTextColor ?? "#757575",
                ButtonTextColor = appSettings.ButtonTextColor,
                HeaderColor = appSettings.HeaderColor
            };

            InitializeDialog();
        }

        private void InitializeDialog()
        {
            Title = LocalizationService.Instance.GetString("Settings.Title", "Settings");
            Width = 600;
            Height = 550;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            
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
                System.Diagnostics.Debug.WriteLine($"Error setting Settings dialog icon: {ex.Message}");
            }
            
            // Apply theme to window
            Background = (System.Windows.Media.Brush)Application.Current.Resources["BackgroundColor"];

            var mainGrid = new Grid { Margin = new Thickness(10) };
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var tabControl = new TabControl
            {
                Background = (System.Windows.Media.Brush)Application.Current.Resources["BackgroundColor"],
                BorderBrush = (System.Windows.Media.Brush)Application.Current.Resources["BorderColor"]
            };

            // Helper method to create styled TabItem
            TabItem CreateStyledTabItem(string header, UIElement content)
            {
                var tabItem = new TabItem
                {
                    Header = header,
                    Content = content,
                    Background = (System.Windows.Media.Brush)Application.Current.Resources["BackgroundColor"],
                    Foreground = (System.Windows.Media.Brush)Application.Current.Resources["TextColor"],
                    FontFamily = (System.Windows.Media.FontFamily)Application.Current.Resources["AppFontFamily"],
                    FontSize = (double)Application.Current.Resources["AppFontSize"]
                };
                
                // Style for selected tab
                var style = new Style(typeof(TabItem));
                style.Setters.Add(new Setter(TabItem.BackgroundProperty, (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]));
                style.Setters.Add(new Setter(TabItem.ForegroundProperty, (System.Windows.Media.Brush)Application.Current.Resources["TextColor"]));
                
                var selectedTrigger = new Trigger { Property = TabItem.IsSelectedProperty, Value = true };
                selectedTrigger.Setters.Add(new Setter(TabItem.BackgroundProperty, (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]));
                style.Triggers.Add(selectedTrigger);
                
                var mouseOverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
                mouseOverTrigger.Setters.Add(new Setter(TabItem.BackgroundProperty, (System.Windows.Media.Brush)Application.Current.Resources["BackgroundColor"]));
                style.Triggers.Add(mouseOverTrigger);
                
                tabItem.Style = style;
                return tabItem;
            }

            // Logging Tab
            _loggingTab = CreateStyledTabItem(LocalizationService.Instance.GetString("Settings.Logging", "Logging"), CreateLoggingTab());
            tabControl.Items.Add(_loggingTab);

            // Notifications Tab
            _notificationsTab = CreateStyledTabItem(LocalizationService.Instance.GetString("Settings.Notifications", "Notifications"), CreateNotificationsTab());
            tabControl.Items.Add(_notificationsTab);

            // Application Tab
            _appTab = CreateStyledTabItem(LocalizationService.Instance.GetString("Settings.Application", "Application"), CreateAppTab());
            tabControl.Items.Add(_appTab);

            // Appearance Tab
            _appearanceTab = CreateStyledTabItem(LocalizationService.Instance.GetString("Settings.Appearance", "Appearance"), CreateAppearanceTab());
            tabControl.Items.Add(_appearanceTab);

            // About Tab
            _aboutTab = CreateStyledTabItem(LocalizationService.Instance.GetString("Settings.About", "About"), CreateAboutTab());
            tabControl.Items.Add(_aboutTab);

            Grid.SetRow(tabControl, 0);
            mainGrid.Children.Add(tabControl);

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(buttonPanel, 1);

            // Helper method to create styled button
            Button CreateStyledButton(string content, bool isDefault = false, bool isCancel = false)
            {
                var button = new Button
                {
                    Content = content,
                    Width = 80,
                    Height = 30,
                    Margin = new Thickness(0, 0, 10, 0),
                    IsDefault = isDefault,
                    IsCancel = isCancel,
                    Background = (System.Windows.Media.Brush)Application.Current.Resources["HighlightColor"],
                    Foreground = System.Windows.Media.Brushes.White,
                    FontFamily = (System.Windows.Media.FontFamily)Application.Current.Resources["AppFontFamily"],
                    FontSize = (double)Application.Current.Resources["AppFontSize"],
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(10, 5, 10, 5),
                    Cursor = System.Windows.Input.Cursors.Hand
                };
                
                // Hover effect
                var style = new Style(typeof(Button));
                var mouseOverTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
                var darkerBrush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(
                        (byte)(((System.Windows.Media.SolidColorBrush)Application.Current.Resources["HighlightColor"]).Color.R * 0.8),
                        (byte)(((System.Windows.Media.SolidColorBrush)Application.Current.Resources["HighlightColor"]).Color.G * 0.8),
                        (byte)(((System.Windows.Media.SolidColorBrush)Application.Current.Resources["HighlightColor"]).Color.B * 0.8)
                    ));
                mouseOverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, darkerBrush));
                style.Triggers.Add(mouseOverTrigger);
                
                button.Style = style;
                return button;
            }

            _okButton = CreateStyledButton(LocalizationService.Instance.GetString("Dialog.OK", "OK"), isDefault: true);
            _okButton.Click += OkButton_Click;

            _applyButton = CreateStyledButton(LocalizationService.Instance.GetString("Dialog.Apply", "Apply"));
            _applyButton.Click += ApplyButton_Click;

            _cancelButton = CreateStyledButton(LocalizationService.Instance.GetString("Dialog.Cancel", "Cancel"), isCancel: true);
            _cancelButton.Background = (System.Windows.Media.Brush)Application.Current.Resources["BorderColor"];
            _cancelButton.Foreground = (System.Windows.Media.Brush)Application.Current.Resources["TextColor"];

            buttonPanel.Children.Add(_okButton);
            buttonPanel.Children.Add(_applyButton);
            buttonPanel.Children.Add(_cancelButton);
            mainGrid.Children.Add(buttonPanel);

            Content = mainGrid;

            // Update UI text with localized strings
            UpdateUIText();
        }

        private ScrollViewer CreateLoggingTab()
        {
            var grid = new Grid 
            { 
                Margin = new Thickness(20),
                Background = (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]
            };
            for (int i = 0; i < 6; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;

            // Enable File Logging
            _enableFileLoggingCheckBox = new CheckBox
            {
                Content = "Enable File Logging",
                IsChecked = LogSettings.EnableFileLogging,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(_enableFileLoggingCheckBox, row++);
            grid.Children.Add(_enableFileLoggingCheckBox);

            // Log Path
            _logPathLabel = AddLabelWithReference(grid, LocalizationService.Instance.GetString("Settings.Logging.LogFileDirectory", "Log File Directory:"), row);
            var pathPanel = new DockPanel { Margin = new Thickness(0, 20, 0, 15) };
            Grid.SetRow(pathPanel, row++);

            _browseFolderButton = new Button
            {
                Content = LocalizationService.Instance.GetString("ProgramEdit.Browse", "Browse..."),
                Width = 80,
                Margin = new Thickness(5, 0, 0, 0)
            };
            _browseFolderButton.Click += BrowseButton_Click;
            DockPanel.SetDock(_browseFolderButton, Dock.Right);

            _logPathTextBox = new TextBox
            {
                Text = LogSettings.LogFilePath,
                Padding = new Thickness(5)
            };

            pathPanel.Children.Add(_browseFolderButton);
            pathPanel.Children.Add(_logPathTextBox);
            grid.Children.Add(pathPanel);

            // Minimum Log Level
            _minLogLevelLabel = AddLabelWithReference(grid, "Minimum Log Level:", row++);
            _logLevelComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            _logLevelComboBox.Items.Add(LogLevel.Debug);
            _logLevelComboBox.Items.Add(LogLevel.Info);
            _logLevelComboBox.Items.Add(LogLevel.Warning);
            _logLevelComboBox.Items.Add(LogLevel.Error);
            _logLevelComboBox.SelectedItem = LogSettings.MinimumLogLevel;
            Grid.SetRow(_logLevelComboBox, row++);
            grid.Children.Add(_logLevelComboBox);

            // Max Log Size
            _maxLogSizeLabel = AddLabelWithReference(grid, "Max Log File Size (MB):", row++);
            _maxLogSizeTextBox = new TextBox
            {
                Text = LogSettings.MaxLogFileSizeMB.ToString(),
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_maxLogSizeTextBox, row++);
            grid.Children.Add(_maxLogSizeTextBox);

            // Keep Logs Days
            _keepLogsDaysLabel = AddLabelWithReference(grid, "Keep Log Files For (days):", row++);
            _keepLogsDaysTextBox = new TextBox
            {
                Text = LogSettings.KeepLogFilesForDays.ToString(),
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_keepLogsDaysTextBox, row++);
            grid.Children.Add(_keepLogsDaysTextBox);

            return new ScrollViewer 
            { 
                Content = grid, 
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]
            };
        }

        private ScrollViewer CreateNotificationsTab()
        {
            var grid = new Grid 
            { 
                Margin = new Thickness(20),
                Background = (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]
            };
            for (int i = 0; i < 18; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;

            // Enable Email
            _enableEmailCheckBox = new CheckBox
            {
                Content = "Enable Email Notifications",
                IsChecked = NotificationSettings.EnableEmailNotifications,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(_enableEmailCheckBox, row++);
            grid.Children.Add(_enableEmailCheckBox);

            // SMTP Server
            _smtpServerLabel = AddLabelWithReference(grid, "SMTP Server:", row++);
            _smtpServerTextBox = new TextBox
            {
                Text = NotificationSettings.SmtpServer ?? string.Empty,
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_smtpServerTextBox, row++);
            grid.Children.Add(_smtpServerTextBox);

            // SMTP Port
            _smtpPortLabel = AddLabelWithReference(grid, "SMTP Port:", row++);
            _smtpPortTextBox = new TextBox
            {
                Text = NotificationSettings.SmtpPort.ToString(),
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_smtpPortTextBox, row++);
            grid.Children.Add(_smtpPortTextBox);

            // Use SSL
            _useSslCheckBox = new CheckBox
            {
                Content = "Use SSL/TLS",
                IsChecked = NotificationSettings.UseSSL,
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(_useSslCheckBox, row++);
            grid.Children.Add(_useSslCheckBox);

            // Sender Email
            _senderEmailLabel = AddLabelWithReference(grid, "Sender Email:", row++);
            _senderEmailTextBox = new TextBox
            {
                Text = NotificationSettings.SenderEmail ?? string.Empty,
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_senderEmailTextBox, row++);
            grid.Children.Add(_senderEmailTextBox);

            // Sender Name
            _senderNameLabel = AddLabelWithReference(grid, "Sender Name:", row++);
            _senderNameTextBox = new TextBox
            {
                Text = NotificationSettings.SenderName ?? string.Empty,
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_senderNameTextBox, row++);
            grid.Children.Add(_senderNameTextBox);

            // Sender Password
            // Note: We don't display the existing encrypted password for security reasons.
            // User must re-enter password if they want to change it.
            _senderPasswordLabel = AddLabelWithReference(grid, "Sender Password:", row++);
            _senderPasswordBox = new PasswordBox
            {
                Password = string.Empty, // Always start empty - don't display encrypted password
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_senderPasswordBox, row++);
            grid.Children.Add(_senderPasswordBox);

            // Recipient Email
            _recipientEmailLabel = AddLabelWithReference(grid, "Recipient Email:", row++);
            _recipientEmailTextBox = new TextBox
            {
                Text = NotificationSettings.RecipientEmail ?? string.Empty,
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_recipientEmailTextBox, row++);
            grid.Children.Add(_recipientEmailTextBox);

            // Notify On Restart
            _notifyOnRestartCheckBox = new CheckBox
            {
                Content = "Notify on successful restart",
                IsChecked = NotificationSettings.NotifyOnRestart,
                Margin = new Thickness(0, 5, 0, 5)
            };
            Grid.SetRow(_notifyOnRestartCheckBox, row++);
            grid.Children.Add(_notifyOnRestartCheckBox);

            // Notify On Failure
            _notifyOnFailureCheckBox = new CheckBox
            {
                Content = "Notify on restart failure",
                IsChecked = NotificationSettings.NotifyOnFailure,
                Margin = new Thickness(0, 5, 0, 5)
            };
            Grid.SetRow(_notifyOnFailureCheckBox, row++);
            grid.Children.Add(_notifyOnFailureCheckBox);

            // Notify On Stop/Crash
            _notifyOnStopCheckBox = new CheckBox
            {
                Content = LocalizationService.Instance.GetString("Settings.Email.NotifyOnStop", "Notify on Stop/Crash"),
                IsChecked = NotificationSettings.NotifyOnStop,
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(_notifyOnStopCheckBox, row++);
            grid.Children.Add(_notifyOnStopCheckBox);

            // Test Email Button
            _testEmailButton = new Button
            {
                Content = "Send Test Email",
                Width = 120,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 20)
            };
            _testEmailButton.Click += TestEmail_Click;
            Grid.SetRow(_testEmailButton, row++);
            grid.Children.Add(_testEmailButton);

            // Separator for Taskbar Notifications section
            var separator = new Separator
            {
                Margin = new Thickness(0, 20, 0, 15)
            };
            Grid.SetRow(separator, row++);
            grid.Children.Add(separator);

            // Taskbar Notifications Section Header
            var taskbarHeader = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Taskbar.Header", "Taskbar Notifications"),
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(taskbarHeader, row++);
            grid.Children.Add(taskbarHeader);

            // Enable Taskbar Notifications
            _enableTaskbarCheckBox = new CheckBox
            {
                Content = LocalizationService.Instance.GetString("Settings.Taskbar.Enable", "Enable Taskbar Notifications"),
                IsChecked = NotificationSettings.EnableTaskbarNotifications,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(_enableTaskbarCheckBox, row++);
            grid.Children.Add(_enableTaskbarCheckBox);

            // Notify On Restart (Taskbar)
            _notifyOnRestartTaskbarCheckBox = new CheckBox
            {
                Content = LocalizationService.Instance.GetString("Settings.Taskbar.NotifyOnRestart", "Notify on Successful Restart"),
                IsChecked = NotificationSettings.NotifyOnRestartTaskbar,
                Margin = new Thickness(0, 5, 0, 5)
            };
            Grid.SetRow(_notifyOnRestartTaskbarCheckBox, row++);
            grid.Children.Add(_notifyOnRestartTaskbarCheckBox);

            // Notify On Failure (Taskbar)
            _notifyOnFailureTaskbarCheckBox = new CheckBox
            {
                Content = LocalizationService.Instance.GetString("Settings.Taskbar.NotifyOnFailure", "Notify on Restart Failure"),
                IsChecked = NotificationSettings.NotifyOnFailureTaskbar,
                Margin = new Thickness(0, 5, 0, 5)
            };
            Grid.SetRow(_notifyOnFailureTaskbarCheckBox, row++);
            grid.Children.Add(_notifyOnFailureTaskbarCheckBox);

            // Notify On Stop/Crash (Taskbar)
            _notifyOnStopTaskbarCheckBox = new CheckBox
            {
                Content = LocalizationService.Instance.GetString("Settings.Taskbar.NotifyOnStop", "Notify on Stop/Crash"),
                IsChecked = NotificationSettings.NotifyOnStopTaskbar,
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(_notifyOnStopTaskbarCheckBox, row++);
            grid.Children.Add(_notifyOnStopTaskbarCheckBox);

            return new ScrollViewer 
            { 
                Content = grid, 
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]
            };
        }

        /// <summary>
        /// Handles the Test Email button click event.
        /// Validates email settings and sends a test email to verify configuration.
        /// Uses SecureString for password handling during the test.
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        private void TestEmail_Click(object sender, RoutedEventArgs e)
        {
            // Validate settings first
            if (string.IsNullOrWhiteSpace(_smtpServerTextBox.Text))
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Settings.Email.SmtpServerRequired", "Please enter SMTP server."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(_smtpPortTextBox.Text, out int port))
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Settings.Email.SmtpPortInvalid", "Please enter valid SMTP port."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_senderEmailTextBox.Text))
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Settings.Email.SenderEmailRequired", "Please enter sender email."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_recipientEmailTextBox.Text))
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Settings.Email.RecipientEmailRequired", "Please enter recipient email."),
                    LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create temp settings for test
            // Convert password to SecureString and encrypt for test (consistent with save behavior)
            SecureString testSecurePassword = null;
            string encryptedTestPassword = string.Empty;
            
            try
            {
                if (!string.IsNullOrEmpty(_senderPasswordBox.Password))
                {
                    testSecurePassword = new SecureString();
                    foreach (char c in _senderPasswordBox.Password)
                    {
                        testSecurePassword.AppendChar(c);
                    }
                    encryptedTestPassword = CredentialManager.EncryptSecureString(testSecurePassword);
                }
                
                var testSettings = new NotificationSettings
                {
                    EnableEmailNotifications = true,
                    SmtpServer = _smtpServerTextBox.Text,
                    SmtpPort = port,
                    UseSSL = _useSslCheckBox.IsChecked ?? true,
                    SenderEmail = _senderEmailTextBox.Text,
                    SenderName = _senderNameTextBox.Text,
                    SenderPassword = encryptedTestPassword,
                    RecipientEmail = _recipientEmailTextBox.Text
                };

                // Send test email
                var emailService = new EmailNotificationService(testSettings);

                try
                {
                    emailService.SendNotification(
                        LocalizationService.Instance.GetString("Settings.Email.TestSubject", "Test Email from RestartIt"),
                        LocalizationService.Instance.GetString("Settings.Email.TestBody", "This is a test email to verify your email notification settings are working correctly.\n\nIf you receive this email, your settings are configured properly!"));

                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Email.TestSuccessMessage", "Test email sent successfully! Check your inbox."),
                        LocalizationService.Instance.GetString("Settings.Email.TestSuccess", "Test Email Sent"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(LocalizationService.Instance.GetString("Settings.Email.TestErrorMessage", "Failed to send test email:\n\n{0}\n\nCommon issues:\n• Wrong SMTP server or port\n• Invalid credentials\n• App password required (Gmail)\n• Firewall blocking SMTP"), ex.Message),
                        LocalizationService.Instance.GetString("Settings.Email.TestError", "Email Test Failed"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            finally
            {
                // Always dispose SecureString to clear it from memory
                testSecurePassword?.Dispose();
            }
        }

        private ScrollViewer CreateAppTab()
        {
            var grid = new Grid { Margin = new Thickness(20) };
            for (int i = 0; i < 9; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;

            // Start with Windows
            _startWithWindowsCheckBox = new CheckBox
            {
                Content = "Start with Windows",
                IsChecked = AppSettings.StartWithWindows,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(_startWithWindowsCheckBox, row++);
            grid.Children.Add(_startWithWindowsCheckBox);

            _startWithWindowsDesc = new TextBlock
            {
                Text = "Automatically start RestartIt when Windows starts.",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(20, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(_startWithWindowsDesc, row++);
            grid.Children.Add(_startWithWindowsDesc);

            // Minimize to Tray
            _minimizeToTrayCheckBox = new CheckBox
            {
                Content = "Minimize to system tray",
                IsChecked = AppSettings.MinimizeToTray,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(_minimizeToTrayCheckBox, row++);
            grid.Children.Add(_minimizeToTrayCheckBox);

            _minimizeToTrayDesc = new TextBlock
            {
                Text = "When minimized, the application will hide to the system tray instead of the taskbar.",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(20, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(_minimizeToTrayDesc, row++);
            grid.Children.Add(_minimizeToTrayDesc);

            // Start Minimized
            _startMinimizedCheckBox = new CheckBox
            {
                Content = "Start minimized",
                IsChecked = AppSettings.StartMinimized,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(_startMinimizedCheckBox, row++);
            grid.Children.Add(_startMinimizedCheckBox);

            _startMinimizedDesc = new TextBlock
            {
                Text = "Start RestartIt minimized to system tray (requires 'Minimize to system tray' enabled).",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(20, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(_startMinimizedDesc, row++);
            grid.Children.Add(_startMinimizedDesc);

            // Minimize on Close
            _minimizeOnCloseCheckBox = new CheckBox
            {
                Content = "Minimize on Close",
                IsChecked = AppSettings.MinimizeOnClose,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(_minimizeOnCloseCheckBox, row++);
            grid.Children.Add(_minimizeOnCloseCheckBox);

            var minimizeOnCloseDesc = new TextBlock
            {
                Text = "When closing the window, minimize to system tray instead of exiting.",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(20, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(minimizeOnCloseDesc, row++);
            grid.Children.Add(minimizeOnCloseDesc);

            // Language
            _languageLabel = AddLabelWithReference(grid, LocalizationService.Instance.GetString("Settings.App.Language", "Language:"), row++);
            _languageComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 15),
                SelectedValuePath = "Code"
            };

            // Create ItemTemplate using DisplayName property which already includes icon + name
            // Using simple TextBlock binding to DisplayName - WPF should handle emoji rendering via font fallback
            var itemTemplate = new DataTemplate();
            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("DisplayName"));
            textBlockFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            // No explicit font set - uses system default, which should support emoji via font fallback
            itemTemplate.VisualTree = textBlockFactory;
            _languageComboBox.ItemTemplate = itemTemplate;

            var availableLanguages = LocalizationService.GetAvailableLanguages();
            foreach (var lang in availableLanguages)
            {
                // Debug: Verify Icon values are loaded
                System.Diagnostics.Debug.WriteLine($"Language: {lang.Code}, Icon: '{lang.Icon}', DisplayName: '{lang.DisplayName}'");
                _languageComboBox.Items.Add(lang);
            }

            // Select current language
            for (int i = 0; i < _languageComboBox.Items.Count; i++)
            {
                if (((LanguageInfo)_languageComboBox.Items[i]).Code == AppSettings.Language)
                {
                    _languageComboBox.SelectedIndex = i;
                    break;
                }
            }

            Grid.SetRow(_languageComboBox, row++);
            grid.Children.Add(_languageComboBox);

            return new ScrollViewer 
            { 
                Content = grid, 
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]
            };
        }

        private ScrollViewer CreateAppearanceTab()
        {
            var grid = new Grid 
            { 
                Margin = new Thickness(20),
                Background = (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]
            };
            // Reduced row count: Font Family, Font Size, Theme Preset, Color Section (2 columns), Reset Button
            for (int i = 0; i < 12; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            int row = 0;

            // Font Family (moved above Theme Preset)
            AddLabel(grid, LocalizationService.Instance.GetString("Settings.Appearance.FontFamily", "Font Family:"), row++);
            _fontFamilyComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            
            // Populate with all installed fonts, sorted alphabetically
            var installedFonts = Fonts.SystemFontFamilies
                .Select(f => f.Source)
                .Distinct()
                .OrderBy(f => f)
                .ToList();
            
            foreach (var font in installedFonts)
            {
                _fontFamilyComboBox.Items.Add(font);
            }
            
            // Select current font, or default to first item if not found
            if (installedFonts.Contains(AppSettings.FontFamily))
            {
                _fontFamilyComboBox.SelectedItem = AppSettings.FontFamily;
            }
            else if (installedFonts.Count > 0)
            {
                // Fallback to "Segoe UI" if available, otherwise first font
                var fallbackFont = installedFonts.Contains("Segoe UI") ? "Segoe UI" : installedFonts[0];
                _fontFamilyComboBox.SelectedItem = fallbackFont;
                AppSettings.FontFamily = fallbackFont;
            }
            _fontFamilyComboBox.SelectionChanged += (s, e) =>
            {
                if (_fontFamilyComboBox.SelectedItem != null)
                {
                    AppSettings.FontFamily = _fontFamilyComboBox.SelectedItem.ToString();
                }
            };
            Grid.SetRow(_fontFamilyComboBox, row++);
            grid.Children.Add(_fontFamilyComboBox);

            // Font Size (moved above Theme Preset)
            AddLabel(grid, LocalizationService.Instance.GetString("Settings.Appearance.FontSize", "Font Size:"), row++);
            var fontSizePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 15)
            };
            _fontSizeSlider = new Slider
            {
                Minimum = 8,
                Maximum = 24,
                Value = AppSettings.FontSize,
                Width = 200,
                TickFrequency = 1,
                IsSnapToTickEnabled = true
            };
            _fontSizeSlider.ValueChanged += (s, e) =>
            {
                AppSettings.FontSize = _fontSizeSlider.Value;
                _fontSizeValueText.Text = ((int)_fontSizeSlider.Value).ToString();
            };
            _fontSizeValueText = new TextBlock
            {
                Text = ((int)AppSettings.FontSize).ToString(),
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 30
            };
            fontSizePanel.Children.Add(_fontSizeSlider);
            fontSizePanel.Children.Add(_fontSizeValueText);
            Grid.SetRow(fontSizePanel, row++);
            grid.Children.Add(fontSizePanel);

            // Theme Presets (moved below Font options)
            AddLabel(grid, LocalizationService.Instance.GetString("Settings.Appearance.ThemePreset", "Theme Preset:"), row++);
            _themePresetComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            
            // Populate with theme options from ThemeManager
            _themePresetComboBox.Items.Add("Custom");
            foreach (var theme in ThemeManager.Instance.GetThemes())
            {
                _themePresetComboBox.Items.Add(theme.DisplayName);
            }
            
            // Select current preset or default to Custom
            var currentPreset = AppSettings.ThemePreset ?? "Custom";
            
            // Handle migration from old Catppuccin theme names
            if (currentPreset == "Latte" || currentPreset == "Frappe")
                currentPreset = "Light";
            else if (currentPreset == "Macchiato" || currentPreset == "Mocha")
                currentPreset = "Dark";
            
            // Try to find matching theme by display name
            var matchingTheme = ThemeManager.Instance.GetThemes()
                .FirstOrDefault(t => t.DisplayName == currentPreset || t.Name == currentPreset);
            
            if (matchingTheme != null)
            {
                _themePresetComboBox.SelectedItem = matchingTheme.DisplayName;
            }
            else
            {
                _themePresetComboBox.SelectedItem = "Custom";
            }
            
            _themePresetComboBox.SelectionChanged += (s, e) =>
            {
                if (_themePresetComboBox.SelectedItem != null)
                {
                    var selectedPreset = _themePresetComboBox.SelectedItem.ToString();
                    AppSettings.ThemePreset = selectedPreset;
                    
                    if (selectedPreset != "Custom")
                    {
                        // Apply theme from ThemeManager
                        var theme = ThemeManager.Instance.GetThemes()
                            .FirstOrDefault(t => t.DisplayName == selectedPreset);
                        
                        if (theme != null)
                        {
                            theme.ApplyToAppSettings(AppSettings);
                            UpdateColorButtons();
                        }
                    }
                }
            };
            Grid.SetRow(_themePresetComboBox, row++);
            grid.Children.Add(_themePresetComboBox);

            // Save/Load Theme Buttons
            var themeButtonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            
            var saveThemeButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Settings.Appearance.SaveTheme", "Save Theme..."),
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(10, 5, 10, 5)
            };
            saveThemeButton.Click += SaveTheme_Click;
            themeButtonsPanel.Children.Add(saveThemeButton);
            
            var deleteThemeButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Settings.Appearance.DeleteTheme", "Delete Theme..."),
                Margin = new Thickness(0, 0, 0, 0),
                Padding = new Thickness(10, 5, 10, 5)
            };
            deleteThemeButton.Click += DeleteTheme_Click;
            themeButtonsPanel.Children.Add(deleteThemeButton);
            
            Grid.SetRow(themeButtonsPanel, row++);
            grid.Children.Add(themeButtonsPanel);

            // Color Options Section Header
            var colorSectionLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.Colors", "Colors:"),
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 20, 0, 10)
            };
            Grid.SetRow(colorSectionLabel, row);
            Grid.SetColumn(colorSectionLabel, 0);
            Grid.SetColumnSpan(colorSectionLabel, 2);
            grid.Children.Add(colorSectionLabel);
            row++;

            // Two-Column Grid for Colors
            var colorGrid = new Grid
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            colorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            colorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10) }); // Spacer
            colorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            // Left Column
            var leftColumn = new StackPanel { Margin = new Thickness(0, 0, 5, 0) };
            Grid.SetColumn(leftColumn, 0);
            colorGrid.Children.Add(leftColumn);
            
            // Right Column
            var rightColumn = new StackPanel { Margin = new Thickness(5, 0, 0, 0) };
            Grid.SetColumn(rightColumn, 2);
            colorGrid.Children.Add(rightColumn);

            // Left Column Colors
            // Background Color
            var bgLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.BackgroundColor", "Background Color:"),
                Margin = new Thickness(0, 0, 0, 5)
            };
            leftColumn.Children.Add(bgLabel);
            _backgroundColorButton = CreateColorPickerButton(AppSettings.BackgroundColor, null);
            leftColumn.Children.Add(_backgroundColorButton);

            // Text Color
            var textLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.TextColor", "Text Color:"),
                Margin = new Thickness(0, 15, 0, 5)
            };
            leftColumn.Children.Add(textLabel);
            _textColorButton = CreateColorPickerButton(AppSettings.TextColor, null);
            leftColumn.Children.Add(_textColorButton);

            // Highlight Color
            var highlightLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.HighlightColor", "Highlight Color:"),
                Margin = new Thickness(0, 15, 0, 5)
            };
            leftColumn.Children.Add(highlightLabel);
            _highlightColorButton = CreateColorPickerButton(AppSettings.HighlightColor, null);
            leftColumn.Children.Add(_highlightColorButton);

            // Button Text Color
            var buttonTextLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.ButtonTextColor", "Button Text Color:"),
                Margin = new Thickness(0, 15, 0, 5)
            };
            leftColumn.Children.Add(buttonTextLabel);
            _buttonTextColorButton = CreateColorPickerButton(AppSettings.ButtonTextColor ?? string.Empty, null);
            leftColumn.Children.Add(_buttonTextColorButton);

            // Right Column Colors
            // Border Color
            var borderLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.BorderColor", "Border Color:"),
                Margin = new Thickness(0, 0, 0, 5)
            };
            rightColumn.Children.Add(borderLabel);
            _borderColorButton = CreateColorPickerButton(AppSettings.BorderColor, null);
            rightColumn.Children.Add(_borderColorButton);

            // Surface Color
            var surfaceLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.SurfaceColor", "Surface Color:"),
                Margin = new Thickness(0, 15, 0, 5)
            };
            rightColumn.Children.Add(surfaceLabel);
            _surfaceColorButton = CreateColorPickerButton(AppSettings.SurfaceColor, null);
            rightColumn.Children.Add(_surfaceColorButton);

            // Secondary Text Color
            var secondaryTextLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.SecondaryTextColor", "Secondary Text Color:"),
                Margin = new Thickness(0, 15, 0, 5)
            };
            rightColumn.Children.Add(secondaryTextLabel);
            _secondaryTextColorButton = CreateColorPickerButton(AppSettings.SecondaryTextColor, null);
            rightColumn.Children.Add(_secondaryTextColorButton);

            // Header Color
            var headerLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.HeaderColor", "Header Color:"),
                Margin = new Thickness(0, 15, 0, 5)
            };
            rightColumn.Children.Add(headerLabel);
            _headerColorButton = CreateColorPickerButton(AppSettings.HeaderColor ?? string.Empty, null);
            rightColumn.Children.Add(_headerColorButton);

            Grid.SetRow(colorGrid, row);
            Grid.SetColumn(colorGrid, 0);
            Grid.SetColumnSpan(colorGrid, 2);
            grid.Children.Add(colorGrid);
            row++;

            // Reset to Defaults Button
            var resetButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Settings.Appearance.ResetToDefaults", "Reset to Defaults"),
                Width = 150,
                Height = 30,
                Margin = new Thickness(0, 20, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            resetButton.Click += (s, e) =>
            {
                var defaults = new AppSettings();
                AppSettings.FontFamily = defaults.FontFamily;
                AppSettings.FontSize = defaults.FontSize;
                AppSettings.BackgroundColor = defaults.BackgroundColor;
                AppSettings.TextColor = defaults.TextColor;
                AppSettings.HighlightColor = defaults.HighlightColor;
                AppSettings.BorderColor = defaults.BorderColor;
                AppSettings.SurfaceColor = defaults.SurfaceColor;
                AppSettings.SecondaryTextColor = defaults.SecondaryTextColor;
                AppSettings.ButtonTextColor = defaults.ButtonTextColor;
                AppSettings.HeaderColor = defaults.HeaderColor;
                AppSettings.ThemePreset = defaults.ThemePreset;
                
                _themePresetComboBox.SelectedItem = AppSettings.ThemePreset;
                _fontFamilyComboBox.SelectedItem = AppSettings.FontFamily;
                _fontSizeSlider.Value = AppSettings.FontSize;
                UpdateColorButtons();
            };
            Grid.SetRow(resetButton, row++);
            grid.Children.Add(resetButton);

            return new ScrollViewer 
            { 
                Content = grid, 
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = (System.Windows.Media.Brush)Application.Current.Resources["SurfaceColor"]
            };
        }

        private Button CreateColorPickerButton(string initialColor, Action onColorChanged)
        {
            var button = new Button
            {
                Width = 150,
                Height = 30,
                Margin = new Thickness(0, 0, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // Handle empty/null colors (for optional ButtonTextColor and HeaderColor)
            var displayColor = string.IsNullOrWhiteSpace(initialColor) ? "#000000" : initialColor;

            var colorPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            var colorBox = new Border
            {
                Width = 20,
                Height = 20,
                Background = CreateBrush(displayColor),
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            var textBlock = new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(initialColor) ? "" : initialColor,
                VerticalAlignment = VerticalAlignment.Center
            };
            colorPanel.Children.Add(colorBox);
            colorPanel.Children.Add(textBlock);
            button.Content = colorPanel;

            button.Click += (s, e) =>
            {
                try
                {
                    string currentColor = button == _backgroundColorButton ? AppSettings.BackgroundColor :
                                      button == _textColorButton ? AppSettings.TextColor :
                                      button == _highlightColorButton ? AppSettings.HighlightColor :
                                      button == _borderColorButton ? AppSettings.BorderColor :
                                      button == _surfaceColorButton ? AppSettings.SurfaceColor :
                                      button == _secondaryTextColorButton ? AppSettings.SecondaryTextColor :
                                      button == _buttonTextColorButton ? (AppSettings.ButtonTextColor ?? "#FFFFFF") :
                                      (AppSettings.HeaderColor ?? AppSettings.SurfaceColor);

                    // Ensure currentColor is valid hex color
                    if (string.IsNullOrWhiteSpace(currentColor) || !currentColor.StartsWith("#"))
                        currentColor = "#000000";

                    var colorDialog = new Forms.ColorDialog
                    {
                        Color = System.Drawing.ColorTranslator.FromHtml(currentColor),
                        FullOpen = true
                    };

                    if (colorDialog.ShowDialog() == Forms.DialogResult.OK)
                    {
                        // Convert color to hex format manually to avoid named colors
                        var color = colorDialog.Color;
                        var newColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                        
                        // Update AppSettings
                        if (button == _backgroundColorButton)
                            AppSettings.BackgroundColor = newColor;
                        else if (button == _textColorButton)
                            AppSettings.TextColor = newColor;
                        else if (button == _highlightColorButton)
                            AppSettings.HighlightColor = newColor;
                        else if (button == _borderColorButton)
                            AppSettings.BorderColor = newColor;
                        else if (button == _surfaceColorButton)
                            AppSettings.SurfaceColor = newColor;
                        else if (button == _secondaryTextColorButton)
                            AppSettings.SecondaryTextColor = newColor;
                        else if (button == _buttonTextColorButton)
                            AppSettings.ButtonTextColor = newColor;
                        else if (button == _headerColorButton)
                            AppSettings.HeaderColor = newColor;
                        
                        // When user changes a color, switch to "Custom" theme to preserve changes
                        if (!string.IsNullOrWhiteSpace(AppSettings.ThemePreset) && AppSettings.ThemePreset != "Custom")
                        {
                            AppSettings.ThemePreset = "Custom";
                            // Update combo box to reflect the change
                            if (_themePresetComboBox != null)
                            {
                                _themePresetComboBox.SelectedItem = "Custom";
                            }
                        }
                        
                        // Update button display
                        colorBox.Background = CreateBrush(newColor);
                        textBlock.Text = newColor;
                        
                        onColorChanged?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error selecting color: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            return button;
        }

        private void UpdateColorButtons()
        {
            if (_backgroundColorButton != null)
            {
                var panel = _backgroundColorButton.Content as StackPanel;
                if (panel != null && panel.Children.Count >= 2)
                {
                    (panel.Children[0] as Border).Background = CreateBrush(AppSettings.BackgroundColor);
                    (panel.Children[1] as TextBlock).Text = AppSettings.BackgroundColor;
                }
            }
            if (_textColorButton != null)
            {
                var panel = _textColorButton.Content as StackPanel;
                if (panel != null && panel.Children.Count >= 2)
                {
                    (panel.Children[0] as Border).Background = CreateBrush(AppSettings.TextColor);
                    (panel.Children[1] as TextBlock).Text = AppSettings.TextColor;
                }
            }
            if (_highlightColorButton != null)
            {
                var panel = _highlightColorButton.Content as StackPanel;
                if (panel != null && panel.Children.Count >= 2)
                {
                    (panel.Children[0] as Border).Background = CreateBrush(AppSettings.HighlightColor);
                    (panel.Children[1] as TextBlock).Text = AppSettings.HighlightColor;
                }
            }
            if (_borderColorButton != null)
            {
                var panel = _borderColorButton.Content as StackPanel;
                if (panel != null && panel.Children.Count >= 2)
                {
                    (panel.Children[0] as Border).Background = CreateBrush(AppSettings.BorderColor);
                    (panel.Children[1] as TextBlock).Text = AppSettings.BorderColor;
                }
            }
            if (_surfaceColorButton != null)
            {
                var panel = _surfaceColorButton.Content as StackPanel;
                if (panel != null && panel.Children.Count >= 2)
                {
                    (panel.Children[0] as Border).Background = CreateBrush(AppSettings.SurfaceColor);
                    (panel.Children[1] as TextBlock).Text = AppSettings.SurfaceColor;
                }
            }
            if (_secondaryTextColorButton != null)
            {
                var panel = _secondaryTextColorButton.Content as StackPanel;
                if (panel != null && panel.Children.Count >= 2)
                {
                    (panel.Children[0] as Border).Background = CreateBrush(AppSettings.SecondaryTextColor);
                    (panel.Children[1] as TextBlock).Text = AppSettings.SecondaryTextColor;
                }
            }
            if (_buttonTextColorButton != null)
            {
                var panel = _buttonTextColorButton.Content as StackPanel;
                if (panel != null && panel.Children.Count >= 2)
                {
                    var buttonTextColor = AppSettings.ButtonTextColor ?? string.Empty;
                    (panel.Children[0] as Border).Background = CreateBrush(string.IsNullOrWhiteSpace(buttonTextColor) ? "#000000" : buttonTextColor);
                    (panel.Children[1] as TextBlock).Text = buttonTextColor;
                }
            }
            if (_headerColorButton != null)
            {
                var panel = _headerColorButton.Content as StackPanel;
                if (panel != null && panel.Children.Count >= 2)
                {
                    var headerColor = AppSettings.HeaderColor ?? string.Empty;
                    (panel.Children[0] as Border).Background = CreateBrush(string.IsNullOrWhiteSpace(headerColor) ? AppSettings.SurfaceColor : headerColor);
                    (panel.Children[1] as TextBlock).Text = headerColor;
                }
            }
        }

        private SolidColorBrush CreateBrush(string hexColor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hexColor))
                    return new SolidColorBrush(Colors.White);

                if (!hexColor.StartsWith("#"))
                    hexColor = "#" + hexColor;

                var color = (Color)ColorConverter.ConvertFromString(hexColor);
                return new SolidColorBrush(color);
            }
            catch
            {
                return new SolidColorBrush(Colors.White);
            }
        }


        private ScrollViewer CreateAboutTab()
        {
            var grid = new Grid { Margin = new Thickness(20) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int row = 0;

            // Application Name
            var appName = new TextBlock
            {
                Text = "RestartIt",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 5)
            };
            Grid.SetRow(appName, row++);
            grid.Children.Add(appName);

            // Version
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _aboutVersionText = new TextBlock
            {
                Text = $"Version {version.Major}.{version.Minor}.{version.Build}",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid.SetRow(_aboutVersionText, row++);
            grid.Children.Add(_aboutVersionText);

            // Description
            _aboutDescription = new TextBlock
            {
                Text = "Windows Application Monitor - Automatically restart programs that stop running",
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid.SetRow(_aboutDescription, row++);
            grid.Children.Add(_aboutDescription);

            // GitHub Section
            _aboutGithubHeader = new TextBlock
            {
                Text = "GitHub Repository",
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(_aboutGithubHeader, row++);
            grid.Children.Add(_aboutGithubHeader);

            var githubLink = new TextBlock
            {
                Margin = new Thickness(0, 0, 0, 20)
            };
            var hyperlink = new System.Windows.Documents.Hyperlink
            {
                NavigateUri = new Uri("https://github.com/Antik79/RestartIt")
            };
            hyperlink.Inlines.Add("https://github.com/Antik79/RestartIt");
            hyperlink.RequestNavigate += (s, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = e.Uri.AbsoluteUri,
                        UseShellExecute = true
                    });
                }
                catch { }
            };
            githubLink.Inlines.Add(hyperlink);
            Grid.SetRow(githubLink, row++);
            grid.Children.Add(githubLink);

            // Copyright
            _aboutCopyright = new TextBlock
            {
                Text = "Copyright © 2025 Antik79",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(_aboutCopyright, row++);
            grid.Children.Add(_aboutCopyright);

            // Technologies
            _aboutTechStack = new TextBlock
            {
                Text = "Built with .NET 8.0 and WPF",
                FontSize = 10,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 5, 0, 0)
            };
            Grid.SetRow(_aboutTechStack, row++);
            grid.Children.Add(_aboutTechStack);

            return new ScrollViewer { Content = grid, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        }

        private void AddLabel(Grid grid, string text, int row)
        {
            var label = new TextBlock
            {
                Text = text,
                Margin = new Thickness(0, 0, 0, 5),
                FontWeight = FontWeights.SemiBold
            };
            Grid.SetRow(label, row);
            grid.Children.Add(label);
        }

        private TextBlock AddLabelWithReference(Grid grid, string text, int row)
        {
            var label = new TextBlock
            {
                Text = text,
                Margin = new Thickness(0, 0, 0, 5),
                FontWeight = FontWeights.SemiBold
            };
            Grid.SetRow(label, row);
            grid.Children.Add(label);
            return label;
        }

        private TextBox AddTextBox(Grid grid, string text, int row)
        {
            var textBox = new TextBox
            {
                Text = text ?? string.Empty,
                Margin = new Thickness(0, 20, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(textBox, row);
            grid.Children.Add(textBox);
            return textBox;
        }

        /// <summary>
        /// Updates all UI text elements with localized strings.
        /// Called when the language changes to refresh the dialog text.
        /// </summary>
        private void UpdateUIText()
        {
            // Update window title
            Title = LocalizationService.Instance.GetString("Settings.Title", "Settings");

            // Update tab headers
            _loggingTab.Header = LocalizationService.Instance.GetString("Settings.Logging", "Logging");
            _notificationsTab.Header = LocalizationService.Instance.GetString("Settings.Notifications", "Notifications");
            _appTab.Header = LocalizationService.Instance.GetString("Settings.Application", "Application");
            if (_appearanceTab != null)
                _appearanceTab.Header = LocalizationService.Instance.GetString("Settings.Appearance", "Appearance");
            _aboutTab.Header = LocalizationService.Instance.GetString("Settings.About", "About");

            // Update buttons
            _okButton.Content = LocalizationService.Instance.GetString("Dialog.OK", "OK");
            _cancelButton.Content = LocalizationService.Instance.GetString("Dialog.Cancel", "Cancel");

            // Update Logging tab
            if (_enableFileLoggingCheckBox != null)
                _enableFileLoggingCheckBox.Content = LocalizationService.Instance.GetString("Settings.Logging.EnableFileLogging", "Enable File Logging");
            if (_logPathLabel != null)
                _logPathLabel.Text = LocalizationService.Instance.GetString("Settings.Logging.LogFilePath", "Log File Directory:");
            if (_browseFolderButton != null)
                _browseFolderButton.Content = LocalizationService.Instance.GetString("ProgramEdit.Browse", "Browse...");
            if (_minLogLevelLabel != null)
                _minLogLevelLabel.Text = LocalizationService.Instance.GetString("Settings.Logging.MinLogLevel", "Minimum Log Level:");
            if (_maxLogSizeLabel != null)
                _maxLogSizeLabel.Text = LocalizationService.Instance.GetString("Settings.Logging.MaxFileSize", "Max Log File Size (MB):");
            if (_keepLogsDaysLabel != null)
                _keepLogsDaysLabel.Text = LocalizationService.Instance.GetString("Settings.Logging.KeepLogsFor", "Keep Log Files For (days):");

            // Update Email tab
            if (_enableEmailCheckBox != null)
                _enableEmailCheckBox.Content = LocalizationService.Instance.GetString("Settings.Email.Enable", "Enable Email Notifications");
            if (_smtpServerLabel != null)
                _smtpServerLabel.Text = LocalizationService.Instance.GetString("Settings.Email.SmtpServer", "SMTP Server:");
            if (_smtpPortLabel != null)
                _smtpPortLabel.Text = LocalizationService.Instance.GetString("Settings.Email.SmtpPort", "SMTP Port:");
            if (_useSslCheckBox != null)
                _useSslCheckBox.Content = LocalizationService.Instance.GetString("Settings.Email.UseSsl", "Use SSL/TLS");
            if (_senderEmailLabel != null)
                _senderEmailLabel.Text = LocalizationService.Instance.GetString("Settings.Email.SenderEmail", "Sender Email:");
            if (_senderNameLabel != null)
                _senderNameLabel.Text = LocalizationService.Instance.GetString("Settings.Email.SenderName", "Sender Name:");
            if (_senderPasswordLabel != null)
                _senderPasswordLabel.Text = LocalizationService.Instance.GetString("Settings.Email.SenderPassword", "Sender Password:");
            if (_recipientEmailLabel != null)
                _recipientEmailLabel.Text = LocalizationService.Instance.GetString("Settings.Email.RecipientEmail", "Recipient Email:");
            if (_notifyOnRestartCheckBox != null)
                _notifyOnRestartCheckBox.Content = LocalizationService.Instance.GetString("Settings.Email.NotifyOnRestart", "Notify on Successful Restart");
            if (_notifyOnFailureCheckBox != null)
                _notifyOnFailureCheckBox.Content = LocalizationService.Instance.GetString("Settings.Email.NotifyOnFailure", "Notify on Restart Failure");
            if (_testEmailButton != null)
                _testEmailButton.Content = LocalizationService.Instance.GetString("Settings.Email.TestEmail", "Send Test Email");

            // Update Application tab
            if (_startWithWindowsCheckBox != null)
                _startWithWindowsCheckBox.Content = LocalizationService.Instance.GetString("Settings.App.StartWithWindows", "Start with Windows");
            if (_startWithWindowsDesc != null)
                _startWithWindowsDesc.Text = LocalizationService.Instance.GetString("Settings.App.StartWithWindowsDesc", "Automatically start RestartIt when Windows starts.");
            if (_minimizeToTrayCheckBox != null)
                _minimizeToTrayCheckBox.Content = LocalizationService.Instance.GetString("Settings.App.MinimizeToTray", "Minimize to System Tray");
            if (_minimizeToTrayDesc != null)
                _minimizeToTrayDesc.Text = LocalizationService.Instance.GetString("Settings.App.MinimizeToTrayDesc", "When minimized, the application will hide to the system tray instead of the taskbar.");
            if (_minimizeOnCloseCheckBox != null)
                _minimizeOnCloseCheckBox.Content = LocalizationService.Instance.GetString("Settings.App.MinimizeOnClose", "Minimize on Close");
            if (_startMinimizedCheckBox != null)
                _startMinimizedCheckBox.Content = LocalizationService.Instance.GetString("Settings.App.StartMinimized", "Start Minimized");
            if (_startMinimizedDesc != null)
                _startMinimizedDesc.Text = LocalizationService.Instance.GetString("Settings.App.StartMinimizedDesc", "Start RestartIt minimized to system tray (requires 'Minimize to system tray' enabled).");
            if (_languageLabel != null)
                _languageLabel.Text = LocalizationService.Instance.GetString("Settings.App.Language", "Language:");

            // Update About tab
            if (_aboutVersionText != null)
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                _aboutVersionText.Text = $"{LocalizationService.Instance.GetString("Settings.About.Version", "Version")} {version.Major}.{version.Minor}.{version.Build}";
            }
            if (_aboutDescription != null)
                _aboutDescription.Text = LocalizationService.Instance.GetString("Settings.About.Description", "A Windows desktop application that monitors specified programs and automatically restarts them if they stop running.");
            if (_aboutGithubHeader != null)
                _aboutGithubHeader.Text = LocalizationService.Instance.GetString("Settings.About.GitHub", "GitHub Repository");
            if (_aboutCopyright != null)
                _aboutCopyright.Text = LocalizationService.Instance.GetString("Settings.About.Copyright", "Copyright © 2025 Antik79");
            if (_aboutTechStack != null)
                _aboutTechStack.Text = LocalizationService.Instance.GetString("Settings.About.TechStack", "Built with .NET 8.0 and WPF");
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Forms.FolderBrowserDialog
            {
                SelectedPath = _logPathTextBox.Text
            };

            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                _logPathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void SaveTheme_Click(object sender, RoutedEventArgs e)
        {
            // Create a simple dialog to get theme metadata
            var dialog = new Window
            {
                Title = LocalizationService.Instance.GetString("Settings.Appearance.SaveThemeDialog.Title", "Save Theme"),
                Width = 400,
                Height = 380,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Theme Name label
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Theme Name textbox
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Display Name label
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Display Name textbox
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Description label
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Description textbox
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Author label
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Author textbox
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Buttons

            int row = 0;

            // Theme Name
            var nameLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.SaveThemeDialog.Name", "Theme Name:"),
                Margin = new Thickness(10, 10, 10, 5)
            };
            Grid.SetRow(nameLabel, row++);
            grid.Children.Add(nameLabel);

            var nameTextBox = new TextBox
            {
                Margin = new Thickness(10, 0, 10, 10),
                Text = AppSettings.ThemePreset == "Custom" ? "" : AppSettings.ThemePreset
            };
            Grid.SetRow(nameTextBox, row++);
            grid.Children.Add(nameTextBox);

            // Display Name
            var displayNameLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.SaveThemeDialog.DisplayName", "Display Name:"),
                Margin = new Thickness(10, 0, 10, 5)
            };
            Grid.SetRow(displayNameLabel, row++);
            grid.Children.Add(displayNameLabel);

            var displayNameTextBox = new TextBox
            {
                Margin = new Thickness(10, 0, 10, 10),
                Text = AppSettings.ThemePreset == "Custom" ? "" : AppSettings.ThemePreset
            };
            Grid.SetRow(displayNameTextBox, row++);
            grid.Children.Add(displayNameTextBox);

            // Description
            var descLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.SaveThemeDialog.Description", "Description:"),
                Margin = new Thickness(10, 0, 10, 5)
            };
            Grid.SetRow(descLabel, row++);
            grid.Children.Add(descLabel);

            var descTextBox = new TextBox
            {
                Margin = new Thickness(10, 0, 10, 10),
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                Height = 60,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MinHeight = 60
            };
            Grid.SetRow(descTextBox, row++);
            grid.Children.Add(descTextBox);

            // Author
            var authorLabel = new TextBlock
            {
                Text = LocalizationService.Instance.GetString("Settings.Appearance.SaveThemeDialog.Author", "Author:"),
                Margin = new Thickness(10, 0, 10, 5)
            };
            Grid.SetRow(authorLabel, row++);
            grid.Children.Add(authorLabel);

            var authorTextBox = new TextBox
            {
                Margin = new Thickness(10, 0, 10, 10)
            };
            Grid.SetRow(authorTextBox, row++);
            grid.Children.Add(authorTextBox);

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 10, 10, 10)
            };

            var okButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Common.OK", "OK"),
                Width = 75,
                Height = 25,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };

            var cancelButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Common.Cancel", "Cancel"),
                Width = 75,
                Height = 25,
                IsCancel = true
            };

            bool? result = null;
            okButton.Click += (s, e2) =>
            {
                if (string.IsNullOrWhiteSpace(nameTextBox.Text))
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Appearance.SaveThemeDialog.NameRequired", "Theme name is required."),
                        LocalizationService.Instance.GetString("Common.ValidationError", "Validation Error"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(displayNameTextBox.Text))
                {
                    displayNameTextBox.Text = nameTextBox.Text;
                }

                result = true;
                dialog.DialogResult = true;
                dialog.Close();
            };

            cancelButton.Click += (s, e2) =>
            {
                result = false;
                dialog.DialogResult = false;
                dialog.Close();
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            Grid.SetRow(buttonPanel, row);
            grid.Children.Add(buttonPanel);

            dialog.Content = grid;
            dialog.ShowDialog();

            if (result == true)
            {
                // Create theme definition from current AppSettings
                var theme = new ThemeDefinition
                {
                    Name = nameTextBox.Text.Trim(),
                    DisplayName = displayNameTextBox.Text.Trim(),
                    Description = descTextBox.Text.Trim(),
                    Author = authorTextBox.Text.Trim(),
                    Version = "1.0",
                    FontFamily = AppSettings.FontFamily,
                    FontSize = AppSettings.FontSize,
                    Colors = new ThemeColors
                    {
                        BackgroundColor = AppSettings.BackgroundColor,
                        TextColor = AppSettings.TextColor,
                        HighlightColor = AppSettings.HighlightColor,
                        BorderColor = AppSettings.BorderColor,
                        SurfaceColor = AppSettings.SurfaceColor,
                        SecondaryTextColor = AppSettings.SecondaryTextColor,
                        ButtonTextColor = AppSettings.ButtonTextColor,
                        HeaderColor = AppSettings.HeaderColor
                    }
                };

                if (ThemeManager.Instance.SaveTheme(theme))
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Appearance.SaveThemeDialog.Success", "Theme saved successfully!"),
                        LocalizationService.Instance.GetString("Common.Success", "Success"),
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reload theme list in combo box
                    _themePresetComboBox.Items.Clear();
                    _themePresetComboBox.Items.Add("Custom");
                    foreach (var t in ThemeManager.Instance.GetThemes())
                    {
                        _themePresetComboBox.Items.Add(t.DisplayName);
                    }

                    // Select the newly saved theme
                    _themePresetComboBox.SelectedItem = theme.DisplayName;
                    AppSettings.ThemePreset = theme.DisplayName;
                }
                else
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Appearance.SaveThemeDialog.Error", "Failed to save theme."),
                        LocalizationService.Instance.GetString("Common.Error", "Error"),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteTheme_Click(object sender, RoutedEventArgs e)
        {
            // Get currently selected theme
            if (_themePresetComboBox.SelectedItem == null)
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Settings.Appearance.DeleteThemeDialog.NoSelection", "Please select a theme to delete."),
                    LocalizationService.Instance.GetString("Common.Error", "Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedPreset = _themePresetComboBox.SelectedItem.ToString();
            
            // Don't allow deleting "Custom" or default themes
            if (selectedPreset == "Custom")
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Settings.Appearance.DeleteThemeDialog.CannotDeleteCustom", "Cannot delete 'Custom' theme."),
                    LocalizationService.Instance.GetString("Common.Error", "Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Find the theme
            var theme = ThemeManager.Instance.GetThemes()
                .FirstOrDefault(t => t.DisplayName == selectedPreset || t.Name == selectedPreset);

            if (theme == null)
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Settings.Appearance.DeleteThemeDialog.NotFound", "Theme not found."),
                    LocalizationService.Instance.GetString("Common.Error", "Error"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if it's a default theme (Light or Dark)
            if (ThemeManager.Instance.IsDefaultTheme(theme.Name))
            {
                MessageBox.Show(
                    LocalizationService.Instance.GetString("Settings.Appearance.DeleteThemeDialog.CannotDeleteDefault", "Cannot delete default themes (Light or Dark)."),
                    LocalizationService.Instance.GetString("Common.Error", "Error"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Confirm deletion
            var result = MessageBox.Show(
                string.Format(LocalizationService.Instance.GetString("Settings.Appearance.DeleteThemeDialog.Confirm", "Are you sure you want to delete the theme '{0}'?\n\nThis action cannot be undone."), theme.DisplayName),
                LocalizationService.Instance.GetString("Settings.Appearance.DeleteThemeDialog.Title", "Delete Theme"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                if (ThemeManager.Instance.DeleteTheme(theme.Name))
                {
                    // Reload theme list in combo box
                    _themePresetComboBox.Items.Clear();
                    _themePresetComboBox.Items.Add("Custom");
                    foreach (var t in ThemeManager.Instance.GetThemes())
                    {
                        _themePresetComboBox.Items.Add(t.DisplayName);
                    }

                    // If deleted theme was selected, switch to Custom
                    if (AppSettings.ThemePreset == selectedPreset)
                    {
                        AppSettings.ThemePreset = "Custom";
                        _themePresetComboBox.SelectedItem = "Custom";
                    }
                    else
                    {
                        // Try to keep current selection if it still exists
                        var currentTheme = ThemeManager.Instance.GetThemes()
                            .FirstOrDefault(t => t.DisplayName == AppSettings.ThemePreset);
                        if (currentTheme != null)
                        {
                            _themePresetComboBox.SelectedItem = currentTheme.DisplayName;
                        }
                        else
                        {
                            AppSettings.ThemePreset = "Custom";
                            _themePresetComboBox.SelectedItem = "Custom";
                        }
                    }

                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Appearance.DeleteThemeDialog.Success", "Theme deleted successfully!"),
                        LocalizationService.Instance.GetString("Common.Success", "Success"),
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Appearance.DeleteThemeDialog.Error", "Failed to delete theme."),
                        LocalizationService.Instance.GetString("Common.Error", "Error"),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate logging settings (same as OK button)
            if (!int.TryParse(_maxLogSizeTextBox.Text, out int maxSize) || maxSize < 1)
            {
                MessageBox.Show("Max log file size must be a positive number.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(_keepLogsDaysTextBox.Text, out int keepDays) || keepDays < 1)
            {
                MessageBox.Show("Keep log files for days must be a positive number.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate email settings
            if (_enableEmailCheckBox.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(_smtpServerTextBox.Text))
                {
                    MessageBox.Show("SMTP server is required for email notifications.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(_smtpPortTextBox.Text, out int port) || port < 1 || port > 65535)
                {
                    MessageBox.Show("SMTP port must be between 1 and 65535.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_senderEmailTextBox.Text) || !_senderEmailTextBox.Text.Contains("@"))
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Email.SenderEmailRequired", "Please enter sender email."),
                        LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_recipientEmailTextBox.Text) || !_recipientEmailTextBox.Text.Contains("@"))
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Email.RecipientEmailRequired", "Please enter recipient email."),
                        LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Save all settings (same as OK button)
            SaveAllSettings();

            // Apply theme changes immediately
            // If preset is set and not "Custom", use ThemeManager
            if (!string.IsNullOrWhiteSpace(AppSettings.ThemePreset) && AppSettings.ThemePreset != "Custom")
            {
                // Try to get theme by internal name first
                var theme = ThemeManager.Instance.GetTheme(AppSettings.ThemePreset);
                
                // If not found, try to find by display name
                if (theme == null)
                {
                    theme = ThemeManager.Instance.GetThemes()
                        .FirstOrDefault(t => t.DisplayName == AppSettings.ThemePreset);
                }
                
                if (theme != null)
                {
                    ThemeService.Instance.ApplyTheme(theme);
                }
                else
                {
                    ThemeService.Instance.ApplyTheme(AppSettings);
                }
            }
            else
            {
                ThemeService.Instance.ApplyTheme(AppSettings);
            }

            // Apply language changes immediately
            if (_languageComboBox.SelectedItem is LanguageInfo selectedLang)
            {
                LocalizationService.Instance.LoadLanguage(selectedLang.Code);
            }

            // Update UI text with new language
            UpdateUIText();

            // Show brief feedback
            _applyButton.Content = LocalizationService.Instance.GetString("Dialog.Applied", "Applied!");
            var originalContent = LocalizationService.Instance.GetString("Dialog.Apply", "Apply");
            System.Threading.Tasks.Task.Delay(1000).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    _applyButton.Content = originalContent;
                });
            });
        }

        private void SaveAllSettings()
        {
            // Save log settings
            LogSettings.LogFilePath = _logPathTextBox.Text;
            LogSettings.MinimumLogLevel = (LogLevel)_logLevelComboBox.SelectedItem;
            LogSettings.EnableFileLogging = _enableFileLoggingCheckBox.IsChecked ?? true;
            LogSettings.MaxLogFileSizeMB = int.Parse(_maxLogSizeTextBox.Text);
            LogSettings.KeepLogFilesForDays = int.Parse(_keepLogsDaysTextBox.Text);

            // Save email settings
            NotificationSettings.EnableEmailNotifications = _enableEmailCheckBox.IsChecked ?? false;
            NotificationSettings.SmtpServer = _smtpServerTextBox.Text;
            NotificationSettings.SmtpPort = int.Parse(_smtpPortTextBox.Text);
            NotificationSettings.UseSSL = _useSslCheckBox.IsChecked ?? true;
            NotificationSettings.SenderEmail = _senderEmailTextBox.Text;
            NotificationSettings.SenderName = _senderNameTextBox.Text;
            
            // Convert PasswordBox password to SecureString and encrypt immediately
            // Only update password if user entered a new one (PasswordBox is not empty)
            System.Security.SecureString securePassword = null;
            try
            {
                if (!string.IsNullOrEmpty(_senderPasswordBox.Password))
                {
                    // User entered a new password - convert to SecureString and encrypt
                    securePassword = new System.Security.SecureString();
                    foreach (char c in _senderPasswordBox.Password)
                    {
                        securePassword.AppendChar(c);
                    }
                    // Encrypt SecureString immediately - password never stored as plain text
                    NotificationSettings.SenderPassword = CredentialManager.EncryptSecureString(securePassword);
                }
                else
                {
                    // User didn't enter a new password - keep the original encrypted password
                    NotificationSettings.SenderPassword = _originalEncryptedPassword ?? string.Empty;
                }
            }
            finally
            {
                // Clear SecureString from memory immediately
                securePassword?.Dispose();
            }
            
            NotificationSettings.RecipientEmail = _recipientEmailTextBox.Text;
            NotificationSettings.NotifyOnRestart = _notifyOnRestartCheckBox.IsChecked ?? true;
            NotificationSettings.NotifyOnFailure = _notifyOnFailureCheckBox.IsChecked ?? true;
            NotificationSettings.NotifyOnStop = _notifyOnStopCheckBox.IsChecked ?? true;
            NotificationSettings.EnableTaskbarNotifications = _enableTaskbarCheckBox.IsChecked ?? true;
            NotificationSettings.NotifyOnRestartTaskbar = _notifyOnRestartTaskbarCheckBox.IsChecked ?? true;
            NotificationSettings.NotifyOnFailureTaskbar = _notifyOnFailureTaskbarCheckBox.IsChecked ?? true;
            NotificationSettings.NotifyOnStopTaskbar = _notifyOnStopTaskbarCheckBox.IsChecked ?? true;

            // Save app settings
            AppSettings.StartWithWindows = _startWithWindowsCheckBox.IsChecked ?? false;
            AppSettings.MinimizeToTray = _minimizeToTrayCheckBox.IsChecked ?? true;
            AppSettings.StartMinimized = _startMinimizedCheckBox.IsChecked ?? false;
            AppSettings.MinimizeOnClose = _minimizeOnCloseCheckBox.IsChecked ?? false;

            // Save language selection
            if (_languageComboBox.SelectedItem is LanguageInfo selectedLang)
            {
                AppSettings.Language = selectedLang.Code;
            }

            // Save appearance settings
            AppSettings.FontFamily = _fontFamilyComboBox?.SelectedItem?.ToString() ?? AppSettings.FontFamily;
            AppSettings.FontSize = _fontSizeSlider?.Value ?? AppSettings.FontSize;
            AppSettings.BackgroundColor = AppSettings.BackgroundColor;
            AppSettings.TextColor = AppSettings.TextColor;
            AppSettings.HighlightColor = AppSettings.HighlightColor;
            AppSettings.BorderColor = AppSettings.BorderColor;
            AppSettings.SurfaceColor = AppSettings.SurfaceColor;
            AppSettings.SecondaryTextColor = AppSettings.SecondaryTextColor;
            AppSettings.ButtonTextColor = AppSettings.ButtonTextColor;
            AppSettings.HeaderColor = AppSettings.HeaderColor;
            AppSettings.ThemePreset = AppSettings.ThemePreset;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate logging settings
            if (!int.TryParse(_maxLogSizeTextBox.Text, out int maxSize) || maxSize < 1)
            {
                MessageBox.Show("Max log file size must be a positive number.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(_keepLogsDaysTextBox.Text, out int keepDays) || keepDays < 1)
            {
                MessageBox.Show("Keep log files for days must be a positive number.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate email settings
            if (_enableEmailCheckBox.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(_smtpServerTextBox.Text))
                {
                    MessageBox.Show("SMTP server is required for email notifications.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(_smtpPortTextBox.Text, out int port) || port < 1 || port > 65535)
                {
                    MessageBox.Show("SMTP port must be between 1 and 65535.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_senderEmailTextBox.Text) || !_senderEmailTextBox.Text.Contains("@"))
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Email.SenderEmailRequired", "Please enter sender email."),
                        LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_recipientEmailTextBox.Text) || !_recipientEmailTextBox.Text.Contains("@"))
                {
                    MessageBox.Show(
                        LocalizationService.Instance.GetString("Settings.Email.RecipientEmailRequired", "Please enter recipient email."),
                        LocalizationService.Instance.GetString("ProgramEdit.ValidationError", "Validation Error"),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Save all settings using shared method
            SaveAllSettings();

            // Apply theme changes immediately
            // If preset is set and not "Custom", use ThemeManager
            if (!string.IsNullOrWhiteSpace(AppSettings.ThemePreset) && AppSettings.ThemePreset != "Custom")
            {
                // Try to get theme by internal name first
                var theme = ThemeManager.Instance.GetTheme(AppSettings.ThemePreset);
                
                // If not found, try to find by display name
                if (theme == null)
                {
                    theme = ThemeManager.Instance.GetThemes()
                        .FirstOrDefault(t => t.DisplayName == AppSettings.ThemePreset);
                }
                
                if (theme != null)
                {
                    ThemeService.Instance.ApplyTheme(theme);
                }
                else
                {
                    ThemeService.Instance.ApplyTheme(AppSettings);
                }
            }
            else
            {
                ThemeService.Instance.ApplyTheme(AppSettings);
            }

            // Apply language changes immediately
            if (_languageComboBox.SelectedItem is LanguageInfo selectedLang)
            {
                LocalizationService.Instance.LoadLanguage(selectedLang.Code);
            }

            DialogResult = true;
            Close();
        }
    }
}