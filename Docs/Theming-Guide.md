# Theming Guide

Complete guide to creating, customizing, and managing themes in RestartIt.

## Table of Contents

1. [Introduction](#introduction)
2. [Theme File Format](#theme-file-format)
3. [Color Properties](#color-properties)
4. [Font Properties](#font-properties)
5. [Creating Themes](#creating-themes)
6. [Saving Themes](#saving-themes)
7. [Deleting Themes](#deleting-themes)
8. [Sharing Themes](#sharing-themes)
9. [Theme Examples](#theme-examples)
10. [Troubleshooting](#troubleshooting)

## Introduction

RestartIt uses a file-based theming system that allows you to customize the appearance of the application. Themes are stored as JSON files in the `Themes/` folder and can be created, saved, and deleted directly from the application.

### Default Themes

RestartIt comes with two default themes:
- **Light** - Clean, modern light theme
- **Dark** - Easy on the eyes dark theme

Default themes cannot be deleted but can be used as starting points for custom themes.

## Theme File Format

Each theme file is a JSON file with the following structure:

```json
{
  "name": "MyTheme",
  "displayName": "My Custom Theme",
  "description": "A description of your theme",
  "author": "Your Name",
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

### Required Fields

- **name** (string, required) - Internal identifier for the theme. Must be unique and cannot be "Custom", "Light", or "Dark".
- **colors** (object, required) - Object containing all color values
- All color properties within the colors object are required

### Optional Fields

- **displayName** (string, optional) - Name shown in the Theme Preset dropdown. If not provided, uses `name`.
- **description** (string, optional) - Description of the theme
- **author** (string, optional) - Author name
- **version** (string, optional) - Theme version
- **fontFamily** (string, optional) - Font family name. If not provided, uses system default.
- **fontSize** (number, optional) - Font size in points. If not provided, uses system default.

## Color Properties

All colors must be specified in hexadecimal format with a `#` prefix.

### Color Format

- **#RRGGBB** - Standard format (e.g., `#FF0000` for red)
- **#AARRGGBB** - With alpha channel (e.g., `#80FF0000` for semi-transparent red)

### Available Colors

#### BackgroundColor
- **Purpose**: Main window background color
- **Usage**: Application window background
- **Example**: `#F5F5F5` (light gray) or `#1E1E1E` (dark gray)

#### TextColor
- **Purpose**: Primary text color
- **Usage**: Main text throughout the application
- **Example**: `#212121` (dark text) or `#E0E0E0` (light text)

#### HighlightColor
- **Purpose**: Accent color for buttons, links, and highlights
- **Usage**: Primary buttons, links, selected items, status indicators
- **Example**: `#0078D4` (blue) or `#00A4EF` (lighter blue)

#### BorderColor
- **Purpose**: Border and divider color
- **Usage**: Borders around controls, dividers between sections
- **Example**: `#E0E0E0` (light border) or `#404040` (dark border)

#### SurfaceColor
- **Purpose**: Surface/card background color
- **Usage**: Card backgrounds, elevated surfaces, input fields
- **Example**: `#FFFFFF` (white) or `#2D2D2D` (dark surface)

#### SecondaryTextColor
- **Purpose**: Secondary text and labels color
- **Usage**: Labels, secondary information, hints
- **Example**: `#757575` (gray) or `#B0B0B0` (light gray)

#### ButtonTextColor
- **Purpose**: Text color for buttons
- **Usage**: Text on primary buttons
- **Example**: `#FFFFFF` (white) or `#000000` (black)
- **Note**: Should contrast well with HighlightColor. Automatically calculated if not specified.

#### HeaderColor
- **Purpose**: Background color for headers and toolbar areas
- **Usage**: Toolbar background, header sections
- **Example**: `#FFFFFF` (white) or `#2D2D2D` (dark header)

## Font Properties

### fontFamily

- **Type**: String
- **Description**: Font family name (e.g., "Segoe UI", "Arial", "Consolas")
- **Default**: System default (usually "Segoe UI" on Windows)
- **Note**: Must be an installed font on the system

### fontSize

- **Type**: Number (double)
- **Description**: Font size in points
- **Default**: System default (usually 12.0)
- **Range**: Typically 8-24 points for readability
- **Example**: `12.0`, `14.0`, `16.0`

## Creating Themes

### Method 1: Save from Current Settings

This is the easiest way to create a theme:

1. Go to **Settings** → **Appearance** tab
2. Customize colors, fonts, and other appearance settings
3. Click **"Save Theme..."** button
4. Enter theme metadata:
   - **Theme Name** - Internal identifier (required, must be unique)
   - **Display Name** - Name shown in dropdown (optional)
   - **Description** - Description of your theme (optional)
   - **Author** - Your name (optional)
5. Click **OK** to save
6. Your theme will appear in the Theme Preset dropdown immediately

**Advantages:**
- No need to edit JSON files
- Immediate feedback
- Validates all settings before saving

### Method 2: Create Theme File Manually

For more control or to create themes programmatically:

1. **Copy an existing theme** as a starting point:
   - Copy `Themes/Light.json` or `Themes/Dark.json`
   - Rename to your theme name (e.g., `MyTheme.json`)

2. **Edit the JSON file**:
   - Open in a text editor (VS Code, Notepad++, etc.)
   - Modify colors, fonts, and metadata
   - Ensure JSON syntax is valid

3. **Validate the file**:
   - Use a JSON validator to check syntax
   - Ensure all required fields are present
   - Verify color formats are correct

4. **Place in Themes folder**:
   - Save the file to `Themes/` folder in RestartIt directory
   - File must have `.json` extension

5. **Load the theme**:
   - Restart RestartIt (themes are loaded on startup)
   - Go to Settings → Appearance → Theme Preset
   - Select your theme from the dropdown

**Advantages:**
- Full control over theme structure
- Can create themes programmatically
- Easy to version control themes

## Saving Themes

### From Application

1. Customize appearance settings in Settings → Appearance
2. Click **"Save Theme..."** button
3. Fill in theme metadata
4. Click **OK**

The theme file will be created in the `Themes/` folder automatically.

### Theme File Location

Themes are saved to:
```
[RestartIt Installation Directory]/Themes/[ThemeName].json
```

Example:
```
C:\Program Files\RestartIt\Themes\MyTheme.json
```

### File Naming

- Theme files are named based on the `name` field
- Invalid filename characters are replaced with underscores
- File extension is always `.json`
- Theme names are case-insensitive

## Deleting Themes

### From Application

1. Go to **Settings** → **Appearance** tab
2. Select the theme you want to delete from the Theme Preset dropdown
3. Click **"Delete Theme..."** button
4. Confirm deletion in the dialog
5. The theme file will be removed from the Themes folder

### Restrictions

- **Cannot delete "Custom"** - This is not a real theme, just a state
- **Cannot delete default themes** - "Light" and "Dark" are protected
- **Must select theme first** - Theme must be selected before deletion

### Manual Deletion

You can also delete theme files manually:
1. Close RestartIt
2. Navigate to `Themes/` folder
3. Delete the theme JSON file
4. Restart RestartIt

## Sharing Themes

Themes are just JSON files, making them easy to share:

### Sharing Your Theme

1. **Locate theme file**:
   - Go to `Themes/` folder in RestartIt directory
   - Find your theme JSON file

2. **Share the file**:
   - Upload to file sharing service
   - Share via email or messaging
   - Post on GitHub, forums, or social media

3. **Include instructions**:
   - Tell users to place file in `Themes/` folder
   - Mention they need to restart RestartIt
   - Provide preview screenshot if possible

### Installing Shared Themes

1. **Download the theme file**
2. **Place in Themes folder**:
   - Navigate to RestartIt installation directory
   - Open `Themes/` folder
   - Copy the theme JSON file here

3. **Restart RestartIt**
4. **Select the theme**:
   - Go to Settings → Appearance
   - Select theme from Theme Preset dropdown

## Theme Examples

### Light Theme

```json
{
  "name": "Light",
  "displayName": "Light",
  "description": "Clean light theme",
  "author": "RestartIt",
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

### Dark Theme

```json
{
  "name": "Dark",
  "displayName": "Dark",
  "description": "Easy on the eyes dark theme",
  "author": "RestartIt",
  "version": "1.0",
  "colors": {
    "BackgroundColor": "#1E1E1E",
    "TextColor": "#E0E0E0",
    "HighlightColor": "#0078D4",
    "BorderColor": "#404040",
    "SurfaceColor": "#2D2D2D",
    "SecondaryTextColor": "#B0B0B0",
    "ButtonTextColor": "#FFFFFF",
    "HeaderColor": "#2D2D2D"
  },
  "fontFamily": "Segoe UI",
  "fontSize": 12.0
}
```

### High Contrast Theme

```json
{
  "name": "HighContrast",
  "displayName": "High Contrast",
  "description": "High contrast theme for better visibility",
  "author": "Your Name",
  "version": "1.0",
  "colors": {
    "BackgroundColor": "#000000",
    "TextColor": "#FFFFFF",
    "HighlightColor": "#FFFF00",
    "BorderColor": "#FFFFFF",
    "SurfaceColor": "#000000",
    "SecondaryTextColor": "#CCCCCC",
    "ButtonTextColor": "#000000",
    "HeaderColor": "#000000"
  },
  "fontFamily": "Segoe UI",
  "fontSize": 14.0
}
```

### Custom Color Scheme Example

```json
{
  "name": "Ocean",
  "displayName": "Ocean Blue",
  "description": "Calming ocean-inspired color scheme",
  "author": "Your Name",
  "version": "1.0",
  "colors": {
    "BackgroundColor": "#E8F4F8",
    "TextColor": "#1A3A52",
    "HighlightColor": "#2E86AB",
    "BorderColor": "#A23B72",
    "SurfaceColor": "#F18F01",
    "SecondaryTextColor": "#6B8E9F",
    "ButtonTextColor": "#FFFFFF",
    "HeaderColor": "#C73E1D"
  },
  "fontFamily": "Segoe UI",
  "fontSize": 12.0
}
```

## Color Selection Tips

### Contrast

- Ensure sufficient contrast between text and background colors
- Use online contrast checkers to verify accessibility
- WCAG recommends at least 4.5:1 contrast ratio for normal text

### Color Harmony

- Use color wheel tools to find complementary colors
- Consider using color palette generators
- Test colors together before finalizing theme

### Readability

- Light themes: Use dark text on light backgrounds
- Dark themes: Use light text on dark backgrounds
- Avoid pure black (#000000) or pure white (#FFFFFF) for large areas

### Button Text Color

- ButtonTextColor should contrast well with HighlightColor
- White (#FFFFFF) works for most dark highlight colors
- Black (#000000) works for light highlight colors
- RestartIt can automatically calculate this if not specified

## Troubleshooting

### Theme Not Loading

**Issue**: Theme doesn't appear in dropdown or doesn't apply

**Solutions:**
1. Verify JSON syntax is valid
2. Ensure all required color fields are present
3. Check that theme name is unique
4. Restart RestartIt (themes load on startup)
5. Check debug output for error messages

### Invalid Theme File

**Issue**: Error message about invalid theme

**Solutions:**
1. Validate JSON syntax using online validator
2. Ensure all colors are in hex format (#RRGGBB)
3. Check that all required fields are present
4. Verify no trailing commas in JSON
5. Ensure proper quote escaping

### Colors Not Applying

**Issue**: Theme selected but colors don't change

**Solutions:**
1. Click "Apply" or "OK" button after selecting theme
2. Verify theme file is valid JSON
3. Check that all color values are correct hex format
4. Restart RestartIt if changes don't apply
5. Verify you're not in "Custom" mode (which overrides theme)

### Theme Deletion Fails

**Issue**: Cannot delete theme

**Solutions:**
1. Ensure theme is selected before clicking Delete
2. Verify theme is not a default theme (Light/Dark)
3. Check file permissions on Themes folder
4. Close RestartIt and delete file manually if needed
5. Ensure theme file exists in Themes folder

### Font Not Applying

**Issue**: Font family doesn't change

**Solutions:**
1. Verify font is installed on your system
2. Use exact font name as shown in system font list
3. Check font name spelling and case
4. Some fonts may not support all characters
5. Restart RestartIt after changing font

## Best Practices

1. **Test Your Theme**: Apply the theme and test all UI elements
2. **Check Contrast**: Ensure text is readable on all backgrounds
3. **Use Descriptive Names**: Choose clear, descriptive theme names
4. **Document Colors**: Add descriptions explaining color choices
5. **Version Your Themes**: Use version field to track theme updates
6. **Share Responsibly**: Test themes thoroughly before sharing
7. **Backup Themes**: Keep copies of custom themes you create

## Additional Resources

- [Color Contrast Checker](https://webaim.org/resources/contrastchecker/)
- [Color Palette Generators](https://coolors.co/)
- [JSON Validator](https://jsonlint.com/)
- [WPF Color Reference](https://docs.microsoft.com/dotnet/api/system.windows.media.colors)

---

For technical details, see [Developer Guide - Theming System](Developer-Guide#theming-system).

