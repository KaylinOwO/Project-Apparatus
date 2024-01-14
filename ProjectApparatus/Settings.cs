using UnityEngine;
using System.Collections.Generic;
using GameNetcodeStuff;
using System.IO;
using System.Reflection;
using System.Text;

namespace ProjectApparatus
{
    [System.Serializable]
    public class SettingsData
    {
        public readonly string version = "1.2.0";

        /* ESP */
        public bool b_EnableESP;
        public bool b_ItemESP;
        public bool b_EnemyESP;
        public bool b_PlayerESP;
        public bool b_DoorESP;
        public bool b_LandmineESP;
        public bool b_TurretESP;
        public bool b_ShipESP;
        public bool b_SteamHazard;
        public bool b_DisplayHP, b_DisplaySpeaking, b_DisplayWorth, b_DisplayDistance;
        public bool b_ItemDistanceLimit = true, b_MineDistanceLimit = true, b_TurretDistanceLimit = true, b_EnemyDistanceLimit;
        public float fl_ItemDistanceLimit = 80f, fl_MineDistanceLimit = 80f, fl_TurretDistanceLimit = 80f, fl_EnemyDistanceLimit = 120f;

        /* Self */
        public bool b_GodMode;
        public bool b_Invisibility;
        public bool b_InfiniteStam, b_InfiniteCharge, b_InfiniteZapGun, b_InfiniteShotgunAmmo, b_InfiniteItems, b_UnlimitedGrabDistance;
        public bool b_RemoveWeight;
        public bool b_RemoveVisor;
        public bool b_CameraResolution;
        public bool b_OneHandAllObjects;
        public bool b_DisableFallDamage;
        public bool b_DisableInteractCooldowns;
        public bool b_InstantInteractions;
        public bool b_NightVision;
        public bool b_FastLadderClimbing;
        public bool b_PlaceAnywhere;
        public bool b_InteractThroughWalls;
        public bool b_TauntSlide;
        public bool b_HearEveryone;
        public bool b_ChargeAnyItem;
        public bool b_WalkSpeed;
        public int i_NightVision = 30;
        public int i_WalkSpeed = 1;
        public bool b_SprintSpeed;
        public int i_SprintSpeed = 1;
        public bool b_JumpHeight;
        public int i_JumpHeight = 1;
        public int i_FieldofView = 66;

        /* Misc */
        public bool b_AlwaysShowClock;
        public bool b_AllJetpacksExplode;
        public bool b_LightShow;
        public bool b_NoMoreCredits;
        public bool b_SensitiveLandmines;
        public bool b_LandmineEarrape;
        public bool b_ForceCloseDoors;
        public bool b_TerminalNoisemaker;
        public bool b_Noclip;
        public bool b_AnonChatSpam;
        public float fl_NoclipSpeed = 7f;
        public float fl_ThirdpersonDistance = 2f;
        public string str_TerminalSignal = LocalizationManager.GetString("hello_world");
        public string str_ChatMessage = LocalizationManager.GetString("hello_world");
        public string str_MoneyToGive = "0";
        public string str_QuotaFulfilled = "0", str_Quota = "130";

        /* Graphics */
        public bool b_DisableFog, b_DisableBloom, b_DisableDepthOfField, b_DisableVignette, b_DisableFilmGrain, b_DisableExposure;

        /* Settings*/
        public bool b_DisplayGroupCredits = true;
        public bool b_DisplayLootInShip = true;
        public bool b_DisplayQuota = true;
        public bool b_DisplayDaysLeft = true;
        public bool b_CenteredIndicators = false;
        public bool b_Crosshair;
        public bool b_DeadPlayers;
        public bool b_Tooltips = true;

        public Color c_Theme = new Color(1f, 1f, 1f, 1f);
        public Color c_Spectator = new Color(0.996f, 0.635f, 0.667f, 1.0f);
        public Color c_Valve = new Color(1f, 0.49f, 0.851f, 1f);
        public Color c_Enemy = new Color(0.996f, 0.635f, 0.667f, 1.0f);
        public Color c_Turret = new Color(0.996f, 0.635f, 0.667f, 1.0f);
        public Color c_Landmine = new Color(0.996f, 0.635f, 0.667f, 1.0f);
        public Color c_Player = new Color(0.698f, 0.808f, 0.996f, 1.0f);
        public Color c_Door = new Color(0.74f, 0.74f, 1f, 1f);
        public Color c_Loot = new Color(0.5f, 1f, 1f, 1f);
        public Color c_smallLoot = new Color(0.518f, 0.682f, 0.729f, 1f);
        public Color c_medLoot = new Color(0.5f, 0.816f, 1f, 1f);
        public Color c_bigLoot = new Color(1f, 0.629f, 1f, 1f);

        public int keyNoclip = new int();
        public int keyThirdperson = new int();
    }

    public class Settings
    {
        private string settingsFilePath;
        private const string settingsFileName = "PASettings.json";

        public Settings()
        {
            settingsFilePath = Path.Combine(Application.persistentDataPath, settingsFileName);
            LoadSettings();
        }

        public static Settings Instance
        {
            get
            {
                if (Settings.instance == null)
                {
                    Settings.instance = new Settings();
                }
                return Settings.instance;
            }
        }

        /* UI */
        public static float TEXT_HEIGHT = 30f;
        public Rect windowRect = new Rect(50f, 50f, 545f, 400f);
        public bool b_isMenuOpen;

        public static class Changelog
        {
            public static StringBuilder changes;

            public static void ReadChanges()
            {
                changes = new StringBuilder(ResourceReader.ReadTextFile("ProjectApparatus.Resources.Changelog.txt"));
            }
        }

        public static class Credits
        {
            public static StringBuilder credits;

            public static void ReadCredits()
            {
                credits = new StringBuilder(ResourceReader.ReadTextFile("ProjectApparatus.Resources.Credits.txt"));
            }
        }

        /* Players */
        public Dictionary<PlayerControllerB, bool> b_DemiGod = new Dictionary<PlayerControllerB, bool>();
        public Dictionary<PlayerControllerB, bool> b_SpamObjects = new Dictionary<PlayerControllerB, bool>();
        public Dictionary<PlayerControllerB, bool> b_SpamChat = new Dictionary<PlayerControllerB, bool>();
        public bool b_HideObjects = false;
        public string str_DamageToGive = "1", str_HealthToHeal = "1", str_ChatAsPlayer = LocalizationManager.GetString("hello_world");

        /* SettingsData */
        public SettingsData settingsData = new SettingsData();

        public static class ResourceReader
        {
            public static string ReadTextFile(string resourceName)
            {
                string text = string.Empty;
                using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    using (StreamReader streamReader = new StreamReader(manifestResourceStream))
                    {
                        text = streamReader.ReadToEnd();
                    }
                }
                return text;
            }
        }

        public void InitializeDictionaries(PlayerControllerB key)
        {
            if (!b_DemiGod.ContainsKey(key))
                b_DemiGod[key] = false;
            if (!b_SpamObjects.ContainsKey(key))
                b_SpamObjects[key] = false;
            if (!b_SpamChat.ContainsKey(key))
                b_SpamChat[key] = false;
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                settingsData = JsonUtility.FromJson<SettingsData>(json);
            }
            else
            {
                settingsData = new SettingsData();
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            string json = JsonUtility.ToJson(settingsData, true);
            File.WriteAllText(settingsFilePath, json);
        }

        private static Settings instance;
    }
}
