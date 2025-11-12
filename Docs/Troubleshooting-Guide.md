# Troubleshooting Guide

This guide helps you resolve common issues with RestartIt.

## Table of Contents

1. [Installation Issues](#installation-issues)
2. [Program Monitoring Issues](#program-monitoring-issues)
3. [Email Notification Issues](#email-notification-issues)
4. [Taskbar Notification Issues](#taskbar-notification-issues)
5. [Logging Issues](#logging-issues)
6. [Theme Issues](#theme-issues)
7. [Language Issues](#language-issues)
8. [Performance Issues](#performance-issues)
9. [Configuration Issues](#configuration-issues)
10. [General Issues](#general-issues)

## Installation Issues

### "Windows protected your PC" Warning

**Issue:** Windows SmartScreen blocks the application when first running.

**Solution:**
1. Click **"More info"** on the warning dialog
2. Click **"Run anyway"**

**Why does this happen?**
- RestartIt is not code-signed (certificates cost money for open-source projects)
- Windows SmartScreen warns about unsigned applications as a security precaution
- The application is safe - all source code is available on GitHub for review

**Alternative:** Build from source (see [Developer Guide](Developer-Guide#development-setup)) to avoid this warning entirely.

### .NET 8.0 Runtime Not Found

**Issue:** Application won't start, shows error about missing .NET runtime.

**Solution:**
1. Download and install .NET 8.0 Runtime from [Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Choose the "Desktop Runtime" version for Windows x64
3. Restart your computer after installation
4. Try running RestartIt again

**Verify Installation:**
```powershell
dotnet --version
```
Should show version 8.0.x or higher.

## Program Monitoring Issues

### Program Won't Restart

**Symptoms:**
- Program shows as "Stopped" but doesn't restart
- Status shows "Failed" after restart attempt
- Activity log shows error messages

**Troubleshooting Steps:**

1. **Verify Executable Path:**
   - Check that the path is correct and the file exists
   - Ensure the path doesn't contain typos
   - Try browsing to the file again using the "Browse..." button

2. **Check Permissions:**
   - Ensure you have permission to run the program
   - Try running the program manually to verify it works
   - Check if antivirus is blocking the program

3. **Validate Arguments:**
   - If using command-line arguments, verify they're correct
   - Test the arguments by running the program manually with them
   - Check for special characters that might need escaping

4. **Check Working Directory:**
   - Verify the working directory exists
   - Ensure the program can access files from that directory
   - Some programs require a specific working directory

5. **Review Activity Log:**
   - Check the activity log for specific error messages
   - Look for "ValidationFailed", "WorkingDirectoryInvalid", or "ArgumentsInvalid" messages
   - Export logs for detailed analysis

**Common Error Messages:**

- **"Executable path is required"** - Path field is empty
- **"File not found"** - Executable doesn't exist at specified path
- **"Path is directory"** - Path points to a folder, not a file
- **"Access denied"** - Insufficient permissions to run the program
- **"Working directory invalid"** - Working directory doesn't exist

### Program Restarts Repeatedly

**Symptoms:**
- Program restarts immediately after starting
- Status alternates between "Running" and "Restarting"
- High CPU usage

**Possible Causes:**

1. **Program Crashes Immediately:**
   - The program starts but crashes right away
   - Check if the program runs correctly when started manually
   - Review program's own logs if available

2. **Process Name Mismatch:**
   - RestartIt checks both process name and executable path
   - If the process name doesn't match, it may not detect the program is running
   - Verify the process name in Task Manager matches what's configured

3. **Check Interval Too Short:**
   - If check interval is too short, RestartIt may check before the program fully starts
   - Increase the check interval (e.g., from 5 seconds to 30 seconds)
   - Some programs need time to initialize

**Solution:**
1. Increase the check interval to give the program more time to start
2. Verify the program runs correctly when started manually
3. Check the program's own error logs
4. Review RestartIt's activity log for specific error messages

### Program Not Detected as Running

**Symptoms:**
- Program is actually running (visible in Task Manager)
- RestartIt shows it as "Stopped"
- RestartIt tries to restart an already-running program

**Possible Causes:**

1. **Process Name Mismatch:**
   - The process name in Task Manager doesn't match what's configured
   - Process name might be different from executable name
   - Check Task Manager → Details tab for exact process name

2. **Multiple Instances:**
   - Multiple instances of the program running
   - RestartIt may be checking for a different instance
   - Verify which process instance RestartIt should monitor

3. **Path Mismatch:**
   - Executable path doesn't match the running process
   - Program might be running from a different location
   - Verify the exact path of the running process

**Solution:**
1. Check Task Manager → Details tab for exact process name
2. Verify the executable path matches the running process
3. If multiple instances exist, ensure RestartIt monitors the correct one
4. Try stopping and restarting the program manually

## Email Notification Issues

### Not Receiving Email Notifications

**Symptoms:**
- Email notifications enabled but no emails received
- Test email fails to send
- No error messages shown

**Troubleshooting Steps:**

1. **Verify Email Settings:**
   - Email notifications are enabled in Settings
   - SMTP server address is correct
   - SMTP port is correct (587 for TLS, 465 for SSL, 25 for unencrypted)
   - SSL/TLS is enabled if required by your email provider

2. **Check Credentials:**
   - Sender email address is correct
   - Password is correct (for Gmail, use App Password)
   - Recipient email address is correct

3. **Gmail-Specific Issues:**
   - **Use App Password:** Gmail requires an [App Password](https://support.google.com/accounts/answer/185833) instead of your regular password
   - **Enable 2-Step Verification:** App passwords require 2-step verification to be enabled
   - **SMTP Settings:** Use `smtp.gmail.com`, port `587`, with TLS enabled

4. **Firewall/Network Issues:**
   - Check if firewall is blocking SMTP ports (587, 465, 25)
   - Verify network connection is active
   - Some corporate networks block SMTP ports

5. **Test Email:**
   - Use the "Send Test Email" button in Settings
   - Check for error messages in the dialog
   - Review the error message for specific issues

**Common Error Messages:**

- **"SMTP server required"** - SMTP server field is empty
- **"Invalid SMTP port"** - Port number is invalid or out of range
- **"Authentication failed"** - Wrong email or password
- **"Connection timeout"** - Cannot connect to SMTP server (firewall/network issue)
- **"SSL/TLS required"** - Server requires secure connection but it's not enabled

### Test Email Fails

**Symptoms:**
- "Send Test Email" button shows error
- Error dialog appears with details

**Solution:**
1. Review the error message in the dialog
2. Check the [Email Notification Issues](#email-notification-issues) section above
3. Verify all SMTP settings are correct
4. For Gmail, ensure you're using an App Password
5. Check firewall settings

**Error Message Format:**
The test email dialog shows detailed error information including:
- Connection errors
- Authentication failures
- SSL/TLS issues
- Common solutions

## Taskbar Notification Issues

### Notifications Not Appearing

**Symptoms:**
- Taskbar notifications enabled but not showing
- No balloon tips appear

**Troubleshooting Steps:**

1. **Verify Settings:**
   - Taskbar notifications are enabled globally in Settings → Notifications
   - Per-program taskbar notifications are enabled (if applicable)
   - Notification preferences are configured (restart success, failure, stop/crash)

2. **Check Windows Settings:**
   - Windows notifications are enabled for RestartIt
   - Go to Windows Settings → System → Notifications
   - Ensure "Get notifications from apps and other senders" is enabled

3. **Focus Assist:**
   - Windows Focus Assist may be blocking notifications
   - Check Windows Settings → System → Focus Assist
   - Temporarily disable Focus Assist to test

4. **Notification History:**
   - Check Windows notification center (click notification icon in system tray)
   - Notifications may have been dismissed automatically
   - Review notification history

**Solution:**
1. Enable taskbar notifications in RestartIt Settings
2. Check Windows notification settings
3. Disable Focus Assist temporarily
4. Test by manually stopping a monitored program

## Logging Issues

### Log Files Not Created

**Symptoms:**
- File logging enabled but no log files appear
- Log directory is empty

**Troubleshooting Steps:**

1. **Verify Settings:**
   - File logging is enabled in Settings → Logging
   - Log file path/directory is specified and valid
   - Check that the directory exists and is writable

2. **Check Permissions:**
   - Ensure RestartIt has write permissions to the log directory
   - Try creating a test file in the directory manually
   - Some directories require administrator permissions

3. **Verify Path:**
   - Check that the log file path is valid
   - Ensure the directory exists (RestartIt won't create parent directories)
   - Use absolute paths instead of relative paths

4. **Check Disk Space:**
   - Ensure there's enough disk space
   - Log files require free space to write

**Solution:**
1. Verify log directory exists and is writable
2. Check file permissions
3. Ensure sufficient disk space
4. Try a different log directory (e.g., `C:\Logs\RestartIt`)

### Per-Program Logs Not Created

**Symptoms:**
- Per-program logging enabled but no individual log files
- Only global logs are created

**Troubleshooting Steps:**

1. **Verify Per-Program Settings:**
   - Per-program file logging is enabled in Program Edit dialog
   - Log file directory is specified (or uses global directory)
   - Minimum log level is set appropriately

2. **Check Program Status:**
   - Program must be monitored and running to generate logs
   - Enable monitoring for the program
   - Verify the program is actually running

3. **Review Global Logs:**
   - Per-program log events also appear in global logs
   - Check global logs to see if events are being logged
   - Per-program logs are only created when events occur

**Solution:**
1. Enable per-program logging in Program Edit dialog
2. Ensure the program is being monitored
3. Verify log directory settings
4. Check global logs to confirm events are being logged

### Log Files Too Large

**Symptoms:**
- Log files grow very large
- Disk space running out

**Solution:**
1. **Configure Log Rotation:**
   - Set "Max Log File Size" in Settings → Logging
   - Logs will rotate when size limit is reached
   - Default is 10 MB

2. **Configure Retention:**
   - Set "Keep Log Files For" in Settings → Logging
   - Old log files are automatically deleted
   - Default is 30 days

3. **Reduce Log Level:**
   - Change "Minimum Log Level" to Warning or Error
   - This reduces the number of log entries
   - Debug and Info messages won't be logged

4. **Manual Cleanup:**
   - Manually delete old log files
   - Use "Open Log Folder" button in Settings to access logs
   - Delete files older than needed

## Theme Issues

### Theme Not Applying

**Symptoms:**
- Selected theme doesn't change appearance
- Colors don't update when theme is selected

**Troubleshooting Steps:**

1. **Verify Theme File:**
   - Theme file exists in `Themes/` folder
   - Theme file is valid JSON
   - Check for JSON syntax errors

2. **Check Theme Selection:**
   - Theme is selected in Settings → Appearance → Theme Preset
   - Click "Apply" or "OK" to apply changes
   - Some changes require clicking Apply

3. **Custom Theme Issues:**
   - If using "Custom", colors must be set manually
   - Changing a color automatically switches to "Custom"
   - Select a theme preset to apply theme colors

4. **Restart Application:**
   - Some theme changes may require restart
   - Close and reopen RestartIt
   - Themes are loaded on startup

**Solution:**
1. Verify theme file is valid JSON
2. Select theme and click Apply
3. Restart application if needed
4. Check debug output for theme loading errors

### Custom Theme Not Saving

**Symptoms:**
- "Save Theme" button doesn't work
- Theme doesn't appear in dropdown after saving

**Troubleshooting Steps:**

1. **Check Theme Name:**
   - Theme name is required and cannot be empty
   - Theme name must be unique
   - Default themes (Light, Dark) cannot be overwritten

2. **Verify Permissions:**
   - Ensure RestartIt has write permissions to Themes folder
   - Check if Themes folder exists
   - Verify file isn't locked by another process

3. **Check for Errors:**
   - Error dialog should appear if save fails
   - Review error message for details
   - Check debug output for additional information

**Solution:**
1. Provide a unique theme name
2. Check file permissions
3. Review error messages
4. Try saving to a different location first

### Theme File Invalid

**Symptoms:**
- Theme doesn't load
- Error message about invalid theme

**Troubleshooting Steps:**

1. **Validate JSON:**
   - Check JSON syntax (commas, brackets, quotes)
   - Use a JSON validator online
   - Ensure all required fields are present

2. **Check Required Fields:**
   - `name` field is required
   - `colors` object is required
   - All color properties must be present

3. **Verify Color Format:**
   - Colors must be in hex format: `#RRGGBB` or `#AARRGGBB`
   - Colors must start with `#`
   - Invalid colors will cause theme to fail loading

**Solution:**
1. Validate JSON syntax
2. Ensure all required fields are present
3. Verify color formats are correct
4. Check debug output for specific error messages

## Language Issues

### Language Not Changing

**Symptoms:**
- Language selected but UI doesn't update
- Some text remains in English

**Troubleshooting Steps:**

1. **Apply Changes:**
   - Click "Apply" or "OK" button after selecting language
   - Language changes apply immediately
   - No restart required

2. **Verify Language File:**
   - Language file exists in `Localization/` folder
   - Language file is valid JSON
   - Check for JSON syntax errors

3. **Check Missing Keys:**
   - Some UI elements may fall back to English if translation key is missing
   - Check language file for missing keys
   - Compare with `en.json` to find missing translations

**Solution:**
1. Click Apply or OK after selecting language
2. Verify language file is valid
3. Check for missing translation keys
4. Report missing translations via GitHub Issues

### Language File Not Loading

**Symptoms:**
- Language doesn't appear in dropdown
- Error when selecting language

**Troubleshooting Steps:**

1. **Verify File Exists:**
   - Language file must be in `Localization/` folder
   - File name must match language code (e.g., `pt.json` for Portuguese)
   - File must have `.json` extension

2. **Check Metadata:**
   - Language file must have `_metadata` section
   - Metadata must include `code`, `name`, `nativeName`
   - Verify metadata format is correct

3. **Validate JSON:**
   - Language file must be valid JSON
   - Check for syntax errors
   - Use JSON validator if needed

**Solution:**
1. Verify file exists and is named correctly
2. Check JSON syntax
3. Ensure metadata section is present
4. Compare with existing language files

## Performance Issues

### High CPU Usage

**Symptoms:**
- RestartIt uses high CPU (10%+)
- Computer becomes slow
- Task Manager shows high CPU for RestartIt

**Possible Causes:**

1. **Too Many Programs:**
   - Monitoring many programs simultaneously
   - Each program is checked at its interval
   - More programs = more CPU usage

2. **Short Check Intervals:**
   - Very short check intervals (e.g., 5 seconds)
   - Frequent process checks consume CPU
   - Multiple programs with short intervals compound the issue

3. **Program Restart Loop:**
   - Program keeps crashing and restarting
   - RestartIt continuously tries to restart
   - Creates high CPU usage

**Solution:**
1. **Increase Check Intervals:**
   - Change check intervals to 30-60 seconds or longer
   - Less frequent checks = lower CPU usage
   - Most programs don't need checking every 5 seconds

2. **Reduce Number of Programs:**
   - Monitor only essential programs
   - Disable monitoring for programs that don't need it
   - Use Windows Task Scheduler for less critical programs

3. **Fix Restart Loop:**
   - Identify program that's restarting repeatedly
   - Check program's own logs for crash reasons
   - Fix the underlying issue with the program
   - Temporarily disable monitoring for that program

4. **Check Activity Log:**
   - Review activity log for programs restarting frequently
   - Look for error patterns
   - Address the root cause

### High Memory Usage

**Symptoms:**
- RestartIt uses excessive memory
- Memory usage grows over time

**Possible Causes:**

1. **Large Activity Log:**
   - Activity log buffer may grow if many events occur
   - Log buffer is bounded but can still use memory
   - Clear log periodically

2. **Many Log Files:**
   - Multiple per-program log files open
   - Each logger uses some memory
   - Old loggers not disposed properly

**Solution:**
1. **Clear Activity Log:**
   - Click "Clear Log" button periodically
   - Activity log is bounded but clearing helps
   - Export logs before clearing if needed

2. **Reduce Logging:**
   - Increase minimum log level to reduce log entries
   - Disable per-program logging for non-critical programs
   - Reduce log retention period

3. **Restart Application:**
   - Restart RestartIt periodically to free memory
   - Memory leaks are rare but restart helps
   - Schedule restart during maintenance windows

## Configuration Issues

### Settings Not Saving

**Symptoms:**
- Changes to settings don't persist
- Settings revert after restart

**Troubleshooting Steps:**

1. **Verify Save:**
   - Click "OK" or "Apply" button in Settings dialog
   - "Applied!" message should appear when using Apply
   - Settings are saved when dialog closes with OK

2. **Check Permissions:**
   - Ensure RestartIt has write permissions to `%APPDATA%\RestartIt`
   - Check if config.json file is read-only
   - Verify disk space is available

3. **Verify Config File:**
   - Check if `config.json` exists in `%APPDATA%\RestartIt`
   - Verify file isn't corrupted
   - Check file permissions

**Solution:**
1. Ensure you click OK or Apply
2. Check file permissions
3. Verify config.json isn't corrupted
4. Check debug output for save errors

### Configuration File Corrupted

**Symptoms:**
- Application won't start
- Error message about configuration
- Settings are lost

**Solution:**
1. **Backup Current Config:**
   - Copy `config.json` to a backup location
   - Located in `%APPDATA%\RestartIt\config.json`

2. **Delete Corrupted Config:**
   - Close RestartIt
   - Delete or rename `config.json`
   - Restart RestartIt (will create new default config)

3. **Restore Settings:**
   - Manually reconfigure settings
   - Or try to fix JSON syntax in config file
   - Use a JSON validator to find errors

4. **Prevent Future Issues:**
   - Don't edit config.json manually while RestartIt is running
   - Always use the UI to change settings
   - Backup config.json regularly

## General Issues

### Application Won't Start

**Symptoms:**
- Double-clicking RestartIt.exe does nothing
- Application crashes immediately
- Error dialog appears

**Troubleshooting Steps:**

1. **Check .NET Runtime:**
   - Verify .NET 8.0 Runtime is installed
   - Download from [Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Restart computer after installation

2. **Check Event Viewer:**
   - Open Windows Event Viewer
   - Look for application errors
   - Review error details

3. **Check Antivirus:**
   - Antivirus may be blocking the application
   - Add RestartIt to antivirus exclusions
   - Temporarily disable antivirus to test

4. **Check Dependencies:**
   - Ensure all required files are present
   - Don't delete files from the RestartIt folder
   - Re-download if files are missing

**Solution:**
1. Install .NET 8.0 Runtime
2. Check Event Viewer for errors
3. Add to antivirus exclusions
4. Re-download if needed

### System Tray Icon Not Appearing

**Symptoms:**
- RestartIt is running but no tray icon
- Cannot access tray menu

**Troubleshooting Steps:**

1. **Check System Tray Settings:**
   - Windows may be hiding the icon
   - Click the up arrow in system tray
   - Look for RestartIt icon in hidden icons

2. **Windows Notification Area:**
   - Go to Windows Settings → System → Notifications
   - Check "Select which icons appear on the taskbar"
   - Enable RestartIt icon

3. **Restart Application:**
   - Close and restart RestartIt
   - Icon should appear after restart
   - Check if icon appears briefly then disappears

**Solution:**
1. Check hidden icons in system tray
2. Enable icon in Windows notification settings
3. Restart RestartIt
4. Check if other tray icons work (may be Windows issue)

### Getting More Help

If you've tried the solutions above and still have issues:

1. **Check GitHub Issues:**
   - Search existing issues for similar problems
   - Check if issue is already reported
   - Review solutions in closed issues

2. **Create New Issue:**
   - Provide detailed description of the problem
   - Include steps to reproduce
   - Attach log files (export logs first)
   - Specify Windows version and .NET version

3. **Export Logs:**
   - Use "Export Logs" button to create log file
   - Attach log file to GitHub issue
   - Include relevant time period

4. **Check Debug Output:**
   - Run RestartIt from command line to see debug messages
   - Use DebugView (Sysinternals) to capture debug output
   - Include debug output in issue report

---

For more information, see:
- [Developer Guide](Developer-Guide) - Technical details and debugging
- [Configuration Guide](Configuration-Guide) - Advanced configuration
- [GitHub Issues](https://github.com/Antik79/RestartIt/issues) - Report bugs or ask questions

