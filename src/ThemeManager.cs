using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;

namespace ProjectApparatus
{
    public class ThemeManager //credit https://github.com/Coopyy/EgguWare-Unturned
    {
        public static SettingsData settings = Settings.Instance.settingsData;
        //public static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        public static GUISkin skin;
        public static GUISkin vanillaSkin = GUI.skin;
        public static void LoadExisting()
        {
            if (!Directory.Exists(Application.persistentDataPath + "/Project Apparatus/Themes/"))
                Directory.CreateDirectory(Application.persistentDataPath + "/Project Apparatus/Themes/");
            //foreach (Shader s in Bundle.LoadAllAssets<Shader>()) //example how we could use shaders
            //    Shaders.Add(s.name, s);
            if (!String.IsNullOrEmpty(settings.str_GUISkin)) //auto load skin on load and if not valid we just dont do anything leaving normal unity gui
                LoadThemeFromName(settings.str_GUISkin);
        }
        public static void LoadThemeFromName(string name)
        {
            if (File.Exists(Application.persistentDataPath + "/Project Apparatus/Themes/" + name + ".unity3d"))
            {
                AssetBundle tempAsset = AssetBundle.LoadFromMemory(File.ReadAllBytes(Application.persistentDataPath + "/Project Apparatus/Themes/" + name + ".unity3d"));
                skin = tempAsset.LoadAllAssets<GUISkin>().First();
                tempAsset.Unload(false);
                settings.str_GUISkin = name;
            }
            else
            {
                skin = vanillaSkin;
                settings.str_GUISkin = "";
            }
        }
        public static List<string> GetThemes(bool Extensions = false)
        {
            List<string> files = new List<string>();
            DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath + "/Project Apparatus/Themes/");
            FileInfo[] Files = d.GetFiles("*.unity3d");
            foreach (FileInfo file in Files)
            {
                if (Extensions)
                    files.Add(file.Name.Substring(0, file.Name.Length));
                else
                    files.Add(file.Name.Substring(0, file.Name.Length - 8)); // - 8 removes .unity3d
            }
            return files;
        }
    }
}