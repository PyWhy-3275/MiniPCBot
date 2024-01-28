using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Log = Serilog.Log;

namespace MiniPC.Discord.Services;

public class LocalizationService
{
    private static Dictionary<string, Dictionary<string, string>> _translations;

    public LocalizationService()
    {
        _translations = new Dictionary<string, Dictionary<string, string>>();

        // Load translations for each language
        LoadTranslations("en.json");
        LoadTranslations("ru.json");
        // Add more languages as needed
    }

    private static void LoadTranslations(string fileName)
    {
        try
        {
            string jsonContent = File.ReadAllText(fileName);
            var languageTranslations = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

            // Assuming the file name follows the pattern "{language}.json"
            string languageCode = Path.GetFileNameWithoutExtension(fileName);

            _translations[languageCode] = languageTranslations;
        }
        catch (Exception ex)
        {
            Log.Error($"Error loading translations from file '{fileName}': {ex.Message}");
            // Handle the exception as needed (e.g., log, throw, etc.)
        }
    }

    public string GetTranslation(string language, string key, params object[] args)
    {
        if (_translations.TryGetValue(language, out var languageTranslations) &&
            languageTranslations.TryGetValue(key, out var translation))
        {
            return string.Format(translation, args);
        }

        // If translation is not found, return a default or fallback message
        return $"[Translation not found for key '{key}' in language '{language}']";
    }
}