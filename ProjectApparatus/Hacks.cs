using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Windows.Forms;
using static GameObjectManager;

namespace ProjectApparatus
{

    internal class Hacks : MonoBehaviour
    {
        private static GUIStyle Style = null;
        private readonly SettingsData settingsData = Settings.Instance.settingsData;

        bool IsPlayerValid(PlayerControllerB plyer)
        {
            return (plyer != null &&
                    !plyer.disconnectedMidGame &&
                    !plyer.playerUsername.Contains("Player #"));
        }

        public void OnGUI()
        {
            if (!Settings.Instance.b_isMenuOpen && Event.current.type != EventType.Repaint)
                return;

            UI.Reset();

            Color darkBackground = new Color(23f / 255f, 23f / 255f, 23f / 255f, 1f);

            GUI.backgroundColor = darkBackground;
            GUI.contentColor = Color.white;

            Style = new GUIStyle(GUI.skin.label);
            Style.normal.textColor = Color.white;
            Style.fontStyle = FontStyle.Bold;

            menuButton.Enable();
            unloadMenu.Enable();

            if (settingsData.b_EnableESP)
            {
                DisplayLoot();
                DisplayPlayers();
                DisplayDoors();
                DisplayLandmines();
                DisplayTurrets();
                DisplaySteamHazard();
                DisplayEnemyAI();
                DisplayShip();
                DisplayDeadPlayers();
            }

            Vector2 centeredPos = new Vector2(UnityEngine.Screen.width / 2f, UnityEngine.Screen.height / 2f);

            GUI.color = settingsData.c_Theme;

            if (settingsData.b_CenteredIndicators)
            {
                float iY = Settings.TEXT_HEIGHT;
                if (settingsData.b_DisplayGroupCredits && Instance.shipTerminal != null) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Group Credits: " + Instance.shipTerminal.groupCredits, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (settingsData.b_DisplayQuota && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Profit Quota: " + TimeOfDay.Instance.quotaFulfilled + "/" + TimeOfDay.Instance.profitQuota, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (settingsData.b_DisplayDaysLeft && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, "Days Left: " + TimeOfDay.Instance.daysUntilDeadline, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
            }

            string Watermark = "Project Apparatus";
            if (!settingsData.b_CenteredIndicators)
            {
                if (settingsData.b_DisplayGroupCredits && Instance.shipTerminal != null)
                    Watermark += $" | Group Credits: {Instance.shipTerminal.groupCredits}";
                if (settingsData.b_DisplayQuota && TimeOfDay.Instance)
                    Watermark += $" | Profit Quota: {TimeOfDay.Instance.quotaFulfilled} / {TimeOfDay.Instance.profitQuota}";
                if (settingsData.b_DisplayDaysLeft && TimeOfDay.Instance)
                    Watermark += $" | Days Left: {TimeOfDay.Instance.daysUntilDeadline}";
                Watermark += " | v" + settingsData.version;
            }

            Render.String(Style, 10f, 5f, 150f, Settings.TEXT_HEIGHT, Watermark, GUI.color);

            if (Settings.Instance.b_isMenuOpen)
            {
                Settings.Instance.windowRect = GUILayout.Window(0, Settings.Instance.windowRect, new GUI.WindowFunction(MenuContent), "Project Apparatus", Array.Empty<GUILayoutOption>());
            }

            if (settingsData.b_Crosshair)
            {
                Render.FilledCircle(centeredPos, 5, Color.black);
                Render.FilledCircle(centeredPos, 3, settingsData.c_Theme);
            }
        }

        private PlayerControllerB selectedPlayer = null;

        private void MenuContent(int windowID)
        {

            GUILayout.BeginHorizontal();
            UI.Tab("Start", ref UI.nTab, UI.Tabs.Start);
            UI.Tab("Self", ref UI.nTab, UI.Tabs.Self);
            UI.Tab("Misc", ref UI.nTab, UI.Tabs.Misc);
            UI.Tab("ESP", ref UI.nTab, UI.Tabs.ESP);
            UI.Tab("Players", ref UI.nTab, UI.Tabs.Players);
            UI.Tab("Graphics", ref UI.nTab, UI.Tabs.Graphics);
            UI.Tab("Upgrades", ref UI.nTab, UI.Tabs.Upgrades);
            UI.Tab("Settings", ref UI.nTab, UI.Tabs.Settings);
            GUILayout.EndHorizontal();


            UI.TabContents("Start", UI.Tabs.Start, () =>
            {
                GUILayout.Label($"Welcome to Project Apparatus v{settingsData.version}!\n\n" +
                                $"If you have suggestions, please create a pull request in the repo or reply to the UC thread.\n" +
                                $"If you find bugs, please provide some steps on how to reproduce the problem and create an issue or pull request in the repo or reply to the UC thread");
                GUILayout.Space(20f);
                GUILayout.Label($"Changelog {settingsData.version}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300f));
                GUILayout.TextArea(Settings.Changelog.changes.ToString(), GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
                GUILayout.Space(20f);
                GUILayout.Label($"Credits", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                GUILayout.Label(Settings.Credits.credits.ToString());
            });


            UI.TabContents("Self", UI.Tabs.Self, () =>
            {
                UI.Checkbox(ref settingsData.b_GodMode, "God Mode", "Prevents you from taking any damage.");
                UI.Checkbox(ref settingsData.b_InfiniteStam, "Infinite Stamina", "Prevents you from losing any stamina.");
                UI.Checkbox(ref settingsData.b_InfiniteCharge, "Infinite Charge", "Prevents your items from losing any charge.");
                UI.Checkbox(ref settingsData.b_InfiniteZapGun, "Infinite Zap Gun", "Infinitely stuns enemies with the zap-gun.");
                UI.Checkbox(ref settingsData.b_InfiniteShotgunAmmo, "Infinite Shotgun Ammo", "Prevents you from out of ammo.");
                UI.Checkbox(ref settingsData.b_InfiniteItems, "Infinite Item Use", "Allows you to infinitely use items like the gift box and stun grenade. (Buggy)");
                UI.Checkbox(ref settingsData.b_NightVision, "Night Vision", "Allows you to see in the dark.");
                UI.Checkbox(ref settingsData.b_InteractThroughWalls, "Interact Through Walls", "Allows you to interact with anything through walls.");
                UI.Checkbox(ref settingsData.b_UnlimitedGrabDistance, "No Grab Distance Limit", "Allows you to interact with anything no matter the distance.");
                UI.Checkbox(ref settingsData.b_OneHandAllObjects, "One Hand All Objects", "Allows you to one-hand any two-handed objects.");
                UI.Checkbox(ref settingsData.b_DisableInteractCooldowns, "Disable Interact Cooldowns", "Disables all interact cooldowns (e.g., noisemakers, toilets, etc).");
                UI.Checkbox(ref settingsData.b_InstantInteractions, "Instant Interactions", "Makes all hold interactions instantaneous.");
                UI.Checkbox(ref settingsData.b_PlaceAnywhere, "Place Anywhere", "Place objects from the ship anywhere you want.");
                UI.Checkbox(ref settingsData.b_TauntSlide, "Taunt Slide", "Allows you to emote and move at the same time.");
                UI.Checkbox(ref settingsData.b_FastLadderClimbing, "Fast Ladder Climbing", "Instantly climbs up ladders.");
                UI.Checkbox(ref settingsData.b_HearEveryone, "Hear Everyone", "Allows you to hear everyone no matter the distance.");
                UI.Checkbox(ref settingsData.b_ChargeAnyItem, "Charge Any Item", "Allows you to put any grabbable item in the charger.");
                UI.Checkbox(ref settingsData.b_WalkSpeed, $"Adjust Walk Speed ({settingsData.i_WalkSpeed})", "Allows you to modify your walk speed.");
                settingsData.i_WalkSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_WalkSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_SprintSpeed, $"Adjust Sprint Speed ({settingsData.i_SprintSpeed})", "Allows you to modify your sprint speed.");
                settingsData.i_SprintSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_SprintSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_JumpHeight, $"Jump Height ({settingsData.i_JumpHeight})", "Allows you to modify your jump height.");
                settingsData.i_JumpHeight = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_JumpHeight, 1, 100));
                UI.Button("Respawn", "Respawns you. You will be invisible to both players and enemies.", () =>
                {
                    ReviveLocalPlayer();
                });

                UI.Button("Teleport To Ship", "Teleports you into the ship.", () =>
                {
                    Instance.localPlayer.TeleportPlayer(Instance.shipRoom.transform.position);
                });

                GUILayout.BeginHorizontal();
                UI.Checkbox(ref settingsData.b_Noclip, $"Noclip ({settingsData.fl_NoclipSpeed})", "Allows you to fly and clip through walls.");
                settingsData.keyNoclip.Menu();
                GUILayout.EndHorizontal();
                settingsData.fl_NoclipSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.fl_NoclipSpeed, 1, 100));
            });

