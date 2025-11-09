using System;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;

namespace RestartIt
{
    // Program Edit Dialog
    public class ProgramEditDialog : Window
    {
        public MonitoredProgram Program { get; private set; }
        private bool _isNewProgram;

        private TextBox _nameTextBox;
        private TextBox _pathTextBox;
        private TextBox _argsTextBox;
        private TextBox _workingDirTextBox;
        private TextBox _checkIntervalTextBox;
        private TextBox _restartDelayTextBox;

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
                Status = "Disabled",
                WorkingDirectory = Path.GetDirectoryName(executablePath)
            };

            InitializeDialog();
        }

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
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            var grid = new Grid { Margin = new Thickness(20) };
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
            _pathTextBox = AddTextBox(grid, Program.ExecutablePath, 1);
            _pathTextBox.IsReadOnly = _isNewProgram;

            // Arguments
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.ArgumentsOptional", "Arguments (optional):"), 2);
            _argsTextBox = AddTextBox(grid, Program.Arguments, 2);

            // Working Directory
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.WorkingDirectoryOptional", "Working Directory (optional):"), 3);
            _workingDirTextBox = AddTextBox(grid, Program.WorkingDirectory, 3);

            // Check Interval
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.CheckInterval", "Check Interval (seconds):"), 4);
            _checkIntervalTextBox = AddTextBox(grid, Program.CheckIntervalSeconds.ToString(), 4);

            // Restart Delay
            AddLabel(grid, LocalizationService.Instance.GetString("ProgramEdit.RestartDelay", "Restart Delay (seconds):"), 5);
            _restartDelayTextBox = AddTextBox(grid, Program.RestartDelaySeconds.ToString(), 5);

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };
            Grid.SetRow(buttonPanel, 7);

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

            Content = grid;
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

            DialogResult = true;
            Close();
        }
    }

    // Settings Dialog with Tabs
    public class SettingsDialog : Window
    {
        public LogSettings LogSettings { get; private set; }
        public NotificationSettings NotificationSettings { get; private set; }
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

        private CheckBox _startWithWindowsCheckBox;
        private CheckBox _minimizeToTrayCheckBox;
        private CheckBox _startMinimizedCheckBox;
        private ComboBox _languageComboBox;
        private string _originalEncryptedPassword; // Store original encrypted password

        // UI elements for localization
        private TabItem _loggingTab, _emailTab, _appTab, _aboutTab;
        private Button _okButton, _cancelButton, _browseFolderButton, _testEmailButton;
        private TextBlock _logPathLabel, _minLogLevelLabel, _maxLogSizeLabel, _keepLogsDaysLabel;
        private TextBlock _smtpServerLabel, _smtpPortLabel, _senderEmailLabel, _senderNameLabel;
        private TextBlock _senderPasswordLabel, _recipientEmailLabel, _languageLabel;
        private TextBlock _startWithWindowsDesc, _minimizeToTrayDesc, _startMinimizedDesc;
        private TextBlock _aboutDescription, _aboutGithubHeader, _aboutCopyright, _aboutTechStack, _aboutVersionText;

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
                Language = appSettings.Language
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

            var mainGrid = new Grid { Margin = new Thickness(10) };
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var tabControl = new TabControl();

            // Logging Tab
            _loggingTab = new TabItem { Header = LocalizationService.Instance.GetString("Settings.Logging", "Logging") };
            _loggingTab.Content = CreateLoggingTab();
            tabControl.Items.Add(_loggingTab);

            // Email Notifications Tab
            _emailTab = new TabItem { Header = LocalizationService.Instance.GetString("Settings.EmailNotifications", "Email Notifications") };
            _emailTab.Content = CreateEmailTab();
            tabControl.Items.Add(_emailTab);

            // Application Tab
            _appTab = new TabItem { Header = LocalizationService.Instance.GetString("Settings.Application", "Application") };
            _appTab.Content = CreateAppTab();
            tabControl.Items.Add(_appTab);

            // About Tab
            _aboutTab = new TabItem { Header = LocalizationService.Instance.GetString("Settings.About", "About") };
            _aboutTab.Content = CreateAboutTab();
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

            _okButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Dialog.OK", "OK"),
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            _okButton.Click += OkButton_Click;

            _cancelButton = new Button
            {
                Content = LocalizationService.Instance.GetString("Dialog.Cancel", "Cancel"),
                Width = 80,
                Height = 30,
                IsCancel = true
            };

            buttonPanel.Children.Add(_okButton);
            buttonPanel.Children.Add(_cancelButton);
            mainGrid.Children.Add(buttonPanel);

            Content = mainGrid;

            // Update UI text with localized strings
            UpdateUIText();
        }

        private ScrollViewer CreateLoggingTab()
        {
            var grid = new Grid { Margin = new Thickness(20) };
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
            _minLogLevelLabel = AddLabelWithReference(grid, "Minimum Log Level:", row);
            _logLevelComboBox = new ComboBox { Margin = new Thickness(0, 20, 0, 15) };
            _logLevelComboBox.Items.Add(LogLevel.Debug);
            _logLevelComboBox.Items.Add(LogLevel.Info);
            _logLevelComboBox.Items.Add(LogLevel.Warning);
            _logLevelComboBox.Items.Add(LogLevel.Error);
            _logLevelComboBox.SelectedItem = LogSettings.MinimumLogLevel;
            Grid.SetRow(_logLevelComboBox, row++);
            grid.Children.Add(_logLevelComboBox);

            // Max Log Size
            _maxLogSizeLabel = AddLabelWithReference(grid, "Max Log File Size (MB):", row);
            _maxLogSizeTextBox = new TextBox
            {
                Text = LogSettings.MaxLogFileSizeMB.ToString(),
                Margin = new Thickness(0, 20, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_maxLogSizeTextBox, row++);
            grid.Children.Add(_maxLogSizeTextBox);

            // Keep Logs Days
            _keepLogsDaysLabel = AddLabelWithReference(grid, "Keep Log Files For (days):", row);
            _keepLogsDaysTextBox = new TextBox
            {
                Text = LogSettings.KeepLogFilesForDays.ToString(),
                Margin = new Thickness(0, 20, 0, 0),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_keepLogsDaysTextBox, row++);
            grid.Children.Add(_keepLogsDaysTextBox);

            return new ScrollViewer { Content = grid, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        }

        private ScrollViewer CreateEmailTab()
        {
            var grid = new Grid { Margin = new Thickness(20) };
            for (int i = 0; i < 12; i++)
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
            _smtpServerLabel = AddLabelWithReference(grid, "SMTP Server:", row);
            _smtpServerTextBox = AddTextBox(grid, NotificationSettings.SmtpServer, row++);

            // SMTP Port
            _smtpPortLabel = AddLabelWithReference(grid, "SMTP Port:", row);
            _smtpPortTextBox = AddTextBox(grid, NotificationSettings.SmtpPort.ToString(), row++);

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
            _senderEmailLabel = AddLabelWithReference(grid, "Sender Email:", row);
            _senderEmailTextBox = AddTextBox(grid, NotificationSettings.SenderEmail, row++);

            // Sender Name
            _senderNameLabel = AddLabelWithReference(grid, "Sender Name:", row);
            _senderNameTextBox = AddTextBox(grid, NotificationSettings.SenderName, row++);

            // Sender Password
            // Note: We don't display the existing encrypted password for security reasons.
            // User must re-enter password if they want to change it.
            _senderPasswordLabel = AddLabelWithReference(grid, "Sender Password:", row);
            _senderPasswordBox = new PasswordBox
            {
                Password = string.Empty, // Always start empty - don't display encrypted password
                Margin = new Thickness(0, 20, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_senderPasswordBox, row++);
            grid.Children.Add(_senderPasswordBox);

            // Recipient Email
            _recipientEmailLabel = AddLabelWithReference(grid, "Recipient Email:", row);
            _recipientEmailTextBox = AddTextBox(grid, NotificationSettings.RecipientEmail, row++);

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
                Margin = new Thickness(0, 0, 0, 15)
            };
            Grid.SetRow(_notifyOnFailureCheckBox, row++);
            grid.Children.Add(_notifyOnFailureCheckBox);

            // Test Email Button
            _testEmailButton = new Button
            {
                Content = "Send Test Email",
                Width = 120,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            _testEmailButton.Click += TestEmail_Click;
            Grid.SetRow(_testEmailButton, row++);
            grid.Children.Add(_testEmailButton);

            return new ScrollViewer { Content = grid, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        }

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
            for (int i = 0; i < 8; i++)
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

            // Language
            _languageLabel = new TextBlock
            {
                Text = "Language:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(_languageLabel, row++);
            grid.Children.Add(_languageLabel);

            _languageComboBox = new ComboBox
            {
                Margin = new Thickness(20, 0, 0, 15),
                DisplayMemberPath = "DisplayName",
                SelectedValuePath = "Code"
            };

            var availableLanguages = LocalizationService.GetAvailableLanguages();
            foreach (var lang in availableLanguages)
            {
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

            return new ScrollViewer { Content = grid, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
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

        private void UpdateUIText()
        {
            // Update window title
            Title = LocalizationService.Instance.GetString("Settings.Title", "Settings");

            // Update tab headers
            _loggingTab.Header = LocalizationService.Instance.GetString("Settings.Logging", "Logging");
            _emailTab.Header = LocalizationService.Instance.GetString("Settings.EmailNotifications", "Email Notifications");
            _appTab.Header = LocalizationService.Instance.GetString("Settings.Application", "Application");
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

            // Save log settings
            LogSettings.LogFilePath = _logPathTextBox.Text;
            LogSettings.MinimumLogLevel = (LogLevel)_logLevelComboBox.SelectedItem;
            LogSettings.EnableFileLogging = _enableFileLoggingCheckBox.IsChecked ?? true;
            LogSettings.MaxLogFileSizeMB = maxSize;
            LogSettings.KeepLogFilesForDays = keepDays;

            // Save email settings
            NotificationSettings.EnableEmailNotifications = _enableEmailCheckBox.IsChecked ?? false;
            NotificationSettings.SmtpServer = _smtpServerTextBox.Text;
            NotificationSettings.SmtpPort = int.Parse(_smtpPortTextBox.Text);
            NotificationSettings.UseSSL = _useSslCheckBox.IsChecked ?? true;
            NotificationSettings.SenderEmail = _senderEmailTextBox.Text;
            NotificationSettings.SenderName = _senderNameTextBox.Text;
            
            // Convert PasswordBox password to SecureString and encrypt immediately
            // Only update password if user entered a new one (PasswordBox is not empty)
            SecureString securePassword = null;
            try
            {
                if (!string.IsNullOrEmpty(_senderPasswordBox.Password))
                {
                    // User entered a new password - convert to SecureString and encrypt
                    securePassword = new SecureString();
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

            // Save app settings
            AppSettings.StartWithWindows = _startWithWindowsCheckBox.IsChecked ?? false;
            AppSettings.MinimizeToTray = _minimizeToTrayCheckBox.IsChecked ?? true;
            AppSettings.StartMinimized = _startMinimizedCheckBox.IsChecked ?? false;

            // Save language selection
            System.Diagnostics.Debug.WriteLine($"Language ComboBox SelectedItem: {_languageComboBox.SelectedItem}");
            System.Diagnostics.Debug.WriteLine($"Language ComboBox SelectedItem Type: {_languageComboBox.SelectedItem?.GetType()}");

            if (_languageComboBox.SelectedItem is LanguageInfo selectedLang)
            {
                System.Diagnostics.Debug.WriteLine($"Selected language: {selectedLang.Code} - {selectedLang.NativeName}");
                AppSettings.Language = selectedLang.Code;
                LocalizationService.Instance.LoadLanguage(selectedLang.Code);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SelectedItem is NOT a LanguageInfo!");
            }

            DialogResult = true;
            Close();
        }
    }
}