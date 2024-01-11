#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
class InvisiblePatch {
    static Vector3 LastNewPos { get; set; }
    static bool LastInElevator { get; set; }
    static bool LastExhausted { get; set; }
    static bool LastIsPlayerGrounded { get; set; }

    [HarmonyPrefix]
    [HarmonyPatch("UpdatePlayerPositionServerRpc")]
    static void UpdatePlayerPositionServerRpcPrefix(
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool exhausted,
        ref bool isPlayerGrounded
    ) {
        if (!Setting.EnableInvisible) return;

        InvisiblePatch.LastNewPos = newPos;
        InvisiblePatch.LastInElevator = inElevator;
        InvisiblePatch.LastExhausted = exhausted;
        InvisiblePatch.LastIsPlayerGrounded = isPlayerGrounded;

        newPos = new Vector3(0, -100, 0);
        inElevator = false;
        exhausted = false;
        isPlayerGrounded = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch("UpdatePlayerPositionClientRpc")]
    static void UpdatePlayerPositionClientRpcPrefix(
        PlayerControllerB __instance,
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool exhausted,
        ref bool isPlayerGrounded
    ) {
        if (!Setting.EnableInvisible) return;
        if (Helper.LocalPlayer?.actualClientId != __instance.actualClientId) return;

        newPos = InvisiblePatch.LastNewPos;
        inElevator = InvisiblePatch.LastInElevator;
        exhausted = InvisiblePatch.LastExhausted;
        isPlayerGrounded = InvisiblePatch.LastIsPlayerGrounded;
    }
}
