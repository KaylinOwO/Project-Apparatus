#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemActivate))]
class InfiniteShotgunAmmoPatch {
    static void Prefix(ref int ___shellsLoaded, ref EnemyAI ___heldByEnemy) {
        if (___heldByEnemy is not null) return;
        ___shellsLoaded = 3;
    }
}
