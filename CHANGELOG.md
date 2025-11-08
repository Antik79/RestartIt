# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[Unreleased]: https://github.com/Antik79/RestartIt/compare/v1.2.0...HEAD
[1.2.0]: https://github.com/Antik79/RestartIt/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Antik79/RestartIt/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/Antik79/RestartIt/releases/tag/v1.0.0
