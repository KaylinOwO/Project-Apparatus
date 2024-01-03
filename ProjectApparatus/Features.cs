using Dissonance;
using GameNetcodeStuff;
using HarmonyLib;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static GameObjectManager;
using static ProjectApparatus.Features.Thirdperson;

namespace ProjectApparatus
{
    public static class Features
    {  
        public class Thirdperson : MonoBehaviour // credits: https://thunderstore.io/c/lethal-company/p/Verity/3rdPerson/
        {
            [HarmonyPatch(typeof(QuickMenuManager), "OpenQuickMenu")]
            public class QuickMenuManager_OpenQuickMenu_Patch
            {
                public static void Prefix()
                {
                    _previousState = ThirdpersonCamera.ViewState;
                    if (ThirdpersonCamera.ViewState)
                    {
                        Instance.localPlayer.quickMenuManager.isMenuOpen = false;
                        ThirdpersonCamera.Toggle();
                    }
                }
            }

            [HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenu")]
            public class QuickMenuManager_CloseQuickMenu_Patch
            {
                public static void Prefix()
                {
                    if (_previousState)
                    {
                        Instance.localPlayer.inTerminalMenu = false;
                        ThirdpersonCamera.Toggle();
                    }
                }
            }

            [HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
            public class Terminal_BeginUsingTerminal_Patch
            {
                public static void Prefix()
                {
                    _previousState = ThirdpersonCamera.ViewState;
                    if (ThirdpersonCamera.ViewState)
                    {
                        Instance.localPlayer.quickMenuManager.isMenuOpen = false;
                        ThirdpersonCamera.Toggle();
                    }
                }
            }

            [HarmonyPatch(typeof(Terminal), "QuitTerminal")]
            public class Terminal_QuitTerminal_Patch
            {
                public static void Prefix()
                {
                    if (_previousState)
                    {
                        Instance.localPlayer.inTerminalMenu = false;
                        ThirdpersonCamera.Toggle();
                    }
                }
            }

            [HarmonyPatch(typeof(PlayerControllerB), "KillPlayer")]
            public class PlayerControllerB_KillPlayer_Patch
            {
                public static void Prefix()
                {
                    if (ThirdpersonCamera.ViewState)
                        ThirdpersonCamera.Toggle();
                }
            }

            public void Start()
            {
                ThirdpersonCamera ThirdpersonCamera = base.gameObject.AddComponent<ThirdpersonCamera>();
                ThirdpersonCamera.hideFlags = HideFlags.HideAndDontSave;
                UnityEngine.Object.DontDestroyOnLoad(ThirdpersonCamera);
            }

            public class ThirdpersonCamera : MonoBehaviour
            {
                private void Awake()
                {
                    ThirdpersonCamera._camera = base.gameObject.AddComponent<Camera>();
                    ThirdpersonCamera._camera.hideFlags = HideFlags.HideAndDontSave;
                    ThirdpersonCamera._camera.fieldOfView = 66f;
                    ThirdpersonCamera._camera.nearClipPlane = 0.1f;
                    ThirdpersonCamera._camera.cullingMask = 557520895;
                    ThirdpersonCamera._camera.enabled = false;
                    Object.DontDestroyOnLoad(ThirdpersonCamera._camera);
                }

                private void Update()
                {
                    if (Instance.localPlayer == null
                        || Instance.localPlayer.quickMenuManager.isMenuOpen || Instance.localPlayer.inTerminalMenu || Instance.localPlayer.isPlayerDead)
                    {
                        return;
                    }

                    ThirdpersonUpdate();
                    if ((PAUtils.GetAsyncKeyState(Settings.Instance.settingsData.keyThirdperson) & 1) != 0)
                        ThirdpersonCamera.Toggle();
                }

                private void ThirdpersonUpdate()
                {
                    Camera gameplayCamera = Instance.localPlayer.gameplayCamera;
                    Vector3 a = gameplayCamera.transform.forward * -1f;
                    Vector3 b = gameplayCamera.transform.TransformDirection(Vector3.right) * 0.6f;
                    Vector3 b2 = Vector3.up * 0.1f;
                    float value = Settings.Instance.settingsData.fl_ThirdpersonDistance;
                    ThirdpersonCamera._camera.transform.position = gameplayCamera.transform.position + a * value + b + b2;
                    ThirdpersonCamera._camera.transform.rotation = Quaternion.LookRotation(gameplayCamera.transform.forward);
                }

                public static void Toggle()
                {
                    if (Instance.localPlayer == null || Instance.localPlayer.isTypingChat || Instance.localPlayer.quickMenuManager.isMenuOpen || Instance.localPlayer.inTerminalMenu || Instance.localPlayer.isPlayerDead)
                    {
                        return;
                    }
                    ThirdpersonCamera.ViewState = !ThirdpersonCamera.ViewState;
                    GameObject gameObject = GameObject.Find("Systems/UI/Canvas/Panel/");
                    Canvas component = GameObject.Find("Systems/UI/Canvas/").GetComponent<Canvas>();
                    if (ThirdpersonCamera.ViewState)
                    {
                        Instance.localPlayer.thisPlayerModel.shadowCastingMode = ShadowCastingMode.On;
                        gameObject.SetActive(false);
                        component.worldCamera = ThirdpersonCamera._camera;
                        component.renderMode = 0;
                        Instance.localVisor.SetActive(false);
                        Instance.localPlayer.thisPlayerModelArms.enabled = false;
                        Instance.localPlayer.gameplayCamera.enabled = false;
                        ThirdpersonCamera._camera.enabled = true;
                        return;
                    }
                    Instance.localPlayer.thisPlayerModel.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    gameObject.SetActive(true);
                    component.worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
                    component.renderMode = (RenderMode)1;
                    Instance.localVisor.SetActive(!Settings.Instance.settingsData.b_RemoveVisor);
                    Instance.localPlayer.thisPlayerModelArms.enabled = (Possession.possessedEnemy == null);
                    Instance.localPlayer.gameplayCamera.enabled = true;
                    ThirdpersonCamera._camera.enabled = false;
                }

                public static Camera _camera;
                public static bool ViewState;
            }

            private static bool _previousState;
        }

        public static class Possession
        {
            public static bool beginPossession = false;
            public static EnemyAI lastpossessedEnemy = null, possessedEnemy = null;
            public static void StartPossession()
            {
                PlayerControllerB localPlayer = GameObjectManager.Instance.localPlayer;
                if (!localPlayer
                    || localPlayer.isPlayerDead) return;

                float closestDistance = float.MaxValue;
                EnemyAI nearestEnemy = null;

                foreach (EnemyAI enemy in Instance.enemies)
                {
                    if (enemy == lastpossessedEnemy
                        || enemy.isEnemyDead) continue;

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
                PlayerControllerB localPlayer = GameObjectManager.Instance.localPlayer;

                if (localPlayer && !localPlayer.isPlayerDead)
                {
                    localPlayer.DisablePlayerModel(localPlayer.playersManager.allPlayerObjects[localPlayer.playerClientId], true, false);
                    localPlayer.thisPlayerModelArms.enabled = true;
                }

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
                    PlayerControllerB localPlayer = GameObjectManager.Instance.localPlayer;
                    if (!localPlayer
                        || localPlayer.isPlayerDead)
                    {
                        StopPossession();
                        return;
                    }

                    if (possessedEnemy.isEnemyDead)
                    {
                        StopPossession();
                        return;
                    }

                    if (beginPossession)
                    {
                        localPlayer.DisablePlayerModel(localPlayer.playersManager.allPlayerObjects[localPlayer.playerClientId], false, true); // This is client-side
                        Instance.localPlayer.TeleportPlayer(possessedEnemy.transform.position);
                        beginPossession = false;
                    }

                    foreach (Collider col in possessedEnemy.GetComponentsInChildren<Collider>())
                        col.enabled = false;

                    possessedEnemy.ChangeEnemyOwnerServerRpc(Instance.localPlayer.actualClientId);
                    possessedEnemy.updatePositionThreshold = 0;
                    possessedEnemy.moveTowardsDestination = false;

                    possessedEnemy.transform.eulerAngles = Instance.localPlayer.transform.eulerAngles;
                    possessedEnemy.transform.position = Instance.localPlayer.transform.position;

                    lastpossessedEnemy = possessedEnemy;
                }
            }
        }

        public static class Misc
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
                    && (Settings.Instance.settingsData.keyNoclip == 0
                        || PAUtils.GetAsyncKeyState(Settings.Instance.settingsData.keyNoclip) != 0));

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
        }
    }
}
