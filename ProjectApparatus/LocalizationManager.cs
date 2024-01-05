using ProjectApparatus.Lang;
using ProjectApparatus.Properties;
using System;
using System.Resources;

public class LocalizationManager
{
    private static ResourceManager resourceManager;
    public static string currentLanguage = "en_US";

    static LocalizationManager()
    {
        SetLanguage(currentLanguage);
    }

    public static void SetLanguage(string language)
    {
        switch (language)
        {
            case "en_US":
                resourceManager = new ResourceManager(typeof(en_US));
                break;
            case "ru_RU":
                resourceManager = new ResourceManager(typeof(ru_RU));
                break;
            // add next language here
            default:
                throw new ArgumentException("Unsupported language: " + language);
        }
        currentLanguage = language;
    }

    public static string GetString(string key)
    {
        try
        {
            return resourceManager.GetString(key);
        }
        catch (Exception)
        {
            return "Missing translation for key: " + key;
        }
    }
}