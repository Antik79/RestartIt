using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RestartIt
{
    public class LocalizationService
    {
        private static LocalizationService _instance;
        private Dictionary<string, string> _translations;
        private string _currentLanguage;

        public static LocalizationService Instance => _instance ??= new LocalizationService();

        public event EventHandler LanguageChanged;

        private LocalizationService()
        {
            _translations = new Dictionary<string, string>();
            _currentLanguage = "en";
        }

        public void LoadLanguage(string languageCode)
        {
            try
            {
                string localizationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", $"{languageCode}.json");

                if (!File.Exists(localizationPath))
                {
                    // Fallback to English if language file not found
                    languageCode = "en";
                    localizationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", $"{languageCode}.json");
                }

                if (File.Exists(localizationPath))
                {
                    string json = File.ReadAllText(localizationPath);
                    _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                    _currentLanguage = languageCode;
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading language file: {ex.Message}");
                _translations = new Dictionary<string, string>();
            }
        }

        public string GetString(string key, string defaultValue = null)
        {
            if (_translations.TryGetValue(key, out string value))
            {
                return value;
            }
            return defaultValue ?? key;
        }

        public string CurrentLanguage => _currentLanguage;

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

    public class LanguageInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Icon { get; set; }

        // Display name for dropdown: "{icon} {nativeName}" if icon exists, otherwise just nativeName
        public string DisplayName => string.IsNullOrEmpty(Icon) ? NativeName : $"{Icon} {NativeName}";
    }

    // Internal class for parsing metadata from JSON files
    internal class LanguageMetadata
    {
        public string code { get; set; }
        public string name { get; set; }
        public string nativeName { get; set; }
        public string icon { get; set; }
    }
}
