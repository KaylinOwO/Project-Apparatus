using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public IEnumerator CollectObjects()
    {
        while (true)
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
