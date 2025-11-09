using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RestartIt
{
    /// <summary>
    /// Singleton service for managing application localization (i18n).
    /// Loads translations from JSON files in the Localization folder.
    /// Provides English fallback for missing translations.
    /// </summary>
    public class LocalizationService
    {
        private static LocalizationService _instance;
        private Dictionary<string, string> _translations;
        private Dictionary<string, string> _englishTranslations; // Fallback translations
        private string _currentLanguage;

        /// <summary>
        /// Gets the singleton instance of LocalizationService.
        /// </summary>
        public static LocalizationService Instance => _instance ??= new LocalizationService();

        /// <summary>
        /// Event raised when the language is changed.
        /// Subscribers should update their UI elements.
        /// </summary>
        public event EventHandler LanguageChanged;

        private LocalizationService()
        {
            _translations = new Dictionary<string, string>();
            _englishTranslations = new Dictionary<string, string>();
            _currentLanguage = "en";
            
            // Load English translations as fallback
            LoadEnglishFallback();
        }

        /// <summary>
        /// Loads English translations as a fallback for missing keys.
        /// </summary>
        private void LoadEnglishFallback()
        {
            try
            {
                string englishPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", "en.json");
                if (File.Exists(englishPath))
                {
                    string json = File.ReadAllText(englishPath);
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        _englishTranslations = new Dictionary<string, string>();

                        foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                        {
                            if (property.Name == "_metadata")
                                continue;

                            if (property.Value.ValueKind == JsonValueKind.String)
                            {
                                _englishTranslations[property.Name] = property.Value.GetString();
                            }
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"English fallback loaded: {_englishTranslations.Count} translations");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading English fallback: {ex.Message}");
                _englishTranslations = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Loads translations for the specified language code.
        /// Falls back to English if the language file is not found.
        /// </summary>
        /// <param name="languageCode">The two-letter language code (e.g., "en", "de", "fr")</param>
        public void LoadLanguage(string languageCode)
        {
            System.Diagnostics.Debug.WriteLine($"LoadLanguage called with code: {languageCode}");
            try
            {
                string localizationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", $"{languageCode}.json");
                System.Diagnostics.Debug.WriteLine($"Looking for language file at: {localizationPath}");

                if (!File.Exists(localizationPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Language file not found, falling back to English");
                    // Fallback to English if language file not found
                    languageCode = "en";
                    localizationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", $"{languageCode}.json");
                }

                if (File.Exists(localizationPath))
                {
                    string json = File.ReadAllText(localizationPath);

                    // Parse JSON and extract only string key-value pairs (skip _metadata object)
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        _translations = new Dictionary<string, string>();

                        foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                        {
                            // Skip _metadata since it's an object, not a string
                            if (property.Name == "_metadata")
                                continue;

                            // Only add if the value is a string
                            if (property.Value.ValueKind == JsonValueKind.String)
                            {
                                _translations[property.Name] = property.Value.GetString();
                            }
                        }
                    }

                    _currentLanguage = languageCode;
                    System.Diagnostics.Debug.WriteLine($"Language loaded successfully: {languageCode}, Translations count: {_translations.Count}");
                    System.Diagnostics.Debug.WriteLine($"Firing LanguageChanged event, subscribers: {LanguageChanged?.GetInvocationList().Length ?? 0}");
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Language file still not found after fallback!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading language file: {ex.Message}");
                _translations = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Gets a localized string for the given key.
        /// Falls back to English translation if the key is missing in the current language.
        /// Falls back to defaultValue or the key itself if not found in English either.
        /// </summary>
        /// <param name="key">The translation key</param>
        /// <param name="defaultValue">Optional default value if key is not found in current language or English</param>
        /// <returns>The translated string, English fallback, defaultValue, or the key itself</returns>
        public string GetString(string key, string defaultValue = null)
        {
            // First, try the current language
            if (_translations.TryGetValue(key, out string value))
            {
                return value;
            }

            // If not found in current language and current language is not English, try English fallback
            if (_currentLanguage != "en" && _englishTranslations.TryGetValue(key, out string englishValue))
            {
                System.Diagnostics.Debug.WriteLine($"Translation key '{key}' not found in {_currentLanguage}, using English fallback");
                return englishValue;
            }

            // If not found in English either (or current language is English), use defaultValue or the key itself
            System.Diagnostics.Debug.WriteLine($"Translation key '{key}' not found in {_currentLanguage} or English, using default");
            return defaultValue ?? key;
        }

        /// <summary>
        /// Gets the currently loaded language code.
        /// </summary>
        public string CurrentLanguage => _currentLanguage;

        /// <summary>
        /// Scans the Localization folder and returns a list of available languages.
        /// Languages are discovered from JSON files with _metadata sections.
        /// </summary>
        /// <returns>List of available languages, sorted by native name</returns>
        public static List<LanguageInfo> GetAvailableLanguages()
        {
            var languages = new List<LanguageInfo>();

            try
            {
                string localizationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization");

                if (!Directory.Exists(localizationFolder))
                {
                    System.Diagnostics.Debug.WriteLine("Localization folder not found, returning empty language list");
                    return languages;
                }

                // Scan all .json files in Localization folder
                var jsonFiles = Directory.GetFiles(localizationFolder, "*.json");

                foreach (var jsonFile in jsonFiles)
                {
                    try
                    {
                        string json = File.ReadAllText(jsonFile);

                        // Try to parse the entire JSON to check for _metadata
                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            LanguageInfo langInfo = null;

                            // Check if _metadata exists
                            if (doc.RootElement.TryGetProperty("_metadata", out JsonElement metadataElement))
                            {
                                // Parse metadata
                                var metadata = JsonSerializer.Deserialize<LanguageMetadata>(metadataElement.GetRawText());

                                if (metadata != null && !string.IsNullOrEmpty(metadata.code))
                                {
                                    langInfo = new LanguageInfo
                                    {
                                        Code = metadata.code,
                                        Name = metadata.name ?? metadata.code,
                                        NativeName = metadata.nativeName ?? metadata.name ?? metadata.code,
                                        Icon = metadata.icon
                                    };
                                }
                            }
                            else
                            {
                                // Fallback: Derive from filename (backwards compatibility)
                                string fileName = Path.GetFileNameWithoutExtension(jsonFile);
                                langInfo = new LanguageInfo
                                {
                                    Code = fileName,
                                    Name = fileName,
                                    NativeName = fileName,
                                    Icon = null
                                };
                                System.Diagnostics.Debug.WriteLine($"No metadata found in {jsonFile}, using filename as code: {fileName}");
                            }

                            if (langInfo != null)
                            {
                                languages.Add(langInfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading language file {jsonFile}: {ex.Message}");
                        // Continue to next file
                    }
                }

                // Sort by NativeName for better UX
                languages = languages.OrderBy(l => l.NativeName).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error scanning language files: {ex.Message}");
            }

            // Ensure at least English is available as fallback
            if (languages.Count == 0)
            {
                languages.Add(new LanguageInfo
                {
                    Code = "en",
                    Name = "English",
                    NativeName = "English",
                    Icon = "🇬🇧"
                });
            }

            return languages;
        }
    }

    /// <summary>
    /// Represents information about an available language.
    /// </summary>
    public class LanguageInfo
    {
        /// <summary>Gets or sets the two-letter language code (e.g., "en", "de").</summary>
        public string Code { get; set; }
        /// <summary>Gets or sets the English name of the language.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets the native name of the language (e.g., "Deutsch" for German).</summary>
        public string NativeName { get; set; }
        /// <summary>Gets or sets the flag emoji icon for the language (e.g., "🇬🇧").</summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets the display name for the language dropdown: "{icon} {nativeName}" if icon exists, otherwise just nativeName.
        /// </summary>
        public string DisplayName => string.IsNullOrEmpty(Icon) ? NativeName : $"{Icon} {NativeName}";
    }

    /// <summary>
    /// Internal class for parsing language metadata from JSON files.
    /// </summary>
    internal class LanguageMetadata
    {
        public string code { get; set; }
        public string name { get; set; }
        public string nativeName { get; set; }
        public string icon { get; set; }
    }
}
