using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RestartIt
{
    /// <summary>
    /// Manages loading and caching of theme definitions from JSON files.
    /// Provides methods to discover, load, and validate themes.
    /// </summary>
    public class ThemeManager
    {
        private static ThemeManager _instance;
        private Dictionary<string, ThemeDefinition> _themes;
        private readonly string _themesPath;
        private readonly object _lock = new object();

        /// <summary>
        /// Gets the singleton instance of ThemeManager.
        /// </summary>
        public static ThemeManager Instance => _instance ??= new ThemeManager();

        /// <summary>
        /// Event raised when themes are reloaded.
        /// </summary>
        public event EventHandler ThemesReloaded;

        private ThemeManager()
        {
            // Themes folder in application directory
            _themesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Themes");
            _themes = new Dictionary<string, ThemeDefinition>();
            LoadThemes();
        }

        /// <summary>
        /// Gets all available theme names.
        /// </summary>
        public IEnumerable<string> GetThemeNames()
        {
            lock (_lock)
            {
                return _themes.Keys.ToList();
            }
        }

        /// <summary>
        /// Gets all available theme definitions.
        /// </summary>
        public IEnumerable<ThemeDefinition> GetThemes()
        {
            lock (_lock)
            {
                return _themes.Values.ToList();
            }
        }

        /// <summary>
        /// Gets a theme by name (case-insensitive).
        /// </summary>
        /// <param name="name">The theme name</param>
        /// <returns>The theme definition, or null if not found</returns>
        public ThemeDefinition GetTheme(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            lock (_lock)
            {
                // Case-insensitive lookup
                var key = _themes.Keys.FirstOrDefault(k => 
                    string.Equals(k, name, StringComparison.OrdinalIgnoreCase));
                
                return key != null ? _themes[key] : null;
            }
        }

        /// <summary>
        /// Reloads all themes from the Themes folder.
        /// </summary>
        public void ReloadThemes()
        {
            LoadThemes();
            ThemesReloaded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Saves a theme definition to a JSON file in the Themes folder.
        /// </summary>
        /// <param name="theme">The theme definition to save</param>
        /// <returns>True if saved successfully, false otherwise</returns>
        public bool SaveTheme(ThemeDefinition theme)
        {
            if (theme == null || !theme.IsValid())
                return false;

            try
            {
                lock (_lock)
                {
                    // Ensure themes directory exists
                    if (!Directory.Exists(_themesPath))
                    {
                        Directory.CreateDirectory(_themesPath);
                    }

                    // Create filename from theme name (sanitize for filesystem)
                    var fileName = SanitizeFileName(theme.Name) + ".json";
                    var filePath = Path.Combine(_themesPath, fileName);

                    // Serialize theme to JSON with pretty formatting
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    var json = JsonSerializer.Serialize(theme, options);
                    File.WriteAllText(filePath, json);

                    // Reload themes to include the new one
                    LoadThemes();
                    ThemesReloaded?.Invoke(this, EventArgs.Empty);

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving theme: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads a theme from a file path (can be outside the Themes folder).
        /// </summary>
        /// <param name="filePath">Path to the theme JSON file</param>
        /// <returns>The loaded theme definition, or null if failed</returns>
        public ThemeDefinition LoadThemeFromPath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return null;

            try
            {
                var theme = LoadThemeFromFile(filePath);
                return theme;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading theme from {filePath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if a theme is a default theme (Light or Dark).
        /// </summary>
        /// <param name="themeName">The theme name to check</param>
        /// <returns>True if it's a default theme, false otherwise</returns>
        public bool IsDefaultTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
                return false;

            return string.Equals(themeName, "Light", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(themeName, "Dark", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Deletes a theme file from the Themes folder.
        /// </summary>
        /// <param name="themeName">The name of the theme to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public bool DeleteTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName))
                return false;

            // Prevent deletion of default themes
            if (IsDefaultTheme(themeName))
                return false;

            try
            {
                lock (_lock)
                {
                    // Find the theme file
                    var fileName = SanitizeFileName(themeName) + ".json";
                    var filePath = Path.Combine(_themesPath, fileName);

                    if (!File.Exists(filePath))
                    {
                        System.Diagnostics.Debug.WriteLine($"Theme file not found: {filePath}");
                        return false;
                    }

                    // Delete the file
                    File.Delete(filePath);

                    // Reload themes to remove it from the list
                    LoadThemes();
                    ThemesReloaded?.Invoke(this, EventArgs.Empty);

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting theme: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sanitizes a string to be safe for use as a filename.
        /// </summary>
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "Theme";

            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = fileName;
            
            foreach (var c in invalidChars)
            {
                sanitized = sanitized.Replace(c, '_');
            }

            // Remove leading/trailing spaces and dots
            sanitized = sanitized.Trim(' ', '.');

            // Ensure it's not empty
            if (string.IsNullOrWhiteSpace(sanitized))
                sanitized = "Theme";

            return sanitized;
        }

        /// <summary>
        /// Loads all theme files from the Themes folder.
        /// </summary>
        private void LoadThemes()
        {
            lock (_lock)
            {
                _themes.Clear();

                if (!Directory.Exists(_themesPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Themes directory not found: {_themesPath}");
                    return;
                }

                var themeFiles = Directory.GetFiles(_themesPath, "*.json", SearchOption.TopDirectoryOnly);

                foreach (var filePath in themeFiles)
                {
                    try
                    {
                        var theme = LoadThemeFromFile(filePath);
                        if (theme != null && theme.IsValid())
                        {
                            _themes[theme.Name] = theme;
                            System.Diagnostics.Debug.WriteLine($"Loaded theme: {theme.Name} from {Path.GetFileName(filePath)}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid theme file: {Path.GetFileName(filePath)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading theme from {Path.GetFileName(filePath)}: {ex.Message}");
                    }
                }

                // Ensure at least Light and Dark themes exist (create defaults if missing)
                if (!_themes.ContainsKey("Light"))
                {
                    _themes["Light"] = CreateDefaultLightTheme();
                }
                if (!_themes.ContainsKey("Dark"))
                {
                    _themes["Dark"] = CreateDefaultDarkTheme();
                }
            }
        }

        /// <summary>
        /// Loads a theme definition from a JSON file.
        /// </summary>
        private ThemeDefinition LoadThemeFromFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };

                var theme = JsonSerializer.Deserialize<ThemeDefinition>(json, options);
                
                // If Name is not set, use filename without extension
                if (theme != null && string.IsNullOrWhiteSpace(theme.Name))
                {
                    theme.Name = Path.GetFileNameWithoutExtension(filePath);
                }
                
                // If DisplayName is not set, use Name
                if (theme != null && string.IsNullOrWhiteSpace(theme.DisplayName))
                {
                    theme.DisplayName = theme.Name;
                }

                return theme;
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON parsing error in {Path.GetFileName(filePath)}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reading theme file {Path.GetFileName(filePath)}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates a default light theme if no theme file is found.
        /// </summary>
        private ThemeDefinition CreateDefaultLightTheme()
        {
            return new ThemeDefinition
            {
                Name = "Light",
                DisplayName = "Light",
                Description = "Modern light theme",
                Author = "RestartIt",
                Version = "1.0",
                FontFamily = "Segoe UI",
                FontSize = 12.0,
                Colors = new ThemeColors
                {
                    BackgroundColor = "#F5F5F5",
                    TextColor = "#212121",
                    HighlightColor = "#0078D4",
                    BorderColor = "#E0E0E0",
                    SurfaceColor = "#FFFFFF",
                    SecondaryTextColor = "#757575"
                }
            };
        }

        /// <summary>
        /// Creates a default dark theme if no theme file is found.
        /// </summary>
        private ThemeDefinition CreateDefaultDarkTheme()
        {
            return new ThemeDefinition
            {
                Name = "Dark",
                DisplayName = "Dark",
                Description = "Modern dark theme",
                Author = "RestartIt",
                Version = "1.0",
                FontFamily = "Segoe UI",
                FontSize = 12.0,
                Colors = new ThemeColors
                {
                    BackgroundColor = "#1E1E1E",
                    TextColor = "#E0E0E0",
                    HighlightColor = "#0078D4",
                    BorderColor = "#404040",
                    SurfaceColor = "#2D2D2D",
                    SecondaryTextColor = "#B0B0B0"
                }
            };
        }
    }
}

