using GameNetcodeStuff;
using System.Windows.Forms;
using UnityEngine;
using static GameObjectManager;

namespace ProjectApparatus
{
    public static class Features
    {
        public static void Noclip()
        {
            PlayerControllerB localPlayer = Instance.localPlayer;
            if (!localPlayer) return;

            Collider localCollider = localPlayer.GetComponent<CharacterController>();
            if (!localCollider) return;

            Transform localTransform = localPlayer.transform;
            localCollider.enabled = !(localTransform
                && Settings.Instance.settingsData.b_Noclip
                && (Settings.Instance.settingsData.keyNoclip.inKey == 0 
                    || PAUtils.GetAsyncKeyState(Settings.Instance.settingsData.keyNoclip.inKey) != 0));

            if (!localCollider.enabled)
            {
                bool WKey = PAUtils.GetAsyncKeyState((int)Keys.W) != 0,
                    AKey = PAUtils.GetAsyncKeyState((int)Keys.A) != 0,
                    SKey = PAUtils.GetAsyncKeyState((int)Keys.S) != 0,
                    DKey = PAUtils.GetAsyncKeyState((int)Keys.D) != 0,
                    SpaceKey = PAUtils.GetAsyncKeyState((int)Keys.Space) != 0,
                    CtrlKey = PAUtils.GetAsyncKeyState((int)Keys.LControlKey) != 0;

                Vector3 inVec = new Vector3(0, 0, 0);

                if (WKey)
                    inVec += localTransform.forward;
                if (SKey)
                    inVec -= localTransform.forward;
                if (AKey)
                    inVec -= localTransform.right;
                if (DKey)
                    inVec += localTransform.right;
                if (SpaceKey)
                    inVec.y += localTransform.up.y;
                if (CtrlKey)
                    inVec.y -= localTransform.up.y;

                localPlayer.transform.position += inVec * (Settings.Instance.settingsData.fl_NoclipSpeed * Time.deltaTime);
            }
        }