            UI.TabContents("Misc", UI.Tabs.Misc, () =>
            {
                UI.Checkbox(ref settingsData.b_NoMoreCredits, "No More Credits", "Prevents your group from receiving any credits. (Doesn't apply to quota)");
                UI.Checkbox(ref settingsData.b_SensitiveLandmines, "Sensitive Landmines", "Automatically detonates landmines when a player is in kill range.");
                UI.Checkbox(ref settingsData.b_AllJetpacksExplode, "All Jetpacks Explode", "When a player tries to equip a jetpack they will be greeted with an explosion.");
                UI.Checkbox(ref settingsData.b_LightShow, "Light Show", "Rapidly turns on/off the light switch and TV.");
                UI.Checkbox(ref settingsData.b_TerminalNoisemaker, "Terminal Noisemaker", "Plays a very annoying noise from the terminal.");
                UI.Checkbox(ref settingsData.b_AlwaysShowClock, "Always Show Clock", "Displays the clock even when you are in the facility.");

                settingsData.str_TerminalSignal = GUILayout.TextField(settingsData.str_TerminalSignal, Array.Empty<GUILayoutOption>());
                UI.Button("Send Signal", "Remotely sends a signal.", () =>
                {
                    if (!StartOfRound.Instance.unlockablesList.unlockables[(int)UnlockableUpgrade.SignalTranslator].hasBeenUnlockedByPlayer)
                    {
                        StartOfRound.Instance.BuyShipUnlockableServerRpc((int)UnlockableUpgrade.SignalTranslator, instance.shipTerminal.groupCredits);
                        StartOfRound.Instance.SyncShipUnlockablesServerRpc();
                    }
               
                    HUDManager.Instance.UseSignalTranslatorServerRpc(settingsData.str_TerminalSignal);
                });

                if (!settingsData.b_NoMoreCredits)
                {
                    settingsData.str_MoneyToGive = GUILayout.TextField(settingsData.str_MoneyToGive, Array.Empty<GUILayoutOption>());
                    UI.Button("Give Credits", "Give your group however many credits you want. (Doesn't apply to quota)", () =>
                    {
                        if (Instance.shipTerminal)
                        {
                            Instance.shipTerminal.groupCredits += int.Parse(settingsData.str_MoneyToGive);
                            Instance.shipTerminal.SyncGroupCreditsServerRpc(Instance.shipTerminal.groupCredits, 
                                Instance.shipTerminal.numberOfItemsInDropship);
                        }
                    });

                    GUILayout.BeginHorizontal();
                    settingsData.str_QuotaFulfilled = GUILayout.TextField(settingsData.str_QuotaFulfilled, GUILayout.Width(42));
                    GUILayout.Label("/", GUILayout.Width(4));
                    settingsData.str_Quota = GUILayout.TextField(settingsData.str_Quota, GUILayout.Width(42));
                    GUILayout.EndHorizontal();

                    UI.Button("Set Quota", "Allows you to set the quota. (Host only)", () =>
                    {
                        if (TimeOfDay.Instance)
                        {
                            TimeOfDay.Instance.profitQuota = int.Parse(settingsData.str_Quota);
                            TimeOfDay.Instance.quotaFulfilled = int.Parse(settingsData.str_QuotaFulfilled);
                            TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                        }
                    });
                }

                UI.Button($"Teleport All Items ({Instance.items.Count})", "Teleports all items on the planet to you.", () =>
                {
                    TeleportAllItems();
                });

                UI.Button("Land Ship", "Lands the ship.", () => StartOfRound.Instance.StartGameServerRpc());
                UI.Button("Start Ship", "Ship will leave the planet it's currently on.", () => StartOfRound.Instance.EndGameServerRpc(0));
                UI.Button("Unlock All Doors", "Unlocks all locked doors.", () =>
                {
                    foreach (DoorLock obj in Instance.doorLocks)
                        obj?.UnlockDoorServerRpc();
                });
                UI.Button("Open All Mechanical Doors", "Opens all mechanical doors.", () =>
                {
                    foreach (TerminalAccessibleObject obj in Instance.bigDoors)
                        obj?.SetDoorOpenServerRpc(true);
                });
                UI.Button("Close All Mechanical Doors", "Closes all mechanical doors.", () =>
                {
                    foreach (TerminalAccessibleObject obj in Instance.bigDoors)
                        obj?.SetDoorOpenServerRpc(false);
                });
                UI.Button("Explode All Mines", "Explodes every single mine on the level.", () =>
                {
                    foreach (Landmine obj in Instance.landmines)
                        obj?.ExplodeMineServerRpc();
                });
                UI.Button("Kill All Enemies", "Kills all enemies.", () =>
                {
                    foreach (EnemyAI obj in Instance.enemies)
                        obj?.KillEnemyServerRpc(false);
                });
                UI.Button("Delete All Enemies", "Deletes all enemies.", () =>
                {
                    foreach (EnemyAI obj in Instance.enemies)
                        obj?.KillEnemyServerRpc(true);
                });
                UI.Button("Attack Players at Deposit Desk", "Forces the tentacle monster to attack, killing a nearby player.", () =>
                {
                    if (Instance.itemsDesk)
                        Instance.itemsDesk.AttackPlayersServerRpc();
                });
            });


