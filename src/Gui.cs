using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using static GameObjectManager;
using static LocalizationManager;
using System.Windows.Forms;

namespace ProjectApparatus
{
    internal class Gui : MonoBehaviour
    {
        private static GUIStyle Style = null;
        private readonly SettingsData settingsData = Settings.Instance.settingsData;

        private int selectedLanguageIndex = 0;
        private Dictionary<string, string> availableLanguages = new Dictionary<string, string>
    {
        {"en_US", "English"},
        {"ru_RU", "Русский"},
        {"de_DE", "German"},
        //new languages here, for example:
        //{"ts_TS", "Test Language" }
    };

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint && !Settings.Instance.b_isMenuOpen)
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
                if (settingsData.b_DisplayGroupCredits && Instance.shipTerminal != null) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, GetString("group_credits") + ": " + Instance.shipTerminal.groupCredits, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (settingsData.b_DisplayLootInShip && Instance.shipTerminal) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, GetString("loot_in_ship") + ": " + Instance.shipValue, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (settingsData.b_DisplayQuota && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, GetString("profit_quota") + ": " + TimeOfDay.Instance.quotaFulfilled + "/" + TimeOfDay.Instance.profitQuota, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (settingsData.b_DisplayDaysLeft && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, GetString("days_left") + ": " + TimeOfDay.Instance.daysUntilDeadline, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
            }

            string Watermark = GetString("watermark");
            Watermark += " | v" + settingsData.version;
            if (!Settings.Instance.b_isMenuOpen) Watermark += " | " + GetString("tgl_insert");
            if (!settingsData.b_CenteredIndicators)
            {
                if (settingsData.b_DisplayGroupCredits && Instance.shipTerminal != null)
                    Watermark += $" | " + $"{GetString("group_credits")}" + $": {Instance.shipTerminal.groupCredits}";
                if (settingsData.b_DisplayLootInShip && Instance.shipTerminal)
                    Watermark += $" | " + $"{GetString("loot_in_ship")}" + $": {Instance.shipValue}";
                if (settingsData.b_DisplayQuota && TimeOfDay.Instance)
                    Watermark += $" | " + $"{GetString("profit_quota")}" + $": {TimeOfDay.Instance.quotaFulfilled} / {TimeOfDay.Instance.profitQuota}";
                if (settingsData.b_DisplayDaysLeft && TimeOfDay.Instance)
                    Watermark += $" | " + $"{GetString("days_left")}" + $": {TimeOfDay.Instance.daysUntilDeadline}"; ;
            }

            Render.String(Style, 10f, 5f, 150f, Settings.TEXT_HEIGHT, Watermark, GUI.color);

            if (settingsData.b_DebugLogger)
            {
                Settings.Instance.consoleRect = GUILayout.Window(1, Settings.Instance.consoleRect, new GUI.WindowFunction(Log.Instance.ConsoleWindow), "Logger", Array.Empty<GUILayoutOption>());
            }

            if (Settings.Instance.b_isMenuOpen)
            {
                Settings.Instance.windowRect = GUILayout.Window(0, Settings.Instance.windowRect, new GUI.WindowFunction(MenuContent), GetString("watermark"), Array.Empty<GUILayoutOption>());
            }

            if (settingsData.b_Crosshair)
            {
                Render.FilledCircle(Features.Thirdperson.ThirdpersonCamera.ViewState ? new Vector2(826.14f, 562.32f) : centeredPos, 5, Color.black);
                Render.FilledCircle(Features.Thirdperson.ThirdpersonCamera.ViewState ? new Vector2(826.14f, 562.32f) : centeredPos, 3, settingsData.c_Theme);
            }
        }

        private PlayerControllerB selectedPlayer = null;
        private string objToSpawn = GetString("obj_to_spawn");
        private int objScrapValue = 0;
        private string objScrapValueStr = "99999";

