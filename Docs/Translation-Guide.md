# Translation Guide

Complete guide to adding new languages and contributing translations to RestartIt.

## Table of Contents

1. [Introduction](#introduction)
2. [Language File Format](#language-file-format)
3. [Adding a New Language](#adding-a-new-language)
4. [Translation Keys](#translation-keys)
5. [Translation Guidelines](#translation-guidelines)
6. [Contributing Translations](#contributing-translations)
7. [Updating Existing Translations](#updating-existing-translations)
8. [Testing Translations](#testing-translations)
9. [Troubleshooting](#troubleshooting)

## Introduction

RestartIt uses a modular language system that allows you to add new languages without modifying code. All translations are stored in JSON files in the `Localization/` folder.

### Currently Supported Languages

RestartIt currently supports **17 languages**:
- English (en)
- Dutch (nl)
- French (fr)
- German (de)
- Spanish (es)
- Italian (it)
- Japanese (ja)
- Korean (ko)
- Chinese (cn)
- Turkish (tr)
- Greek (el)
- Swedish (sv)
- Norwegian (no)
- Danish (da)
- Finnish (fi)
- Polish (pl)
- Swahili (sw)

## Language File Format

Each language file is a JSON file named with the language code (e.g., `en.json`, `de.json`, `pt.json`).

### Basic Structure

```json
{
  "_metadata": {
    "code": "pt",
    "name": "Portuguese",
    "nativeName": "Português",
    "icon": "🇵🇹"
  },
  "App.Title": "RestartIt - Monitor de Aplicações",
  "App.TrayTitle": "RestartIt - Monitor de Aplicações",
  "MainWindow.AddProgram": "➕ Adicionar Programa",
  ...
}
```

### Metadata Section

The `_metadata` object contains information about the language:

- **code** (string, required) - Two-letter language code (ISO 639-1)
- **name** (string, required) - English name of the language
- **nativeName** (string, required) - Native name of the language
- **icon** (string, optional) - Flag emoji for the language

**Example:**
```json
{
  "_metadata": {
    "code": "pt",
    "name": "Portuguese",
    "nativeName": "Português",
    "icon": "🇵🇹"
  }
}
```

## Adding a New Language

### Step 1: Create Language File

1. **Copy English file as template:**
   - Copy `Localization/en.json`
   - Rename to your language code (e.g., `pt.json` for Portuguese)

2. **Update metadata:**
   - Change `code` to your language code
   - Change `name` to English name of your language
   - Change `nativeName` to native name
   - Add `icon` with flag emoji (optional)

### Step 2: Translate All Keys

1. **Keep all keys unchanged** - Only translate the values
2. **Translate all strings** - Don't leave any in English
3. **Maintain placeholders** - Keep `{0}`, `{1}`, etc. as-is
4. **Preserve formatting** - Keep `\n` for line breaks

**Example Translation:**
```json
{
  // English (en.json)
  "App.Title": "RestartIt - Application Monitor",
  
  // Portuguese (pt.json)
  "App.Title": "RestartIt - Monitor de Aplicações"
}
```

### Step 3: Test the Translation

1. **Place file in Localization folder**
2. **Restart RestartIt**
3. **Select your language** in Settings → Application
4. **Verify all UI elements** are translated
5. **Check for missing keys** (will fall back to English)

### Step 4: Submit Translation

1. **Create a Pull Request** on GitHub
2. **Include language file** in the PR
3. **Test thoroughly** before submitting
4. **Mention any missing keys** in PR description

## Translation Keys

### Key Categories

Translation keys are organized by category:

#### App.*
Application-wide strings:
- `App.Title` - Main window title
- `App.TrayTitle` - System tray tooltip

#### MainWindow.*
Main window UI elements:
- `MainWindow.AddProgram` - Add Program button
- `MainWindow.Remove` - Remove button
- `MainWindow.Settings` - Settings button
- `MainWindow.MonitoringActive` - Status text when active
- `MainWindow.MonitoringStopped` - Status text when stopped

#### DataGrid.*
DataGrid column headers:
- `DataGrid.Enabled` - Enabled column
- `DataGrid.ProgramName` - Program Name column
- `DataGrid.Path` - Path column
- `DataGrid.Status` - Status column

#### Status.*
Program status labels:
- `Status.Running` - Running status
- `Status.Stopped` - Stopped status
- `Status.Restarting` - Restarting status
- `Status.Failed` - Failed status

#### Dialog.*
Dialog buttons and messages:
- `Dialog.OK` - OK button
- `Dialog.Cancel` - Cancel button
- `Dialog.Apply` - Apply button
- `Dialog.Yes` - Yes button
- `Dialog.No` - No button

#### ProgramEdit.*
Program edit dialog:
- `ProgramEdit.Title` - Dialog title
- `ProgramEdit.ProgramName` - Program Name label
- `ProgramEdit.ExecutablePath` - Executable Path label
- `ProgramEdit.ValidationError` - Validation error title

#### Settings.*
Settings dialog:
- `Settings.Title` - Settings dialog title
- `Settings.Logging` - Logging tab
- `Settings.EmailNotifications` - Email Notifications tab
- `Settings.Application` - Application tab
- `Settings.Appearance` - Appearance tab

#### Log.*
Log messages:
- `Log.Started` - Application started message
- `Log.ProgramAdded` - Program added message
- `Log.ProgramRestarted` - Program restarted message
- `Log.FailedToRestart` - Failed to restart message

#### Email.*
Email notification strings:
- `Email.Subject.Restart` - Restart email subject
- `Email.Body.Restarted` - Restart email body
- `Email.Body.Time` - Time label in email

#### Validation.*
Validation error messages:
- `Validation.ExecutablePathRequired` - Path required error
- `Validation.FileNotFound` - File not found error
- `Validation.PathTooLong` - Path too long error

#### Notification.*
Notification messages:
- `Notification.RestartSuccess` - Restart success notification
- `Notification.RestartFailure` - Restart failure notification

#### Tray.*
System tray menu items:
- `Tray.ShowWindow` - Show Window menu item
- `Tray.Exit` - Exit menu item
- `Tray.StartMonitoring` - Start Monitoring menu item

### Placeholders

Some translation strings contain placeholders that are replaced at runtime:

- `{0}`, `{1}`, etc. - Positional placeholders
- Keep placeholders exactly as shown
- Don't translate placeholder names

**Example:**
```json
{
  // English
  "Log.ProgramAdded": "Added program: {0}",
  
  // Portuguese (correct)
  "Log.ProgramAdded": "Programa adicionado: {0}",
  
  // Portuguese (wrong - don't translate {0})
  "Log.ProgramAdded": "Programa adicionado: {programa}"
}
```

## Translation Guidelines

### General Guidelines

1. **Be Consistent:**
   - Use consistent terminology throughout
   - Maintain consistency with Windows terminology
   - Use standard technical terms where appropriate

2. **Keep It Natural:**
   - Translate meaning, not word-for-word
   - Use natural phrasing in your language
   - Avoid literal translations that sound awkward

3. **Maintain Context:**
   - Consider where the text appears
   - Short labels may need different translations than full sentences
   - Keep UI elements concise

4. **Preserve Formatting:**
   - Keep `\n` for line breaks
   - Preserve spacing and punctuation
   - Maintain email formatting

### Specific Guidelines

#### Button Labels
- Keep short and action-oriented
- Use imperative form (e.g., "Save", "Cancel", "Apply")
- Match Windows conventions

#### Error Messages
- Be clear and helpful
- Explain what went wrong
- Suggest how to fix it

#### Email Content
- Maintain professional tone
- Keep formatting consistent
- Preserve placeholders for dynamic content

#### Tooltips and Descriptions
- Be concise but informative
- Explain what the setting does
- Use clear, simple language

## Contributing Translations

### How to Contribute

1. **Fork the Repository:**
   - Fork [RestartIt on GitHub](https://github.com/Antik79/RestartIt)
   - Clone your fork locally

2. **Create Language File:**
   - Copy `Localization/en.json` to `Localization/[code].json`
   - Replace `[code]` with your language code

3. **Translate All Keys:**
   - Translate all string values
   - Keep all keys unchanged
   - Maintain placeholders

4. **Test Your Translation:**
   - Place file in Localization folder
   - Restart RestartIt
   - Test all UI elements
   - Verify no missing translations

5. **Submit Pull Request:**
   - Commit your language file
   - Push to your fork
   - Create Pull Request on GitHub
   - Describe your translation in PR

### Pull Request Guidelines

- **Complete Translation**: All keys must be translated
- **Tested**: Translation must be tested in the application
- **Valid JSON**: File must be valid JSON
- **Proper Metadata**: Metadata section must be correct
- **No Hardcoded English**: Don't leave English strings

## Updating Existing Translations

### Adding Missing Keys

If new features add translation keys:

1. **Check English file** for new keys
2. **Add missing keys** to your language file
3. **Translate the new keys**
4. **Test the translation**
5. **Submit Pull Request** with updates

### Improving Existing Translations

If you find translation issues:

1. **Identify the problem** (typo, awkward phrasing, etc.)
2. **Propose better translation**
3. **Test the improvement**
4. **Submit Pull Request** with fix

### Keeping Translations Up to Date

- **Watch the repository** for new keys
- **Check English file** periodically
- **Update your language file** when new features are added
- **Submit updates** via Pull Request

## Testing Translations

### Manual Testing Checklist

- [ ] All UI elements translate correctly
- [ ] Dialog boxes are fully translated
- [ ] Error messages are translated
- [ ] Log messages are translated
- [ ] Email notifications are translated
- [ ] System tray menu is translated
- [ ] Settings dialog is translated
- [ ] No English text remains
- [ ] Placeholders work correctly
- [ ] Text fits in UI elements (no overflow)

### Testing Steps

1. **Select Your Language:**
   - Go to Settings → Application
   - Select your language
   - Click OK

2. **Test Each Section:**
   - Main window
   - Add/Edit Program dialog
   - Settings dialog (all tabs)
   - System tray menu
   - Error dialogs
   - Notifications

3. **Check Log Messages:**
   - Add a program
   - Enable monitoring
   - Stop a program to trigger restart
   - Check activity log for translated messages

4. **Test Email Notifications:**
   - Configure email settings
   - Send test email
   - Verify email content is translated

## Troubleshooting

### Language Not Appearing in Dropdown

**Issue**: Language file exists but doesn't appear in Settings

**Solutions:**
1. Verify file is in `Localization/` folder
2. Check file name matches language code (e.g., `pt.json`)
3. Ensure JSON syntax is valid
4. Verify `_metadata` section is present and correct
5. Restart RestartIt (languages load on startup)

### Missing Translations

**Issue**: Some UI elements show in English

**Solutions:**
1. Check English file for missing keys
2. Add missing keys to your language file
3. Translate the missing keys
4. Restart RestartIt
5. Verify all keys are present

### JSON Syntax Errors

**Issue**: Language file doesn't load, JSON error

**Solutions:**
1. Validate JSON using online validator
2. Check for missing commas
3. Verify all strings are quoted
4. Check for trailing commas
5. Ensure proper bracket/brace matching

### Placeholders Not Working

**Issue**: Placeholders like `{0}` show literally instead of values

**Solutions:**
1. Ensure placeholders are exactly `{0}`, `{1}`, etc.
2. Don't translate placeholder names
3. Keep placeholder order correct
4. Verify placeholder count matches English version

## Translation Resources

### Language Codes

Use ISO 639-1 two-letter codes:
- `en` - English
- `es` - Spanish
- `fr` - French
- `de` - German
- `pt` - Portuguese
- etc.

### Flag Emojis

Common flag emojis for languages:
- 🇬🇧 English
- 🇪🇸 Spanish
- 🇫🇷 French
- 🇩🇪 German
- 🇵🇹 Portuguese
- 🇮🇹 Italian
- 🇯🇵 Japanese
- 🇰🇷 Korean
- 🇨🇳 Chinese
- 🇹🇷 Turkish
- 🇬🇷 Greek
- 🇸🇪 Swedish
- 🇳🇴 Norwegian
- 🇩🇰 Danish
- 🇫🇮 Finnish
- 🇵🇱 Polish

### Translation Tools

- **JSON Validator**: [jsonlint.com](https://jsonlint.com/)
- **Translation Memory**: Use existing translations as reference
- **Glossary**: Maintain consistency with technical terms

## Best Practices

1. **Test Thoroughly**: Test all UI elements after translating
2. **Be Consistent**: Use consistent terminology
3. **Keep Updated**: Update translations when new features are added
4. **Ask for Help**: Don't hesitate to ask questions via GitHub Issues
5. **Review Existing**: Review similar applications for terminology
6. **Native Speakers**: Have native speakers review your translation

## Getting Help

If you need help with translations:

1. **Check Existing Translations**: Look at other language files for examples
2. **GitHub Issues**: Ask questions in GitHub Issues
3. **Pull Request Comments**: Discuss translations in PR comments
4. **Community**: Engage with other translators

---

Thank you for contributing translations! Your help makes RestartIt accessible to users worldwide. 🌍