            UI.TabContents("ESP", UI.Tabs.ESP, () =>
            {
                UI.Checkbox(ref settingsData.b_EnableESP, "Enabled", "Enables the ESP.");
                UI.Checkbox(ref settingsData.b_ItemESP, "Items", "Shows all items.");
                UI.Checkbox(ref settingsData.b_EnemyESP, "Enemies", "Shows all enemies.");
                UI.Checkbox(ref settingsData.b_PlayerESP, "Players", "Shows all players.");
                UI.Checkbox(ref settingsData.b_ShipESP, "Ships", "Shows the ship.");
                UI.Checkbox(ref settingsData.b_DoorESP, "Doors", "Shows all doors.");
                UI.Checkbox(ref settingsData.b_SteamHazard, "Steam Hazards", "Shows all hazard zones.");
                UI.Checkbox(ref settingsData.b_LandmineESP, "Landmines", "Shows all landmines.");
                UI.Checkbox(ref settingsData.b_TurretESP, "Turrets", "Shows all turrets.");
                UI.Checkbox(ref settingsData.b_DisplayHP, "Show Health", "Shows players' health.");
                UI.Checkbox(ref settingsData.b_DisplayWorth, "Show Value", "Shows items' value.");
                UI.Checkbox(ref settingsData.b_DisplayDistance, "Show Distance", "Shows the distance between you and the entity.");
                UI.Checkbox(ref settingsData.b_DisplaySpeaking, "Show Is Speaking", "Shows if the player is speaking.");

                UI.Checkbox(ref settingsData.b_ItemDistanceLimit, "Item Distance Limit (" + Mathf.RoundToInt(settingsData.fl_ItemDistanceLimit) + ")", "Toggle to set the item distance limit.");
                settingsData.fl_ItemDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_ItemDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_EnemyDistanceLimit, "Enemy Distance Limit (" + Mathf.RoundToInt(settingsData.fl_EnemyDistanceLimit) + ")", "Toggle to set the enemy distance limit.");
                settingsData.fl_EnemyDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_EnemyDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_MineDistanceLimit, "Landmine Distance Limit (" + Mathf.RoundToInt(settingsData.fl_MineDistanceLimit) + ")", "Toggle to set the landmine distance limit.");
                settingsData.fl_MineDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_MineDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_TurretDistanceLimit, "Turret Distance Limit (" + Mathf.RoundToInt(settingsData.fl_TurretDistanceLimit) + ")", "Toggle to set the turret distance limit.");
                settingsData.fl_TurretDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_TurretDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());
            });

            UI.TabContents(null, UI.Tabs.Players, () =>
            {
                GUILayout.BeginHorizontal();
                foreach (PlayerControllerB player in Instance.players)
                {
                    if (!IsPlayerValid(player)) continue;
                    UI.Tab(PAUtils.TruncateString(player.playerUsername, 12), ref selectedPlayer, player, true);
                }
                GUILayout.EndHorizontal();

                if (!IsPlayerValid(selectedPlayer))
                    selectedPlayer = null;

                if (selectedPlayer)
                {
                    UI.Header("Selected Player: " + selectedPlayer.playerUsername);
                    Settings.Instance.InitializeDictionaries(selectedPlayer);

                    bool DemigodCheck = Settings.Instance.b_DemiGod[selectedPlayer];
                    UI.Checkbox(ref DemigodCheck, "Demigod", "Automatically refills the selected player's health if below zero.");
                    Settings.Instance.b_DemiGod[selectedPlayer] = DemigodCheck;

                    UI.Button("Kill", "Kills the currently selected player.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(selectedPlayer.health + 1, new Vector3(900, 900, 900), 0); });
                    UI.Button("Teleport To", "Teleports you to the currently selected player.", () => { Instance.localPlayer.TeleportPlayer(selectedPlayer.playerGlobalHead.position); });
                    UI.Button("Teleport Player To Ship", "Teleports the selected into the ship. (Host only)", () =>
                    {
                        selectedPlayer.TeleportPlayer(Instance.shipRoom.transform.position);
                    });
  
                    Settings.Instance.str_DamageToGive = GUILayout.TextField(Settings.Instance.str_DamageToGive, Array.Empty<GUILayoutOption>());
                    UI.Button("Damage", "Damages the player for a given amount.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(int.Parse(Settings.Instance.str_DamageToGive), new Vector3(900, 900, 900), 0); });

                    Settings.Instance.str_HealthToHeal = GUILayout.TextField(Settings.Instance.str_HealthToHeal, Array.Empty<GUILayoutOption>());
                    UI.Button("Heal", "Heals the player for a given amount.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(-int.Parse(Settings.Instance.str_HealthToHeal), new Vector3(900, 900, 900), 0); });

                    Settings.Instance.str_ChatAsPlayer = GUILayout.TextField(Settings.Instance.str_ChatAsPlayer, Array.Empty<GUILayoutOption>());
                    UI.Button("Send Message", "Sends a message in chat as the selected player.", () => { HUDManager.Instance.AddTextToChatOnServer(Settings.Instance.str_ChatAsPlayer, (int)selectedPlayer.playerClientId); } );

                    UI.Button("Steam Profile", "Opens the selected player's steam profile in your overlay.", () => { SteamFriends.OpenUserOverlay(selectedPlayer.playerSteamId, "steamid"); });
                }
            });

            if (StartOfRound.Instance && instance.shipTerminal)
            {
                UI.TabContents("Upgrades", UI.Tabs.Upgrades, () =>
                {
                    bool allUpgradesUnlocked = true;
                    bool allSuitsUnlocked = true;

                    for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
                    {
                        if (i == (int)UnlockableUpgrade.OrangeSuit
                            || i == (int)UnlockableUpgrade.Cupboard
                            || i == (int)UnlockableUpgrade.FileCabinet
                            || i == (int)UnlockableUpgrade.LightSwitch
                            || i == (int)UnlockableUpgrade.Bunkbeds
                            || i == (int)UnlockableUpgrade.Terminal)
                            continue;

                        if (!StartOfRound.Instance.unlockablesList.unlockables[i].hasBeenUnlockedByPlayer)
                        {
                            allUpgradesUnlocked = false;
                            break;
                        }
                    }

                    for (int i = 1; i <= 3; i++)
                    {
                        if (!StartOfRound.Instance.unlockablesList.unlockables[i]?.hasBeenUnlockedByPlayer ?? false)
                        {
                            allSuitsUnlocked = false;
                            break;
                        }
                    }

                    if (allUpgradesUnlocked && allSuitsUnlocked)
                    {
                        GUILayout.Label("You've already unlocked all upgrades.");
                    }
                    else
                    {
                        UI.Button("Unlock All Upgrades", "Unlocks all ship upgrades.", () =>
                        {
                            for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
                            {
                                if (StartOfRound.Instance.unlockablesList.unlockables[i].hasBeenUnlockedByPlayer) continue;

                                StartOfRound.Instance.BuyShipUnlockableServerRpc(i, instance.shipTerminal.groupCredits);
                                StartOfRound.Instance.SyncShipUnlockablesServerRpc();
                            }
                        });

                        if (!allSuitsUnlocked)
                        {
                            UI.Button("Unlock All Suits", "Unlocks all suits.", () =>
                            {
                                for (int i = 1; i <= 3; i++)
                                {
                                    StartOfRound.Instance.BuyShipUnlockableServerRpc(i, instance.shipTerminal.groupCredits);
                                }
                            });
                        }

                        for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
                        {
                            if (i == (int)UnlockableUpgrade.OrangeSuit
                                || i == (int)UnlockableUpgrade.Cupboard
                                || i == (int)UnlockableUpgrade.FileCabinet
                                || i == (int)UnlockableUpgrade.LightSwitch
                                || i == (int)UnlockableUpgrade.Bunkbeds
                                || i == (int)UnlockableUpgrade.Terminal)
                                continue;

                            if (StartOfRound.Instance.unlockablesList.unlockables[i].hasBeenUnlockedByPlayer)
                                continue;

                            string unlockableName = PAUtils.ConvertFirstLetterToUpperCase(StartOfRound.Instance.unlockablesList.unlockables[i].unlockableName);

                            UI.Button(unlockableName, $"Unlock {unlockableName}", () =>
                            {
                                StartOfRound.Instance.BuyShipUnlockableServerRpc(i, instance.shipTerminal.groupCredits);
                                StartOfRound.Instance.SyncShipUnlockablesServerRpc();
                            });
                        }
                    }
                });
            }

            UI.TabContents("Graphics", UI.Tabs.Graphics, () =>
            {
                UI.Checkbox(ref settingsData.b_DisableFog, "Disable Fog", "Disables the fog effect.");
                UI.Checkbox(ref settingsData.b_DisableDepthOfField, "Disable Depth of Field", "Disables the depth of field effect.");
            });

            UI.TabContents("Settings", UI.Tabs.Settings, () =>
            {
                UI.Checkbox(ref settingsData.b_Crosshair, "Crosshair", "Displays a crosshair on the screen.");
                UI.Checkbox(ref settingsData.b_DisplayGroupCredits, "Display Group Credits", "Shows how many credits you have.");
                UI.Checkbox(ref settingsData.b_DisplayQuota, "Display Quota", "Shows the current quota.");
                UI.Checkbox(ref settingsData.b_DisplayDaysLeft, "Display Days Left", "Shows the time you have left to meet quota.");
                UI.Checkbox(ref settingsData.b_CenteredIndicators, "Centered Indicators", "Displays the above indicators at the center of the screen.");
                UI.Checkbox(ref settingsData.b_DeadPlayers, "Dead Player List", "Shows a list of currently dead players.");
                UI.Checkbox(ref settingsData.b_Tooltips, "Tooltips", "Shows information about the currently hovered menu item.");

                UI.Header("Colors");
                UI.ColorPicker("Theme", ref settingsData.c_Theme);
                UI.ColorPicker("Valve", ref settingsData.c_Valve);
                UI.ColorPicker("Enemy", ref settingsData.c_Enemy);
                UI.ColorPicker("Turret", ref settingsData.c_Turret);
                UI.ColorPicker("Landmine", ref settingsData.c_Landmine);
                UI.ColorPicker("Player", ref settingsData.c_Player);
                UI.ColorPicker("Door", ref settingsData.c_Door);
                UI.ColorPicker("Loot", ref settingsData.c_Loot);
                UI.ColorPicker("Small Loot", ref settingsData.c_smallLoot);
                UI.ColorPicker("Medium Loot", ref settingsData.c_medLoot);
                UI.ColorPicker("Big Loot", ref settingsData.c_bigLoot);
            });

            UI.RenderTooltip();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }

        public static void TeleportAllItems()
        {
            if (Instance != null && HUDManager.Instance != null && Instance.localPlayer != null)
            {
                PlayerControllerB localPlayer = Instance.localPlayer;
                foreach (GrabbableObject grabbableObject in Instance.items)
                {
                    if (!grabbableObject.isHeld && !grabbableObject.isPocketed && !grabbableObject.isInShipRoom)
                    {
                        Vector3 point = new Ray(localPlayer.gameplayCamera.transform.position, localPlayer.gameplayCamera.transform.forward).GetPoint(1f);
                        grabbableObject.gameObject.transform.position = point;
                        grabbableObject.startFallingPosition = point;
                        grabbableObject.targetFloorPosition = point;
                    }
                }
            }

        }

        private void DisplayObjects<T>(IEnumerable<T> objects, bool shouldDisplay, Func<T, string> labelSelector, Func<T, Color> colorSelector) where T : Component
        {
            if (!shouldDisplay) return;

            foreach (T obj in objects)
            {
                if (obj != null && obj.gameObject.activeSelf)
                {
                    float distanceToPlayer = PAUtils.GetDistance(Instance.localPlayer.gameplayCamera.transform.position,
                        obj.transform.position);
                    Vector3 pos;
                    if (PAUtils.WorldToScreen(Instance.localPlayer.gameplayCamera, obj.transform.position, out pos))
                    {
                        string ObjName = PAUtils.ConvertFirstLetterToUpperCase(labelSelector(obj));
                        if (settingsData.b_DisplayDistance)
                            ObjName += " [" + distanceToPlayer.ToString().ToUpper() + "M]";
                        Render.String(Style, pos.x, pos.y, 150f, 50f, ObjName, colorSelector(obj));
                    }
                }
            }
        }

        public void DisplayDeadPlayers()
        {
            if (!settingsData.b_DeadPlayers) return;

            float yOffset = 30f;

            foreach (PlayerControllerB playerControllerB in Instance.players)
            {
                if (playerControllerB != null && playerControllerB.isPlayerDead)
                {
                    string strPlayer = playerControllerB.playerUsername;
                    Render.String(Style, 10f, yOffset, 200f, Settings.TEXT_HEIGHT, strPlayer, GUI.color);
                    yOffset += (Settings.TEXT_HEIGHT - 10f);
                }
            }
        }

        private void DisplayShip()
        {
            DisplayObjects(
                new[] { Instance.shipDoor },
                settingsData.b_ShipESP,
                _ => "Ship",
                _ => settingsData.c_Door
            );
        }

        private void DisplayDoors()
        {
            DisplayObjects(
                Instance.entranceTeleports,
                settingsData.b_DoorESP,
                entranceTeleport => entranceTeleport.isEntranceToBuilding ? "Entrance" : "Exit",
                _ => settingsData.c_Door
            );
        }

        private void DisplayLandmines()
        {
            DisplayObjects(
                Instance.landmines.Where(landmine => landmine != null && landmine.IsSpawned && !landmine.hasExploded &&
                    ((settingsData.b_MineDistanceLimit &&
                    PAUtils.GetDistance(Instance.localPlayer.gameplayCamera.transform.position,
                        landmine.transform.position) < settingsData.fl_MineDistanceLimit) ||
                        !settingsData.b_MineDistanceLimit)),
                settingsData.b_LandmineESP,
                _ => "Landmine",
                _ => settingsData.c_Landmine
            );
        }

        private void DisplayTurrets()
        {
            DisplayObjects(
                Instance.turrets.Where(turret => turret != null && turret.IsSpawned &&
                    ((settingsData.b_TurretDistanceLimit &&
                    PAUtils.GetDistance(Instance.localPlayer.gameplayCamera.transform.position,
                        turret.transform.position) < settingsData.fl_TurretDistanceLimit) ||
                        !settingsData.b_TurretDistanceLimit)),
                settingsData.b_TurretESP,
                _ => "Turret",
                _ => settingsData.c_Turret
            );
        }

        private void DisplaySteamHazard()
        {
            DisplayObjects(
                Instance.steamValves.Where(steamValveHazard => steamValveHazard != null && steamValveHazard.triggerScript.interactable),
                settingsData.b_SteamHazard,
                _ => "Steam Valve",
                _ => settingsData.c_Valve
            );
        }

        private void DisplayPlayers()
        {
            DisplayObjects(
                Instance.players.Where(playerControllerB =>
                    IsPlayerValid(playerControllerB) &&
                    !playerControllerB.IsLocalPlayer &&
                     playerControllerB.playerUsername != Instance.localPlayer.playerUsername &&
                    !playerControllerB.isPlayerDead
                ),
                settingsData.b_PlayerESP,
                playerControllerB =>
                {
                    string str = playerControllerB.playerUsername;
                    if (settingsData.b_DisplaySpeaking && playerControllerB.voicePlayerState.IsSpeaking)
                        str += " [VC]";
                    if (settingsData.b_DisplayHP)
                        str += " [" + playerControllerB.health + "HP]";
                    return str;
                },
                _ => settingsData.c_Player
            );
        }

        private void DisplayEnemyAI()
        {
            DisplayObjects(
                Instance.enemies.Where(enemyAI =>
                    enemyAI != null &&
                    enemyAI.eye != null &&
                    enemyAI.enemyType != null &&
                    !enemyAI.isEnemyDead &&
                    ((settingsData.b_EnemyDistanceLimit &&
                    PAUtils.GetDistance(Instance.localPlayer.gameplayCamera.transform.position,
                        enemyAI.transform.position) < settingsData.fl_EnemyDistanceLimit) ||
                        !settingsData.b_EnemyDistanceLimit)
                ),
                settingsData.b_EnemyESP,
                enemyAI =>
                {
                    string name = enemyAI.enemyType.enemyName;
                    return string.IsNullOrWhiteSpace(name) ? "Enemy" : name;
                },
                _ => settingsData.c_Enemy
            );
        }

        private Color GetLootColor(int value)
        {
            if (value <= 15) return settingsData.c_smallLoot;
            if (value > 15 && value <= 35) return settingsData.c_medLoot;
            if (value >= 36) return settingsData.c_bigLoot;
            else return settingsData.c_Loot;
        }

        private void DisplayLoot()
        {
            DisplayObjects(
                Instance.items.Where(grabbableObject =>
                    grabbableObject != null &&
                    !grabbableObject.isHeld &&
                    !grabbableObject.isPocketed &&
                    grabbableObject.itemProperties != null &&
                    ((settingsData.b_ItemDistanceLimit &&
                    PAUtils.GetDistance(Instance.localPlayer.gameplayCamera.transform.position,
                        grabbableObject.transform.position) < settingsData.fl_ItemDistanceLimit) ||
                        !settingsData.b_ItemDistanceLimit)
                ),
                settingsData.b_ItemESP,
                grabbableObject =>
                {
                    string text = "Object";
                    Item itemProperties = grabbableObject.itemProperties;
                    if (itemProperties.itemName != null)
                        text = itemProperties.itemName;
                    int scrapValue = grabbableObject.scrapValue;
                    if (settingsData.b_DisplayWorth && scrapValue > 0)
                        text += " [" + scrapValue.ToString() + "C]";
                    return text;
                },
                grabbableObject => GetLootColor(grabbableObject.scrapValue)
            );
        }

        public void Start()
        {
            Harmony harmony = new Harmony("com.waxxyTF2.ProjectApparatus");
            harmony.PatchAll();

            StartCoroutine(Instance.CollectObjects());

            Settings.Changelog.ReadChanges();
            Settings.Credits.ReadCredits();

            Settings.Instance.ResetBindStates();
        }

        public void Update()
        {
            if (menuButton.WasPerformedThisFrame())
            {
                Settings.Instance.SaveSettings();
                Settings.Instance.b_isMenuOpen = !Settings.Instance.b_isMenuOpen;
            }
            if (unloadMenu.WasPressedThisFrame())
            {
                Loader.Unload();
                StopCoroutine(Instance.CollectObjects());
            }

            if (settingsData.b_AlwaysShowClock && HUDManager.Instance)
            {
                HUDManager.Instance.SetClockVisible(true);
            }

            if (settingsData.b_LightShow)
            {
                if (Instance.shipLights)
                    Instance.shipLights.SetShipLightsServerRpc(!Instance.shipLights.areLightsOn);

                if (Instance.tvScript)
                {
                    if (Instance.tvScript.tvOn)
                        Instance.tvScript.TurnOffTVServerRpc();
                    else
                        Instance.tvScript.TurnOnTVServerRpc();
                }
            }

            if (Instance.shipTerminal)
            {
                if (settingsData.b_NoMoreCredits)
                    Instance.shipTerminal.groupCredits = 0;

                if (settingsData.b_TerminalNoisemaker)
                    instance.shipTerminal.PlayTerminalAudioServerRpc(1);
            }

            Noclip();

            settingsData.keyNoclip.Update();
        }

        private void Noclip()
        {
            PlayerControllerB localPlayer = Instance.localPlayer;
            if (!localPlayer) return;

            Collider localCollider = localPlayer.GetComponent<CharacterController>();
            if (!localCollider) return;

            Transform localTransform = localPlayer.transform;
            localCollider.enabled = !(localTransform
                && settingsData.b_Noclip
                && (settingsData.keyNoclip.inKey == 0 || PAUtils.GetAsyncKeyState(settingsData.keyNoclip.inKey) != 0));

            if (!localCollider.enabled)
            {
                bool WKey = PAUtils.GetAsyncKeyState((int)Keys.W) != 0,
                    AKey = PAUtils.GetAsyncKeyState((int)Keys.A) != 0,
                    SKey = PAUtils.GetAsyncKeyState((int)Keys.S) != 0,
                    DKey = PAUtils.GetAsyncKeyState((int)Keys.D) != 0,
                    SpaceKey = PAUtils.GetAsyncKeyState((int)Keys.Space) != 0,
                    CtrlKey = PAUtils.GetAsyncKeyState((int)Keys.LControlKey) != 0;

                Vector3 inVec = new Vector3(0, 0, 0);

                if (WKey)
                    inVec += localTransform.forward;
                if (SKey)
                    inVec -= localTransform.forward;
                if (AKey)
                    inVec -= localTransform.right;
                if (DKey)
                    inVec += localTransform.right;
                if (SpaceKey)
                    inVec.y += localTransform.up.y;
                if (CtrlKey)
                    inVec.y -= localTransform.up.y;

                localPlayer.transform.position += inVec * (settingsData.fl_NoclipSpeed * Time.deltaTime);
            }
        }

        private void ReviveLocalPlayer() // This is a modified version of StartOfRound.ReviveDeadPlayers
        {
            PlayerControllerB localPlayer = Instance.localPlayer;
            StartOfRound.Instance.allPlayersDead = false;
            localPlayer.ResetPlayerBloodObjects(localPlayer.isPlayerDead);
            if (localPlayer.isPlayerDead || localPlayer.isPlayerControlled)
            {
                localPlayer.isClimbingLadder = false;
                localPlayer.ResetZAndXRotation();
                localPlayer.thisController.enabled = true;
                localPlayer.health = 100;
                localPlayer.disableLookInput = false;
                if (localPlayer.isPlayerDead)
                {
                    localPlayer.isPlayerDead = false;
                    localPlayer.isPlayerControlled = true;
                    localPlayer.isInElevator = true;
                    localPlayer.isInHangarShipRoom = true;
                    localPlayer.isInsideFactory = false;
                    localPlayer.wasInElevatorLastFrame = false;
                    StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
                    localPlayer.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].position, false, 0f, false, true);
                    localPlayer.setPositionOfDeadPlayer = false;
                    localPlayer.DisablePlayerModel(StartOfRound.Instance.allPlayerObjects[localPlayer.playerClientId], true, true);
                    localPlayer.helmetLight.enabled = false;
                    localPlayer.Crouch(false);
                    localPlayer.criticallyInjured = false;
                    localPlayer.playerBodyAnimator?.SetBool("Limp", false);
                    localPlayer.bleedingHeavily = false;
                    localPlayer.activatingItem = false;
                    localPlayer.twoHanded = false;
                    localPlayer.inSpecialInteractAnimation = false;
                    localPlayer.disableSyncInAnimation = false;
                    localPlayer.inAnimationWithEnemy = null;
                    localPlayer.holdingWalkieTalkie = false;
                    localPlayer.speakingToWalkieTalkie = false;
                    localPlayer.isSinking = false;
                    localPlayer.isUnderwater = false;
                    localPlayer.sinkingValue = 0f;
                    localPlayer.statusEffectAudio.Stop();
                    localPlayer.DisableJetpackControlsLocally();
                    localPlayer.health = 100;
                    localPlayer.mapRadarDotAnimator.SetBool("dead", false);
                    if (localPlayer.IsOwner)
                    {
                        HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
                        localPlayer.hasBegunSpectating = false;
                        HUDManager.Instance.RemoveSpectateUI();
                        HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                        localPlayer.hinderedMultiplier = 1f;
                        localPlayer.isMovementHindered = 0;
                        localPlayer.sourcesCausingSinking = 0;
                        localPlayer.reverbPreset = StartOfRound.Instance.shipReverb;
                    }
                }
                SoundManager.Instance.earsRingingTimer = 0f;
                localPlayer.voiceMuffledByEnemy = false;
                SoundManager.Instance.playerVoicePitchTargets[localPlayer.playerClientId] = 1f;
                SoundManager.Instance.SetPlayerPitch(1f, (int)localPlayer.playerClientId);
                if (localPlayer.currentVoiceChatIngameSettings == null)
                {
                    StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
                }
                if (localPlayer.currentVoiceChatIngameSettings != null)
                {
                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        localPlayer.currentVoiceChatIngameSettings.InitializeComponents();

                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        return;

                    localPlayer.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
                }
            }
            PlayerControllerB playerControllerB = GameNetworkManager.Instance.localPlayerController;
            playerControllerB.bleedingHeavily = false;
            playerControllerB.criticallyInjured = false;
            playerControllerB.playerBodyAnimator.SetBool("Limp", false);
            playerControllerB.health = 100;
            HUDManager.Instance.UpdateHealthUI(100, false);
            playerControllerB.spectatedPlayerScript = null;
            HUDManager.Instance.audioListenerLowPass.enabled = false;
            StartOfRound.Instance.SetSpectateCameraToGameOverMode(false, playerControllerB);
            RagdollGrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<RagdollGrabbableObject>();
            for (int j = 0; j < array.Length; j++)
            {
                if (!array[j].isHeld)
                {
                    if (StartOfRound.Instance.IsServer)
                    {
                        if (array[j].NetworkObject.IsSpawned)
                            array[j].NetworkObject.Despawn(true);
                        else
                            UnityEngine.Object.Destroy(array[j].gameObject);
                    }
                }
                else if (array[j].isHeld && array[j].playerHeldBy != null)
                {
                    array[j].playerHeldBy.DropAllHeldItems(true, false);
                }
            }
            DeadBodyInfo[] array2 = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
            for (int k = 0; k < array2.Length; k++)
            {
                UnityEngine.Object.Destroy(array2[k].gameObject);
            }
            StartOfRound.Instance.livingPlayers = StartOfRound.Instance.connectedPlayersAmount + 1;
            StartOfRound.Instance.allPlayersDead = false;
            StartOfRound.Instance.UpdatePlayerVoiceEffects();
            StartOfRound.Instance.shipAnimator.ResetTrigger("ShipLeave");
        }

        public LayerMask s_layerMask = LayerMask.GetMask(new string[]
        {
            "Room"
        });
        private Vector2 scrollPos;
        private readonly InputAction menuButton = new InputAction(null, InputActionType.Button, "<Keyboard>/insert", null, null, null);
        private readonly InputAction unloadMenu = new InputAction(null, InputActionType.Button, "<Keyboard>/pause", null, null, null);
    }
}
