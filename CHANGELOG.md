# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- **Per-Program Logging** - Individual log files and settings for each monitored program
  - Enable/disable file logging per program
  - Custom log file directories per program
  - Individual log level, file size, and retention settings
  - Program-specific log files named `{ProgramName}_{YYYY-MM-DD}.log`
  - All per-program log events also forwarded to global logger for UI display
- **Theme Management** - Save and delete custom themes from the application
  - Save current appearance settings as a theme with metadata (name, display name, description, author)
  - Delete user-created themes (default themes protected)
  - Theme files automatically created in Themes folder
  - Themes immediately available in Theme Preset dropdown after saving
- **Enhanced Color System** - Added two new color roles for better customization
  - **ButtonTextColor**: Text color for buttons (automatically calculated based on highlight color brightness)
  - **HeaderColor**: Background color for headers and toolbar areas
  - Both colors customizable in Appearance settings
  - Two-column layout for color pickers in Appearance settings
- **Font Selector Enhancement** - Font family selector now includes all installed system fonts
  - Sorted alphabetically for easy selection
  - Fallback logic for missing fonts
  - All user-installed fonts available for selection

### Changed
- **Theming System** - Refactored from hardcoded themes to file-based JSON theme system
  - Themes now stored in separate JSON files in `Themes/` folder
  - Default themes: Light and Dark (modern, professional color schemes)
  - Removed hardcoded Catppuccin themes (Latte, Frappe, Macchiato, Mocha)
  - Users can create custom themes by adding JSON files or saving from UI
  - Automatic migration from old Catppuccin theme names to new system
  - Theme files can be edited without recompiling the application
- **Appearance Settings Layout** - Improved organization and compactness
  - Font Family and Font Size moved above Theme Preset selector
  - Color options split into two columns for more compact layout
  - Removed Preview section (theme changes apply immediately)
- **Program Edit Dialog** - Enhanced with per-program logging options
  - New Logging section with per-program settings
  - Browse button for log file directory
  - Validation for log file size and retention days
- **Language Files** - Updated all language files with new keys
  - Added translations for theme save/delete functionality
  - Added translations for per-program logging
  - Added translations for new color roles
  - Polish language added (17 languages total)

### Fixed
- **Theme Application** - Fixed issue where color changes didn't apply when pressing Apply or OK
  - Manual color changes now automatically switch Theme Preset to "Custom"
  - Theme lookup now tries internal name first, then display name
  - Custom settings properly applied when no theme preset matches
- **Save Theme Dialog** - Fixed layout issue where description textbox overlapped buttons
  - Increased dialog height to 380px
  - Adjusted grid row definitions for proper spacing
  - Added minimum height to description textbox

## [1.3.3] - 2025-01-27

### Added
- **Taskbar Notifications** - Windows taskbar balloon tip notifications as an alternative to email
  - Global enable/disable toggle for taskbar notifications
  - Per-program taskbar notification toggles
  - Separate toggles for restart success, restart failure, and stop/crash events
  - Configurable in Settings → Notifications tab
  - Per-program settings available in Program Edit dialog
- **Enhanced Tray Icon Menu** - Redesigned system tray context menu with improved organization
  - **Start/Stop Monitoring** - Global toggle to start/stop all monitoring (green ✓ when active, red ✗ when inactive)
  - **Monitor** subfolder - List of all monitored programs with enable/disable toggles
  - **Notifications** subfolder - Per-program taskbar notification toggles
  - **Minimize to System Tray** - Quick toggle in tray menu
  - **Minimize on Close** - New setting to minimize to tray when closing window instead of exiting
  - Visual indicators: Only show ✓ when enabled, space for alignment when disabled
- **Monitoring Active Button** - Clickable status button in main window toolbar
  - Click to toggle monitoring service on/off globally
  - Visual feedback: Green circle when active, red circle when inactive
  - Updates status text dynamically ("Monitoring Active" / "Monitoring Stopped")
- **Minimize on Close Setting** - New independent setting for window close behavior
  - Separate from "Minimize to System Tray" setting
  - When enabled, closing the window minimizes to tray instead of exiting
  - Configurable in Settings → Application tab and tray icon menu

