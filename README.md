# RestartIt

**Version 1.0.0** | [Download Latest Release](https://github.com/Antik79/RestartIt/releases/latest)

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
- **Clean Modern UI** - Built with WPF for a professional appearance
- **System Tray Integration** - Runs quietly in the background
- **Easy Configuration** - Simple dialogs for adding and editing monitored programs
- **Per-Program Settings** - Individual check intervals and restart delays

### Security & Reliability
- **Password Encryption** - Email credentials encrypted using Windows DPAPI
- **Resource Management** - Proper disposal of process handles to prevent leaks
- **Memory Efficient** - Bounded log buffers prevent unbounded memory growth
- **Startup Integration** - Optional startup with Windows

## Screenshots

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

### Configuring Logging

1. Click **Settings** → **Logging** tab
2. Configure:
   - **Log File Path** - Where to store log files
   - **Minimum Log Level** - Debug, Info, Warning, or Error
   - **Enable File Logging** - Toggle file logging on/off
   - **Max Log File Size** - Maximum size before rotation (MB)
   - **Keep Log Files For** - How many days to retain logs

### Application Settings

1. Click **Settings** → **Application** tab
2. Configure:
   - **Start with Windows** - Launch at Windows startup
   - **Minimize to Tray** - Hide to system tray when minimized
   - **Start Minimized** - Launch minimized to tray

## Configuration

Settings are stored in `%APPDATA%\RestartIt\config.json`

### Configuration File Location
```
C:\Users\[YourUsername]\AppData\Roaming\RestartIt\config.json
```

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

## Troubleshooting

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

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with ❤️ using .NET and WPF
- Icons and UI design inspired by modern Windows applications
- Thanks to all contributors and users!

## Support

If you encounter issues or have questions:
- Open an [Issue](https://github.com/Antik79/RestartIt/issues) on GitHub
- Check existing issues for solutions
- Review the troubleshooting section above

---

**RestartIt** - Keep your critical applications running, automatically.