        private void MenuContent(int windowID)
        {
            GUILayout.BeginHorizontal();
            UI.Tab(GetString("start"), ref UI.nTab, UI.Tabs.Start);
            UI.Tab(GetString("self"), ref UI.nTab, UI.Tabs.Self);
            UI.Tab(GetString("misc"), ref UI.nTab, UI.Tabs.Misc);
            UI.Tab(GetString("esp"), ref UI.nTab, UI.Tabs.ESP);
            UI.Tab(GetString("players"), ref UI.nTab, UI.Tabs.Players);
            UI.Tab(GetString("graphics"), ref UI.nTab, UI.Tabs.Graphics);
            UI.Tab(GetString("upgrades"), ref UI.nTab, UI.Tabs.Upgrades);
            UI.Tab(GetString("moons"), ref UI.nTab, UI.Tabs.Moons);
            UI.Tab(GetString("server"), ref UI.nTab, UI.Tabs.Server);
            UI.Tab(GetString("settings"), ref UI.nTab, UI.Tabs.Settings);
            GUILayout.EndHorizontal();


            UI.TabContents(GetString("start"), UI.Tabs.Start, () =>
            {
                GUILayout.Label($"{GetString("wlc_stp_1")}" + $" v{settingsData.version}. \n\n" + $"{GetString("wlc_stp_2")} \n" + $"{GetString("wlc_stp_3")}");
                GUILayout.Space(20f);
                GUILayout.Label($"{GetString("changelog")}" + $" {settingsData.version}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300f));
                GUILayout.TextArea(Settings.Changelog.changes.ToString(), GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
                GUILayout.Space(20f);
                GUILayout.Label($"{GetString("credits")}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                GUILayout.Label(Settings.Credits.credits.ToString());
            });


            UI.TabContents(GetString("self"), UI.Tabs.Self, () =>
            {
                UI.Checkbox(ref settingsData.b_GodMode, GetString("god_mode") , GetString("god_mode_descr"));
                UI.Checkbox(ref settingsData.b_Untargetable, GetString("untargetable"), GetString("untargetable_descr"));
                UI.Checkbox(ref settingsData.b_Invisibility, GetString("invisibility"), GetString("invisibility_desc"));
                UI.Checkbox(ref settingsData.b_InfiniteStam, GetString("infinite_stam"), GetString("infinite_stam_descr"));
                UI.Checkbox(ref settingsData.b_InfiniteCharge, GetString("infinite_charge"), GetString("infinite_charge_descr"));
                UI.Checkbox(ref settingsData.b_InfiniteZapGun, GetString("infinite_zap_gun"), GetString("infinite_zap_gun_descr"));
                UI.Checkbox(ref settingsData.b_InfiniteShotgunAmmo, GetString("infinite_shotgun_ammo"), GetString("infinite_shotgun_ammo_descr"));
                UI.Checkbox(ref settingsData.b_InfiniteItems, GetString("infinite_items"), GetString("infinite_items_descr"));
                UI.Checkbox(ref settingsData.b_RemoveWeight, GetString("remove_weight"), GetString("remove_weight_descr"));
                UI.Checkbox(ref settingsData.b_InteractThroughWalls, GetString("interact_through_walls"), GetString("interact_through_walls_descr"));
                UI.Checkbox(ref settingsData.b_UnlimitedGrabDistance, GetString("unlimited_grab_distance"), GetString("unlimited_grab_distance_descr"));
                UI.Checkbox(ref settingsData.b_OneHandAllObjects, GetString("one_hand_all_objects"), GetString("one_hand_all_objects_descr"));
                UI.Checkbox(ref settingsData.b_DisableFallDamage, GetString("disable_fall_damage"), GetString("disable_fall_damage_descr"));
                UI.Checkbox(ref settingsData.b_DisableInteractCooldowns, GetString("disable_interact_cooldowns"), GetString("disable_interact_cooldowns_descr"));
                UI.Checkbox(ref settingsData.b_InstantInteractions, GetString("instant_interactions"), GetString("instant_interactions_descr"));
                UI.Checkbox(ref settingsData.b_PlaceAnywhere, GetString("place_anywhere"), GetString("place_anywhere_descr"));
                UI.Checkbox(ref settingsData.b_TauntSlide, GetString("taunt_slide"), GetString("taunt_slide_descr"));
                UI.Checkbox(ref settingsData.b_FastLadderClimbing, GetString("fast_ladder_climbing"), GetString("fast_ladder_climbing_descr"));
                UI.Checkbox(ref settingsData.b_HearEveryone, GetString("hear_everyone"), GetString("hear_everyone_descr"));
                UI.Checkbox(ref settingsData.b_ChargeAnyItem, GetString("charge_any_item"), GetString("charge_any_item_descr"));
                UI.Checkbox(ref settingsData.b_NightVision, $"{GetString("night_vision")} ({settingsData.i_NightVision}%)", GetString("night_vision_descr"));
                settingsData.i_NightVision = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_NightVision, 1, 100));

                UI.Checkbox(ref settingsData.b_WalkSpeed, $"{GetString("adjust_walk_speed")} ({settingsData.i_WalkSpeed})", GetString("adjust_walk_speed_descr"));
                settingsData.i_WalkSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_WalkSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_SprintSpeed, $"{GetString("adjust_sprint_speed")} ({settingsData.i_SprintSpeed})", GetString("adjust_sprint_speed_descr"));
                settingsData.i_SprintSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_SprintSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_JumpHeight, $"{GetString("adjust_jump_height")}  ({settingsData.i_JumpHeight})", GetString("adjust_jump_height_descr"));
                settingsData.i_JumpHeight = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_JumpHeight, 1, 100));

