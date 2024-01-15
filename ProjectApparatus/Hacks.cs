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
        private int selectedLanguageIndex = 0;

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
                if (settingsData.b_DisplayGroupCredits && Instance.shipTerminal != null) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, LocalizationManager.GetString("group_credits") + ": " + Instance.shipTerminal.groupCredits, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (settingsData.b_DisplayLootInShip && Instance.shipTerminal) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, LocalizationManager.GetString("loot_in_ship") + ": " + Instance.shipValue, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (settingsData.b_DisplayQuota && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, LocalizationManager.GetString("profit_quota") + ": " + TimeOfDay.Instance.quotaFulfilled + "/" + TimeOfDay.Instance.profitQuota, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
                if (settingsData.b_DisplayDaysLeft && TimeOfDay.Instance) Render.String(Style, centeredPos.x, centeredPos.y + 7 + iY, 150f, Settings.TEXT_HEIGHT, LocalizationManager.GetString("days_left") + ": " + TimeOfDay.Instance.daysUntilDeadline, GUI.color, true, true); iY += Settings.TEXT_HEIGHT - 10f;
            }

            string Watermark = LocalizationManager.GetString("watermark");
            Watermark += " | v" + settingsData.version;
            if (!Settings.Instance.b_isMenuOpen) Watermark += " | " + LocalizationManager.GetString("tgl_insert");
            if (!settingsData.b_CenteredIndicators)
            {
                if (settingsData.b_DisplayGroupCredits && Instance.shipTerminal != null)
                    Watermark += $" | " + $"{LocalizationManager.GetString("group_credits")}" + $": {Instance.shipTerminal.groupCredits}";
                if (settingsData.b_DisplayLootInShip && Instance.shipTerminal)
                    Watermark += $" | " + $"{LocalizationManager.GetString("loot_in_ship")}" + $": {Instance.shipValue}";
                if (settingsData.b_DisplayQuota && TimeOfDay.Instance)
                    Watermark += $" | " + $"{LocalizationManager.GetString("profit_quota")}" + $": {TimeOfDay.Instance.quotaFulfilled} / {TimeOfDay.Instance.profitQuota}";
                if (settingsData.b_DisplayDaysLeft && TimeOfDay.Instance)
                    Watermark += $" | " + $"{LocalizationManager.GetString("days_left")}" + $": {TimeOfDay.Instance.daysUntilDeadline}"; ;
            }

            Render.String(Style, 10f, 5f, 150f, Settings.TEXT_HEIGHT, Watermark, GUI.color);

            if (Settings.Instance.b_isMenuOpen)
            {
                Settings.Instance.windowRect = GUILayout.Window(0, Settings.Instance.windowRect, new GUI.WindowFunction(MenuContent), LocalizationManager.GetString("watermark"), Array.Empty<GUILayoutOption>());
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
            UI.Tab(LocalizationManager.GetString("start"), ref UI.nTab, UI.Tabs.Start);
            UI.Tab(LocalizationManager.GetString("self"), ref UI.nTab, UI.Tabs.Self);
            UI.Tab(LocalizationManager.GetString("misc"), ref UI.nTab, UI.Tabs.Misc);
            UI.Tab(LocalizationManager.GetString("esp"), ref UI.nTab, UI.Tabs.ESP);
            UI.Tab(LocalizationManager.GetString("players"), ref UI.nTab, UI.Tabs.Players);
            UI.Tab(LocalizationManager.GetString("graphics"), ref UI.nTab, UI.Tabs.Graphics);
            UI.Tab(LocalizationManager.GetString("upgrades"), ref UI.nTab, UI.Tabs.Upgrades);
            UI.Tab(LocalizationManager.GetString("settings"), ref UI.nTab, UI.Tabs.Settings);
            GUILayout.EndHorizontal();


            UI.TabContents(LocalizationManager.GetString("start"), UI.Tabs.Start, () =>
            {
                string versionTxt = $"v{settingsData.version}";
                string wlc_stp_1 = LocalizationManager.GetString("wlc_stp_1");
                string wlc_stp_2 = LocalizationManager.GetString("wlc_stp_2");
                string wlc_stp_3 = LocalizationManager.GetString("wlc_stp_3");

                GUILayout.Label($"{LocalizationManager.GetString("wlc_stp_1")}" + $" v{settingsData.version}. \n\n" + $"{LocalizationManager.GetString("wlc_stp_2")} \n" + $"{LocalizationManager.GetString("wlc_stp_3")}");
                GUILayout.Space(20f);
                GUILayout.Label($"{LocalizationManager.GetString("changelog")}" + $" {settingsData.version}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300f));
                GUILayout.TextArea(Settings.Changelog.changes.ToString(), GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();
                GUILayout.Space(20f);
                GUILayout.Label($"{LocalizationManager.GetString("credits")}", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
                GUILayout.Label(Settings.Credits.credits.ToString());
            });


            UI.TabContents(LocalizationManager.GetString("self"), UI.Tabs.Self, () =>
            {
                UI.Checkbox(ref settingsData.b_GodMode, LocalizationManager.GetString("god_mode"), LocalizationManager.GetString("god_mode_descr"));
                UI.Checkbox(ref settingsData.b_Invisibility, LocalizationManager.GetString("invisibility"), LocalizationManager.GetString("invisibility_desc"));
                UI.Checkbox(ref settingsData.b_InfiniteStam, LocalizationManager.GetString("infinite_stam"), LocalizationManager.GetString("infinite_stam_descr"));
                UI.Checkbox(ref settingsData.b_InfiniteCharge, LocalizationManager.GetString("infinite_charge"), LocalizationManager.GetString("infinite_charge_descr"));
                UI.Checkbox(ref settingsData.b_InfiniteZapGun, LocalizationManager.GetString("infinite_zap_gun"), LocalizationManager.GetString("infinite_zap_gun_descr"));
                UI.Checkbox(ref settingsData.b_InfiniteShotgunAmmo, LocalizationManager.GetString("infinite_shotgun_ammo"), LocalizationManager.GetString("infinite_shotgun_ammo_descr"));
                UI.Checkbox(ref settingsData.b_InfiniteItems, LocalizationManager.GetString("infinite_items"), LocalizationManager.GetString("infinite_items_descr"));
                UI.Checkbox(ref settingsData.b_RemoveWeight, LocalizationManager.GetString("remove_weight"), LocalizationManager.GetString("remove_weight_descr"));
                UI.Checkbox(ref settingsData.b_InteractThroughWalls, LocalizationManager.GetString("interact_through_walls"), LocalizationManager.GetString("interact_through_walls_descr"));
                UI.Checkbox(ref settingsData.b_UnlimitedGrabDistance, LocalizationManager.GetString("unlimited_grab_distance"), LocalizationManager.GetString("unlimited_grab_distance_descr"));
                UI.Checkbox(ref settingsData.b_OneHandAllObjects, LocalizationManager.GetString("one_hand_all_objects"), LocalizationManager.GetString("one_hand_all_objects_descr"));
                UI.Checkbox(ref settingsData.b_DisableFallDamage, LocalizationManager.GetString("disable_fall_damage"), LocalizationManager.GetString("disable_fall_damage_descr"));
                UI.Checkbox(ref settingsData.b_DisableInteractCooldowns, LocalizationManager.GetString("disable_interact_cooldowns"), LocalizationManager.GetString("disable_interact_cooldowns_descr"));
                UI.Checkbox(ref settingsData.b_InstantInteractions, LocalizationManager.GetString("instant_interactions"), LocalizationManager.GetString("instant_interactions_descr"));
                UI.Checkbox(ref settingsData.b_PlaceAnywhere, LocalizationManager.GetString("place_anywhere"), LocalizationManager.GetString("place_anywhere_descr"));
                UI.Checkbox(ref settingsData.b_TauntSlide, LocalizationManager.GetString("taunt_slide"), LocalizationManager.GetString("taunt_slide_descr"));
                UI.Checkbox(ref settingsData.b_FastLadderClimbing, LocalizationManager.GetString("fast_ladder_climbing"), LocalizationManager.GetString("fast_ladder_climbing_descr"));
                UI.Checkbox(ref settingsData.b_HearEveryone, LocalizationManager.GetString("hear_everyone"), LocalizationManager.GetString("hear_everyone_descr"));
                UI.Checkbox(ref settingsData.b_ChargeAnyItem, LocalizationManager.GetString("charge_any_item"), LocalizationManager.GetString("charge_any_item_descr"));
                UI.Checkbox(ref settingsData.b_NightVision, $"{LocalizationManager.GetString("night_vision")} ({settingsData.i_NightVision}%)", LocalizationManager.GetString("night_vision_descr"));
                settingsData.i_NightVision = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_NightVision, 1, 100));

                UI.Checkbox(ref settingsData.b_WalkSpeed, $"{LocalizationManager.GetString("adjust_walk_speed")} ({settingsData.i_WalkSpeed})", LocalizationManager.GetString("adjust_walk_speed_descr"));
                settingsData.i_WalkSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_WalkSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_SprintSpeed, $"{LocalizationManager.GetString("adjust_sprint_speed")} ({settingsData.i_SprintSpeed})", LocalizationManager.GetString("adjust_sprint_speed_descr"));
                settingsData.i_SprintSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_SprintSpeed, 1, 20));
                UI.Checkbox(ref settingsData.b_JumpHeight, $"{LocalizationManager.GetString("adjust_jump_height")}  ({settingsData.i_JumpHeight})", LocalizationManager.GetString("adjust_jump_height_descr"));
                settingsData.i_JumpHeight = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_JumpHeight, 1, 100));

                UI.Button(LocalizationManager.GetString("suicide"), LocalizationManager.GetString("suicide_descr"), () =>
                {
                    Instance.localPlayer.DamagePlayerFromOtherClientServerRpc(100, new Vector3(), -1);
                });

                UI.Button(LocalizationManager.GetString("respawn"), LocalizationManager.GetString("respawn_descr"), () =>
                {
                    Features.Misc.RespawnLocalPlayer();
                });

                UI.Button(LocalizationManager.GetString("teleport_to_ship"), LocalizationManager.GetString("teleport_to_ship_descr"), () =>
                {
                    if (Instance.shipRoom)
                        Instance.localPlayer?.TeleportPlayer(Instance.shipRoom.transform.position);
                });

                UI.Button(LocalizationManager.GetString("possess_nearest_enemy"), LocalizationManager.GetString("possess_nearest_enemy_descr"), () =>
                {
                    Features.Possession.StartPossession();
                });

                UI.Button(LocalizationManager.GetString("stop_possessing"), LocalizationManager.GetString("stop_possessing_descr"), () =>
                {
                    Features.Possession.StopPossession();
                });

                GUILayout.BeginHorizontal();
                UI.Checkbox(ref settingsData.b_Noclip, $"{LocalizationManager.GetString("noclip")} ({settingsData.fl_NoclipSpeed})", LocalizationManager.GetString("noclip_descr"));
                UI.Keybind(ref settingsData.keyNoclip);
                GUILayout.EndHorizontal();
                settingsData.fl_NoclipSpeed = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.fl_NoclipSpeed, 1, 100));
            });

            UI.TabContents(LocalizationManager.GetString("misc"), UI.Tabs.Misc, () =>
            {
                UI.Checkbox(ref settingsData.b_NoMoreCredits, LocalizationManager.GetString("no_more_credits"), LocalizationManager.GetString("no_more_credits_descr"));
                UI.Checkbox(ref settingsData.b_SensitiveLandmines, LocalizationManager.GetString("sensitive_landmines"), LocalizationManager.GetString("sensitive_landmines_descr"));
                UI.Checkbox(ref settingsData.b_AllJetpacksExplode, LocalizationManager.GetString("all_jetpacks_explode"), LocalizationManager.GetString("all_jetpacks_explode_descr"));
                UI.Checkbox(ref settingsData.b_LightShow, LocalizationManager.GetString("light_show"), LocalizationManager.GetString("light_show_descr"));
                UI.Checkbox(ref settingsData.b_TerminalNoisemaker, LocalizationManager.GetString("terminal_noisemaker"), LocalizationManager.GetString("terminal_noisemaker_descr"));
                UI.Checkbox(ref settingsData.b_AlwaysShowClock, LocalizationManager.GetString("always_show_clock"), LocalizationManager.GetString("always_show_clock_descr"));


                settingsData.str_ChatMessage = GUILayout.TextField(settingsData.str_ChatMessage, Array.Empty<GUILayoutOption>());
                UI.Button(LocalizationManager.GetString("send_message_misc"), LocalizationManager.GetString("send_message_misc_descr"), () =>
                {
                    PAUtils.SendChatMessage(settingsData.str_ChatMessage);
                });

                UI.Checkbox(ref settingsData.b_AnonChatSpam, LocalizationManager.GetString("spam_message_misc"), LocalizationManager.GetString("spam_message_misc_descr"));

                settingsData.str_TerminalSignal = GUILayout.TextField(settingsData.str_TerminalSignal, Array.Empty<GUILayoutOption>());
                UI.Button(LocalizationManager.GetString("send_signal"), LocalizationManager.GetString("send_signal_descr"), () =>
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
                    UI.Button(LocalizationManager.GetString("give_credits"), LocalizationManager.GetString("give_credits_descr"), () =>
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

                    UI.Button(LocalizationManager.GetString("set_quota"), LocalizationManager.GetString("set_quota_descr"), () =>
                    {
                        if (TimeOfDay.Instance)
                        {
                            TimeOfDay.Instance.profitQuota = int.Parse(settingsData.str_Quota);
                            TimeOfDay.Instance.quotaFulfilled = int.Parse(settingsData.str_QuotaFulfilled);
                            TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                        }
                    });
                }

                UI.Button($"{LocalizationManager.GetString("teleport_all_items")} ({Instance.items.Count})", LocalizationManager.GetString("teleport_all_items_descr"), () =>
                {
                    TeleportAllItems();
                });

                UI.Button(LocalizationManager.GetString("land_ship"), LocalizationManager.GetString("land_ship_descr"), () => StartOfRound.Instance.StartGameServerRpc());
                UI.Button(LocalizationManager.GetString("start_ship"), LocalizationManager.GetString("start_ship_descr"), () => StartOfRound.Instance.EndGameServerRpc(0));
                UI.Button(LocalizationManager.GetString("unlock_all_door"), LocalizationManager.GetString("unlock_all_door_descr"), () =>
                {
                    foreach (DoorLock obj in Instance.doorLocks)
                        obj?.UnlockDoorSyncWithServer();
                });
                UI.Button(LocalizationManager.GetString("open_all_mechanical_doors"), LocalizationManager.GetString("open_all_mechanical_doors_descr"), () =>
                {
                    foreach (TerminalAccessibleObject obj in Instance.bigDoors)
                        obj?.SetDoorOpenServerRpc(true);
                });
                UI.Button(LocalizationManager.GetString("close_all_mechanical_doors"), LocalizationManager.GetString("close_all_mechanical_doors_descr"), () =>
                {
                    foreach (TerminalAccessibleObject obj in Instance.bigDoors)
                        obj?.SetDoorOpenServerRpc(false);
                });
                UI.Button(LocalizationManager.GetString("explode_all_mines"), LocalizationManager.GetString("explode_all_mines_descr"), () =>
                {
                    foreach (Landmine obj in Instance.landmines)
                        obj?.ExplodeMineServerRpc();
                });
                UI.Button(LocalizationManager.GetString("kill_all_enemies"), LocalizationManager.GetString("kill_all_enemies_descr"), () =>
                {
                    foreach (EnemyAI obj in Instance.enemies)
                        obj?.KillEnemyServerRpc(false);
                });
                UI.Button(LocalizationManager.GetString("delete_all_enemies"), LocalizationManager.GetString("delete_all_enemies_descr"), () =>
                {
                    foreach (EnemyAI obj in Instance.enemies)
                        obj?.KillEnemyServerRpc(true);
                });
                UI.Button(LocalizationManager.GetString("attack_players_at_deposit_desk"), LocalizationManager.GetString("attack_players_at_deposit_desk_descr"), () =>
                {
                    if (Instance.itemsDesk)
                        Instance.itemsDesk.AttackPlayersServerRpc();
                });
            });


            UI.TabContents(LocalizationManager.GetString("esp"), UI.Tabs.ESP, () =>
            {
                UI.Checkbox(ref settingsData.b_EnableESP, LocalizationManager.GetString("enable_esp"), LocalizationManager.GetString("enable_esp_descr"));
                UI.Checkbox(ref settingsData.b_ItemESP, LocalizationManager.GetString("item_esp"), LocalizationManager.GetString("item_esp_descr"));
                UI.Checkbox(ref settingsData.b_EnemyESP, LocalizationManager.GetString("enemy_esp"), LocalizationManager.GetString("enemy_esp_descr"));
                UI.Checkbox(ref settingsData.b_PlayerESP, LocalizationManager.GetString("player_esp"), LocalizationManager.GetString("players_esp_descr"));
                UI.Checkbox(ref settingsData.b_ShipESP, LocalizationManager.GetString("ship_esp"), LocalizationManager.GetString("ship_esp_descr"));
                UI.Checkbox(ref settingsData.b_DoorESP, LocalizationManager.GetString("door_esp"), LocalizationManager.GetString("door_esp_descr"));
                UI.Checkbox(ref settingsData.b_SteamHazard, LocalizationManager.GetString("steam_hazard"), LocalizationManager.GetString("steam_hazard_esp_descr"));
                UI.Checkbox(ref settingsData.b_LandmineESP, LocalizationManager.GetString("landmine_esp"), LocalizationManager.GetString("landmine_esp_descr"));
                UI.Checkbox(ref settingsData.b_TurretESP, LocalizationManager.GetString("turret_esp"), LocalizationManager.GetString("turret_esp_descr"));
                UI.Checkbox(ref settingsData.b_DisplayHP, LocalizationManager.GetString("display_hp"), LocalizationManager.GetString("display_hp_esp_descr"));
                UI.Checkbox(ref settingsData.b_DisplayWorth, LocalizationManager.GetString("display_worth"), LocalizationManager.GetString("display_worth_esp_descr"));
                UI.Checkbox(ref settingsData.b_DisplayDistance, LocalizationManager.GetString("display_distance"), LocalizationManager.GetString("display_distance_esp_descr"));
                UI.Checkbox(ref settingsData.b_DisplaySpeaking, LocalizationManager.GetString("display_speaking"), LocalizationManager.GetString("display_speaking_esp_descr"));

                UI.Checkbox(ref settingsData.b_ItemDistanceLimit, LocalizationManager.GetString("item_distance_limit") + " (" + Mathf.RoundToInt(settingsData.fl_ItemDistanceLimit) + ")", LocalizationManager.GetString("item_distance_limit_descr"));
                settingsData.fl_ItemDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_ItemDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_EnemyDistanceLimit, LocalizationManager.GetString("enemy_distance_limit") + " (" + Mathf.RoundToInt(settingsData.fl_EnemyDistanceLimit) + ")", LocalizationManager.GetString("enemy_distance_limit_descr"));
                settingsData.fl_EnemyDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_EnemyDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_MineDistanceLimit, LocalizationManager.GetString("mine_distance_limit") + " (" + Mathf.RoundToInt(settingsData.fl_MineDistanceLimit) + ")", LocalizationManager.GetString("landmine_distance_limit_descr"));
                settingsData.fl_MineDistanceLimit = GUILayout.HorizontalSlider(settingsData.fl_MineDistanceLimit, 50, 500, Array.Empty<GUILayoutOption>());

                UI.Checkbox(ref settingsData.b_TurretDistanceLimit, LocalizationManager.GetString("turret_distance_limit") + " (" + Mathf.RoundToInt(settingsData.fl_TurretDistanceLimit) + ")", LocalizationManager.GetString("turret_distance_limit_descr"));
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
                    UI.Header(LocalizationManager.GetString("selected_player") + ": " + selectedPlayer.playerUsername);
                    Settings.Instance.InitializeDictionaries(selectedPlayer);

                    // We keep toggles outside of the isPlayerDead check so that users can toggle them on/off no matter their condition.

                    bool b_DemiGod = Settings.Instance.b_DemiGod[selectedPlayer];
                    UI.Checkbox(ref b_DemiGod, LocalizationManager.GetString("demigod"), LocalizationManager.GetString("demigod_descr"));
                    Settings.Instance.b_DemiGod[selectedPlayer] = b_DemiGod;

                    bool b_SpamObjects = Settings.Instance.b_SpamObjects[selectedPlayer];
                    UI.Checkbox(ref b_SpamObjects, LocalizationManager.GetString("object_spam"), LocalizationManager.GetString("object_spam_descr"));
                    Settings.Instance.b_SpamObjects[selectedPlayer] = b_SpamObjects;

                    UI.Checkbox(ref Settings.Instance.b_HideObjects, LocalizationManager.GetString("hide_objects"), LocalizationManager.GetString("hide_objects_descr"));

                    if (!selectedPlayer.isPlayerDead)
                    {
                        UI.Button(LocalizationManager.GetString("spawn_enemy"), LocalizationManager.GetString("spawn_enemy_descr"), () => { RoundManager.Instance.SpawnEnemyOnServer(selectedPlayer.gameplayCamera.transform.position, 50); });
                        UI.Button(LocalizationManager.GetString("kill"), LocalizationManager.GetString("kill_descr"), () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(selectedPlayer.health + 1, new Vector3(900, 900, 900), 0); });
                        UI.Button(LocalizationManager.GetString("teleport_to"), LocalizationManager.GetString("teleport_to_descr"), () => { Instance.localPlayer.TeleportPlayer(selectedPlayer.playerGlobalHead.position); });
                        UI.Button(LocalizationManager.GetString("teleport_enemies_to"), LocalizationManager.GetString("teleport_enemies_to_descr"), () =>
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
                        UI.Button(LocalizationManager.GetString("teleport_player_to_ship"), LocalizationManager.GetString("teleport_player_to_ship_descr"), () =>
                        {
                            Instance.shipTeleporter.TeleportPlayerOutServerRpc((int)selectedPlayer.playerClientId, Instance.shipRoom.transform.position);
                        });

                        UI.Button(LocalizationManager.GetString("aggro_enemies"), LocalizationManager.GetString("aggro_enemies_descr_1") + "\n" + LocalizationManager.GetString("aggro_enemies_descr_2"), () =>
                        {
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
                        UI.Button(LocalizationManager.GetString("damage"), LocalizationManager.GetString("damage_descr"), () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(int.Parse(Settings.Instance.str_DamageToGive), new Vector3(900, 900, 900), 0); });

                        Settings.Instance.str_HealthToHeal = GUILayout.TextField(Settings.Instance.str_HealthToHeal, Array.Empty<GUILayoutOption>());
                        UI.Button(LocalizationManager.GetString("heal"), LocalizationManager.GetString("heal_descr"), () => { selectedPlayer.DamagePlayerFromOtherClientServerRpc(-int.Parse(Settings.Instance.str_HealthToHeal), new Vector3(900, 900, 900), 0); });
                    }

                    Settings.Instance.str_ChatAsPlayer = GUILayout.TextField(Settings.Instance.str_ChatAsPlayer, Array.Empty<GUILayoutOption>());
                    UI.Button(LocalizationManager.GetString("send_message_player"), LocalizationManager.GetString("send_message_player_descr"), () =>
                    {
                        PAUtils.SendChatMessage(Settings.Instance.str_ChatAsPlayer, (int)selectedPlayer.playerClientId);
                    });

                    bool SpamChatCheck = Settings.Instance.b_SpamChat[selectedPlayer];
                    UI.Checkbox(ref SpamChatCheck, LocalizationManager.GetString("spam_message_player"), LocalizationManager.GetString("spam_message_player_descr"));
                    Settings.Instance.b_SpamChat[selectedPlayer] = SpamChatCheck;

                    UI.Button(LocalizationManager.GetString("steam_profile"), LocalizationManager.GetString("steam_profile_descr"), () => { SteamFriends.OpenUserOverlay(selectedPlayer.playerSteamId, "steamid"); });
                }
            });

            if (StartOfRound.Instance && Instance.shipTerminal)
            {
                UI.TabContents(LocalizationManager.GetString("upgrades"), UI.Tabs.Upgrades, () =>
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
                        GUILayout.Label(LocalizationManager.GetString("u_alrd_unlc_all"));
                    }
                    else
                    {
                        UI.Button(LocalizationManager.GetString("unlc_all_upgrd_ship"), LocalizationManager.GetString("unlc_all_upgrd_ship_descr"), () =>
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
                            UI.Button(LocalizationManager.GetString("unlcs_all_suits"), LocalizationManager.GetString("unlcs_all_suits_descr"), () =>
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

                                UI.Button(LocalizationManager.TryGetString("object_", unlockableName), $"{LocalizationManager.GetString("unlock")} {LocalizationManager.TryGetString("object_", unlockableName)}", () =>
                                {
                                    StartOfRound.Instance.BuyShipUnlockableServerRpc(i, Instance.shipTerminal.groupCredits);
                                    StartOfRound.Instance.SyncShipUnlockablesServerRpc();
                                });
                            }
                        }
                    }
                });
            }

            UI.TabContents(LocalizationManager.GetString("graphics"), UI.Tabs.Graphics, () =>
            {
                UI.Checkbox(ref settingsData.b_DisableFog, LocalizationManager.GetString("disable_fog"), LocalizationManager.GetString("disable_fog_descr"));
                UI.Checkbox(ref settingsData.b_DisableDepthOfField, LocalizationManager.GetString("disable_dof"), LocalizationManager.GetString("disable_dof_descr"));
                if (UI.Checkbox(ref settingsData.b_RemoveVisor, LocalizationManager.GetString("disable_visor"), LocalizationManager.GetString("disable_visor_descr")))
                {
                    if (!settingsData.b_RemoveVisor && !Features.Thirdperson.ThirdpersonCamera.ViewState)
                        Instance.localVisor?.SetActive(true);
                }
                UI.Checkbox(ref settingsData.b_CameraResolution, LocalizationManager.GetString("render_resolution"), LocalizationManager.GetString("camera_res_descr_1") + "\n " + LocalizationManager.GetString("camera_res_descr_1"));
                GUILayout.Label($"{LocalizationManager.GetString("fov")} ({settingsData.i_FieldofView})");
                settingsData.i_FieldofView = Mathf.RoundToInt(GUILayout.HorizontalSlider(settingsData.i_FieldofView, 50, 110, Array.Empty<GUILayoutOption>()));

                GUILayout.BeginHorizontal();
                GUILayout.Label(LocalizationManager.GetString("thirdperson"));
                UI.Keybind(ref settingsData.keyThirdperson);
                GUILayout.EndHorizontal();

                GUILayout.Label($"{LocalizationManager.GetString("distance")} ({settingsData.fl_ThirdpersonDistance})");
                settingsData.fl_ThirdpersonDistance = GUILayout.HorizontalSlider(settingsData.fl_ThirdpersonDistance, 1, 4);
            });

            UI.TabContents(LocalizationManager.GetString("settings"), UI.Tabs.Settings, () =>
            {
                UI.Checkbox(ref settingsData.b_Crosshair, LocalizationManager.GetString("crosshair"), LocalizationManager.GetString("crosshair_descr"));
                UI.Checkbox(ref settingsData.b_DisplayGroupCredits, LocalizationManager.GetString("display_group_credits"), LocalizationManager.GetString("display_group_credits_descr"));
                UI.Checkbox(ref settingsData.b_DisplayLootInShip, LocalizationManager.GetString("display_loot_in_ship"), LocalizationManager.GetString("display_loot_in_ship_descr"));
                UI.Checkbox(ref settingsData.b_DisplayQuota, LocalizationManager.GetString("display_quota"), LocalizationManager.GetString("display_quota_descr"));
                UI.Checkbox(ref settingsData.b_DisplayDaysLeft, LocalizationManager.GetString("display_days_left"), LocalizationManager.GetString("display_days_left_descr"));
                UI.Checkbox(ref settingsData.b_CenteredIndicators, LocalizationManager.GetString("centered_indicators"), LocalizationManager.GetString("centered_indicators_descr"));
                UI.Checkbox(ref settingsData.b_DeadPlayers, LocalizationManager.GetString("dead_players"), LocalizationManager.GetString("dead_players_descr"));
                UI.Checkbox(ref settingsData.b_Tooltips, LocalizationManager.GetString("tooltips"), LocalizationManager.GetString("tooltips_descr"));

                UI.Header(LocalizationManager.GetString("colors"));
                UI.ColorPicker(LocalizationManager.GetString("theme"), ref settingsData.c_Theme);
                UI.ColorPicker(LocalizationManager.GetString("valve"), ref settingsData.c_Valve);
                UI.ColorPicker(LocalizationManager.GetString("enemy"), ref settingsData.c_Enemy);
                UI.ColorPicker(LocalizationManager.GetString("turret"), ref settingsData.c_Turret);
                UI.ColorPicker(LocalizationManager.GetString("landmine"), ref settingsData.c_Landmine);
                UI.ColorPicker(LocalizationManager.GetString("player"), ref settingsData.c_Player);
                UI.ColorPicker(LocalizationManager.GetString("door"), ref settingsData.c_Door);
                UI.ColorPicker(LocalizationManager.GetString("loot"), ref settingsData.c_Loot);
                UI.ColorPicker(LocalizationManager.GetString("small_loot"), ref settingsData.c_smallLoot);
                UI.ColorPicker(LocalizationManager.GetString("medium_loot"), ref settingsData.c_medLoot);
                UI.ColorPicker(LocalizationManager.GetString("big_loot"), ref settingsData.c_bigLoot);

                GUILayout.Space(20);
                GUILayout.Label(LocalizationManager.GetString("select_language") + ":");
                List<string> languageNames = new List<string>();
                foreach (var item in LocalizationManager.Languages.Keys)
                {
                    languageNames.Add(LocalizationManager.GetString(item + "_descr"));
                }
                selectedLanguageIndex = GUILayout.Toolbar(selectedLanguageIndex, languageNames.ToArray(), GUILayout.ExpandWidth(true));
                GUILayout.Space(10);
                if (GUILayout.Button(LocalizationManager.GetString("apply")))
                {
                    string selectedLanguage = LocalizationManager.Languages.Keys.ToArray()[selectedLanguageIndex];
                    settingsData.str_Language = selectedLanguage;
                    LocalizationManager.SetLanguage(selectedLanguage);
                    Settings.Instance.SaveSettings();
                    Debug.Log("Selected Language: " + selectedLanguage);
                }
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
                            ObjName += " [" + distanceToPlayer.ToString().ToUpper() + LocalizationManager.GetString("M") + "]";
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
                _ => LocalizationManager.GetString("ship"),
                _ => settingsData.c_Door
            );
        }

        private void DisplayDoors()
        {
            DisplayObjects(
                Instance.entranceTeleports,
                settingsData.b_DoorESP,
                entranceTeleport => entranceTeleport.isEntranceToBuilding ? LocalizationManager.GetString("entrance") : LocalizationManager.GetString("exit"),
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
                _ => LocalizationManager.GetString("landmine"),
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
                _ => LocalizationManager.GetString("turret"),
                _ => settingsData.c_Turret
            );
        }

        private void DisplaySteamHazard()
        {
            DisplayObjects(
                Instance.steamValves.Where(steamValveHazard => steamValveHazard != null && steamValveHazard.triggerScript.interactable),
                settingsData.b_SteamHazard,
                _ => LocalizationManager.GetString("steam_valve"),
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
                        str += " [" + LocalizationManager.GetString("VC") + "]";
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
                    return string.IsNullOrWhiteSpace(name) ? LocalizationManager.GetString("enemy") : LocalizationManager.TryGetString("enemy_", name);
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
                    string text = LocalizationManager.GetString("object");
                    Item itemProperties = grabbableObject.itemProperties;
                    if (itemProperties.itemName != null)
                        text = LocalizationManager.TryGetString("object_", itemProperties.itemName);
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
