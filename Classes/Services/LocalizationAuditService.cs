using System.Text.Json;

namespace HabitTrackerApp.Classes.Services
{
    public class LocalizationAuditResult
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<string> MissingKeys { get; set; } = new();
        public List<string> EmptyKeys { get; set; } = new();

        public int MissingCount => MissingKeys.Count;
        public int EmptyCount => EmptyKeys.Count;
        public bool NeedsUpdate => MissingCount > 0 || EmptyCount > 0;
    }

    public static class LocalizationAuditService
    {
        private const string MasterLanguageCode = "en";

        public static async Task<List<LocalizationAuditResult>> AuditAllAsync()
        {
            var results = new List<LocalizationAuditResult>();

            var master = await LoadLocalizationAsync(MasterLanguageCode);
            var languages = LanguageService.GetLanguages();

            foreach (var language in languages)
            {
                var code = language.Key;
                var displayName = language.Value;

                if (code == MasterLanguageCode)
                    continue;

                var current = await LoadLocalizationAsync(code);

                var result = new LocalizationAuditResult
                {
                    LanguageCode = code,
                    DisplayName = displayName
                };

                foreach (var masterEntry in master)
                {
                    if (!current.TryGetValue(masterEntry.Key, out var value))
                    {
                        result.MissingKeys.Add(masterEntry.Key);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        result.EmptyKeys.Add(masterEntry.Key);
                    }
                }

                results.Add(result);
            }

            return results;
        }

        public static async Task<LocalizationAuditResult?> AuditLanguageAsync(string languageCode)
        {
            var languages = LanguageService.GetLanguages();

            if (!languages.TryGetValue(languageCode, out var displayName))
                return null;

            if (languageCode == MasterLanguageCode)
            {
                return new LocalizationAuditResult
                {
                    LanguageCode = languageCode,
                    DisplayName = displayName
                };
            }

            var master = await LoadLocalizationAsync(MasterLanguageCode);
            var current = await LoadLocalizationAsync(languageCode);

            var result = new LocalizationAuditResult
            {
                LanguageCode = languageCode,
                DisplayName = displayName
            };

            foreach (var masterEntry in master)
            {
                if (!current.TryGetValue(masterEntry.Key, out var value))
                {
                    result.MissingKeys.Add(masterEntry.Key);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    result.EmptyKeys.Add(masterEntry.Key);
                }
            }

            return result;
        }

        private static async Task<Dictionary<string, string>> LoadLocalizationAsync(string cultureName)
        {
            try
            {
                var filePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "Resources",
                    "Localizations",
                    $"{cultureName}.json");

                if (!File.Exists(filePath))
                    return new Dictionary<string, string>();

                using var stream = File.OpenRead(filePath);
                var dict = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream);

                return dict ?? new Dictionary<string, string>();
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
    }
}