                UI.Button(GetString("suicide"), GetString("suicide_descr"), () =>
                {
                    Instance.localPlayer.DamagePlayerFromOtherClientServerRpc(100, new Vector3(), -1);
                });

                UI.Button(GetString("respawn"), GetString("respawn_descr"), () =>
                {
                    Features.Misc.RespawnLocalPlayer();
                });

                UI.Button(GetString("teleport_to_ship"), GetString("teleport_to_ship_descr"), () =>
                {
                    if (Instance.shipRoom)
                        Instance.localPlayer?.TeleportPlayer(Instance.shipRoom.transform.position);
                });

                UI.Button(GetString("possess_nearest_enemy"), GetString("possess_nearest_enemy_descr"), () =>
                {
                    Features.Possession.StartPossession();
                });

                UI.Button(GetString("stop_possessing"), GetString("stop_possessing_descr"), () =>
                {
                    Features.Possession.StopPossession();
                });

                GUILayout.BeginHorizontal();
                UI.Checkbox(ref settingsData.b_Noclip, $"{GetString("noclip")} ({settingsData.fl_NoclipSpeed})", GetString("noclip_descr"));
                UI.Keybind(ref settingsData.keyNoclip);
                GUILayout.EndHorizontal();
                settingsData.fl_NoclipSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.fl_NoclipSpeed, 1, 100));
            });

            UI.TabContents(GetString("misc"), UI.Tabs.Misc, () =>
            {
                UI.Checkbox(ref settingsData.b_NoMoreCredits, GetString("no_more_credits"), GetString("no_more_credits_descr"));
                UI.Checkbox(ref settingsData.b_SensitiveLandmines, GetString("sensitive_landmines"), GetString("sensitive_landmines_descr"));
                UI.Checkbox(ref settingsData.b_AllJetpacksExplode, GetString("all_jetpacks_explode"), GetString("all_jetpacks_explode_descr"));
                UI.Checkbox(ref settingsData.b_LightShow, GetString("light_show"), GetString("light_show_descr"));
                UI.Checkbox(ref settingsData.b_TerminalNoisemaker, GetString("terminal_noisemaker"), GetString("terminal_noisemaker_descr"));
                UI.Checkbox(ref settingsData.b_AlwaysShowClock, GetString("always_show_clock"), GetString("always_show_clock_descr"));

                objToSpawn = GUILayout.TextField(objToSpawn, Array.Empty<GUILayoutOption>());
                UI.Button($"{GetString("spawn_object")}: {objToSpawn}", GetString("spawn_object_desc"), () =>
                { Instance.SpawnObject(objToSpawn, new Ray(Instance.localPlayer.gameplayCamera.transform.position, Instance.localPlayer.gameplayCamera.transform.forward).GetPoint(1f)); });
                UI.Button($"{GetString("del_object")}: {(Instance.currentlyHeldObjectServer ? Instance.currentlyHeldObjectServer?.itemProperties.itemName : GetString("no_held_object"))}", GetString("del_object_desc"), () =>
                {
                    Instance.DeleteHeldObject();
                });
                objScrapValueStr = GUILayout.TextField(objScrapValueStr);
                int.TryParse(objScrapValueStr, out objScrapValue);
                UI.Button($"{GetString("held_item_val")}: {objScrapValue}", GetString("held_item_val_desc"), () => {   Instance.localPlayer.currentlyHeldObjectServer.SetScrapValue(objScrapValue); });

                settingsData.str_ChatMessage = GUILayout.TextField(settingsData.str_ChatMessage, Array.Empty<GUILayoutOption>());
                UI.Button(GetString("send_message_misc"), GetString("send_message_misc_descr"), () =>
                {
                    PAUtils.SendChatMessage(settingsData.str_ChatMessage);
                });

                UI.Checkbox(ref settingsData.b_AnonChatSpam, GetString("spam_message_misc"), GetString("spam_message_misc_descr"));

                settingsData.str_TerminalSignal = GUILayout.TextField(settingsData.str_TerminalSignal, Array.Empty<GUILayoutOption>());
                UI.Button(GetString("send_signal"), GetString("send_signal_descr"), () =>
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

                    GUILayout.BeginHorizontal();

                    UI.Button(GetString("give_credits"), GetString("give_credits_descr"), () =>
                    {
                        if (Instance.shipTerminal)
                        {
                            Instance.shipTerminal.groupCredits += int.Parse(settingsData.str_MoneyToGive);
                            Instance.shipTerminal.SyncGroupCreditsServerRpc(Instance.shipTerminal.groupCredits, 
                                Instance.shipTerminal.numberOfItemsInDropship);
                        }
                    });

                    UI.Button(GetString("group_set_credits"), GetString("group_set_credits_descr"), () =>
                    {
                        if (Instance.shipTerminal)
                        {
                            Instance.shipTerminal.groupCredits = int.Parse(settingsData.str_MoneyToGive);
                            Instance.shipTerminal.SyncGroupCreditsServerRpc(Instance.shipTerminal.groupCredits,
                                Instance.shipTerminal.numberOfItemsInDropship);
                        }
                    });

                    UI.Button(GetString("take_credits"), GetString("take_credits_descr"), () =>
                    {
                        if (Instance.shipTerminal)
                        {
                            Instance.shipTerminal.groupCredits -= int.Parse(settingsData.str_MoneyToGive);
                            Instance.shipTerminal.SyncGroupCreditsServerRpc(Instance.shipTerminal.groupCredits,
                                Instance.shipTerminal.numberOfItemsInDropship);
                        }
                    });

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    settingsData.str_QuotaFulfilled = GUILayout.TextField(settingsData.str_QuotaFulfilled, GUILayout.Width(42));
                    GUILayout.Label("/", GUILayout.Width(4));
                    settingsData.str_Quota = GUILayout.TextField(settingsData.str_Quota, GUILayout.Width(42));
                    GUILayout.EndHorizontal();

                    UI.Button(GetString("set_quota"), GetString("set_quota_descr"), () =>
                    {
                        if (TimeOfDay.Instance)
                        {
                            TimeOfDay.Instance.profitQuota = int.Parse(settingsData.str_Quota);
                            TimeOfDay.Instance.quotaFulfilled = int.Parse(settingsData.str_QuotaFulfilled);
                            TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                        }
                    });
                }

                UI.Button($"{GetString("teleport_all_items")} ({Instance.items.Count})", GetString("teleport_all_items_descr"), () =>
                {
                    TeleportAllItems();
                });

                UI.Button(GetString("use_pocket_items"), GetString("use_pocket_items_desc"), () =>
                {
                    foreach (GrabbableObject obj in Instance.localPlayer.ItemSlots)
                    {
                        obj.isHeld = true;
                        obj.isPocketed = false;
                        obj.heldByPlayerOnServer = true;
                        obj.playerHeldBy = Instance.localPlayer;
                        if (obj.GetType() == typeof(ShotgunItem))
                        {
                            ShotgunItem shotgun = (ShotgunItem)obj;
                            shotgun.ShootGunAndSync(false);
                        }
                        else if(obj.GetType() == typeof(Shovel))
                        {
                            Shovel shovel = (Shovel)obj;
                            shovel.HitShovel();
                        }
                        else
                            obj.InteractItem();

                        obj.isHeld = false;
                        obj.isPocketed = true;
                        obj.heldByPlayerOnServer = false;
                        obj.playerHeldBy = null;
                    }
                });

                UI.Button(GetString("land_ship"), GetString("land_ship_descr"), () => StartOfRound.Instance.StartGameServerRpc());
                UI.Button(GetString("start_ship"), GetString("start_ship_descr"), () => StartOfRound.Instance.EndGameServerRpc(0));
                UI.Button(GetString("unlock_all_door"), GetString("unlock_all_door_descr"), () =>
                {
                    foreach (DoorLock obj in Instance.doorLocks) 
                        obj?.UnlockDoorSyncWithServer();
                });
                UI.Button(GetString("open_all_mechanical_doors"), GetString("open_all_mechanical_doors_descr"), () =>
                {
                    foreach (TerminalAccessibleObject obj in Instance.bigDoors)
                        obj?.SetDoorOpenServerRpc(true);
                });
                UI.Button(GetString("close_all_mechanical_doors"), GetString("close_all_mechanical_doors_descr"), () =>
                {
                    foreach (TerminalAccessibleObject obj in Instance.bigDoors)
                        obj?.SetDoorOpenServerRpc(false);
                });
                UI.Button(GetString("explode_all_mines"), GetString("explode_all_mines_descr"), () =>
                {
                    foreach (Landmine obj in Instance.landmines)
                        obj?.ExplodeMineServerRpc();
                });
                UI.Button(GetString("kill_all_enemies"), GetString("kill_all_enemies_descr"), () =>
                {
                    foreach (EnemyAI obj in Instance.enemies)
                        obj?.KillEnemyServerRpc(false);
                });
                UI.Button(GetString("delete_all_enemies"), GetString("delete_all_enemies_descr"), () =>
                {
                    foreach (EnemyAI obj in Instance.enemies)
                        obj?.KillEnemyServerRpc(true); //look into here for deleting 
                });
                UI.Button(GetString("attack_players_at_deposit_desk"), GetString("attack_players_at_deposit_desk_descr"), () =>
                {
                    if (Instance.itemsDesk)
                        Instance.itemsDesk.AttackPlayersServerRpc();
                });
            });


            UI.TabContents(GetString("esp"), UI.Tabs.ESP, () =>
            {
                UI.Checkbox(ref settingsData.b_EnableESP, GetString("enable_esp"), GetString("enable_esp_descr"));
                UI.Checkbox(ref settingsData.b_ItemESP, GetString("item_esp"), GetString("item_esp_descr"));
                UI.Checkbox(ref settingsData.b_EnemyESP, GetString("enemy_esp"), GetString("enemy_esp_descr"));
                UI.Checkbox(ref settingsData.b_PlayerESP, GetString("player_esp"), GetString("players_esp_descr"));
                UI.Checkbox(ref settingsData.b_ShipESP, GetString("ship_esp"), GetString("ship_esp_descr"));
                UI.Checkbox(ref settingsData.b_DoorESP, GetString("door_esp"), GetString("door_esp_descr"));
                UI.Checkbox(ref settingsData.b_SteamHazard, GetString("steam_hazard"), GetString("steam_hazard_esp_descr"));
                UI.Checkbox(ref settingsData.b_LandmineESP, GetString("landmine_esp"), GetString("landmine_esp_descr"));
                UI.Checkbox(ref settingsData.b_TurretESP, GetString("turret_esp"), GetString("turret_esp_descr"));
                UI.Checkbox(ref settingsData.b_DisplayHP, GetString("display_hp"), GetString("display_hp_esp_descr"));
                UI.Checkbox(ref settingsData.b_DisplayWorth, GetString("display_worth"), GetString("display_worth_esp_descr"));
                UI.Checkbox(ref settingsData.b_DisplayDistance, GetString("display_distance"), GetString("display_distance_esp_descr"));
                UI.Checkbox(ref settingsData.b_DisplaySpeaking, GetString("display_speaking"), GetString("display_speaking_esp_descr"));

                UI.Checkbox(ref settingsData.b_ItemDistanceLimit, GetString("item_distance_limit") +" (" + Mathf.RoundToInt(settingsData.fl_ItemDistanceLimit) + ")", GetString("item_distance_limit_descr"));
                settingsData.fl_ItemDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_ItemDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_EnemyDistanceLimit, GetString("enemy_distance_limit") + " (" + Mathf.RoundToInt(settingsData.fl_EnemyDistanceLimit) + ")", GetString("enemy_distance_limit_descr"));
                settingsData.fl_EnemyDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_EnemyDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_MineDistanceLimit, GetString("mine_distance_limit") + " (" + Mathf.RoundToInt(settingsData.fl_MineDistanceLimit) + ")", GetString("landmine_distance_limit_descr"));
                settingsData.fl_MineDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_MineDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_TurretDistanceLimit, GetString("turret_distance_limit") + " (" + Mathf.RoundToInt(settingsData.fl_TurretDistanceLimit) + ")", GetString("turret_distance_limit_descr"));
                settingsData.fl_TurretDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_TurretDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());
            });

            UI.TabContents(null, UI.Tabs.Players, () =>
            {
                GUILayout.BeginHorizontal();
                foreach (PlayerControllerB player in Instance.players)
                {
                    if (!PAUtils.IsPlayerValid(player)) continue;
                    UI.Tab(PAUtils.TruncateString(player.playerUsername, 12), ref selectedPlayer, player, true);
                }
                GUILayout.EndHorizontal();

                if (!PAUtils.IsPlayerValid(selectedPlayer))
                    selectedPlayer = null;

                if (selectedPlayer)
                {
                    UI.Header(GetString("selected_player") + ": " + selectedPlayer.playerUsername + (selectedPlayer.IsHost ? "[HOST]" : ""));
                    Settings.Instance.InitializeDictionaries(selectedPlayer);

                    // We keep toggles outside of the isPlayerDead check so that users can toggle them on/off no matter their condition.

                    bool b_DemiGod = Settings.Instance.b_DemiGod[selectedPlayer];
                    UI.Checkbox(ref b_DemiGod, GetString("demigod"), GetString("demigod_descr"));
                    Settings.Instance.b_DemiGod[selectedPlayer] = b_DemiGod;

                    bool b_SpamObjects = Settings.Instance.b_SpamObjects[selectedPlayer];
                    UI.Checkbox(ref b_SpamObjects, GetString("object_spam"), GetString("object_spam_descr"));
                    Settings.Instance.b_SpamObjects[selectedPlayer] = b_SpamObjects;

                    UI.Checkbox(ref Settings.Instance.b_HideObjects, GetString("hide_objects"), GetString("hide_objects_descr"));

                    if (!selectedPlayer.isPlayerDead)
                    {
                        UI.Button(GetString("spawn_enemy"), GetString("spawn_enemy_descr"), () =>
                        {
                            RoundManager.Instance.SpawnEnemyGameObject(selectedPlayer.gameplayCamera.transform.position, 0, -1);
                        });
                        UI.Button(GetString("kill"), GetString("kill_descr"), () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(selectedPlayer.health + 1, new Vector3(900, 900, 900), 0); });
                        UI.Button(GetString("teleport_to"), GetString("teleport_to_descr"), () => { Instance.localPlayer.TeleportPlayer(selectedPlayer.playerGlobalHead.position); });
                        UI.Button(GetString("teleport_enemies_to"), GetString("teleport_enemies_to_descr"), () =>
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
                        UI.Button(GetString("teleport_player_to_ship"), GetString("teleport_player_to_ship_descr"), () =>
                        {
                            Instance.shipTeleporter.TeleportPlayerOutServerRpc((int)selectedPlayer.playerClientId, Instance.shipRoom.transform.position);
                        });

                        UI.Button(GetString("aggro_enemies"), GetString("aggro_enemies_descr_1") + "\n" + GetString("aggro_enemies_descr_2"), () => {
                            foreach (EnemyAI enemy in Instance.enemies)
                            {
                                enemy.targetPlayer = selectedPlayer;
                                
                                enemy.SwitchToBehaviourServerRpc(1); // I believe this just angers all enemies.
                                if(enemy.GetType() == typeof(FlowermanAI))
                                {
                                    FlowermanAI flowerman = (FlowermanAI)enemy;
                                    flowerman.SetMovingTowardsTargetPlayer(selectedPlayer);
                                }
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
                        UI.Button(GetString("damage"), GetString("damage_descr"), () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(int.Parse(Settings.Instance.str_DamageToGive), new Vector3(900, 900, 900), 0); });

                        Settings.Instance.str_HealthToHeal = GUILayout.TextField(Settings.Instance.str_HealthToHeal, Array.Empty<GUILayoutOption>());
                        UI.Button(GetString("heal"), GetString("heal_descr"), () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(-int.Parse(Settings.Instance.str_HealthToHeal), new Vector3(900, 900, 900), 0); });
                    }

                    Settings.Instance.str_ChatAsPlayer = GUILayout.TextField(Settings.Instance.str_ChatAsPlayer, Array.Empty<GUILayoutOption>());
                    UI.Button(GetString("send_message_player"), GetString("send_message_player_descr"), () => { PAUtils.SendChatMessage(Settings.Instance.str_ChatAsPlayer, (int)selectedPlayer.playerClientId); });

                    bool SpamChatCheck = Settings.Instance.b_SpamChat[selectedPlayer];
                    UI.Checkbox(ref SpamChatCheck, GetString("spam_message_player"), GetString("spam_message_player_descr"));
                    Settings.Instance.b_SpamChat[selectedPlayer] = SpamChatCheck;

                    UI.Button(GetString("steam_profile"), GetString("steam_profile_descr"), () => { SteamFriends.OpenUserOverlay(selectedPlayer.playerSteamId, "steamid"); });
                }
            });

            if (StartOfRound.Instance && Instance.shipTerminal)
            {
                UI.TabContents(GetString("upgrades"), UI.Tabs.Upgrades, () =>
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
                        GUILayout.Label(GetString("u_alrd_unlc_all"));
                    }
                    else
                    {
                        UI.Button(GetString("unlc_all_upgrd_ship"), GetString("unlc_all_upgrd_ship_descr"), () =>
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
                            UI.Button(GetString("unlc_all_suits"), GetString("unlc_all_suits_descr"), () =>
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

                                UI.Button(unlockableName, $"{GetString("unlock")} {unlockableName}", () =>
                                {
                                    StartOfRound.Instance.BuyShipUnlockableServerRpc(i, Instance.shipTerminal.groupCredits);
                                    StartOfRound.Instance.SyncShipUnlockablesServerRpc();
                                });
                            }
                        }
                    }
                });
            }

            if (StartOfRound.Instance && GameObjectManager.Instance.shipTerminal)
            {
                UI.TabContents(GetString("moons"), UI.Tabs.Moons, () =>
                {
                    int[] levelOrder = { 3, 0, 1, 2, 7, 4, 5, 6, 8 };

                    Dictionary<int, int> order = levelOrder
                        .Select((id, index) => new { id, index })
                        .ToDictionary(item => item.id, item => item.index);

                    foreach (SelectableLevel x in StartOfRound.Instance.levels.OrderBy(x => order[x.levelID]))
                    {
                        if (x.levelID == StartOfRound.Instance.currentLevel.levelID) continue;
                        string weather = x.currentWeather == LevelWeatherType.None ? "" : $" ({x.currentWeather})";
                        UI.Button($"{x.PlanetName}{weather}", "", () => Features.ChangeMoon(x.levelID));

                    }
                });
            }

            UI.TabContents(GetString("server"), UI.Tabs.Server, () =>
            {
                GUILayout.Label(GetString("lobbyid"));
                settingsData.textlobbyid = GUILayout.TextField(settingsData.textlobbyid, Array.Empty<GUILayoutOption>());
                GUILayout.BeginHorizontal();
                UI.Button(GetString("getlobbyid"), GetString("getlobbyid_descr"), () =>
                {
                    settingsData.textlobbyid = GUILayout.TextField(Settings.Str_lobbyid.ToString(), Array.Empty<GUILayoutOption>());
                    GUIUtility.systemCopyBuffer = Settings.Str_lobbyid.ToString();
                });
                UI.Button(GetString("connect"), GetString("connect_descr"), () =>
                {
                    SteamId? steamId = TryParseSteamId(Settings.Str_lobbyid.ToString()) ?? Settings.Str_lobbyid;

                    if (!(steamId is SteamId lobbyId))
                    {
                        return;
                    }

                    GameNetworkManager.Instance.StartClient(lobbyId);
                    GUIUtility.systemCopyBuffer = "";
                });
                UI.Button(GetString("disconnect"), GetString("disconnect_descr"), () =>
                {
                    GameNetworkManager.Instance.Disconnect();
                    Settings.DisconnectedVoluntarily = true;
                });
                GUILayout.EndHorizontal();
            });

            UI.TabContents(GetString("graphics"), UI.Tabs.Graphics, () =>
            {
                UI.Checkbox(ref settingsData.b_DisableFog, GetString("disable_fog"), GetString("disable_fog_descr"));
                UI.Checkbox(ref settingsData.b_DisableDepthOfField, GetString("disable_dof"), GetString("disable_dof_descr"));
                if (UI.Checkbox(ref settingsData.b_RemoveVisor, GetString("disable_visor"), GetString("disable_visor_descr")))
                {
                    if (!settingsData.b_RemoveVisor && !Features.Thirdperson.ThirdpersonCamera.ViewState)
                        Instance.localVisor?.SetActive(true);
                }
                UI.Checkbox(ref settingsData.b_CameraResolution, GetString("render_resolution"), GetString("camera_res_descr_1") + "\n " + GetString("camera_res_descr_2"));
                GUILayout.Label($"{GetString("fov")} ({settingsData.i_FieldofView})");
                settingsData.i_FieldofView = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_FieldofView, 50, 110, Array.Empty<GUILayoutOption>()));

                GUILayout.BeginHorizontal();
                GUILayout.Label(GetString("thirdperson"));
                UI.Keybind(ref settingsData.keyThirdperson);
                GUILayout.EndHorizontal();

                GUILayout.Label($"{GetString("distance")} ({settingsData.fl_ThirdpersonDistance})");
                settingsData.fl_ThirdpersonDistance = GUILayout.HorizontalSlider(settingsData.fl_ThirdpersonDistance, 1, 4);
            });

            UI.TabContents(GetString("settings"), UI.Tabs.Settings, () =>
            {
                UI.Checkbox(ref settingsData.b_Crosshair, GetString("crosshair"), GetString("crosshair_descr"));
                UI.Checkbox(ref settingsData.b_DisplayGroupCredits, GetString("display_group_credits"), GetString("display_group_credits_descr"));
                UI.Checkbox(ref settingsData.b_DisplayLootInShip, GetString("display_loot_in_ship"), GetString("display_loot_in_ship_descr"));
                UI.Checkbox(ref settingsData.b_DisplayQuota, GetString("display_quota"), GetString("display_quota_descr"));
                UI.Checkbox(ref settingsData.b_DisplayDaysLeft, GetString("display_days_left"), GetString("display_days_left_descr"));
                UI.Checkbox(ref settingsData.b_CenteredIndicators, GetString("centered_indicators"), GetString("centered_indicators_descr"));
                UI.Checkbox(ref settingsData.b_DeadPlayers, GetString("dead_players"), GetString("dead_players_descr"));
                UI.Checkbox(ref settingsData.b_Tooltips, GetString("tooltips"), GetString("tooltips_descr"));
                UI.Checkbox(ref settingsData.b_DebugLogger, GetString("dbg_log"), GetString("dbg_log_desc"));

                if(settingsData.b_DebugLogger) //leaving here for now
                {
                    if (GUILayout.Button("test info"))
                        Log.Info("test info");
                    if (GUILayout.Button("test warning"))
                        Log.Warning("test warning");
                    if (GUILayout.Button("test error"))
                        Log.Error("test error");
                    if (GUILayout.Button("test Message"))
                        Log.Message("test Message");
                }

                UI.Header(GetString("colors"));
                UI.ColorPicker(GetString("theme"), ref settingsData.c_Theme);
                UI.ColorPicker(GetString("valve"), ref settingsData.c_Valve);
                UI.ColorPicker(GetString("enemy"), ref settingsData.c_Enemy);
                UI.ColorPicker(GetString("turret"), ref settingsData.c_Turret);
                UI.ColorPicker(GetString("landmine"), ref settingsData.c_Landmine);
                UI.ColorPicker(GetString("player"), ref settingsData.c_Player);
                UI.ColorPicker(GetString("door"), ref settingsData.c_Door);
                UI.ColorPicker(GetString("loot"), ref settingsData.c_Loot);
                UI.ColorPicker(GetString("small_loot"), ref settingsData.c_smallLoot);
                UI.ColorPicker(GetString("medium_loot"), ref settingsData.c_medLoot);
                UI.ColorPicker(GetString("big_loot"), ref settingsData.c_bigLoot);

                GUILayout.Space(20);
                GUILayout.Label(GetString("select_language")+":");

                List<string> languageNames = new List<string>(availableLanguages.Values);
                selectedLanguageIndex = GUILayout.Toolbar(selectedLanguageIndex, languageNames.ToArray(), GUILayout.ExpandWidth(true));

                GUILayout.Space(10);

                if (GUILayout.Button(GetString("apply")))
                {
                    string selectedLanguage = availableLanguages.Keys.ToArray()[selectedLanguageIndex];
                    LocalizationManager.SetLanguage(selectedLanguage);
                    Debug.Log("Selected Language: " + selectedLanguage);
                }

            });

            UI.RenderTooltip();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }
        public static SteamId? TryParseSteamId(string input)
        {
            if (ulong.TryParse(input, out ulong result))
            {
                return (SteamId)result;
            }
            return null;
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
                        Vector3 point = new Ray(Instance.localPlayer.gameplayCamera.transform.position, Instance.localPlayer.gameplayCamera.transform.forward).GetPoint(1f);
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

            PAUtils.ForEach(objects, (obj) =>
            {
                if (obj.gameObject.activeSelf)
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
            });
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
                _ => GetString("ship"),
                _ => settingsData.c_Door
            );
        }

        private void DisplayDoors()
        {
            DisplayObjects(
                Instance.entranceTeleports,
                settingsData.b_DoorESP,
                entranceTeleport => entranceTeleport.isEntranceToBuilding ? GetString("entrance") : GetString("exit"),
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
                _ => GetString("landmine"),
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
                _ => GetString("turret"),
                _ => settingsData.c_Turret
            );
        }

        private void DisplaySteamHazard()
        {
            DisplayObjects(
                Instance.steamValves.Where(steamValveHazard => steamValveHazard != null && steamValveHazard.triggerScript.interactable),
                settingsData.b_SteamHazard,
                _ => GetString("steam_valve"),
                _ => settingsData.c_Valve
            );
        }

        private void DisplayPlayers()
        {
            DisplayObjects(
                Instance.players.Where(playerControllerB =>
                    PAUtils.IsPlayerValid(playerControllerB) &&
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
                    return string.IsNullOrWhiteSpace(name) ? GetString("enemy") : name;
                },
                _ => settingsData.c_Enemy
            );
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
                    string text = GetString("object");
                    Item itemProperties = grabbableObject.itemProperties;
                    if (itemProperties.itemName != null)
                        text = itemProperties.itemName;
                    int scrapValue = grabbableObject.scrapValue;
                    if (settingsData.b_DisplayWorth && scrapValue > 0)
                        text += " [" + scrapValue.ToString() + "C]";
                    return text;
                },
                grabbableObject => PAUtils.GetLootColor(grabbableObject.scrapValue)
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
