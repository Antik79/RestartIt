# RestartIt

**Version 1.3.2** | [Download Latest Release](https://github.com/Antik79/RestartIt/releases/latest)

**RestartIt** is a Windows desktop application that monitors specified programs and automatically restarts them if they stop running. Perfect for maintaining critical applications, services, or any software that needs to stay running 24/7.

## Features

### Core Functionality
- **Automatic Monitoring** - Continuously monitors configured applications at customizable intervals
- **Smart Restart** - Automatically relaunches stopped programs with configurable delay
- **Status Tracking** - Real-time display of program states (Running, Stopped, Restarting, Failed)
- **Process Verification** - Validates both process name and full executable path for accuracy

### Notification System
- **Email Alerts** - SMTP-based email notifications when programs restart or fail
- **Customizable Sender** - Configure sender name and email for professional notifications
- **Flexible Rules** - Choose to be notified on successful restart, failure, or both

### Logging & Monitoring
- **Comprehensive Logging** - Records all monitoring activities with timestamps
- **File-Based Logs** - Persistent log files with automatic rotation
- **Log Management** - Configurable retention period and maximum file size
- **Activity Display** - Real-time activity log in the UI with bounded buffer (prevents memory leaks)

### User Interface
- **Modern Enhanced UI** - Complete redesign with card-based layout, gradient buttons, and subtle shadows
- **Customizable Theming** - Full theme support with Catppuccin presets (Latte, Frappe, Macchiato, Mocha)
- **Appearance Settings** - Customize fonts, colors, and theme presets with live preview
- **Clean Professional Design** - Built with WPF and custom styling (zero dependencies)
- **Multi-Language Support** - Available in 16 languages with real-time switching
- **Modular Language System** - Add new languages by simply dropping JSON files
- **System Tray Integration** - Runs quietly in the background
- **Easy Configuration** - Simple dialogs for adding and editing monitored programs
- **Per-Program Settings** - Individual check intervals and restart delays

### Security & Reliability
- **Password Encryption** - Email credentials encrypted using Windows DPAPI
- **Resource Management** - Proper disposal of process handles to prevent leaks
- **Memory Efficient** - Bounded log buffers prevent unbounded memory growth
- **Startup Integration** - Optional startup with Windows

## Screenshots

> **Note:** Screenshots are not currently included in this documentation. The application features a modern card-based UI with gradient buttons, shadows, and a clean professional design. You can see the interface by running the application.

### Main Window
The main interface shows all monitored programs with their current status, check intervals, and allows easy management.

### Settings Dialog
Configure logging, email notifications, and application behavior through an intuitive tabbed interface.

### System Tray
Minimal footprint - runs in the system tray with quick access to show/hide and exit options.

## Installation

### Prerequisites
- Windows 10 or later
- .NET 8.0 Runtime ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))