### Changed
- **Tray Icon Menu Structure** - Complete redesign with new organization
  - Removed global notification toggles from Notifications subfolder
  - Changed "Monitors" to "Monitor" (singular)
  - Removed ✗ indicators - only show ✓ when enabled for cleaner appearance
  - Added space padding when disabled to maintain text alignment
- **Settings Dialog** - Renamed "Email Notifications" tab to "Notifications"
  - Now includes both email and taskbar notification settings
  - Taskbar notification section with global toggles
  - "Send Test Email" button moved under email notification toggles
- **Notification System** - Enhanced notification capabilities
  - Added "Notify on Stop/Crash" option for both email and taskbar notifications
  - Triggers when a monitored program is detected as stopped (before restart attempt)
  - Separate from restart success/failure notifications

### Fixed
- **Language Changes in Settings** - Language changes now apply immediately when pressing OK button
  - Previously only worked with Apply button
  - Now both OK and Apply buttons apply language and theme changes correctly
- **Taskbar Notifications** - Fixed taskbar notification display issues
  - Improved threading handling for Windows Forms NotifyIcon
  - Better error handling and debug logging
  - Default value for EnableTaskbarNotifications set to true for new programs

## [1.3.2] - 2025-01-27

### Added
- **Catppuccin Theme Support** - Complete integration with Catppuccin color palette
  - 4 theme flavors: Latte (light), Frappe, Macchiato, and Mocha (dark)
  - Theme preset selector in Appearance settings tab
  - Automatic color mapping following Catppuccin style guide
  - All palette colors included for future extensibility
- **Theme Service** - New ThemeService singleton for dynamic theme application
  - Real-time theme updates without application restart
  - Automatic resource dictionary updates
  - Derived color generation (hover states, light variants)
- **Apply Button** - New Apply button in Settings dialog
  - Instant preview of theme and language changes
  - Apply changes without closing the dialog
  - Visual feedback with "Applied!" confirmation
- **Appearance Settings Tab** - New tab in Settings dialog
  - Font family selection (10 common system fonts)
  - Font size slider (8-24pt)
  - 6 color pickers with live preview
  - Theme preset dropdown
  - Reset to defaults functionality
- **Localization Updates** - Added appearance-related strings to all 16 language files

### Fixed
- **Color Picker Bug** - Fixed issue where color picker returned named colors (e.g., "Black") instead of hex values
  - Now always returns hex format (e.g., "#000000")
  - Ensures proper color application across all themes
- **DataGrid Text Visibility** - Fixed barely visible text on light themes
  - Proper theme color mapping for row backgrounds
  - Dynamic resource binding for all text elements
  - Alternating row colors now use theme colors
- **Settings Dialog Theming** - Applied theme styling to Settings dialog
  - Window background uses theme colors
  - TabControl and TabItems styled with theme
  - Buttons use theme highlight colors
  - All content areas properly themed
- **Status Badge Visibility** - Improved text contrast on status indicator
  - Changed to use HighlightColor background with white text
  - Better visibility across all themes

### Changed
- **Theme System** - Enhanced theming infrastructure
  - All UI elements now use DynamicResource for theme colors
  - Settings dialog fully integrated with theme system
  - Real-time theme application throughout application

## [1.3.1] - 2025-01-27

### Added
- Comprehensive XML documentation comments throughout the codebase
  - Added documentation to all public classes and methods in Services.cs
  - Added documentation to all data models and ConfigurationManager in Models.cs
  - Added documentation to LocalizationService and language-related classes
  - Added documentation to ProgramEditDialog and SettingsDialog in Dialogs.cs
  - Added documentation to MainWindow and event handlers in MainWindow.xaml.cs
  - Added documentation to IconHelper icon creation methods
- Improved code maintainability with detailed method summaries, parameter descriptions, and remarks

## [1.3.0] - 2025-01-27

### Added
- Enhanced Custom Styling - Modern UI overhaul with zero dependencies
  - Modern color palette with professional blue theme
  - Card-based layout with subtle shadows for depth
  - Gradient primary buttons with hover effects
  - Rounded corners (6-10px radius) throughout
  - Improved spacing and padding (16-20px)
  - Elevated containers with shadow effects
  - Modern status badges with rounded backgrounds
