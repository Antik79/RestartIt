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

## Quick Start

### Installation

1. Download the latest release from the [Releases](https://github.com/Antik79/RestartIt/releases) page
2. Extract the ZIP file to your desired location
3. Run `RestartIt.exe`

**Requirements:** Windows 10 or later, .NET 8.0 Runtime ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))

### Windows SmartScreen Warning

When running RestartIt for the first time, you may see a **"Windows protected your PC"** message. This is normal for unsigned applications. Click **"More info"** then **"Run anyway"** to proceed. The application is safe and open-source.

## Basic Usage

1. **Add Programs**: Click **"➕ Add Program"** to monitor an application
2. **Configure Notifications**: Go to **Settings** → **Notifications** to set up email or taskbar alerts
3. **Customize Appearance**: Go to **Settings** → **Appearance** to change themes, colors, and fonts
4. **Change Language**: Go to **Settings** → **Application** to select from 17 available languages

For detailed instructions, see the [Documentation](#documentation) section below.

## Documentation

Comprehensive guides are available in the [`Docs/`](Docs/) folder:

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
A: Settings are stored in `%APPDATA%\RestartIt\config.json`. See the [Configuration Guide](Docs/Configuration-Guide.md) for details.

**Q: Can I contribute translations or themes?**  
A: Yes! Please see our [Translation Guide](Docs/Translation-Guide.md) and [Theming Guide](Docs/Theming-Guide.md).

## Support

If you encounter issues or have questions:
- Open an [Issue](https://github.com/Antik79/RestartIt/issues) on GitHub
- Check the [Troubleshooting Guide](Docs/Troubleshooting-Guide.md)
- Review the [CHANGELOG](CHANGELOG.md) for recent fixes

## Contributing

Contributions are welcome! See our [Developer Guide](Docs/Developer-Guide.md) for development setup, code style guidelines, and how to contribute.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**RestartIt** - Keep your critical applications running. 🚀
