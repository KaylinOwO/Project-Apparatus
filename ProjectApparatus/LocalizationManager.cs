using ProjectApparatus.Lang;
using ProjectApparatus.Properties;
using System;
using System.Resources;

//
//How to add new language:
//1. Create new resource file in Lang folder, like en_US.resx
//2. Add new language to LocalizationManager.cs
//3. Add new language to Hacks.cs in dictionary
//


public class LocalizationManager
{
    private static ResourceManager resourceManager;
    // for default language use English
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
          // add next language here, for example:
          //case "ts_TS":
          //    resourceManager = new ResourceManager(typeof(ts_TS));
          //    break;
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