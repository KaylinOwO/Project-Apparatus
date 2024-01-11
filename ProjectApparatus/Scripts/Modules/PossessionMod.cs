using UnityEngine;
using System.Collections;
using GameNetcodeStuff;
using UnityEngine.AI;

namespace Hax;

//must be enabled and disabled by phantommod. Or else phantom mod can break, and this break, what nots.
public sealed class PossessionMod : MonoBehaviour {
    EnemyAI? EnemyToPossess { get; set; } = null;
    bool FirstUpdate { get; set; } = true;
    RigidbodyMovement? RBKeyboard { get; set; } = null;
    KeyboardMovement? Keyboard { get; set; } = null;
    MousePan? MousePan { get; set; } = null;

    public static PossessionMod? Instance { get; private set; }

    public bool IsPossessed => this.EnemyToPossess != null;
    bool noClip = false;

    void Awake() {
        this.RBKeyboard = this.gameObject.AddComponent<RigidbodyMovement>();
        this.Keyboard = this.gameObject.AddComponent<KeyboardMovement>();
        this.MousePan = this.gameObject.AddComponent<MousePan>();
        this.enabled = false;

        PossessionMod.Instance = this;
    }

    void OnEnable() {
        InputListener.onNPress += this.ToggleNoClip;
        InputListener.onXPress += this.ToggleRealisticPossession;
        InputListener.onZPress += this.Unpossess;

        this.UpdateComponentsOnCurrentState(true);
    }

    void OnDisable() {
        InputListener.onNPress -= this.ToggleNoClip;
        InputListener.onXPress -= this.ToggleRealisticPossession;
        InputListener.onZPress -= this.Unpossess;

        this.UpdateComponentsOnCurrentState(false);
    }

    private void ToggleRealisticPossession() {
        Setting.RealisticPossessionEnabled = !Setting.RealisticPossessionEnabled;
        Chat.Print($"Realistic Possession: {Setting.RealisticPossessionEnabled}");

        if (!this.EnemyToPossess.IsNotNull(out EnemyAI enemy)) {
            return;
        }

        if (!enemy.agent.IsNotNull(out NavMeshAgent navMeshAgent)) {
            return;
        }

        navMeshAgent.updatePosition = Setting.RealisticPossessionEnabled;
        navMeshAgent.updateRotation = Setting.RealisticPossessionEnabled;
    }

    private void ToggleNoClip() {
        this.noClip = !this.noClip;
        Chat.Print($"Possess NoClip: {this.noClip}");

        this.UpdateComponentsOnCurrentState(this.enabled);
    }

    private void UpdateComponentsOnCurrentState(bool thisGameObjectIsEnabled) {
        if (!this.MousePan.IsNotNull(out MousePan mousePan)) {
            return;
        }

        if (!this.RBKeyboard.IsNotNull(out RigidbodyMovement rbKeyboard)) {
            return;
        }

        if (!this.Keyboard.IsNotNull(out KeyboardMovement keyboard)) {
            return;
        }

        mousePan.enabled = thisGameObjectIsEnabled;
        rbKeyboard.enabled = !this.noClip;
        keyboard.enabled = this.noClip;
    }

    void Update() {
        _ = this.StartCoroutine(this.EndOfFrameCoroutine());
    }

    IEnumerator EndOfFrameCoroutine() {
        yield return new WaitForEndOfFrame();
        this.EndOfFrameUpdate();
    }

    private void EndOfFrameUpdate() {
        if (!this.RBKeyboard.IsNotNull(out RigidbodyMovement rbKeyboard) ||
            !this.EnemyToPossess.IsNotNull(out EnemyAI enemy) ||
            !Helper.CurrentCamera.IsNotNull(out Camera camera) ||
            !camera.enabled
        ) {
            return;
        }


        if (this.FirstUpdate) {
            this.SetEnemyColliders(enemy, false);

            //only works if you enable it before FirstUpdate happens
            if (enemy.agent.IsNotNull(out NavMeshAgent agent)) {
                agent.updatePosition = Setting.RealisticPossessionEnabled;
                agent.updateRotation = Setting.RealisticPossessionEnabled;
            }

            rbKeyboard.Init();
            this.transform.position = enemy.transform.position;
            this.UpdateComponentsOnCurrentState(true);
        }

        this.UpdateEnemyPositionToHere(enemy);
        camera.transform.position = this.transform.position + (Vector3.up * 2.5f) - (enemy.transform.forward * 2f);
        camera.transform.rotation = this.transform.rotation;

        this.FirstUpdate = false;
    }

    void UpdateEnemyPositionToHere(EnemyAI enemy) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) return;

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        enemy.updatePositionThreshold = 0;
        Vector3 enemyEuler = enemy.transform.eulerAngles;
        enemyEuler.y = this.transform.eulerAngles.y;
        enemy.transform.eulerAngles = enemyEuler;
        enemy.transform.position = this.transform.position;
    }

    void SetEnemyColliders(EnemyAI enemy, bool enabled) {
        if (!enemy.GetComponentsInChildren<Collider>().IsNotNull(out Collider[] enemyColliders)) return;
        enemyColliders.ForEach(c => c.enabled = enabled);
    }

    public void Possess(EnemyAI enemy) {
        this.Unpossess();

        this.EnemyToPossess = enemy;
        this.FirstUpdate = true;
    }

    public void Unpossess() {
        if (this.EnemyToPossess.IsNotNull(out EnemyAI previousEnemy)) {
            previousEnemy.updatePositionThreshold = 1;

            if (previousEnemy.agent.IsNotNull(out NavMeshAgent navMeshAgent)) {
                navMeshAgent.updatePosition = true;
                navMeshAgent.updateRotation = true;
                this.UpdateEnemyPositionToHere(previousEnemy);
                _ = previousEnemy.agent.Warp(previousEnemy.transform.position);
            }

            this.SetEnemyColliders(previousEnemy, true);
        }

        this.EnemyToPossess = null;
    }
}
