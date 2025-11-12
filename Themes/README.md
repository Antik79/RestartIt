# Custom Themes

You can create custom themes for RestartIt by adding JSON files to this `Themes` folder.

## Theme File Format

Each theme file should be a JSON file with the following structure:

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

## Color Properties

- **BackgroundColor**: Main window background color
- **TextColor**: Primary text color
- **HighlightColor**: Accent color for buttons, links, and highlights
- **BorderColor**: Border and divider color
- **SurfaceColor**: Surface/card background color
- **SecondaryTextColor**: Secondary text and labels color
- **ButtonTextColor**: Text color for buttons (typically white or black based on highlight color)
- **HeaderColor**: Background color for headers and toolbar areas

## Font Properties

- **fontFamily**: Font family name (e.g., "Segoe UI", "Arial", "Consolas")
- **fontSize**: Font size in points (e.g., 12.0)

## Color Format

All colors must be in hexadecimal format with a `#` prefix:
- `#RRGGBB` (e.g., `#FF0000` for red)
- `#AARRGGBB` (e.g., `#80FF0000` for semi-transparent red)

## Theme File Naming

- Theme files must have a `.json` extension
- The filename (without extension) will be used as the theme name if the `name` property is not specified
- Theme names are case-insensitive

## Examples

### Light Theme
```json
{
  "name": "Light",
  "displayName": "Light",
  "description": "Clean light theme",
  "colors": {
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

### Dark Theme
```json
{
  "name": "Dark",
  "displayName": "Dark",
  "description": "Easy on the eyes dark theme",
  "colors": {
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

## Loading Themes

After adding a theme file to this folder:
1. Restart RestartIt
2. Go to Settings → Appearance
3. Select your theme from the "Theme Preset" dropdown

## Saving and Deleting Themes

You can save and delete themes directly from the application:

- **Save Theme**: Customize colors and settings, then click "Save Theme..." in Appearance settings
- **Delete Theme**: Select a custom theme and click "Delete Theme..." (default themes cannot be deleted)

## Notes

- Invalid theme files will be skipped (check the debug console for errors)
- If a required color is missing, the theme will not be loaded
- Theme files are loaded when the application starts
- The "Custom" option allows you to manually set colors without creating a theme file
- Default themes (Light and Dark) are protected and cannot be deleted
- Theme names must be unique (case-insensitive)