### Download
1. Download the latest release from the [Releases](https://github.com/Antik79/RestartIt/releases) page
2. Extract the ZIP file to your desired location
3. Run `RestartIt.exe`

### Windows SmartScreen Warning

When running RestartIt for the first time, you may see a **"Windows protected your PC"** message. This is normal for applications without a code signing certificate.

**To run the application:**
1. Click **"More info"**
2. Click **"Run anyway"**

**Why does this happen?**
- RestartIt is not code-signed (certificates cost money for open-source projects)
- Windows SmartScreen warns about unsigned applications as a security precaution
- The application is safe - all source code is available on GitHub for review

**Alternative:** Build from source (see below) to avoid this warning entirely.

### Build from Source
```bash
git clone https://github.com/Antik79/RestartIt.git
cd RestartIt
dotnet build --configuration Release
```

The compiled application will be in `bin/Release/net8.0-windows/`

## Usage

### Adding a Program to Monitor

1. Click the **"Add Program"** button
2. Browse to select the executable file (.exe)
3. Configure settings:
   - **Program Name** - Display name for the program
   - **Executable Path** - Full path to the .exe file
   - **Arguments** - Command-line arguments (optional)
   - **Working Directory** - Starting directory for the program
   - **Check Interval** - How often to check if running (in seconds)
   - **Restart Delay** - Wait time before restarting (in seconds)
4. Click **OK** to add

### Configuring Email Notifications

1. Click **Settings** → **Email Notifications** tab
2. Enable email notifications
3. Configure SMTP settings:
   - **SMTP Server** - e.g., smtp.gmail.com
   - **SMTP Port** - e.g., 587 for TLS
   - **Use SSL/TLS** - Enable for secure connection
   - **Sender Email** - Your email address
   - **Sender Name** - Display name (e.g., "Server Monitor")
   - **Sender Password** - Email password (encrypted with DPAPI)
   - **Recipient Email** - Where to send notifications
4. Use **"Send Test Email"** to verify settings
5. Choose notification preferences (restart success, failures, or both)

**Note for Gmail users:** You'll need to use an [App Password](https://support.google.com/accounts/answer/185833) instead of your regular password.

### Email Notification Examples

RestartIt sends email notifications with the following format:

#### Successful Restart Notification
```
Subject: [RestartIt] RestartIt: Program Restarted - {ProgramName}

Body:
The program '{ProgramName}' has stopped running and was automatically restarted.

Time: {Timestamp}
Path: {ExecutablePath}

Timestamp: {CurrentDateTime}

Sent by RestartIt Application Monitor
```

#### Failed Restart Notification
```
Subject: [RestartIt] RestartIt: Restart Failed - {ProgramName}

Body:
The program '{ProgramName}' has stopped running but could not be restarted.

Time: {Timestamp}
Path: {ExecutablePath}

Timestamp: {CurrentDateTime}

Sent by RestartIt Application Monitor
```

#### Test Email
```
Subject: [RestartIt] Test Email from RestartIt

Body:
This is a test email to verify your email notification settings are working correctly.

If you receive this email, your settings are configured properly!

Timestamp: {CurrentDateTime}

Sent by RestartIt Application Monitor
```

**Notification Preferences:**
- You can choose to receive notifications on successful restarts only
- Or on failures only
- Or both (default)
- Configure this in Settings → Email Notifications tab

### Configuring Logging

1. Click **Settings** → **Logging** tab
2. Configure:
   - **Log File Path** - Where to store log files
   - **Minimum Log Level** - Debug, Info, Warning, or Error
   - **Enable File Logging** - Toggle file logging on/off
   - **Max Log File Size** - Maximum size before rotation (MB)
   - **Keep Log Files For** - How many days to retain logs

### Exporting Logs

RestartIt allows you to export logs for analysis or sharing:

1. Click the **"Export Logs"** button in the main window toolbar
2. Choose a location and filename for the exported log file
3. The export includes:
   - Current day's log file (if file logging is enabled)
   - Or UI activity log (if file logging is disabled)
4. Exported files are saved as `.txt` or `.log` files with timestamp in filename

**Export Format:**
- Exports are saved with filename pattern: `RestartIt_Export_YYYY-MM-DD_HHmmss.txt`
- Contains all log entries with timestamps and log levels
- Can be opened in any text editor for analysis

### Log File Format

RestartIt creates log files with the following structure:

**File Naming:**
- Format: `RestartIt_YYYY-MM-DD.log`
- Example: `RestartIt_2025-01-27.log`
- Location: Configured in Settings → Logging tab (default: `%APPDATA%\RestartIt\logs`)

**Log Entry Format:**
```
[YYYY-MM-DD HH:mm:ss] [LEVEL] Message
```

**Example Log Entries:**
```
[2025-01-27 10:30:15] [Info] Monitoring started for: My Application
[2025-01-27 10:31:20] [Info] Program 'My Application' is running
[2025-01-27 10:32:45] [Warning] Program 'My Application' is not running
[2025-01-27 10:32:50] [Info] Restarting program: My Application
[2025-01-27 10:32:52] [Info] Successfully restarted: My Application
[2025-01-27 10:33:10] [Error] Failed to restart program: My Application - Access denied
```

**Log Levels:**
- **Debug (0)**: Detailed diagnostic information for troubleshooting
- **Info (1)**: General informational messages about normal operation
- **Warning (2)**: Warning messages for potential issues
- **Error (3)**: Error messages for failures and exceptions

**Log Rotation:**
- Logs rotate daily (new file each day)
- Logs also rotate when file size exceeds `MaxLogFileSizeMB` setting
- Old log files are automatically deleted after `KeepLogFilesForDays` period

### Application Settings

1. Click **Settings** → **Application** tab
2. Configure:
   - **Start with Windows** - Launch at Windows startup
   - **Minimize to Tray** - Hide to system tray when minimized
   - **Start Minimized** - Launch minimized to tray
   - **Language** - Select from 16 available languages

## Language Support

RestartIt supports **16 languages** with complete UI localization:

| Language | Native Name | Flag |
|----------|-------------|------|
| English | English | 🇬🇧 |
| Dutch | Nederlands | 🇳🇱 |
| French | Français | 🇫🇷 |
| German | Deutsch | 🇩🇪 |
| Spanish | Español | 🇪🇸 |
| Italian | Italiano | 🇮🇹 |
| Japanese | 日本語 | 🇯🇵 |
| Korean | 한국어 | 🇰🇷 |
| Chinese | 中文 | 🇨🇳 |
| Turkish | Türkçe | 🇹🇷 |
| Greek | Ελληνικά | 🇬🇷 |
| Swedish | Svenska | 🇸🇪 |
| Norwegian | Norsk | 🇳🇴 |
| Danish | Dansk | 🇩🇰 |
| Finnish | Suomi | 🇫🇮 |
| Swahili | Swahili | - |

### Changing Language

1. Open **Settings** → **Application** tab
2. Select your preferred language from the dropdown
3. Click **OK** to apply
4. The entire UI updates immediately - no restart required!

All UI elements, dialogs, buttons, and log messages are fully localized.

### Adding a New Language

RestartIt uses a modular language system - you can add new languages without modifying code:

1. Create a JSON file in the `Localization` folder (e.g., `pt.json` for Portuguese)
2. Add the metadata and translations:
```json
{
  "_metadata": {
    "code": "pt",
    "name": "Portuguese",
    "nativeName": "Português",
    "icon": "🇵🇹"
  },
  "App.Title": "RestartIt - Monitor de Aplicações",
  ...
}
```
3. The language will automatically appear in the Settings dropdown!

See existing language files in the `Localization` folder for the complete list of translation keys.

## Configuration

Settings are stored in `%APPDATA%\RestartIt\config.json`

### Configuration File Location
```
C:\Users\[YourUsername]\AppData\Roaming\RestartIt\config.json
```

### Configuration File Structure

The `config.json` file uses the following JSON structure:

```json
{
  "Programs": [
    {
      "ProgramName": "My Application",
      "ExecutablePath": "C:\\Program Files\\MyApp\\app.exe",
      "Arguments": "--start",
      "WorkingDirectory": "C:\\Program Files\\MyApp",
      "CheckIntervalSeconds": 60,
      "RestartDelaySeconds": 5,
      "Enabled": true,
      "Status": "Running",
      "LastRestartTime": "2025-01-27T10:30:00"
    }
  ],
  "LogSettings": {
    "LogFilePath": "C:\\Users\\YourUsername\\AppData\\Roaming\\RestartIt\\logs",
    "MinimumLogLevel": 1,
    "EnableFileLogging": true,
    "MaxLogFileSizeMB": 10,
    "KeepLogFilesForDays": 30
  },
  "NotificationSettings": {
    "EnableEmailNotifications": true,
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSSL": true,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "RestartIt Monitor",
    "SenderPassword": "AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAA...",
    "RecipientEmail": "recipient@example.com",
    "NotifyOnRestart": true,
    "NotifyOnFailure": true
  },
  "AppSettings": {
    "StartWithWindows": false,
    "MinimizeToTray": true,
    "StartMinimized": false,
    "Language": "en"
  }
}
```

**Configuration Notes:**
- `MinimumLogLevel`: 0=Debug, 1=Info, 2=Warning, 3=Error
- `SenderPassword`: Encrypted using Windows DPAPI (shown as Base64 string)
- `LastRestartTime`: ISO 8601 format timestamp or null
- `Status`: One of "Running", "Stopped", "Restarting", "Failed"
- The configuration file is automatically created and managed by RestartIt
- Manual editing is possible but not recommended - use the UI instead

### Security Note
Email passwords are encrypted using Windows Data Protection API (DPAPI) and can only be decrypted by the same Windows user account that encrypted them.

## Technology Stack

- **Framework**: .NET 8.0
- **UI**: WPF (Windows Presentation Foundation)
- **System Tray**: Windows Forms NotifyIcon
- **Configuration**: JSON serialization
- **Email**: System.Net.Mail (SMTP)
- **Encryption**: Windows DPAPI (Data Protection API)

## Use Cases

RestartIt is ideal for:
- **System Administrators** - Monitoring critical services and applications
- **Developers** - Keeping development services running
- **IT Departments** - Ensuring high availability of essential software
- **Home Users** - Maintaining media servers, backup tools, or other utilities
- **Remote Systems** - Monitoring headless or remote applications

## Known Limitations

- Windows-only (requires Windows 10 or later)
- Monitored programs must be executables (.exe files)
- Email notifications require SMTP server access
- Cannot monitor Windows services (use Windows Service Manager for that)

## Frequently Asked Questions (FAQ)

### General Questions

**Q: Can RestartIt monitor Windows services?**  
A: No, RestartIt only monitors executable (.exe) files. For Windows services, use the built-in Windows Service Manager or other service monitoring tools.

**Q: Does RestartIt work on Windows 7 or earlier?**  
A: No, RestartIt requires Windows 10 or later and .NET 8.0 Runtime.

**Q: Can I monitor programs on remote computers?**  
A: RestartIt runs locally and can only monitor programs on the same computer where it's installed. For remote monitoring, you would need to install RestartIt on each remote computer.

**Q: How many programs can I monitor at once?**  
A: There's no hard limit, but performance may degrade with a very large number of programs. Monitor only what you need to keep resource usage reasonable.

### Configuration Questions

**Q: Where are my settings stored?**  
A: Settings are stored in `%APPDATA%\RestartIt\config.json`. This is typically `C:\Users\[YourUsername]\AppData\Roaming\RestartIt\config.json`.

**Q: Can I backup my configuration?**  
A: Yes, simply copy the `config.json` file. Note that encrypted passwords are tied to your Windows user account and won't work on other computers.

**Q: How do I reset all settings?**  
A: Close RestartIt, delete or rename the `config.json` file, then restart the application. It will create a new default configuration.

**Q: Can I edit the config.json file directly?**  
A: Yes, but it's not recommended. Use the UI instead to avoid configuration errors. If you do edit manually, ensure the JSON is valid and restart the application.

### Email Notifications

**Q: Why aren't I receiving email notifications?**  
A: Check the following:
- Email notifications are enabled in Settings
- SMTP settings are correct (server, port, SSL/TLS)
- For Gmail, you're using an App Password, not your regular password
- Firewall isn't blocking SMTP ports (587 for TLS, 465 for SSL)
- Test email functionality works (use "Send Test Email" button)

**Q: Can I use multiple recipient email addresses?**  
A: Currently, RestartIt supports only one recipient email address. You can use email aliases or forwarding rules to send to multiple addresses.

**Q: Are my email passwords secure?**  
A: Yes, passwords are encrypted using Windows DPAPI (Data Protection API) and can only be decrypted by the same Windows user account that encrypted them.

### Logging Questions

**Q: Where are log files stored?**  
A: By default, logs are stored in `%APPDATA%\RestartIt\logs`. You can change this in Settings → Logging tab.

**Q: How long are logs kept?**  
A: By default, logs are kept for 30 days. You can configure this in Settings → Logging tab under "Keep Log Files For".

**Q: Can I view logs in real-time?**  
A: Yes, the main window displays a real-time activity log. You can also open log files directly in a text editor.

**Q: What's the difference between UI logs and file logs?**  
A: UI logs show recent activity in the application window (bounded buffer to prevent memory issues). File logs are persistent and stored on disk with full history.

### Performance Questions

**Q: Does RestartIt use a lot of system resources?**  
A: RestartIt is lightweight and uses minimal resources. CPU usage depends on the number of monitored programs and check intervals. Typical usage is less than 1% CPU and a few MB of RAM.

**Q: What if I have high CPU usage?**  
A: Try the following:
- Increase check intervals for monitored programs (check less frequently)
- Reduce the number of monitored programs
- Check if any programs are restarting repeatedly (indicates a problem with the monitored program)

**Q: Can I pause monitoring temporarily?**  
A: Yes, you can disable individual programs using the checkbox in the main window, or disable all monitoring by closing the application.

### Language Support

**Q: How do I add a new language?**  
A: Create a JSON file in the `Localization` folder following the format of existing language files. See the "Adding a New Language" section above for details.

**Q: Are all features translated?**  
A: Yes, all UI elements, dialogs, buttons, and log messages are fully localized in all 16 supported languages.

**Q: Can I contribute translations?**  
A: Yes! Please submit pull requests with new language files or improvements to existing translations.

## Troubleshooting

### "Windows protected your PC" Warning
**Issue:** Windows SmartScreen blocks the application when first running.

**Solution:**
1. Click **"More info"** on the warning dialog
2. Click **"Run anyway"**

This warning appears because RestartIt is not code-signed. The application is safe and open-source. To avoid this warning, you can build from source instead of downloading the pre-built release.

### Program Won't Restart
- Verify the executable path is correct
- Check that arguments and working directory are valid
- Ensure you have permissions to run the program
- Review the activity log for error messages

### Email Notifications Not Working
- Test your SMTP settings using the "Send Test Email" button
- For Gmail, ensure you're using an App Password
- Check firewall settings (port 587 for TLS, 465 for SSL)
- Verify recipient email address is correct

### High CPU Usage
- Increase check intervals for monitored programs
- Reduce the number of monitored programs
- Check for programs that restart repeatedly

## Contributing

Contributions are welcome! We appreciate your help in making RestartIt better.

### How to Contribute

1. **Fork the repository** on GitHub
2. **Create a feature branch** from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make your changes** following the coding standards below
4. **Test your changes** thoroughly
5. **Commit your changes** with clear, descriptive commit messages
6. **Push to your fork** and create a Pull Request

### Coding Standards

- **Code Style**: Follow existing code style and conventions
- **Documentation**: Add XML documentation comments to public classes and methods
- **Localization**: All user-facing strings must use the localization system (no hardcoded text)
- **Testing**: Test your changes on Windows 10/11 with .NET 8.0
- **Commits**: Write clear, descriptive commit messages

### Areas for Contribution

- **Translations**: Add new languages or improve existing translations
- **Bug Fixes**: Fix issues reported in the Issues section
- **Features**: Implement requested features (check existing issues first)
- **Documentation**: Improve README, code comments, or add examples
- **UI/UX**: Enhance the user interface or user experience

### Translation Contributions

To contribute a new language:

1. Copy an existing language file (e.g., `en.json`) from the `Localization` folder
2. Rename it to your language code (e.g., `pt.json` for Portuguese)
3. Update the `_metadata` section with your language information
4. Translate all the string values (keep the keys unchanged)
5. Test the translation in the application
6. Submit a pull request

**Translation Keys:**
- All keys starting with `App.*` are application-wide strings
- Keys starting with `Settings.*` are settings dialog strings
- Keys starting with `Log.*` are log messages
- Keys starting with `Email.*` are email notification strings
- Keys starting with `Validation.*` are validation error messages

### Reporting Issues

When reporting bugs or requesting features:

- **Use the issue templates** if available
- **Provide clear descriptions** of the problem or feature request
- **Include steps to reproduce** for bugs
- **Specify your environment** (Windows version, .NET version)
- **Check existing issues** to avoid duplicates

### Development Setup

1. **Prerequisites:**
   - Windows 10 or later
   - .NET 8.0 SDK
   - Visual Studio 2022 or Visual Studio Code

2. **Clone and Build:**
   ```bash
   git clone https://github.com/Antik79/RestartIt.git
   cd RestartIt
   dotnet restore
   dotnet build
   ```

3. **Run:**
   ```bash
   dotnet run --project RestartIt.csproj
   ```

### Code Structure

- **Models.cs**: Data models and configuration management
- **Services.cs**: Core services (monitoring, logging, email notifications)
- **MainWindow.xaml/cs**: Main application window
- **Dialogs.cs**: Settings and program edit dialogs
- **LocalizationService.cs**: Language and localization management
- **CredentialManager.cs**: Password encryption/decryption
- **PathValidator.cs**: File path validation utilities
- **IconHelper.cs**: Icon creation utilities

### Pull Request Guidelines

- **Keep PRs focused**: One feature or fix per pull request
- **Write clear descriptions**: Explain what and why, not just what
- **Reference issues**: Link to related issues if applicable
- **Update documentation**: Update README if needed
- **Test thoroughly**: Ensure your changes don't break existing functionality

Thank you for contributing to RestartIt! 🎉

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with ❤️ using .NET and WPF
- Icons and UI design inspired by modern Windows applications
- Thanks to all contributors and users!

## Changelog

For a detailed list of changes in each version, see [CHANGELOG.md](CHANGELOG.md).

### Recent Updates

**v1.3.2** (2025-01-27)
- **Catppuccin Themes**: Added support for 4 Catppuccin theme flavors (Latte, Frappe, Macchiato, Mocha)
- **Appearance Settings**: New tab for customizing fonts, colors, and theme presets with live preview
- **Apply Button**: Instant preview of theme and language changes without closing Settings dialog
- **UI Fixes**: Fixed color picker bug, improved DataGrid text visibility, and enhanced Settings dialog theming
- **Status Badge**: Improved text contrast with white text on highlight color background

**v1.3.1** (2025-01-27)
- **Code Documentation**: Added comprehensive XML documentation comments to all public classes and methods for better code maintainability

**v1.3.0** (2025-01-27)
- **UI Overhaul**: Complete redesign with Enhanced Custom Styling - modern card-based layout, gradient buttons, shadows, and improved spacing
- **Localization Improvements**: All hardcoded strings replaced with localization calls, English fallback support added
- **Security Enhancements**: SecureString for password handling, improved input validation
- **Bug Fixes**: Fixed process handle leaks, improved resource management

**v1.2.1** (2025-11-09)
- Fixed critical bug: Enable/disable monitor toggle not working for programs disabled at launch
- Fixed incomplete localization: All log messages now properly translate to selected language
- Added missing localization keys to all 16 language files

**v1.2.0** (2025-11-08)
- Added 12 new languages (total: 16 languages)
- Implemented modular language discovery system
- Added language metadata support with flag emojis

**v1.1.0** (2025-11-08)
- Initial multi-language support (4 languages)
- Real-time language switching
- Fully localized UI

## Support

If you encounter issues or have questions:
- Open an [Issue](https://github.com/Antik79/RestartIt/issues) on GitHub
- Check existing issues for solutions
- Review the troubleshooting section above
- Check the [CHANGELOG](CHANGELOG.md) for recent fixes

---

**RestartIt** - Keep your critical applications running.
