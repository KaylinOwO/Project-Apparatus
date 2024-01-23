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
    public GrabbableObject currentlyHeldObjectServer;

    public int shipValue = 0;

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

            currentlyHeldObjectServer = null;
            shipValue = 0;
            foreach (GrabbableObject item in Instance.items)
            {
                if (!item.heldByPlayerOnServer && item.isInShipRoom && item.name != "ClipboardManual" && item.name != "StickyNoteItem")
                    shipValue += item.scrapValue;

                if (item.heldByPlayerOnServer && item.playerHeldBy == GameNetworkManager.Instance?.localPlayerController)
                    currentlyHeldObjectServer = item;

            }
                
            foreach (PlayerControllerB player in Instance.players)
            {
                if(player.IsHost)
                    hostPlayer = player;
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

    public void spawnObject(string name, Vector3 pos)
    {
        foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
        {
            if (item.name == name)
            {
                GameObject obj = UnityEngine.Object.Instantiate(item.spawnPrefab, pos, Quaternion.identity, StartOfRound.Instance.propsContainer);
                //todo look into networkspawnmanager to fix non host spawning
                //foreach (PlayerControllerB player in players)
                //{
                //if (player.IsHost)
                //{
                //i think by doing this in a patch, will make it so non host can spawn
                //obj.GetComponent<NetworkObject>().OwnerClientId = player.playerClientId;
                ulong originalid = PAUtils.GetClientId(localPlayer); //doing raw form for now this way no worry about if the function works in the first place 
                PAUtils.SetClientId(localPlayer, PAUtils.GetClientId(hostPlayer));
                System.Random rand = new System.Random();
                int valtouse = rand.Next(item.minValue, item.maxValue);
                obj.GetComponent<GrabbableObject>().SetScrapValue(valtouse);
                obj.GetComponent<NetworkObject>().Spawn();
                PAUtils.SetClientId(localPlayer, originalid);
                //}
                //}

                spawnedObjects.AddItem(obj);
            }
        }
    }
    public void deleteObject(string name)
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj.name == name)
                obj.GetComponent<NetworkObject>().Despawn();
        }
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
