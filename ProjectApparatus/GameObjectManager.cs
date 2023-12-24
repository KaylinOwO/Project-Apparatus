using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class GameObjectManager
{
    public static GameObjectManager Instance
    {
        get
        {
            if (GameObjectManager.instance == null)
            {
                GameObjectManager.instance = new GameObjectManager();
            }
            return GameObjectManager.instance;
        }
    }

    public List<GrabbableObject> items = new List<GrabbableObject>();
    public List<Landmine> landmines = new List<Landmine>();
    public List<Turret> turrets = new List<Turret>();
    public List<DoorLock> door_locks = new List<DoorLock>();
    public List<EntranceTeleport> entrance_doors = new List<EntranceTeleport>();
    public List<PlayerControllerB> players = new List<PlayerControllerB>();
    public List<EnemyAI> enemies = new List<EnemyAI>();
    public List<SteamValveHazard> steamValves = new List<SteamValveHazard>();
    public List<PlaceableShipObject> shipObjects = new List<PlaceableShipObject>();
    public PlayerControllerB localPlayer;
    public HangarShipDoor shipDoor;
    public ShipLights shipLights;
    public Terminal shipTerminal;
    public DepositItemsDesk itemsDesk;
    public TVScript tvScript;

    public IEnumerator CollectObjects() // This is awful and I need to look into a better way of doing this
    {
        while (true)
        {
            items.Clear();
            landmines.Clear();
            turrets.Clear();
            door_locks.Clear();
            entrance_doors.Clear();
            players.Clear();
            enemies.Clear();
            steamValves.Clear();
            shipObjects.Clear();

            items.AddRange(UnityEngine.Object.FindObjectsOfType<GrabbableObject>());
            landmines.AddRange(UnityEngine.Object.FindObjectsOfType<Landmine>());
            turrets.AddRange(UnityEngine.Object.FindObjectsOfType<Turret>());
            door_locks.AddRange(UnityEngine.Object.FindObjectsOfType<DoorLock>());
            entrance_doors.AddRange(UnityEngine.Object.FindObjectsOfType<EntranceTeleport>());
            players.AddRange(UnityEngine.Object.FindObjectsOfType<PlayerControllerB>());
            enemies.AddRange(UnityEngine.Object.FindObjectsOfType<EnemyAI>());
            shipObjects.AddRange(UnityEngine.Object.FindObjectsOfType<PlaceableShipObject>());
            steamValves.AddRange(UnityEngine.Object.FindObjectsOfType<SteamValveHazard>());

            if (GameNetworkManager.Instance)
                localPlayer =  GameNetworkManager.Instance.localPlayerController;
            shipLights = UnityEngine.Object.FindObjectOfType<ShipLights>();
            shipTerminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            itemsDesk = UnityEngine.Object.FindObjectOfType<DepositItemsDesk>();
            shipDoor = UnityEngine.Object.FindObjectOfType<HangarShipDoor>();
            tvScript = UnityEngine.Object.FindObjectOfType<TVScript>();

            yield return new WaitForSeconds(3f);
        }
    }

    private static GameObjectManager instance;
}