# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[Unreleased]: https://github.com/Antik79/RestartIt/compare/v1.3.1...HEAD
[1.3.1]: https://github.com/Antik79/RestartIt/compare/v1.3.0...v1.3.1
[1.3.0]: https://github.com/Antik79/RestartIt/compare/v1.2.1...v1.3.0
[1.2.1]: https://github.com/Antik79/RestartIt/compare/v1.2.0...v1.2.1
[1.2.0]: https://github.com/Antik79/RestartIt/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Antik79/RestartIt/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/Antik79/RestartIt/releases/tag/v1.0.0