- English fallback language support - Missing translations automatically fall back to English
- Comprehensive input validation for file paths and command-line arguments
- Secure password handling using SecureString for in-memory operations

### Changed
- Complete UI redesign with Enhanced Custom Styling
  - Toolbar wrapped in card-style container with shadow
  - DataGrid with modern headers and alternating row backgrounds
  - Log panel with card-style design and improved readability
  - Increased row height (52px) for better readability
  - Removed grid lines, added subtle alternating row colors
- All hardcoded English strings replaced with localization calls
- Password encryption now uses SecureString for better security

### Fixed
- Process handle leaks in IsProcessRunning method - all Process objects now properly disposed
- Missing localization keys for validation messages
- Hardcoded strings in dialogs and file dialogs
- Email test validation messages now properly localized
- All user-facing strings now use localization system

### Security
- Enhanced password security with SecureString for in-memory handling
- Improved input validation to prevent path traversal and invalid arguments
- Better resource management with explicit process disposal

## [1.2.1] - 2025-11-09

### Fixed
- **CRITICAL**: Enable/disable monitor toggle not working for programs disabled at launch - PropertyChanged event handlers were not being subscribed for existing programs when monitoring service started
- Incomplete localization - 5 log messages remained in English regardless of selected language (program not running, error monitoring, error checking process, successfully restarted, failed to restart)
- Added missing localization keys to all 16 language files

## [1.2.0] - 2025-11-08

### Added
- 12 new language translations:
  - Spanish (Español)
  - Italian (Italiano)
  - Japanese (日本語)
  - Korean (한국어)
  - Chinese (中文)
  - Turkish (Türkçe)
  - Greek (Ελληνικά)
  - Swedish (Svenska)
  - Norwegian (Norsk)
  - Danish (Dansk)
  - Finnish (Suomi)
  - Swahili
- Modular language discovery system - languages auto-detected from Localization folder
- Language metadata support (code, name, nativeName, icon) in JSON files
- Flag emoji icons in language selection dropdown
- Native language names displayed alongside language codes

### Fixed
- Language switching not working when changing languages in Settings dialog
- JSON deserialization issue when loading language files with metadata objects
- Language dropdown now correctly shows currently selected language

### Changed
- Language dropdown now displays format: "{icon} {nativeName}" (e.g., "🇬🇧 English")
- Languages sorted alphabetically by native name in dropdown
- LoadLanguage() method rewritten to properly handle mixed JSON types

## [1.1.0] - 2025-11-08

### Added
- Multi-language support (4 languages: English, Dutch, French, German)
- Real-time language switching without application restart
- LocalizationService with event-based UI updates
- Language selection in Settings → Application tab
- Fully localized UI (all windows, dialogs, buttons, messages)
- MIT License file
- Version information and About tab in Settings
- Documentation for Windows SmartScreen warning in README

### Fixed
- DataGrid text alignment - all column text now centered vertically

### Changed
- Settings dialog now fully localizes when language changes
- MainWindow subscribes to LanguageChanged event for dynamic updates

## [1.0.0] - 2025-11-07

### Added
- Initial release of RestartIt
- Core application monitoring functionality
- Automatic program restart when processes stop
- Configurable check intervals and restart delays
- System tray integration
- Real-time status monitoring in DataGrid
- File logging with rotation settings
- Email notifications via SMTP
- Application settings (startup options, tray behavior)
- Export logs functionality
- Add/Edit/Remove monitored programs
- Configuration persistence in %AppData%\RestartIt\config.json

[Unreleased]: https://github.com/Antik79/RestartIt/compare/v1.3.3...HEAD
[1.3.3]: https://github.com/Antik79/RestartIt/compare/v1.3.2...v1.3.3
[1.3.2]: https://github.com/Antik79/RestartIt/compare/v1.3.1...v1.3.2
[1.3.1]: https://github.com/Antik79/RestartIt/compare/v1.3.0...v1.3.1
[1.3.0]: https://github.com/Antik79/RestartIt/compare/v1.2.1...v1.3.0
[1.2.1]: https://github.com/Antik79/RestartIt/compare/v1.2.0...v1.2.1
[1.2.0]: https://github.com/Antik79/RestartIt/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Antik79/RestartIt/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/Antik79/RestartIt/releases/tag/v1.0.0
