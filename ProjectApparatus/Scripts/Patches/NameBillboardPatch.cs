#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
class NameBillboardPatch {
    static void Postfix(ref PlayerControllerB __instance) => __instance.ShowNameBillboard();
}
