#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Steamworks;
using Hax;
using System.Collections.Generic;
using GameNetcodeStuff;
using System;
using UnityEngine;

namespace ProjectApparatus;



//[HarmonyPatch(typeof(PlayerControllerB), "SendNewPlayerValuesServerRpc")]
//class AntiKickPatch : MonoBehaviour
//{
//    static bool Prefix(PlayerControllerB __instance)
//    {
//        if (!Setting.EnableAntiKick) return true;

//        //ulong[] playerSteamIds = new ulong[__instance.playersManager.allPlayerScripts.Length];
//        ulong[] actualPlayerSteamIds = null;

//        foreach (PlayerControllerB player in FindObjectsOfType<PlayerControllerB>())
//        {
//            actualPlayerSteamIds.AddItem(player.playerSteamId);
//        }

//        //for (int i = 0; i < __instance.playersManager.allPlayerScripts.Length; i++) {
//        //    playerSteamIds[i] = __instance.playersManager.allPlayerScripts[i].playerSteamId;
//        //}

//        actualPlayerSteamIds[__instance.playerClientId] = SteamClient.SteamId;

//        _ = __instance.Reflect().InvokeInternalMethod("SendNewPlayerValuesClientRpc", actualPlayerSteamIds);

//        return false;
//    }
//}