using System;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace ProjectApparatus
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public class PlayerControllerB_Update_Patch
    {
        public static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance == GameObjectManager.Instance.localPlayer)
            {
                __instance.disableLookInput = (__instance.quickMenuManager.isMenuOpen || Settings.Instance.b_isMenuOpen) ? true : false;
                Cursor.visible = (__instance.quickMenuManager.isMenuOpen || Settings.Instance.b_isMenuOpen) ? true : false;
                Cursor.lockState = (__instance.quickMenuManager.isMenuOpen || Settings.Instance.b_isMenuOpen) ? CursorLockMode.None : CursorLockMode.Locked;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public class PlayerControllerB_LateUpdate_Patch
    {
        public static void Postfix(PlayerControllerB __instance)
        {
            if (Settings.Instance.b_DemiGod.ContainsKey(__instance) && Settings.Instance.b_DemiGod[__instance] && __instance.health < 100)
                __instance.DamagePlayerFromOtherClientServerRpc(-(100 - __instance.health), new Vector3(0, 0, 0), 0);

            if (!StartOfRound.Instance)
                return;

            if (Settings.Instance.settingsData.b_AllJetpacksExplode)
            {
                if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer.GetType() == typeof(JetpackItem))
                {
                    JetpackItem Jetpack = (__instance.currentlyHeldObjectServer as JetpackItem);// fill it in
                    if (Jetpack != null)
                    {
                        PAUtils.SetValue(__instance, "jetpackPower", float.MaxValue, PAUtils.protectedFlags);
                        PAUtils.CallMethod(__instance, "ActivateJetpack", PAUtils.protectedFlags, null);
                        Jetpack.ExplodeJetpackServerRpc();
                        Jetpack.ExplodeJetpackClientRpc();
                    }
                }
            }

            PlayerControllerB Local = GameObjectManager.Instance.localPlayer;
            if (__instance.actualClientId != Local.actualClientId)
                return;

            if (Settings.Instance.settingsData.b_InfiniteStam)
            {
                __instance.sprintMeter = 1f;
                if (__instance.sprintMeterUI != null)
                    __instance.sprintMeterUI.fillAmount = 1f;
            }

            if (Settings.Instance.settingsData.b_InfiniteCharge)
            {
                if (__instance.currentlyHeldObjectServer != null
                    && __instance.currentlyHeldObjectServer.insertedBattery != null)
                {
                    __instance.currentlyHeldObjectServer.insertedBattery.empty = false;
                    __instance.currentlyHeldObjectServer.insertedBattery.charge = 1f;
                }
            }

            if (__instance.currentlyHeldObjectServer != null)
            {
                if (Settings.Instance.settingsData.b_ChargeAnyItem)
                    __instance.currentlyHeldObjectServer.itemProperties.requiresBattery = true;

                if (Settings.Instance.settingsData.b_OneHandAllObjects)
                {
                    __instance.twoHanded = false;
                    __instance.twoHandedAnimation = false;
                    __instance.currentlyHeldObjectServer.itemProperties.twoHanded = false;
                    __instance.currentlyHeldObjectServer.itemProperties.twoHandedAnimation = false;
                }
            }

            if (Settings.Instance.settingsData.b_WalkSpeed && !__instance.isSprinting)
                PAUtils.SetValue(__instance, "sprintMultiplier", Settings.Instance.settingsData.i_WalkSpeed, PAUtils.protectedFlags);

            if (Settings.Instance.settingsData.b_SprintSpeed && __instance.isSprinting)
                PAUtils.SetValue(__instance, "sprintMultiplier", Settings.Instance.settingsData.i_SprintSpeed, PAUtils.protectedFlags);

            __instance.climbSpeed = (Settings.Instance.settingsData.b_FastLadderClimbing) ? 100f : 4f;

            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            PAUtils.SetValue(__instance, "interactableObjectsMask",
                Settings.Instance.settingsData.b_InteractThroughWalls ? LayerMask.GetMask(new string[] { "Props", "InteractableObject" }) : 832,
                bindingAttr);

            __instance.grabDistance = Settings.Instance.settingsData.b_UnlimitedGrabDistance ? 9999f : 5f;

            if (__instance.nightVision)
            {
                /* I see a lot of cheats set nightVision.enabled to false when the feature is off, this is wrong as the game sets it to true when you're in-doors. 
                   Also there's no reason to reset it as the game automatically sets it back every time Update is called. */

                if (Settings.Instance.settingsData.b_NightVision)
                    __instance.nightVision.enabled = true;

                __instance.nightVision.range = (Settings.Instance.settingsData.b_NightVision) ? 9999f : 12f;
                __instance.nightVision.intensity = (Settings.Instance.settingsData.b_NightVision) ? 9999f : 366.9317f;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump")]
    public class PlayerControllerB_PlayerJump_Patch
    {
        public static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance.actualClientId == GameObjectManager.Instance.localPlayer.actualClientId
                && Settings.Instance.settingsData.b_JumpHeight
                && __instance.fallValue == __instance.jumpForce)
            {
                __instance.fallValue = Settings.Instance.settingsData.i_JumpHeight;
                __instance.fallValueUncapped = Settings.Instance.settingsData.i_JumpHeight;
            }   

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects")]
    public class PlayerControllerB_PlayerHitGroundEffects_Patch
    {
        public static bool Prefix(PlayerControllerB __instance)
        {
            if (__instance.actualClientId == GameObjectManager.Instance.localPlayer.actualClientId
                && Settings.Instance.settingsData.b_DisableFallDamage)
                __instance.takingFallDamage = false;

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath")]
    public class PlayerControllerB_AllowPlayerDeath_Patch
    {
        public static bool Prefix(PlayerControllerB __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_GodMode && __instance == GameObjectManager.Instance.localPlayer)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForEmote")]
    public class PlayerControllerB_CheckConditionsForEmote_Patch
    {
        public static bool Prefix(PlayerControllerB __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_TauntSlide)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Landmine), "Update")]
    public class Landmine_Update_Patch
    {
        public static bool Prefix(Landmine __instance)
        {
            if (Settings.Instance.settingsData.b_SensitiveLandmines && !__instance.hasExploded)
            {
                foreach (PlayerControllerB plyr in GameObjectManager.Instance.players)
                {
                    Vector3 plyrPosition = plyr.transform.position,
                        minePosition = __instance.transform.position;

                    float distance = Vector3.Distance(plyrPosition, minePosition);
                    if (distance <= 4f)
                        __instance.ExplodeMineServerRpc();
                }
            }

            if (Settings.Instance.settingsData.b_LandmineEarrape)
                __instance.ExplodeMineServerRpc();

            return true;
        }
    }

    [HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
    public class ShipBuildModeManager_Update_Patch
    {
        public static void Postfix(ShipBuildModeManager __instance)
        {
            if (Settings.Instance.settingsData.b_PlaceAnywhere)
            {
                PlaceableShipObject placingObject = (PlaceableShipObject)PAUtils.GetValue(__instance, "placingObject", PAUtils.protectedFlags);
                placingObject.AllowPlacementOnCounters = true;
                placingObject.AllowPlacementOnWalls = true;
                PAUtils.SetValue(__instance, "CanConfirmPosition", true, PAUtils.protectedFlags);
            }
        }
    }

    [HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
    public class ShipBuildModeManager_PlayerMeetsConditionsToBuild_Patch
    {
        public static bool Prefix(ShipBuildModeManager __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_PlaceAnywhere)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(GrabbableObject), "RequireCooldown")]
    public class GrabbableObject_RequireCooldown_Patch
    {
        public static bool Prefix(GrabbableObject __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableInteractCooldowns)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InteractTrigger), "Interact")]
    public class InteractTrigger_Interact_Patch
    {
        public static bool Prefix(InteractTrigger __instance)
        {
            __instance.interactCooldown = !Settings.Instance.settingsData.b_DisableInteractCooldowns;
            return true;
        }
    }

    [HarmonyPatch(typeof(PatcherTool), "LateUpdate")]
    public class PatcherTool_LateUpdate_Patch
    {
        public static void Postfix(PatcherTool __instance)
        {
            if (Settings.Instance.settingsData.b_InfiniteZapGun)
            {
                __instance.gunOverheat = 0f;
                __instance.bendMultiplier = 999f;
                __instance.pullStrength = 999f;
                PAUtils.SetValue(__instance, "timeSpentShocking", 0.01f, PAUtils.protectedFlags);
            }
        }
    }

    [HarmonyPatch(typeof(ShotgunItem), "ItemActivate")]
    public class ShotgunItem_ItemActivate_Patch
    {
        public static bool Prefix(ShotgunItem __instance)
        {
            if (Settings.Instance.settingsData.b_InfiniteShotgunAmmo)
            {
                __instance.isReloading = false;
                __instance.shellsLoaded++;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "UpdatePlayerVoiceEffects")]
    public class StartOfRound_UpdatePlayerVoiceEffects_Patch
    {
        public static void Postfix(StartOfRound __instance)
        {
            if (Settings.Instance.settingsData.b_HearEveryone
                && !StartOfRound.Instance.shipIsLeaving /* Without this you'll be stuck at "Wait for ship to land" - cba to find out way this happens */)
            {
                for (int i = 0; i < __instance.allPlayerScripts.Length; i++)
                {
                    PlayerControllerB playerControllerB = __instance.allPlayerScripts[i];
                    AudioSource currentVoiceChatAudioSource = playerControllerB.currentVoiceChatAudioSource;

                    currentVoiceChatAudioSource.GetComponent<AudioLowPassFilter>().enabled = false;
                    currentVoiceChatAudioSource.GetComponent<AudioHighPassFilter>().enabled = false;
                    currentVoiceChatAudioSource.panStereo = 0f;
                    SoundManager.Instance.playerVoicePitchTargets[(int)((IntPtr)playerControllerB.playerClientId)] = 1f;
                    SoundManager.Instance.SetPlayerPitch(1f, unchecked((int)playerControllerB.playerClientId));

                    currentVoiceChatAudioSource.spatialBlend = 0f;
                    playerControllerB.currentVoiceChatIngameSettings.set2D = true;
                    playerControllerB.voicePlayerState.Volume = 1f;
                }
            }
        }
    }


    [HarmonyPatch(typeof(HUDManager), "HoldInteractionFill")]
    public class HUDManager_HoldInteractionFill_Patch
    {
        public static bool Prefix(HUDManager __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_InstantInteractions)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SteamLobbyManager), "RefreshServerListButton")] // Removes the refresh cooldown
    public class SteamLobbyManager_RefreshServerListButton_Patch 
    {
        public static bool Prefix(SteamLobbyManager __instance)
        {
            PAUtils.SetValue(__instance, "refreshServerListTimer", 1f, PAUtils.protectedFlags); 
            return true;
        }
    }

    [HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter")] // Forces lobbies with blacklisted names to appear
    public class SteamLobbyManager_loadLobbyListAndFilter_Patch
    {
        public static bool Prefix(SteamLobbyManager __instance)
        {
            __instance.censorOffensiveLobbyNames = false;
            return true;
        }
    }

    /* Graphical */

    [HarmonyPatch(typeof(Fog), "IsFogEnabled")]
    public class Fog_IsFogEnabled_Patch
    {
        public static bool Prefix(Fog __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableFog)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Fog), "IsVolumetricFogEnabled")]
    public class Fog_IsVolumetricFogEnabled_Patch
    {
        public static bool Prefix(Fog __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableFog)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Fog), "IsPBRFogEnabled")]
    public class Fog_IsPBRFogEnabled_Patch
    {
        public static bool Prefix(Fog __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableFog)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Bloom), "IsActive")]
    public class Bloom_IsActive_Patch
    {
        public static bool Prefix(Bloom __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableBloom)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DepthOfField), "IsActive")]
    public class DepthOfField_IsActive_Patch
    {
        public static bool Prefix(DepthOfField __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableDepthOfField)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Vignette), "IsActive")]
    public class Vignette_IsActive_Patch
    {
        public static bool Prefix(Vignette __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableVignette)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FilmGrain), "IsActive")]
    public class FilmGrain_IsActive_Patch
    {
        public static bool Prefix(FilmGrain __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableFilmGrain)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch(typeof(Exposure), "IsActive")]
    public class Exposure_IsActive_Patch
    {
        public static bool Prefix(Exposure __instance, ref bool __result)
        {
            if (Settings.Instance.settingsData.b_DisableFilmGrain)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
