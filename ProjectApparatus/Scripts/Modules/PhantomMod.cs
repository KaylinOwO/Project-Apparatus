using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public sealed class PhantomMod : MonoBehaviour {
    bool IsShiftHeld { get; set; } = false;
    bool EnabledPossession { get; set; } = false;
    int CurrentSpectatorIndex { get; set; } = 0;

    void OnEnable() {
        InputListener.onShiftButtonHold += this.HoldShift;
        InputListener.onEqualsPress += this.TogglePhantom;
        InputListener.onRightArrowKeyPress += this.LookAtNextPlayer;
        InputListener.onLeftArrowKeyPress += this.LookAtPreviousPlayer;
    }

    void OnDisable() {
        InputListener.onShiftButtonHold -= this.HoldShift;
        InputListener.onEqualsPress -= this.TogglePhantom;
        InputListener.onRightArrowKeyPress -= this.LookAtNextPlayer;
        InputListener.onLeftArrowKeyPress -= this.LookAtPreviousPlayer;
    }

    void Update() {
        if (!PossessionMod.Instance.IsNotNull(out PossessionMod possessionMod)) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera) || Helper.Try(() => !camera.enabled)) return;
        if (!camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) return;
        if (!camera.gameObject.TryGetComponent(out MousePan mouse)) return;

        if (Setting.EnablePhantom) {
            //if was enabled possession before, but no longer possesing
            if (this.EnabledPossession && !possessionMod.IsPossessed) {
                this.EnabledPossession = false;
                possessionMod.enabled = false;
                keyboard.enabled = true;
                mouse.enabled = true;
            }

            if (!possessionMod.IsPossessed) {
                return;
            }

            // possessing monster in the first frame
            if (!this.EnabledPossession) {
                this.EnabledPossession = true;
                possessionMod.enabled = true;

                //turn off phantom's keyboard and mouse
                keyboard.enabled = false;
                mouse.enabled = false;
            }
        }

        else {
            if (this.EnabledPossession) {
                possessionMod.Unpossess();
                this.EnabledPossession = false;
                possessionMod.enabled = false;
            }
        }
    }

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void LookAtNextPlayer() => this.LookAtPlayer(1);

    void LookAtPreviousPlayer() => this.LookAtPlayer(-1);

    void LookAtPlayer(int indexChange) {
        if (!Setting.EnablePhantom || !Helper.CurrentCamera.IsNotNull(out Camera camera)) return;

        int playerCount = Helper.Players?.Length ?? 0;
        this.CurrentSpectatorIndex = (this.CurrentSpectatorIndex + indexChange) % playerCount;

        if (!Helper.GetActivePlayer(this.CurrentSpectatorIndex).IsNotNull(out PlayerControllerB targetPlayer)) {
            this.LookAtNextPlayer();
            return;
        }

        camera.transform.position = targetPlayer.playerEye.position;
    }

    void PhantomEnabled(Camera camera) {
        if (!camera.TryGetComponent(out KeyboardMovement keyboard)) {
            keyboard = camera.gameObject.AddComponent<KeyboardMovement>();
        }

        if (!camera.TryGetComponent(out MousePan mouse)) {
            mouse = camera.gameObject.AddComponent<MousePan>();
        }

        keyboard.enabled = true;
        mouse.enabled = true;
        camera.transform.SetParent(null, true);
    }

    void PhantomDisabled(PlayerControllerB player, Camera camera) {
        if (!player.cameraContainerTransform.IsNotNull(out Transform cameraParent)) return;
        if (this.IsShiftHeld) player.TeleportPlayer(camera.transform.position);

        camera.transform.SetParent(cameraParent, false);
        camera.transform.localPosition = Vector3.zero;
        camera.transform.localRotation = Quaternion.identity;

        if (camera.gameObject.TryGetComponent(out KeyboardMovement keyboard)) {
            keyboard.enabled = false;
        }

        if (camera.gameObject.TryGetComponent(out MousePan mouse)) {
            mouse.enabled = false;
        }

        if (!player.gameplayCamera.IsNotNull(out Camera gameplayCamera)) {
            return;
        }

        if (gameplayCamera.TryGetComponent(out KeyboardMovement gameplayKeyboard)) {
            gameplayKeyboard.enabled = false;
        }

        if (gameplayCamera.TryGetComponent(out MousePan gameplayMouse)) {
            gameplayMouse.enabled = false;
        }
    }

    void TogglePhantom() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) return;
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera) || !camera.enabled) return;

        Setting.EnablePhantom = !Setting.EnablePhantom;

        //to handle spectate camera.
        if (player.isPlayerDead) {
            player.enabled = !Setting.EnablePhantom;
        }
        else {
            if (!player.enabled) {
                player.enabled = true;
            }
        }

        player.playerBodyAnimator.enabled = !Setting.EnablePhantom;
        player.thisController.enabled = !Setting.EnablePhantom;
        player.isFreeCamera = Setting.EnablePhantom;


        if (Setting.EnablePhantom) {
            this.PhantomEnabled(camera);
        }

        else {
            this.PhantomDisabled(player, camera);
        }
    }
}