        public static void RespawnLocalPlayer() // This is a modified version of StartOfRound.ReviveDeadPlayers
        {
            PlayerControllerB localPlayer = Instance.localPlayer;
            if (!localPlayer) return;

            StartOfRound.Instance.allPlayersDead = false;
            localPlayer.ResetPlayerBloodObjects(localPlayer.isPlayerDead);
            if (localPlayer.isPlayerDead || localPlayer.isPlayerControlled)
            {
                localPlayer.isClimbingLadder = false;
                localPlayer.ResetZAndXRotation();
                localPlayer.thisController.enabled = true;
                localPlayer.health = 100;
                localPlayer.disableLookInput = false;
                if (localPlayer.isPlayerDead)
                {
                    localPlayer.isPlayerDead = false;
                    localPlayer.isPlayerControlled = true;
                    localPlayer.isInElevator = true;
                    localPlayer.isInHangarShipRoom = true;
                    localPlayer.isInsideFactory = false;
                    localPlayer.wasInElevatorLastFrame = false;
                    StartOfRound.Instance.SetPlayerObjectExtrapolate(false);
                    localPlayer.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[0].position, false, 0f, false, true);
                    localPlayer.setPositionOfDeadPlayer = false;
                    localPlayer.DisablePlayerModel(StartOfRound.Instance.allPlayerObjects[localPlayer.playerClientId], true, true);
                    localPlayer.helmetLight.enabled = false;
                    localPlayer.Crouch(false);
                    localPlayer.criticallyInjured = false;
                    localPlayer.playerBodyAnimator?.SetBool("Limp", false);
                    localPlayer.bleedingHeavily = false;
                    localPlayer.activatingItem = false;
                    localPlayer.twoHanded = false;
                    localPlayer.inSpecialInteractAnimation = false;
                    localPlayer.disableSyncInAnimation = false;
                    localPlayer.inAnimationWithEnemy = null;
                    localPlayer.holdingWalkieTalkie = false;
                    localPlayer.speakingToWalkieTalkie = false;
                    localPlayer.isSinking = false;
                    localPlayer.isUnderwater = false;
                    localPlayer.sinkingValue = 0f;
                    localPlayer.statusEffectAudio.Stop();
                    localPlayer.DisableJetpackControlsLocally();
                    localPlayer.health = 100;
                    localPlayer.mapRadarDotAnimator.SetBool("dead", false);
                    if (localPlayer.IsOwner)
                    {
                        HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
                        localPlayer.hasBegunSpectating = false;
                        HUDManager.Instance.RemoveSpectateUI();
                        HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                        localPlayer.hinderedMultiplier = 1f;
                        localPlayer.isMovementHindered = 0;
                        localPlayer.sourcesCausingSinking = 0;
                        localPlayer.reverbPreset = StartOfRound.Instance.shipReverb;
                    }
                }
                SoundManager.Instance.earsRingingTimer = 0f;
                localPlayer.voiceMuffledByEnemy = false;
                SoundManager.Instance.playerVoicePitchTargets[localPlayer.playerClientId] = 1f;
                SoundManager.Instance.SetPlayerPitch(1f, (int)localPlayer.playerClientId);
                if (localPlayer.currentVoiceChatIngameSettings == null)
                {
                    StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
                }
                if (localPlayer.currentVoiceChatIngameSettings != null)
                {
                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        localPlayer.currentVoiceChatIngameSettings.InitializeComponents();

                    if (localPlayer.currentVoiceChatIngameSettings.voiceAudio == null)
                        return;

                    localPlayer.currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
                }
            }
            PlayerControllerB playerControllerB = GameNetworkManager.Instance.localPlayerController;
            playerControllerB.bleedingHeavily = false;
            playerControllerB.criticallyInjured = false;
            playerControllerB.playerBodyAnimator.SetBool("Limp", false);
            playerControllerB.health = 100;
            HUDManager.Instance.UpdateHealthUI(100, false);
            playerControllerB.spectatedPlayerScript = null;
            HUDManager.Instance.audioListenerLowPass.enabled = false;
            StartOfRound.Instance.SetSpectateCameraToGameOverMode(false, playerControllerB);
            RagdollGrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<RagdollGrabbableObject>();
            for (int j = 0; j < array.Length; j++)
            {
                if (!array[j].isHeld)
                {
                    if (StartOfRound.Instance.IsServer)
                    {
                        if (array[j].NetworkObject.IsSpawned)
                            array[j].NetworkObject.Despawn(true);
                        else
                            UnityEngine.Object.Destroy(array[j].gameObject);
                    }
                }
                else if (array[j].isHeld && array[j].playerHeldBy != null)
                {
                    array[j].playerHeldBy.DropAllHeldItems(true, false);
                }
            }
            DeadBodyInfo[] array2 = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
            for (int k = 0; k < array2.Length; k++)
            {
                UnityEngine.Object.Destroy(array2[k].gameObject);
            }
            StartOfRound.Instance.livingPlayers = StartOfRound.Instance.connectedPlayersAmount + 1;
            StartOfRound.Instance.allPlayersDead = false;
            StartOfRound.Instance.UpdatePlayerVoiceEffects();
            StartOfRound.Instance.shipAnimator.ResetTrigger("ShipLeave");
        }


        public static bool beginPossession = false;
        public static EnemyAI lastpossessedEnemy = null, possessedEnemy = null;
        public static void StartPossession()
        {
            float closestDistance = float.MaxValue;
            EnemyAI nearestEnemy = null;

            foreach (EnemyAI enemy in Instance.enemies)
            {
                if (enemy == lastpossessedEnemy) continue;

                float distanceToEnemy = PAUtils.GetDistance(Instance.localPlayer.transform.position,
                    enemy.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                StopPossession();
                possessedEnemy = nearestEnemy;
                beginPossession = true;
            }
        }

        public static void StopPossession()
        {
            if (lastpossessedEnemy != null)
            {
                foreach (Collider col in lastpossessedEnemy.GetComponentsInChildren<Collider>())
                    col.enabled = true;

                lastpossessedEnemy.ChangeEnemyOwnerServerRpc(0);
                lastpossessedEnemy.updatePositionThreshold = 1f;
                lastpossessedEnemy.moveTowardsDestination = true;
            }

            possessedEnemy = null;
            lastpossessedEnemy = null;
        }

        public static void UpdatePossession()
        {
            if (possessedEnemy)
            {
                if (beginPossession)
                {
                    instance.localPlayer.TeleportPlayer(possessedEnemy.transform.position);
                    beginPossession = false;
                }

                foreach (Collider col in possessedEnemy.GetComponentsInChildren<Collider>())
                    col.enabled = false;

                possessedEnemy.ChangeEnemyOwnerServerRpc(Instance.localPlayer.actualClientId);
                possessedEnemy.updatePositionThreshold = 0;
                possessedEnemy.moveTowardsDestination = false;

                possessedEnemy.transform.eulerAngles = instance.localPlayer.transform.eulerAngles;
                possessedEnemy.transform.position = instance.localPlayer.transform.position;

                lastpossessedEnemy = possessedEnemy;
            }
        }
    }
}
