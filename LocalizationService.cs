using System;
using System.Collections.Generic;
using System.IO;
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
            return new List<LanguageInfo>
            {
                new LanguageInfo { Code = "en", Name = "English" },
                new LanguageInfo { Code = "nl", Name = "Nederlands" },
                new LanguageInfo { Code = "fr", Name = "Français" },
                new LanguageInfo { Code = "de", Name = "Deutsch" }
            };
        }
    }

    public class LanguageInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
