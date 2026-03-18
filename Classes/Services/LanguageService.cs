using System.Text.Json;

namespace HabitTrackerApp.Classes.Services
{
    public static class LanguageService
    {
        private static Dictionary<string, string> _languages = new(StringComparer.OrdinalIgnoreCase);
        private static List<string> _warnings = new();

        public static async Task LoadLanguagesAsync()
        {
            _warnings.Clear();

            var languagesPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Languages.json");
            var localizationsPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Localizations");

            if (!File.Exists(languagesPath))
            {
                _warnings.Add("Languages.json was not found.");
                _languages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return;
            }

            Dictionary<string, string> allKnownLanguages;

            try
            {
                using var stream = File.OpenRead(languagesPath);
                var dict = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream);

                allKnownLanguages = dict is null
                    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _warnings.Add($"Failed to read Languages.json: {ex.Message}");
                _languages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return;
            }

            if (!Directory.Exists(localizationsPath))
            {
                _warnings.Add("Resources/Localizations folder was not found.");
                _languages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return;
            }

            var availableCodes = Directory
                .GetFiles(localizationsPath, "*.json", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileNameWithoutExtension)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var filteredLanguages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var language in allKnownLanguages)
            {
                if (availableCodes.Contains(language.Key))
                {
                    filteredLanguages[language.Key] = language.Value;
                }
            }

            foreach (var code in availableCodes)
            {
                if (!allKnownLanguages.ContainsKey(code))
                {
                    _warnings.Add($"Localization file '{code}.json' was found, but the language is not defined in Languages.json.");
                }
            }

            if (!availableCodes.Contains("en"))
            {
                _warnings.Add("en.json was not found. English fallback localization will be unavailable.");
            }

            _languages = filteredLanguages;
        }

        public static IReadOnlyDictionary<string, string> GetLanguages()
        {
            return _languages;
        }

        public static IReadOnlyList<string> GetWarnings()
        {
            return _warnings.AsReadOnly();
        }

        public static bool IsLanguageSupported(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return _languages.ContainsKey(code);
        }
    }
}