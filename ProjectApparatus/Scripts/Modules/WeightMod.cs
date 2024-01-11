using System.Collections;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public sealed class WeightMod : MonoBehaviour {
    IEnumerator SetWeight(object[] args) {
        while (true) {
            if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) {
                yield return new WaitForEndOfFrame();
                continue;
            }

            player.carryWeight = 1.0f;
            yield return new WaitForSeconds(1.0f);
        }
    }

    void Start() {
        _ = this.StartResilientCoroutine(this.SetWeight);
    }
}
