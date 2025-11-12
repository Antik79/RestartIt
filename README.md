# RestartIt

**Version 1.4.0** | [Download Latest Release](https://github.com/Antik79/RestartIt/releases/latest)

**RestartIt** is a Windows desktop application that monitors specified programs and automatically restarts them if they stop running. Perfect for maintaining critical applications, services, or any software that needs to stay running 24/7.

## Features

- **Automatic Monitoring** - Continuously monitors configured applications at customizable intervals
- **Smart Restart** - Automatically relaunches stopped programs with configurable delay
- **Email & Taskbar Notifications** - Get notified when programs restart or fail
- **Comprehensive Logging** - Global and per-program logging with automatic rotation
- **Customizable Themes** - Create and save custom themes with full color and font control
- **Multi-Language Support** - Available in 17 languages with real-time switching
- **System Tray Integration** - Runs quietly in the background
- **Easy Configuration** - Simple dialogs for adding and editing monitored programs

## Quick Start

### Installation

1. Download the latest release from the [Releases](https://github.com/Antik79/RestartIt/releases) page
2. Extract the ZIP file to your desired location
3. Run `RestartIt.exe`

**Requirements:** Windows 10 or later, .NET 8.0 Runtime ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))

### Windows SmartScreen Warning

When running RestartIt for the first time, you may see a **"Windows protected your PC"** message. This is normal for applications without a code signing certificate.

**To run the application:**
1. Click **"More info"**
2. Click **"Run anyway"**

This warning appears because RestartIt is not code-signed. The application is safe and open-source. To avoid this warning, you can [build from source](https://github.com/Antik79/RestartIt/wiki/Developer-Guide#development-setup).

## Basic Usage

### Adding a Program to Monitor

1. Click the **"➕ Add Program"** button
2. Browse to select the executable file (.exe)
3. Configure settings (name, check interval, restart delay, etc.)
4. Click **OK** to add

### Configuring Notifications

Go to **Settings** → **Notifications** tab to configure:
- **Email Notifications** - SMTP-based email alerts
- **Taskbar Notifications** - Windows balloon tip notifications

**Note for Gmail users:** You'll need to use an [App Password](https://support.google.com/accounts/answer/185833) instead of your regular password.

### Changing Language

1. Open **Settings** → **Application** tab
2. Select your preferred language from the dropdown
3. Click **OK** to apply

RestartIt supports **17 languages** including English, Spanish, French, German, Italian, Japanese, Korean, Chinese, and more.

### Customizing Appearance

Go to **Settings** → **Appearance** tab to:
- Select a theme preset (Light, Dark, or custom themes)
- Customize colors, fonts, and appearance
- Save your custom theme for reuse

## Documentation

For detailed information, see the guides in the [`Docs/`](Docs/) folder:

- **[Developer Guide](Docs/Developer-Guide.md)** - Architecture, development setup, and contributing guidelines
- **[Troubleshooting Guide](Docs/Troubleshooting-Guide.md)** - Common issues and solutions
- **[Theming Guide](Docs/Theming-Guide.md)** - Creating and customizing themes
- **[Translation Guide](Docs/Translation-Guide.md)** - Adding new languages
- **[Configuration Guide](Docs/Configuration-Guide.md)** - Advanced configuration options

These guides are also available on our [GitHub Wiki](https://github.com/Antik79/RestartIt/wiki).

## Frequently Asked Questions

**Q: Can RestartIt monitor Windows services?**  
A: No, RestartIt only monitors executable (.exe) files. For Windows services, use the built-in Windows Service Manager.

**Q: Does RestartIt work on Windows 7 or earlier?**  
A: No, RestartIt requires Windows 10 or later and .NET 8.0 Runtime.

**Q: Where are my settings stored?**  
A: Settings are stored in `%APPDATA%\RestartIt\config.json`.

**Q: How do I backup my configuration?**  
A: Simply copy the `config.json` file. Note that encrypted passwords are tied to your Windows user account.

**Q: Can I contribute translations or themes?**  
A: Yes! Please see our [Translation Guide](Docs/Translation-Guide.md) and [Theming Guide](Docs/Theming-Guide.md).

## Support

If you encounter issues or have questions:
- Open an [Issue](https://github.com/Antik79/RestartIt/issues) on GitHub
- Check the [Troubleshooting Guide](Docs/Troubleshooting-Guide.md)
- Review the [CHANGELOG](CHANGELOG.md) for recent fixes

## Contributing

Contributions are welcome! See our [Developer Guide](Docs/Developer-Guide.md) for:
- Development setup
- Code style guidelines
- How to contribute features, translations, or bug fixes

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**RestartIt** - Keep your critical applications running. 🚀
