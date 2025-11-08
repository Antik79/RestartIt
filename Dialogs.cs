using System;
using System.IO;
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
            Title = _isNewProgram ? "Add Program" : "Edit Program";
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
            AddLabel(grid, "Program Name:", 0);
            _nameTextBox = AddTextBox(grid, Program.ProgramName, 0);

            // Executable Path
            AddLabel(grid, "Executable Path:", 1);
            _pathTextBox = AddTextBox(grid, Program.ExecutablePath, 1);
            _pathTextBox.IsReadOnly = _isNewProgram;

            // Arguments
            AddLabel(grid, "Arguments (optional):", 2);
            _argsTextBox = AddTextBox(grid, Program.Arguments, 2);

            // Working Directory
            AddLabel(grid, "Working Directory (optional):", 3);
            _workingDirTextBox = AddTextBox(grid, Program.WorkingDirectory, 3);

            // Check Interval
            AddLabel(grid, "Check Interval (seconds):", 4);
            _checkIntervalTextBox = AddTextBox(grid, Program.CheckIntervalSeconds.ToString(), 4);

            // Restart Delay
            AddLabel(grid, "Restart Delay (seconds):", 5);
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
                Content = "OK",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            okButton.Click += OkButton_Click;

            var cancelButton = new Button
            {
                Content = "Cancel",
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
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                MessageBox.Show("Please enter a program name.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(_checkIntervalTextBox.Text, out int checkInterval) || checkInterval < 1)
            {
                MessageBox.Show("Check interval must be a positive number.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(_restartDelayTextBox.Text, out int restartDelay) || restartDelay < 0)
            {
                MessageBox.Show("Restart delay must be zero or a positive number.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Program.ProgramName = _nameTextBox.Text;
            Program.ExecutablePath = _pathTextBox.Text;
            Program.Arguments = _argsTextBox.Text;
            Program.WorkingDirectory = _workingDirTextBox.Text;
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

            AppSettings = new AppSettings
            {
                StartWithWindows = appSettings.StartWithWindows,
                MinimizeToTray = appSettings.MinimizeToTray,
                StartMinimized = appSettings.StartMinimized
            };

            InitializeDialog();
        }

        private void InitializeDialog()
        {
            Title = "Settings";
            Width = 600;
            Height = 550;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;

            var mainGrid = new Grid { Margin = new Thickness(10) };
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var tabControl = new TabControl();

            // Logging Tab
            var loggingTab = new TabItem { Header = "Logging" };
            loggingTab.Content = CreateLoggingTab();
            tabControl.Items.Add(loggingTab);

            // Email Notifications Tab
            var emailTab = new TabItem { Header = "Email Notifications" };
            emailTab.Content = CreateEmailTab();
            tabControl.Items.Add(emailTab);

            // Application Tab
            var appTab = new TabItem { Header = "Application" };
            appTab.Content = CreateAppTab();
            tabControl.Items.Add(appTab);

            // About Tab
            var aboutTab = new TabItem { Header = "About" };
            aboutTab.Content = CreateAboutTab();
            tabControl.Items.Add(aboutTab);

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

            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            okButton.Click += OkButton_Click;

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                IsCancel = true
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            mainGrid.Children.Add(buttonPanel);

            Content = mainGrid;
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
            AddLabel(grid, "Log File Directory:", row);
            var pathPanel = new DockPanel { Margin = new Thickness(0, 20, 0, 15) };
            Grid.SetRow(pathPanel, row++);

            var browseButton = new Button
            {
                Content = "Browse...",
                Width = 80,
                Margin = new Thickness(5, 0, 0, 0)
            };
            browseButton.Click += BrowseButton_Click;
            DockPanel.SetDock(browseButton, Dock.Right);

            _logPathTextBox = new TextBox
            {
                Text = LogSettings.LogFilePath,
                Padding = new Thickness(5)
            };

            pathPanel.Children.Add(browseButton);
            pathPanel.Children.Add(_logPathTextBox);
            grid.Children.Add(pathPanel);

            // Minimum Log Level
            AddLabel(grid, "Minimum Log Level:", row);
            _logLevelComboBox = new ComboBox { Margin = new Thickness(0, 20, 0, 15) };
            _logLevelComboBox.Items.Add(LogLevel.Debug);
            _logLevelComboBox.Items.Add(LogLevel.Info);
            _logLevelComboBox.Items.Add(LogLevel.Warning);
            _logLevelComboBox.Items.Add(LogLevel.Error);
            _logLevelComboBox.SelectedItem = LogSettings.MinimumLogLevel;
            Grid.SetRow(_logLevelComboBox, row++);
            grid.Children.Add(_logLevelComboBox);

            // Max Log Size
            AddLabel(grid, "Max Log File Size (MB):", row);
            _maxLogSizeTextBox = new TextBox
            {
                Text = LogSettings.MaxLogFileSizeMB.ToString(),
                Margin = new Thickness(0, 20, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_maxLogSizeTextBox, row++);
            grid.Children.Add(_maxLogSizeTextBox);

            // Keep Logs Days
            AddLabel(grid, "Keep Log Files For (days):", row);
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
            AddLabel(grid, "SMTP Server:", row);
            _smtpServerTextBox = AddTextBox(grid, NotificationSettings.SmtpServer, row++);

            // SMTP Port
            AddLabel(grid, "SMTP Port:", row);
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
            AddLabel(grid, "Sender Email:", row);
            _senderEmailTextBox = AddTextBox(grid, NotificationSettings.SenderEmail, row++);

            // Sender Name
            AddLabel(grid, "Sender Name:", row);
            _senderNameTextBox = AddTextBox(grid, NotificationSettings.SenderName, row++);

            // Sender Password
            AddLabel(grid, "Sender Password:", row);
            _senderPasswordBox = new PasswordBox
            {
                Password = NotificationSettings.SenderPassword,
                Margin = new Thickness(0, 20, 0, 15),
                Padding = new Thickness(5)
            };
            Grid.SetRow(_senderPasswordBox, row++);
            grid.Children.Add(_senderPasswordBox);

            // Recipient Email
            AddLabel(grid, "Recipient Email:", row);
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
            var testEmailButton = new Button
            {
                Content = "Send Test Email",
                Width = 120,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 0)
            };
            testEmailButton.Click += TestEmail_Click;
            Grid.SetRow(testEmailButton, row++);
            grid.Children.Add(testEmailButton);

            return new ScrollViewer { Content = grid, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        }

        private void TestEmail_Click(object sender, RoutedEventArgs e)
        {
            // Validate settings first
            if (string.IsNullOrWhiteSpace(_smtpServerTextBox.Text))
            {
                MessageBox.Show("Please enter SMTP server.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(_smtpPortTextBox.Text, out int port))
            {
                MessageBox.Show("Please enter valid SMTP port.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_senderEmailTextBox.Text))
            {
                MessageBox.Show("Please enter sender email.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_recipientEmailTextBox.Text))
            {
                MessageBox.Show("Please enter recipient email.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create temp settings for test
            var testSettings = new NotificationSettings
            {
                EnableEmailNotifications = true,
                SmtpServer = _smtpServerTextBox.Text,
                SmtpPort = port,
                UseSSL = _useSslCheckBox.IsChecked ?? true,
                SenderEmail = _senderEmailTextBox.Text,
                SenderName = _senderNameTextBox.Text,
                SenderPassword = _senderPasswordBox.Password,
                RecipientEmail = _recipientEmailTextBox.Text
            };

            // Send test email
            var emailService = new EmailNotificationService(testSettings);

            try
            {
                emailService.SendNotification(
                    "Test Email from RestartIt",
                    "This is a test email to verify your email notification settings are working correctly.\n\n" +
                    "If you receive this email, your settings are configured properly!");

                MessageBox.Show(
                    "Test email has been sent! Please check your inbox.\n\n" +
                    "Note: It may take a few moments to arrive. Check your spam folder if you don't see it.",
                    "Test Email Sent",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to send test email:\n\n{ex.Message}\n\n" +
                    "Common issues:\n" +
                    "• Wrong SMTP server or port\n" +
                    "• Invalid credentials\n" +
                    "• App password required (Gmail)\n" +
                    "• Firewall blocking SMTP",
                    "Email Test Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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

            var startWithWindowsDesc = new TextBlock
            {
                Text = "Automatically start RestartIt when Windows starts.",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(20, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(startWithWindowsDesc, row++);
            grid.Children.Add(startWithWindowsDesc);

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

            var minimizeDesc = new TextBlock
            {
                Text = "When minimized, the application will hide to the system tray instead of the taskbar.",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(20, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(minimizeDesc, row++);
            grid.Children.Add(minimizeDesc);

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

            var startMinimizedDesc = new TextBlock
            {
                Text = "Start RestartIt minimized to system tray (requires 'Minimize to system tray' enabled).",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(20, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(startMinimizedDesc, row++);
            grid.Children.Add(startMinimizedDesc);

            // Language
            var languageLabel = new TextBlock
            {
                Text = "Language:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(languageLabel, row++);
            grid.Children.Add(languageLabel);

            _languageComboBox = new ComboBox
            {
                Margin = new Thickness(20, 0, 0, 15),
                DisplayMemberPath = "Name",
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
            var versionText = new TextBlock
            {
                Text = $"Version {version.Major}.{version.Minor}.{version.Build}",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid.SetRow(versionText, row++);
            grid.Children.Add(versionText);

            // Description
            var description = new TextBlock
            {
                Text = "Windows Application Monitor - Automatically restart programs that stop running",
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };
            Grid.SetRow(description, row++);
            grid.Children.Add(description);

            // GitHub Section
            var githubHeader = new TextBlock
            {
                Text = "GitHub Repository",
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            Grid.SetRow(githubHeader, row++);
            grid.Children.Add(githubHeader);

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
            var copyright = new TextBlock
            {
                Text = "Copyright © 2025 Antik79",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(copyright, row++);
            grid.Children.Add(copyright);

            // Technologies
            var techStack = new TextBlock
            {
                Text = "Built with .NET 8.0 and WPF",
                FontSize = 10,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 5, 0, 0)
            };
            Grid.SetRow(techStack, row++);
            grid.Children.Add(techStack);

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
                    MessageBox.Show("Please enter a valid sender email address.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_recipientEmailTextBox.Text) || !_recipientEmailTextBox.Text.Contains("@"))
                {
                    MessageBox.Show("Please enter a valid recipient email address.", "Validation Error",
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
            NotificationSettings.SenderPassword = _senderPasswordBox.Password;
            NotificationSettings.RecipientEmail = _recipientEmailTextBox.Text;
            NotificationSettings.NotifyOnRestart = _notifyOnRestartCheckBox.IsChecked ?? true;
            NotificationSettings.NotifyOnFailure = _notifyOnFailureCheckBox.IsChecked ?? true;

            // Save app settings
            AppSettings.StartWithWindows = _startWithWindowsCheckBox.IsChecked ?? false;
            AppSettings.MinimizeToTray = _minimizeToTrayCheckBox.IsChecked ?? true;
            AppSettings.StartMinimized = _startMinimizedCheckBox.IsChecked ?? false;

            // Save language selection
            if (_languageComboBox.SelectedItem is LanguageInfo selectedLang)
            {
                AppSettings.Language = selectedLang.Code;
                LocalizationService.Instance.LoadLanguage(selectedLang.Code);
            }

            DialogResult = true;
            Close();
        }
    }
}