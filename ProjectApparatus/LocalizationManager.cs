using DunGen;
using ProjectApparatus;
using ProjectApparatus.Lang;
using ProjectApparatus.Properties;
using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

//
//How to add new language:
//1. Create new resource file in Lang folder, like en_US.resx
//2. Add new language to LocalizationManager.cs
//3. Add new language to Hacks.cs in dictionary
//


public class LocalizationManager
{
    private static ResourceManager resourceManager;

    static LocalizationManager()
    {
        SetLanguage(Settings.Instance.settingsData.str_Language);
    }

    public static Dictionary<string, Type> Languages = new Dictionary<string, Type>
    {
        { "en_US", typeof(en_US) },
        { "ru_RU", typeof(ru_RU) },
        { "zh_CN", typeof(zh_CN) }
            //new languages here, for example:
            //{"ts_TS", "typeof(ts_TS)" }
    };

    public static void SetLanguage(string language)
    {
        if (Languages.ContainsKey(language))
        {
            resourceManager = new ResourceManager(Languages[language]);
        }
        else
        {
            resourceManager = new ResourceManager(typeof(en_US));
            Debug.LogError("Unsupported language: " + language);
        }
    }

    public static string TryGetString(string prefix, string key)
    {
        try
        {
            string value = resourceManager.GetString(prefix + key);
            return value == null ? key : value.TrimEnd();
        }
        catch (Exception)
        {
            return key;
        }
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