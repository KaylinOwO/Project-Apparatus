#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
class OneHandedItemPatch {
    static void Postfix(ref bool ___twoHanded) => ___twoHanded = false;
}
