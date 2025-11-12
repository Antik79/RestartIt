# Configuration Guide

Advanced configuration guide for RestartIt, including manual configuration file editing and advanced settings.

## Table of Contents

1. [Configuration File Location](#configuration-file-location)
2. [Configuration File Structure](#configuration-file-structure)
3. [Program Configuration](#program-configuration)
4. [Logging Configuration](#logging-configuration)
5. [Notification Configuration](#notification-configuration)
6. [Application Settings](#application-settings)
7. [Manual Configuration](#manual-configuration)
8. [Backup and Restore](#backup-and-restore)
9. [Advanced Settings](#advanced-settings)
10. [Troubleshooting](#troubleshooting)

## Configuration File Location

RestartIt stores all settings in a JSON configuration file:

**Default Location:**
```
%APPDATA%\RestartIt\config.json
```

**Full Path Example:**
```
C:\Users\[YourUsername]\AppData\Roaming\RestartIt\config.json
```

### Finding Your Config File

1. **Using File Explorer:**
   - Press `Win + R`
   - Type `%APPDATA%\RestartIt`
   - Press Enter
   - Look for `config.json`

2. **Using Command Line:**
   ```powershell
   cd $env:APPDATA\RestartIt
   notepad config.json
   ```

## Configuration File Structure

The configuration file uses the following JSON structure:

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
      "LastRestartTime": "2025-01-27T10:30:00",
      "EnableTaskbarNotifications": true,
      "EnableFileLogging": false,
      "LogFilePath": "",
      "MinimumLogLevel": 1,
      "MaxLogFileSizeMB": 10,
      "KeepLogFilesForDays": 30
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
    "NotifyOnFailure": true,
    "NotifyOnStop": false,
    "EnableTaskbarNotifications": true,
    "TaskbarNotifyOnRestart": true,
    "TaskbarNotifyOnFailure": true,
    "TaskbarNotifyOnStop": false
  },
  "AppSettings": {
    "StartWithWindows": false,
    "MinimizeToTray": true,
    "StartMinimized": false,
    "MinimizeOnClose": false,
    "Language": "en",
    "ThemePreset": "Light",
    "FontFamily": "Segoe UI",
    "FontSize": 12.0,
    "BackgroundColor": "#F5F5F5",
    "TextColor": "#212121",
    "HighlightColor": "#0078D4",
    "BorderColor": "#E0E0E0",
    "SurfaceColor": "#FFFFFF",
    "SecondaryTextColor": "#757575",
    "ButtonTextColor": "#FFFFFF",
    "HeaderColor": "#FFFFFF"
  }
}
```

## Program Configuration

Each program in the `Programs` array has the following properties:

### Required Properties

- **ProgramName** (string) - Display name for the program
- **ExecutablePath** (string) - Full path to the executable file
- **CheckIntervalSeconds** (number) - How often to check if running (seconds)
- **RestartDelaySeconds** (number) - Wait time before restarting (seconds)
- **Enabled** (boolean) - Whether monitoring is enabled for this program

### Optional Properties

- **Arguments** (string) - Command-line arguments (empty string if none)
- **WorkingDirectory** (string) - Working directory (empty string if none)
- **Status** (string) - Current status: "Running", "Stopped", "Restarting", "Failed"
- **LastRestartTime** (string) - ISO 8601 timestamp of last restart (null if never restarted)

### Per-Program Logging Properties

- **EnableFileLogging** (boolean) - Enable individual log file for this program
- **LogFilePath** (string) - Custom log directory (empty uses global directory)
- **MinimumLogLevel** (number) - Log level: 0=Debug, 1=Info, 2=Warning, 3=Error
- **MaxLogFileSizeMB** (number) - Maximum log file size before rotation
- **KeepLogFilesForDays** (number) - How many days to keep log files

### Per-Program Notification Properties

- **EnableTaskbarNotifications** (boolean) - Enable taskbar notifications for this program

### Example Program Entry

```json
{
  "ProgramName": "My Server",
  "ExecutablePath": "C:\\Server\\server.exe",
  "Arguments": "--port 8080",
  "WorkingDirectory": "C:\\Server",
  "CheckIntervalSeconds": 30,
  "RestartDelaySeconds": 5,
  "Enabled": true,
  "Status": "Running",
  "LastRestartTime": "2025-01-27T10:30:00",
  "EnableTaskbarNotifications": true,
  "EnableFileLogging": true,
  "LogFilePath": "C:\\Server\\Logs",
  "MinimumLogLevel": 1,
  "MaxLogFileSizeMB": 50,
  "KeepLogFilesForDays": 7
}
```

## Logging Configuration

The `LogSettings` object controls global logging:

### Properties

- **LogFilePath** (string) - Directory where log files are stored
- **MinimumLogLevel** (number) - Minimum log level to record:
  - `0` = Debug (all messages)
  - `1` = Info (default, informational and above)
  - `2` = Warning (warnings and errors only)
  - `3` = Error (errors only)
- **EnableFileLogging** (boolean) - Enable/disable file logging
- **MaxLogFileSizeMB** (number) - Maximum log file size before rotation (default: 10)
- **KeepLogFilesForDays** (number) - How many days to keep old log files (default: 30)

### Example Log Settings

```json
{
  "LogSettings": {
    "LogFilePath": "C:\\Logs\\RestartIt",
    "MinimumLogLevel": 1,
    "EnableFileLogging": true,
    "MaxLogFileSizeMB": 20,
    "KeepLogFilesForDays": 60
  }
}
```

## Notification Configuration

The `NotificationSettings` object controls email and taskbar notifications:

### Email Notification Properties

- **EnableEmailNotifications** (boolean) - Enable/disable email notifications
- **SmtpServer** (string) - SMTP server address (e.g., "smtp.gmail.com")
- **SmtpPort** (number) - SMTP port (587 for TLS, 465 for SSL, 25 for unencrypted)
- **UseSSL** (boolean) - Enable SSL/TLS encryption
- **SenderEmail** (string) - Email address to send from
- **SenderName** (string) - Display name for sender
- **SenderPassword** (string) - Encrypted password (Base64 encoded, DPAPI encrypted)
- **RecipientEmail** (string) - Email address to send to
- **NotifyOnRestart** (boolean) - Notify on successful restart
- **NotifyOnFailure** (boolean) - Notify on restart failure
- **NotifyOnStop** (boolean) - Notify when program is detected as stopped

### Taskbar Notification Properties

- **EnableTaskbarNotifications** (boolean) - Enable/disable taskbar notifications globally
- **TaskbarNotifyOnRestart** (boolean) - Show taskbar notification on successful restart
- **TaskbarNotifyOnFailure** (boolean) - Show taskbar notification on restart failure
- **TaskbarNotifyOnStop** (boolean) - Show taskbar notification when program stops

### Password Encryption

**Important:** The `SenderPassword` field is encrypted using Windows DPAPI (Data Protection API). This means:
- Passwords are encrypted per Windows user account
- Encrypted passwords cannot be used on other computers
- Encrypted passwords cannot be decrypted by other users
- If you edit the config file manually, you'll need to re-enter the password in the UI

**To change password:**
1. Use the Settings UI to update the password
2. Don't try to manually edit the encrypted password field

## Application Settings

The `AppSettings` object controls application behavior and appearance:

### Behavior Properties

- **StartWithWindows** (boolean) - Launch at Windows startup
- **MinimizeToTray** (boolean) - Hide to system tray when minimized
- **StartMinimized** (boolean) - Launch minimized to tray
- **MinimizeOnClose** (boolean) - Minimize to tray when closing window
- **Language** (string) - Language code (e.g., "en", "de", "fr")

### Appearance Properties

- **ThemePreset** (string) - Theme name ("Light", "Dark", "Custom", or custom theme name)
- **FontFamily** (string) - Font family name (e.g., "Segoe UI")
- **FontSize** (number) - Font size in points (e.g., 12.0)

### Color Properties

All colors are in hexadecimal format (#RRGGBB):

- **BackgroundColor** (string) - Main window background
- **TextColor** (string) - Primary text color
- **HighlightColor** (string) - Accent color for buttons
- **BorderColor** (string) - Border color
- **SurfaceColor** (string) - Surface/card background
- **SecondaryTextColor** (string) - Secondary text color
- **ButtonTextColor** (string) - Button text color
- **HeaderColor** (string) - Header/toolbar background

### Example App Settings

```json
{
  "AppSettings": {
    "StartWithWindows": true,
    "MinimizeToTray": true,
    "StartMinimized": true,
    "MinimizeOnClose": true,
    "Language": "en",
    "ThemePreset": "Dark",
    "FontFamily": "Consolas",
    "FontSize": 11.0,
    "BackgroundColor": "#1E1E1E",
    "TextColor": "#E0E0E0",
    "HighlightColor": "#0078D4",
    "BorderColor": "#404040",
    "SurfaceColor": "#2D2D2D",
    "SecondaryTextColor": "#B0B0B0",
    "ButtonTextColor": "#FFFFFF",
    "HeaderColor": "#2D2D2D"
  }
}
```

## Manual Configuration

### When to Edit Manually

Manual editing is useful for:
- Bulk changes to multiple programs
- Advanced configurations not available in UI
- Backup/restore scenarios
- Troubleshooting configuration issues

### Precautions

⚠️ **Important Warnings:**

1. **Backup First**: Always backup `config.json` before editing
2. **Close RestartIt**: Don't edit while RestartIt is running
3. **Validate JSON**: Ensure JSON syntax is valid
4. **Don't Edit Passwords**: Don't manually edit encrypted password fields
5. **Test After Editing**: Verify configuration works after editing

### Editing Steps

1. **Close RestartIt** completely
2. **Backup config.json** to a safe location
3. **Open config.json** in a text editor (VS Code, Notepad++, etc.)
4. **Make your changes** carefully
5. **Validate JSON** using online validator
6. **Save the file**
7. **Start RestartIt** and verify settings

### JSON Validation

Before saving, validate your JSON:
- Use [jsonlint.com](https://jsonlint.com/)
- Check for missing commas
- Verify all strings are quoted
- Ensure proper bracket/brace matching
- Check for trailing commas

## Backup and Restore

### Backing Up Configuration

**Method 1: Copy File**
1. Navigate to `%APPDATA%\RestartIt`
2. Copy `config.json` to backup location
3. Keep backup in safe location

**Method 2: Export Programs**
1. Use RestartIt UI to add/remove programs
2. Configuration is automatically saved
3. Backup the entire `%APPDATA%\RestartIt` folder

### Restoring Configuration

**Method 1: Restore File**
1. Close RestartIt
2. Copy backup `config.json` to `%APPDATA%\RestartIt`
3. Replace existing file
4. Start RestartIt

**Method 2: Selective Restore**
1. Open backup `config.json`
2. Copy specific sections (e.g., Programs array)
3. Paste into current `config.json`
4. Validate JSON
5. Restart RestartIt

### Password Considerations

⚠️ **Important:** Encrypted passwords in backups are tied to your Windows user account:
- Backups work on the same computer and user account
- Backups won't work on different computers
- Backups won't work for different Windows users
- You'll need to re-enter passwords if restoring on different computer/user

## Advanced Settings

### Program Status Values

Valid status values:
- `"Running"` - Program is currently running
- `"Stopped"` - Program is not running
- `"Restarting"` - Program is being restarted
- `"Failed"` - Last restart attempt failed

**Note:** Status is automatically updated by RestartIt. Manual editing is not recommended.

### LastRestartTime Format

Uses ISO 8601 format:
```
YYYY-MM-DDTHH:mm:ss
```

Example: `"2025-01-27T10:30:00"`

Can be `null` if program has never been restarted.

### Log Level Values

- `0` - Debug: All messages including detailed diagnostics
- `1` - Info: General informational messages (default)
- `2` - Warning: Warning messages and errors
- `3` - Error: Error messages only

### Theme Preset Values

- `"Light"` - Default light theme
- `"Dark"` - Default dark theme
- `"Custom"` - Custom colors (not a saved theme)
- `"[ThemeName]"` - Name of a custom saved theme

## Troubleshooting

### Configuration File Not Found

**Issue**: RestartIt can't find config.json

**Solution:**
1. RestartIt will create a default config if file doesn't exist
2. Ensure `%APPDATA%\RestartIt` folder exists
3. Check file permissions
4. Verify disk space is available

### Configuration File Corrupted

**Issue**: RestartIt won't start, shows config error

**Solution:**
1. **Backup corrupted file** (for reference)
2. **Delete or rename** corrupted `config.json`
3. **Start RestartIt** (will create new default config)
4. **Reconfigure settings** manually
5. **Or restore from backup** if you have one

### Settings Not Persisting

**Issue**: Changes don't save

**Solution:**
1. Ensure you click "OK" or "Apply" in Settings dialog
2. Check file permissions on config.json
3. Verify disk space is available
4. Check if file is read-only
5. Review debug output for save errors

### Invalid JSON After Editing

**Issue**: RestartIt won't start after manual edit

**Solution:**
1. **Validate JSON** using online validator
2. **Check for syntax errors**:
   - Missing commas
   - Unclosed brackets/braces
   - Unquoted strings
   - Trailing commas
3. **Restore from backup** if needed
4. **Fix JSON syntax** and try again

### Password Field Issues

**Issue**: Password doesn't work after restoring config

**Solution:**
1. **Re-enter password** in Settings UI
2. Passwords are encrypted per user account
3. Encrypted passwords from backups only work on same computer/user
4. Use Settings UI to update password, don't edit manually

## Best Practices

1. **Regular Backups**: Backup config.json regularly
2. **Use UI When Possible**: Prefer UI over manual editing
3. **Validate JSON**: Always validate JSON after manual edits
4. **Test Changes**: Test configuration after making changes
5. **Document Custom Settings**: Note any custom configurations
6. **Version Control**: Consider version controlling your config (without passwords)

## Additional Resources

- [Developer Guide - Configuration Management](Developer-Guide.md#configuration-management)
- [Troubleshooting Guide](Troubleshooting-Guide.md#configuration-issues)
- [JSON Validator](https://jsonlint.com/)

---

For user-friendly configuration, use the Settings dialog in RestartIt. Manual editing is only recommended for advanced users.

