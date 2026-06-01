using GameNetcodeStuff;
using HarmonyLib;
using ProjectApparatus;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameObjectManager
{
    public static GameObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObjectManager();
            }
            return instance;
        }
    }

    public const float CollectionInterval = 3f;

    public List<GameObject> spawnedObjects = new List<GameObject>(); //incase we want to make a gui for managing later
    public List<GrabbableObject> items = new List<GrabbableObject>();
    public List<Landmine> landmines = new List<Landmine>();
    public List<Turret> turrets = new List<Turret>();
    public List<DoorLock> doorLocks = new List<DoorLock>();
    public List<EntranceTeleport> entranceTeleports = new List<EntranceTeleport>();
    public List<PlayerControllerB> players = new List<PlayerControllerB>();
    public List<EnemyAI> enemies = new List<EnemyAI>();
    public List<SteamValveHazard> steamValves = new List<SteamValveHazard>();
    public List<PlaceableShipObject> shipObjects = new List<PlaceableShipObject>();
    public List<TerminalAccessibleObject> bigDoors = new List<TerminalAccessibleObject>();

    public ulong ClientId_OG = new ulong(); //going here for now until we determine if it works

    public PlayerControllerB hostPlayer;
    public PlayerControllerB localPlayer;
    public ShipBuildModeManager shipBuildModeManager;
    public HangarShipDoor shipDoor;
    public StartMatchLever shipRoom;
    public ShipLights shipLights;
    public Terminal shipTerminal;
    public ShipTeleporter shipTeleporter;
    public DepositItemsDesk itemsDesk;
    public TVScript tvScript;
    public GameObject localVisor;
    public GrabbableObject currentlyHeldObject;

    public int shipValue = 0;

    private bool IsInShipBounds(Vector3 pos)
    {
        if (shipRoom == null) return false;
        Vector3 delta = pos - shipRoom.transform.position;
        return Math.Abs(delta.x) < 7f && Math.Abs(delta.y) < 4f && Math.Abs(delta.z) < 10f;
    }

    public IEnumerator CollectObjects()
    {
        while (true)
        {
            try
            {
                InitializeReferences();
                ClearLists();

                CollectObjectsOfType(items);
                CollectObjectsOfType(landmines);
                CollectObjectsOfType(turrets);
                CollectObjectsOfType(doorLocks);
                CollectObjectsOfType(entranceTeleports);
                CollectObjectsOfType(players, p => !p.name.StartsWith("Player #"));
                CollectObjectsOfType(enemies);
                CollectObjectsOfType(steamValves);
                CollectObjectsOfType(shipObjects);
                bigDoors = FindObjectsOfType<TerminalAccessibleObject>(obj => obj.isBigDoor);

                currentlyHeldObject = null;
                shipValue = 0;
                foreach (GrabbableObject item in Instance.items)
                {
                    bool inShip = item.isInShipRoom || (!item.heldByPlayerOnServer && IsInShipBounds(item.transform.position));

                    if (inShip && (!ProjectApparatus.Settings.Instance.settingsData.b_ScrapOnly || IsScrapItem(item)))
                        shipValue += item.scrapValue;

                    if (localPlayer != null && Instance.localPlayer.currentItemSlot < Instance.localPlayer.ItemSlots.Length)
                        currentlyHeldObject = localPlayer.ItemSlots[Instance.localPlayer.currentItemSlot];
                }

                foreach (PlayerControllerB player in Instance.players)
                {
                    if (player != null && player.IsHost)
                        hostPlayer = player;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[GameObjectManager] CollectObjects error: {ex.Message}");
            }

            yield return new WaitForSeconds(CollectionInterval);
        }
    }

    public void InitializeReferences()
    {
        localPlayer = GameNetworkManager.Instance?.localPlayerController;
        shipBuildModeManager = UnityEngine.Object.FindObjectOfType<ShipBuildModeManager>();
        shipLights = UnityEngine.Object.FindObjectOfType<ShipLights>();
        shipTerminal = UnityEngine.Object.FindObjectOfType<Terminal>();
        shipRoom = UnityEngine.Object.FindAnyObjectByType<StartMatchLever>();
        shipDoor = UnityEngine.Object.FindObjectOfType<HangarShipDoor>();
        shipTeleporter = UnityEngine.Object.FindObjectOfType<ShipTeleporter>();
        itemsDesk = UnityEngine.Object.FindObjectOfType<DepositItemsDesk>();
        tvScript = UnityEngine.Object.FindObjectOfType<TVScript>();
        
        localVisor = GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/");
    }

    public void ClearLists()
    {
        items.Clear();
        landmines.Clear();
        turrets.Clear();
        doorLocks.Clear();
        entranceTeleports.Clear();
        players.Clear();
        enemies.Clear();
        steamValves.Clear();
        shipObjects.Clear();
        bigDoors.Clear();
    }

    public void SpawnObject(string name, Vector3 pos)
    {
        foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
        {
            if (item.itemName == name)
            {
                GameObject obj = UnityEngine.Object.Instantiate(item.spawnPrefab, pos, Quaternion.identity, StartOfRound.Instance.propsContainer);
                int valtouse = UnityEngine.Random.Range(item.minValue, item.maxValue);
                obj.GetComponent<GrabbableObject>().SetScrapValue(valtouse);
                obj.GetComponent<NetworkObject>().SpawnWithOwnership(hostPlayer.actualClientId, false);
                if (localPlayer.isInHangarShipRoom)
                {
                    obj.GetComponent<GrabbableObject>().OnBroughtToShip();
                    obj.GetComponent<GrabbableObject>().isInShipRoom = true;
                }
                spawnedObjects.AddItem(obj);
            }
        }
    }

    public void SpawnEnemy(string name, Vector3 pos)
    {
        foreach (SpawnableEnemyWithRarity enemy in RoundManager.Instance.currentLevel.Enemies)
        {
            if (enemy.enemyType.enemyName == name)
                RoundManager.Instance.SpawnEnemyGameObject(pos, 0, -1, enemy.enemyType);
        }
    }

    public void DeleteObject(string name)
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj.GetComponent<Item>().itemName == name)
                obj.GetComponent<NetworkObject>().Despawn();
        }
    }

    public void DeleteHeldObject()
    {
        localPlayer.DespawnHeldObject();
        spawnedObjects.Remove(currentlyHeldObject.gameObject);
    }

    public void CollectObjectsOfType<T>(List<T> list, Predicate<T> predicate = null) where T : MonoBehaviour
    {
        foreach (var obj in UnityEngine.Object.FindObjectsOfType<T>())
        {
            if (predicate == null || predicate(obj))
            {
                list.Add(obj);
            }
        }
    }

    public List<T> FindObjectsOfType<T>(Predicate<T> predicate = null) where T : MonoBehaviour
    {
        var objects = UnityEngine.Object.FindObjectsOfType<T>();
        return predicate == null ? new List<T>(objects) : new List<T>(objects).FindAll(predicate);
    }

    private static GameObjectManager instance;

    private static System.Reflection.FieldInfo isScrapField;
    private bool IsScrapItem(GrabbableObject item)
    {
        string typeName = item.GetType().Name;
        if (typeName == "RagdollGrabbableObject")
            return false;

        if (item.itemProperties == null) return false;
        if (isScrapField == null)
            isScrapField = typeof(Item).GetField("isScrap", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (isScrapField != null)
        {
            try { return (bool)isScrapField.GetValue(item.itemProperties); }
            catch { }
        }
        return item.itemProperties.itemName != "ClipboardManual" && item.itemProperties.itemName != "StickyNoteItem";
    }
}

public enum UnlockableUpgrade : int
{
    GreenSuit = 1,
    HazardSuit = 2,
    PajamaSuit = 3,
    CozyLights = 4,
    Teleporter = 5,
    Television = 6,
    Toilet = 9,
    Shower = 10,
    RecordPlayer = 12,
    Table = 13,
    RomanticTable = 14,
    SignalTranslator = 17,
    LoudHorn = 18,
    InverseTeleporter = 19,
    JackOLantern = 20,
    WelcomeMat = 21,
    Goldfish = 22,
    PlushiePajamaMan = 23
}
