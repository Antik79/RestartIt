# Developer Guide

This guide provides comprehensive information for developers who want to understand, modify, or contribute to RestartIt.

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Project Structure](#project-structure)
4. [Key Components](#key-components)
5. [Development Setup](#development-setup)
6. [Building and Running](#building-and-running)
7. [Code Style and Conventions](#code-style-and-conventions)
8. [Adding Features](#adding-features)
9. [Localization System](#localization-system)
10. [Theming System](#theming-system)
11. [Configuration Management](#configuration-management)
12. [Logging System](#logging-system)
13. [Testing](#testing)
14. [Debugging](#debugging)
15. [Contributing](#contributing)

## Project Overview

RestartIt is a Windows desktop application built with .NET 8.0 and WPF that monitors specified programs and automatically restarts them if they stop running. The application features:

- Real-time process monitoring
- Automatic program restart
- Email and taskbar notifications
- Comprehensive logging (global and per-program)
- Multi-language support (17 languages)
- Customizable theming system
- System tray integration

## Architecture

RestartIt follows a service-oriented architecture with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                    MainWindow (UI)                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Dialogs    │  │  DataGrid    │  │  ActivityLog │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
┌───────▼──────┐ ┌──────▼──────┐ ┌─────▼──────┐
│ Configuration│ │   Services  │ │ Localization│
│   Manager    │ │   Manager   │ │   Service   │
└──────────────┘ └──────────────┘ └─────────────┘
        │               │               │
        └───────────────┼───────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
┌───────▼──────┐ ┌──────▼──────┐ ┌─────▼──────┐
│ Process      │ │   Logger    │ │   Theme    │
│ Monitor      │ │   Service   │ │   Service  │
└──────────────┘ └──────────────┘ └─────────────┘
```

### Key Design Patterns

- **Singleton Pattern**: Used for services (LocalizationService, ThemeService, ThemeManager)
- **Observer Pattern**: Event-driven updates for UI (LanguageChanged, PropertyChanged)
- **Service Pattern**: Core functionality encapsulated in service classes
- **Repository Pattern**: ConfigurationManager handles data persistence

## Project Structure

```
RestartIt/
├── MainWindow.xaml/cs          # Main application window
├── Dialogs.cs                   # Settings and program edit dialogs
├── Models.cs                    # Data models and ConfigurationManager
├── Services.cs                  # Core services (monitoring, logging, email)
├── LocalizationService.cs       # Language and localization management
├── ThemeService.cs              # Theme application service
├── ThemeManager.cs              # Theme file management
├── CredentialManager.cs         # Password encryption/decryption
├── PathValidator.cs             # File path validation utilities
├── IconHelper.cs                # Icon creation utilities
├── Localization/                # Language JSON files
│   ├── en.json
│   ├── de.json
│   └── ...
├── Themes/                      # Theme JSON files
│   ├── Light.json
│   ├── Dark.json
│   └── ...
├── README.md                    # User documentation
├── CHANGELOG.md                 # Version history
└── LICENSE                      # MIT License
```

## Key Components

### MainWindow.xaml/cs

The main application window containing:
- DataGrid for displaying monitored programs
- Activity log panel
- Toolbar with action buttons
- System tray integration
- Event handlers for UI interactions

**Key Methods:**
- `InitializeSystemTray()` - Sets up system tray icon and menu
- `UpdateTrayContextMenu()` - Updates tray menu with current program list
- `LoadConfiguration()` - Loads saved configuration on startup
- `OnLanguageChanged()` - Updates UI when language changes

### Dialogs.cs

Contains all dialog windows:
- `ProgramEditDialog` - Add/edit monitored programs
- `SettingsDialog` - Application settings (logging, notifications, appearance)

**Key Features:**
- Dynamic UI generation
- Real-time validation
- Theme integration
- Localization support

### Models.cs

Data models and configuration management:
- `MonitoredProgram` - Represents a monitored program
- `AppSettings` - Application settings
- `LogSettings` - Logging configuration
- `NotificationSettings` - Email and taskbar notification settings
- `ThemeDefinition` - Theme file structure
- `ConfigurationManager` - Handles loading/saving configuration

**Key Methods:**
- `ConfigurationManager.LoadConfiguration()` - Loads config from JSON
- `ConfigurationManager.SaveConfiguration()` - Saves config to JSON
- `ConfigurationManager.MigrateThemePreset()` - Migrates old theme names

### Services.cs

Core application services:

#### ProcessMonitorService

Monitors programs and handles restarts:
- `StartMonitoring()` - Begins monitoring all enabled programs
- `StopMonitoring()` - Stops all monitoring
- `IsProcessRunning()` - Checks if a process is running
- `RestartProgram()` - Restarts a stopped program
- `GetLoggerForProgram()` - Returns appropriate logger (global or per-program)

#### LoggerService

Handles logging functionality:
- `Log()` - Writes log messages
- `InitializeLogFile()` - Sets up log file
- `CheckLogRotation()` - Handles log file rotation
- `CleanupOldLogFiles()` - Removes old log files

#### EmailNotificationService

Sends email notifications:
- `SendEmail()` - Sends email via SMTP
- `SendTestEmail()` - Sends test email

### LocalizationService.cs

Manages language and translations:
- `LoadLanguage()` - Loads translations for a language
- `GetString()` - Gets translated string for a key
- `GetAvailableLanguages()` - Returns list of available languages

**Translation Key Format:**
- `App.*` - Application-wide strings
- `Settings.*` - Settings dialog strings
- `Log.*` - Log messages
- `Email.*` - Email notification strings
- `Validation.*` - Validation error messages

### ThemeService.cs

Applies themes to the application:
- `ApplyTheme()` - Applies theme settings to UI resources
- `CalculateButtonTextColor()` - Calculates optimal button text color

### ThemeManager.cs

Manages theme files:
- `GetThemes()` - Returns all available themes
- `GetTheme()` - Gets a specific theme by name
- `SaveTheme()` - Saves a theme to JSON file
- `DeleteTheme()` - Deletes a theme file
- `IsDefaultTheme()` - Checks if a theme is a default theme

## Development Setup

### Prerequisites

- **Windows 10 or later**
- **.NET 8.0 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Visual Studio 2022** or **Visual Studio Code** (recommended)
- **Git** (for version control)

### Initial Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Antik79/RestartIt.git
   cd RestartIt
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run --project RestartIt.csproj
   ```

## Building and Running

### Debug Build

```bash
dotnet build --configuration Debug
```

Output: `bin/Debug/net8.0-windows/RestartIt.exe`

### Release Build

```bash
dotnet build --configuration Release
```

Output: `bin/Release/net8.0-windows/RestartIt.exe`

### Running Tests

Currently, RestartIt doesn't have automated unit tests. Manual testing is performed by:
1. Running the application
2. Testing each feature manually
3. Checking logs for errors

**Future Improvement:** Add unit tests using xUnit or NUnit.

## Code Style and Conventions

### Naming Conventions

- **Classes**: PascalCase (e.g., `ProcessMonitorService`)
- **Methods**: PascalCase (e.g., `StartMonitoring()`)
- **Properties**: PascalCase (e.g., `ProgramName`)
- **Fields**: camelCase with underscore prefix (e.g., `_programs`)
- **Constants**: PascalCase (e.g., `MaxLogFileSize`)
- **Local Variables**: camelCase (e.g., `programName`)

### Code Organization

- **One class per file** (except related classes like Models.cs)
- **XML documentation** for all public classes and methods
- **Regions** for organizing large files (e.g., `#region Properties`)

### Example Code Structure

```csharp
namespace RestartIt
{
    /// <summary>
    /// Brief description of the class.
    /// </summary>
    public class ExampleService
    {
        private readonly string _field;

        /// <summary>
        /// Brief description of the property.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Brief description of the method.
        /// </summary>
        /// <param name="parameter">Parameter description</param>
        /// <returns>Return value description</returns>
        public string Method(string parameter)
        {
            // Implementation
        }
    }
}
```

## Adding Features

### Adding a New Setting

1. **Add property to model** (`Models.cs`):
   ```csharp
   public class AppSettings
   {
       public bool NewSetting { get; set; } = false;
   }
   ```

2. **Add UI control** (`Dialogs.cs` - `SettingsDialog`):
   ```csharp
   var checkBox = new CheckBox
   {
       Content = LocalizationService.Instance.GetString("Settings.App.NewSetting"),
       IsChecked = _appSettings.NewSetting
   };
   ```

3. **Add localization keys** to all language files:
   ```json
   {
     "Settings.App.NewSetting": "New Setting",
     "Settings.App.NewSettingDesc": "Description of new setting"
   }
   ```

4. **Save setting** in `SaveAllSettings()` method

### Adding a New Language

1. **Create language file** (`Localization/{code}.json`):
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

2. **Copy all keys** from `en.json` and translate values

3. **Test** by selecting the language in Settings

### Adding a New Theme Color

1. **Add to ThemeColors** (`Models.cs`):
   ```csharp
   public class ThemeColors
   {
       public string NewColor { get; set; }
   }
   ```

2. **Add to AppSettings** (`Models.cs`):
   ```csharp
   public class AppSettings
   {
       public string NewColor { get; set; }
   }
   ```

3. **Apply in ThemeService** (`ThemeService.cs`):
   ```csharp
   Application.Current.Resources["NewColor"] = new SolidColorBrush(...);
   ```

4. **Add UI control** in Appearance settings tab

5. **Add localization keys** for the new color

## Localization System

### How It Works

1. **Language files** are JSON files in `Localization/` folder
2. **LocalizationService** loads translations on startup
3. **GetString()** method retrieves translated strings
4. **English fallback** for missing translations
5. **LanguageChanged event** notifies UI to update

### Adding Translation Keys

1. **Add key to English** (`Localization/en.json`):
   ```json
   {
     "New.Key": "English text"
   }
   ```

2. **Add to all other languages** with translations

3. **Use in code**:
   ```csharp
   var text = LocalizationService.Instance.GetString("New.Key");
   ```

### Translation Key Categories

- `App.*` - Application title, tray title
- `MainWindow.*` - Main window UI elements
- `DataGrid.*` - DataGrid column headers
- `Status.*` - Program status labels
- `Dialog.*` - Dialog buttons and messages
- `ProgramEdit.*` - Program edit dialog
- `Settings.*` - Settings dialog
- `Log.*` - Log messages
- `Email.*` - Email notification strings
- `Validation.*` - Validation error messages
- `Notification.*` - Notification messages
- `Tray.*` - System tray menu items

## Theming System

### Theme File Structure

Themes are JSON files in `Themes/` folder:

```json
{
  "name": "ThemeName",
  "displayName": "Display Name",
  "description": "Theme description",
  "author": "Author Name",
  "version": "1.0",
  "colors": {
    "BackgroundColor": "#F5F5F5",
    "TextColor": "#212121",
    "HighlightColor": "#0078D4",
    "BorderColor": "#E0E0E0",
    "SurfaceColor": "#FFFFFF",
    "SecondaryTextColor": "#757575",
    "ButtonTextColor": "#FFFFFF",
    "HeaderColor": "#FFFFFF"
  },
  "fontFamily": "Segoe UI",
  "fontSize": 12.0
}
```

### Applying Themes

1. **ThemeManager** loads themes from `Themes/` folder
2. **ThemeService** applies theme to WPF resources
3. **DynamicResource** bindings update UI automatically
4. **Theme changes** apply immediately without restart

### Creating Custom Themes

See [Theming Guide](Theming-Guide) for detailed instructions.

## Configuration Management

### Configuration File

Location: `%APPDATA%\RestartIt\config.json`

Structure:
```json
{
  "Programs": [...],
  "LogSettings": {...},
  "NotificationSettings": {...},
  "AppSettings": {...}
}
```

### Loading Configuration

1. **ConfigurationManager.LoadConfiguration()** reads JSON file
2. **Migrates old settings** if needed (e.g., theme names)
3. **Creates default config** if file doesn't exist
4. **Validates** configuration data

### Saving Configuration

1. **ConfigurationManager.SaveConfiguration()** writes JSON file
2. **Encrypts passwords** using Windows DPAPI
3. **Preserves existing data** not modified in UI

## Logging System

### Global Logging

- **Single log file** for application events
- **Configurable** in Settings → Logging tab
- **File naming**: `RestartIt_{YYYY-MM-DD}.log`
- **Rotation**: Daily or when size limit reached

### Per-Program Logging

- **Individual log files** for each program
- **Configurable** in Program Edit dialog
- **File naming**: `{ProgramName}_{YYYY-MM-DD}.log`
- **Independent settings** for each program
- **Events forwarded** to global logger for UI display

### Log Levels

- **Debug (0)**: Detailed diagnostic information
- **Info (1)**: General informational messages
- **Warning (2)**: Warning messages
- **Error (3)**: Error messages

### LoggerService Architecture

- **Global logger**: Created on startup
- **Per-program loggers**: Created on-demand
- **Event forwarding**: Per-program events → global logger → UI
- **Automatic cleanup**: Old loggers disposed when programs removed

## Testing

### Manual Testing Checklist

- [ ] Add/Edit/Remove programs
- [ ] Enable/Disable monitoring
- [ ] Program restart functionality
- [ ] Email notifications
- [ ] Taskbar notifications
- [ ] Logging (global and per-program)
- [ ] Theme changes
- [ ] Language switching
- [ ] System tray menu
- [ ] Settings persistence
- [ ] Error handling

### Testing Scenarios

1. **Program Monitoring:**
   - Start a program, stop it manually, verify restart
   - Test with invalid executable path
   - Test with missing working directory

2. **Notifications:**
   - Test email notifications with valid/invalid SMTP settings
   - Test taskbar notifications
   - Test per-program notification toggles

3. **Logging:**
   - Test global logging settings
   - Test per-program logging
   - Test log rotation
   - Test log cleanup

4. **Theming:**
   - Test theme switching
   - Test custom theme creation
   - Test theme deletion
   - Test color customization

5. **Localization:**
   - Test all languages
   - Test language switching
   - Verify all UI elements translate

## Debugging

### Debug Output

RestartIt uses `System.Diagnostics.Debug.WriteLine()` for debug output. View in:
- Visual Studio Output window (Debug)
- DebugView (Sysinternals)

### Common Debug Scenarios

1. **Process Monitoring Issues:**
   - Check `IsProcessRunning()` debug output
   - Verify process name matching
   - Check executable path validation

2. **Email Notification Issues:**
   - Check SMTP connection debug output
   - Verify credential encryption/decryption
   - Test with "Send Test Email" button

3. **Theme Issues:**
   - Check ThemeManager debug output for loaded themes
   - Verify JSON file format
   - Check resource binding errors

4. **Localization Issues:**
   - Check LocalizationService debug output
   - Verify JSON file format
   - Check for missing translation keys

### Debug Configuration

Set breakpoints in:
- `MainWindow.xaml.cs` - UI event handlers
- `Services.cs` - Core service methods
- `Dialogs.cs` - Dialog event handlers
- `Models.cs` - Configuration loading/saving

## Contributing

### Getting Started

1. **Fork the repository** on GitHub
2. **Create a feature branch**: `git checkout -b feature/your-feature`
3. **Make your changes** following code style guidelines
4. **Test thoroughly** using the manual testing checklist
5. **Commit with clear messages**: `git commit -m "Add feature X"`
6. **Push to your fork**: `git push origin feature/your-feature`
7. **Create a Pull Request** on GitHub

### Pull Request Guidelines

- **One feature per PR** - Keep changes focused
- **Update documentation** - Update README if needed
- **Add localization** - Add translation keys for new strings
- **Test all languages** - Verify translations work
- **Update CHANGELOG** - Document your changes

### Code Review Process

1. **Automated checks** - Build must pass
2. **Manual review** - Code style and logic review
3. **Testing** - Reviewer tests the changes
4. **Approval** - Maintainer approves and merges

## Additional Resources

- **.NET 8.0 Documentation**: https://docs.microsoft.com/dotnet/
- **WPF Documentation**: https://docs.microsoft.com/dotnet/desktop/wpf/
- **JSON.NET Documentation**: https://www.newtonsoft.com/json/help/html/Introduction.htm

## Support

For questions or issues:
- **GitHub Issues**: https://github.com/Antik79/RestartIt/issues
- **Documentation**: See README.md and this guide
- **Code Comments**: Inline XML documentation

---

**Happy Coding!** 🚀

