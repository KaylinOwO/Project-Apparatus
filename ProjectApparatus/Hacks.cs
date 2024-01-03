using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameObjectManager;
using System.Windows.Forms;

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
            Watermark += " | v" + settingsData.version;
            if (!Settings.Instance.b_isMenuOpen) Watermark += " | Press INSERT";
            if (!settingsData.b_CenteredIndicators)
            {
                if (settingsData.b_DisplayGroupCredits && Instance.shipTerminal != null)
                    Watermark += $" | Group Credits: {Instance.shipTerminal.groupCredits}";
                if (settingsData.b_DisplayQuota && TimeOfDay.Instance)
                    Watermark += $" | Profit Quota: {TimeOfDay.Instance.quotaFulfilled} / {TimeOfDay.Instance.profitQuota}";
                if (settingsData.b_DisplayDaysLeft && TimeOfDay.Instance)
                    Watermark += $" | Days Left: {TimeOfDay.Instance.daysUntilDeadline}"; ;
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
                UI.Checkbox(ref settingsData.b_RemoveWeight, "No Weight", "Removes speed limitations caused by item weight.");
                UI.Checkbox(ref settingsData.b_InteractThroughWalls, "Interact Through Walls", "Allows you to interact with anything through walls.");
                UI.Checkbox(ref settingsData.b_UnlimitedGrabDistance, "No Grab Distance Limit", "Allows you to interact with anything no matter the distance.");
                UI.Checkbox(ref settingsData.b_OneHandAllObjects, "One Hand All Objects", "Allows you to one-hand any two-handed objects.");
                UI.Checkbox(ref settingsData.b_DisableFallDamage, "Disable Fall Damage", "You no longer take fall damage.");
                UI.Checkbox(ref settingsData.b_DisableInteractCooldowns, "Disable Interact Cooldowns", "Disables all interact cooldowns (e.g., noisemakers, toilets, etc).");
                UI.Checkbox(ref settingsData.b_InstantInteractions, "Instant Interactions", "Makes all hold interactions instantaneous.");
                UI.Checkbox(ref settingsData.b_PlaceAnywhere, "Place Anywhere", "Place objects from the ship anywhere you want.");
                UI.Checkbox(ref settingsData.b_TauntSlide, "Taunt Slide", "Allows you to emote and move at the same time.");
                UI.Checkbox(ref settingsData.b_FastLadderClimbing, "Fast Ladder Climbing", "Instantly climbs up ladders.");
                UI.Checkbox(ref settingsData.b_HearEveryone, "Hear Everyone", "Allows you to hear everyone no matter the distance.");
                UI.Checkbox(ref settingsData.b_ChargeAnyItem, "Charge Any Item", "Allows you to put any grabbable item in the charger.");
                UI.Checkbox(ref settingsData.b_NightVision, $"Night Vision ({settingsData.i_NightVision}%)", "Allows you to see in the dark.");
                settingsData.i_NightVision = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_NightVision, 1, 100));

                UI.Checkbox(ref settingsData.b_WalkSpeed, $"Adjust Walk Speed ({settingsData.i_WalkSpeed})", "Allows you to modify your walk speed.");
                settingsData.i_WalkSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_WalkSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_SprintSpeed, $"Adjust Sprint Speed ({settingsData.i_SprintSpeed})", "Allows you to modify your sprint speed.");
                settingsData.i_SprintSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_SprintSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_JumpHeight, $"Jump Height ({settingsData.i_JumpHeight})", "Allows you to modify your jump height.");
                settingsData.i_JumpHeight = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_JumpHeight, 1, 100));

                UI.Button("Suicide", "Kills local player.", () =>
                {
                   Instance.localPlayer.DamagePlayerFromOtherClientServerRpc(100, new Vector3(), -1);
                });

                UI.Button("Respawn", "Respawns you. You will be invisible to both players and enemies.", () =>
                {
                    Features.Misc.RespawnLocalPlayer();
                });

                UI.Button("Teleport To Ship", "Teleports you into the ship.", () =>
                {
                    if (Instance.shipRoom)
                        Instance.localPlayer?.TeleportPlayer(Instance.shipRoom.transform.position);
                });

                UI.Button("Possess Nearest Enemy", "Possesses the nearest enemy. (Note: You will be visibily within the enemy.)", () =>
                {
                    Features.Possession.StartPossession();
                });

                UI.Button("Stop Possessing", "Stops possessing the currently possessed enemy.", () =>
                {
                    Features.Possession.StopPossession();
                });

                GUILayout.BeginHorizontal();
                UI.Checkbox(ref settingsData.b_Noclip, $"Noclip ({settingsData.fl_NoclipSpeed})", "Allows you to fly and clip through walls.");
                UI.Keybind(ref settingsData.keyNoclip);
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


                settingsData.str_ChatMessage = GUILayout.TextField(settingsData.str_ChatMessage, Array.Empty<GUILayoutOption>());
                UI.Button("Send Message", "Anonymously sends a message in chat.", () =>
                {
                    PAUtils.SendChatMessage(settingsData.str_ChatMessage);
                });

                UI.Checkbox(ref settingsData.b_AnonChatSpam, "Spam Message", "Anonymously spams a message in chat.");

                settingsData.str_TerminalSignal = GUILayout.TextField(settingsData.str_TerminalSignal, Array.Empty<GUILayoutOption>());
                UI.Button("Send Signal", "Remotely sends a signal.", () =>
                {
                    if (!StartOfRound.Instance.unlockablesList.unlockables[(int)UnlockableUpgrade.SignalTranslator].hasBeenUnlockedByPlayer)
                    {
                        StartOfRound.Instance.BuyShipUnlockableServerRpc((int)UnlockableUpgrade.SignalTranslator, Instance.shipTerminal.groupCredits);
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
                        obj?.UnlockDoorSyncWithServer();
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

                    // We keep toggles outside of the isPlayerDead check so that users can toggle them on/off no matter their condition.

                    bool DemigodCheck = Settings.Instance.b_DemiGod[selectedPlayer];
                    UI.Checkbox(ref DemigodCheck, "Demigod", "Automatically refills the selected player's health if below zero.");
                    Settings.Instance.b_DemiGod[selectedPlayer] = DemigodCheck;

                    bool ObjectSpam = Settings.Instance.b_SpamObjects[selectedPlayer];
                    UI.Checkbox(ref ObjectSpam, "Object Spam", "Spam places objects on the player to annoy/trap them.");
                    Settings.Instance.b_SpamObjects[selectedPlayer] = ObjectSpam;

                    UI.Checkbox(ref Settings.Instance.b_HideObjects, "Hide Objects", "Hides spammed objects from the selected player.");

                    if (!selectedPlayer.isPlayerDead)
                    {
                        UI.Button("Kill", "Kills the currently selected player.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(selectedPlayer.health + 1, new Vector3(900, 900, 900), 0); });
                        UI.Button("Teleport To", "Teleports you to the currently selected player.", () => { Instance.localPlayer.TeleportPlayer(selectedPlayer.playerGlobalHead.position); });
                        UI.Button("Teleport Enemies To", "Teleports all enemies to the currently selected player.", () =>
                        {
                            foreach (EnemyAI enemy in Instance.enemies)
                            {
                                if (enemy != null && enemy != Features.Possession.possessedEnemy)
                                {
                                    enemy.ChangeEnemyOwnerServerRpc(Instance.localPlayer.actualClientId);
                                    foreach (Collider col in enemy.GetComponentsInChildren<Collider>()) col.enabled = false; // To prevent enemies from getting stuck in eachother
                                    enemy.transform.position = selectedPlayer.transform.position;
                                    enemy.SyncPositionToClients();
                                }
                            }
                        });
                        UI.Button("Teleport Player To Ship", "Teleports the selected into the ship. (Host only)", () =>
                        {
                            Instance.shipTeleporter.TeleportPlayerOutServerRpc((int)selectedPlayer.playerClientId, Instance.shipRoom.transform.position);
                        });

                        UI.Button("Aggro Enemies", "Makes enemies target the selected player.\nDoesn't work on most monsters, works best on Crawlers & Spiders.", () => { 
                            foreach (EnemyAI enemy in Instance.enemies)
                            {
                                enemy.SwitchToBehaviourServerRpc(1); // I believe this just angers all enemies.
                                if (enemy.GetType() == typeof(CrawlerAI))
                                {
                                    CrawlerAI crawler = (CrawlerAI)enemy;
                                    crawler.BeginChasingPlayerServerRpc((int)selectedPlayer.playerClientId);
                                }
                                if (enemy.GetType() == typeof(NutcrackerEnemyAI))
                                {
                                    NutcrackerEnemyAI nutcracker = (NutcrackerEnemyAI)enemy;
                                    nutcracker.SwitchTargetServerRpc((int)selectedPlayer.playerClientId);
                                }
                                if (enemy.GetType() == typeof(CentipedeAI))
                                {
                                    CentipedeAI centipede = (CentipedeAI)enemy;
                                    centipede.TriggerCentipedeFallServerRpc(selectedPlayer.actualClientId);
                                }
                                if (enemy.GetType() == typeof(SandSpiderAI))
                                {
                                    SandSpiderAI spider = (SandSpiderAI)enemy;
                                    foreach (SandSpiderWebTrap trap in spider?.webTraps)
                                        if (trap)
                                            spider?.PlayerTripWebServerRpc(trap.trapID, (int)selectedPlayer.playerClientId);
                                }
                            }
                        });

                        Settings.Instance.str_DamageToGive = GUILayout.TextField(Settings.Instance.str_DamageToGive, Array.Empty<GUILayoutOption>());
                        UI.Button("Damage", "Damages the player for a given amount.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(int.Parse(Settings.Instance.str_DamageToGive), new Vector3(900, 900, 900), 0); });

                        Settings.Instance.str_HealthToHeal = GUILayout.TextField(Settings.Instance.str_HealthToHeal, Array.Empty<GUILayoutOption>());
                        UI.Button("Heal", "Heals the player for a given amount.", () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(-int.Parse(Settings.Instance.str_HealthToHeal), new Vector3(900, 900, 900), 0); });
                    }

                    Settings.Instance.str_ChatAsPlayer = GUILayout.TextField(Settings.Instance.str_ChatAsPlayer, Array.Empty<GUILayoutOption>());
                    UI.Button("Send Message", "Sends a message in chat as the selected player.", () =>
                    {
                        PAUtils.SendChatMessage(Settings.Instance.str_ChatAsPlayer, (int)selectedPlayer.playerClientId);
                    });

                    bool SpamChatCheck = Settings.Instance.b_SpamChat[selectedPlayer];
                    UI.Checkbox(ref SpamChatCheck, "Spam Message", "Spams the message in chat as the selected player.");
                    Settings.Instance.b_SpamChat[selectedPlayer] = SpamChatCheck;

                    UI.Button("Steam Profile", "Opens the selected player's steam profile in your overlay.", () => { SteamFriends.OpenUserOverlay(selectedPlayer.playerSteamId, "steamid"); });
                }
            });

            if (StartOfRound.Instance && Instance.shipTerminal)
            {
                UI.TabContents("Upgrades", UI.Tabs.Upgrades, () =>
                {
                    bool allUpgradesUnlocked = true;
                    bool allSuitsUnlocked = true;

                    for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
                    {
                        if (Enum.IsDefined(typeof(UnlockableUpgrade), i) &&
                            !StartOfRound.Instance.unlockablesList.unlockables[i].hasBeenUnlockedByPlayer)
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
                                if (Enum.IsDefined(typeof(UnlockableUpgrade), i) &&
                                    !StartOfRound.Instance.unlockablesList.unlockables[i].hasBeenUnlockedByPlayer)
                                {
                                    StartOfRound.Instance.BuyShipUnlockableServerRpc(i, Instance.shipTerminal.groupCredits);
                                    StartOfRound.Instance.SyncShipUnlockablesServerRpc();
                                }
                            }
                        });

                        if (!allSuitsUnlocked)
                        {
                            UI.Button("Unlock All Suits", "Unlocks all suits.", () =>
                            {
                                for (int i = 1; i <= 3; i++)
                                {
                                    StartOfRound.Instance.BuyShipUnlockableServerRpc(i, Instance.shipTerminal.groupCredits);
                                }
                            });
                        }

                        for (int i = 0; i < StartOfRound.Instance.unlockablesList.unlockables.Count; i++)
                        {
                            if (Enum.IsDefined(typeof(UnlockableUpgrade), i) &&
                                !StartOfRound.Instance.unlockablesList.unlockables[i].hasBeenUnlockedByPlayer)
                            {
                                string unlockableName = PAUtils.ConvertFirstLetterToUpperCase(StartOfRound.Instance.unlockablesList.unlockables[i].unlockableName);

                                UI.Button(unlockableName, $"Unlock {unlockableName}", () =>
                                {
                                    StartOfRound.Instance.BuyShipUnlockableServerRpc(i, Instance.shipTerminal.groupCredits);
                                    StartOfRound.Instance.SyncShipUnlockablesServerRpc();
                                });
                            }
                        }
                    }
                });
            }

            UI.TabContents("Graphics", UI.Tabs.Graphics, () =>
            {
                UI.Checkbox(ref settingsData.b_DisableFog, "Disable Fog", "Disables the fog effect.");
                UI.Checkbox(ref settingsData.b_DisableDepthOfField, "Disable Depth of Field", "Disables the depth of field effect.");
                if (UI.Checkbox(ref settingsData.b_RemoveVisor, "Disable Visor", "Disables the visor from your helmet in first person."))
                {
                    if (!settingsData.b_RemoveVisor && !Features.Thirdperson.ThirdpersonCamera.ViewState)
                        Instance.localVisor?.SetActive(true);
                }
                UI.Checkbox(ref settingsData.b_CameraResolution, "Full Render Resolution", "Forces the game to render in full resolution.\n<color=#ff0000>You will need to leave the game for this to activate.</color>");
                GUILayout.Label($"Field of View ({settingsData.i_FieldofView})");
                settingsData.i_FieldofView = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_FieldofView, 50, 110, Array.Empty<GUILayoutOption>()));

                GUILayout.BeginHorizontal();
                GUILayout.Label("Thirdperson");
                UI.Keybind(ref settingsData.keyThirdperson);
                GUILayout.EndHorizontal();

                GUILayout.Label($"Distance ({settingsData.fl_ThirdpersonDistance})");
                settingsData.fl_ThirdpersonDistance = GUILayout.HorizontalSlider(settingsData.fl_ThirdpersonDistance, 1, 4);
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
                    if (PAUtils.WorldToScreen(Features.Thirdperson.ThirdpersonCamera.ViewState ? Features.Thirdperson.ThirdpersonCamera._camera 
                        : Instance.localPlayer.gameplayCamera, obj.transform.position, out pos))
                    {
                        string ObjName = PAUtils.ConvertFirstLetterToUpperCase(labelSelector(obj));
                        if (settingsData.b_DisplayDistance)
                            ObjName += " [" + distanceToPlayer.ToString().ToUpper() + "M]";
                        Render.String(Style, pos.x, pos.y, 150f, 50f, ObjName, colorSelector(obj), true, true);
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
        }

        public void Update()
        {
            if ((PAUtils.GetAsyncKeyState((int)Keys.Insert) & 1) != 0)
            {
                Settings.Instance.SaveSettings();
                Settings.Instance.b_isMenuOpen = !Settings.Instance.b_isMenuOpen;
            }
            if ((PAUtils.GetAsyncKeyState((int)Keys.Delete) & 1) != 0)
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
                    Instance.shipTerminal.PlayTerminalAudioServerRpc(1);
            }

            Features.Possession.UpdatePossession();
            Features.Misc.Noclip();

            if (settingsData.b_RemoveVisor) 
                Instance.localVisor?.SetActive(false);

            if (settingsData.b_AnonChatSpam)
                PAUtils.SendChatMessage(settingsData.str_ChatMessage);
        }

        private Vector2 scrollPos;
    }
}
