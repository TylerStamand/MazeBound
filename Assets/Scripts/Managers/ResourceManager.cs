using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// A class that manages the loading of resources from the Resources folder
/// </summary>
public class ResourceManager {

    public GameObject DialogManagerPrefab { get; private set; }
    public GameObject ChestInventoryPrefab { get; private set; }
    public GameObject InventoryPrefab { get; private set; }
    public GameObject MazePauseMenuPrefab { get; private set; }
    public GameObject HubPauseMenuPrefab { get; private set; }
    public GameObject LoadMenuPrefab { get; private set; }
    public GameObject PlayerPrefab { get; private set; }
    public GameObject FaderPrefab { get; private set; }

    public AudioClip ButtonClickSound { get; private set; }

    static private ResourceManager instance;
    public static ResourceManager Instance {
        get {
            if (instance == null) {
                instance = new ResourceManager();
            }
            return instance;
        }
    }


    List<Room>[] roomLists = new List<Room>[3];
    Room[] shrineRooms = new Room[3];

    Dictionary<string, ItemData>[] itemDataDics = new Dictionary<string, ItemData>[3];
    List<Enemy>[] enemiesLists = new List<Enemy>[3];

    ResourceManager() {
        for (int i = 0; i < 3; i++) {
            roomLists[i] = new List<Room>();
            enemiesLists[i] = new List<Enemy>();
            itemDataDics[i] = new Dictionary<string, ItemData>();
        }

        AssembleResources();
    }

    /// <summary>
    /// This loads certain resources in the Resource folder and makes them easily available to classes that need them 
    /// </summary>
    void AssembleResources() {
        Debug.Log("Assembling Resources");

        //Load player prefab
        PlayerPrefab = Resources.Load<GameObject>("Entities/PlayerPrefab");

        //Load room prefabs 
        LoadRooms();

        //load enemy prefabs
        LoadEnemies();

        //load ui prefabs
        LoadUI();

        //load item data
        LoadItems();

        //load sounds
        LoadSounds();

    }

    void LoadSounds() {
        ButtonClickSound = Resources.Load<AudioClip>("Sounds/ButtonClick");
    }

    void LoadRooms() {
        for (int i = 0; i < 3; i++) {
            List<Room> roomPrefabList = Resources.LoadAll<Room>("Rooms/Maze" + (i + 1)).ToList();
            Debug.Log($"Rooms Found: {roomPrefabList.Count}");
            foreach (Room room in roomPrefabList) {

                if (room.name != "Shrine Room")
                    roomLists[i].Add(room);
                else
                    shrineRooms[i] = room;
            }
        }

    }

    void LoadEnemies() {
        for (int i = 0; i < 3; i++) {
            enemiesLists[i] = Resources.LoadAll<Enemy>("Entities/Enemies/Maze" + (i + 1)).ToList();
        }
    }

    void LoadItems() {
        Debug.Log("Loading Items");
        for (int i = 0; i < 3; i++) {
            List<ItemData> itemDataList = Resources.LoadAll<ItemData>("MazeData/Maze" + (i + 1)).ToList();
            Debug.Log($"Items Found Maze {i + 1}: {itemDataList.Count}");
            itemDataDics[i] = itemDataList.ToDictionary(r => {
                if (r.Name != ItemData.DefaultName) {
                    return r.Name;
                } else {
                    Debug.LogError("Weapon not given a name");
                    return "";
                }
            },
                r => r
            );
        }
    }

    void LoadUI() {
        DialogManagerPrefab = Resources.Load<GameObject>("UI/Dialog/DialogManager");
        ChestInventoryPrefab = Resources.Load<GameObject>("UI/Inventory/ChestInventory");
        LoadMenuPrefab = Resources.Load<GameObject>("UI/Menu/LoadMenu");
        MazePauseMenuPrefab = Resources.Load<GameObject>("UI/Menu/MazePauseMenu");
        HubPauseMenuPrefab = Resources.Load<GameObject>("UI/Menu/HubPauseMenu");
        InventoryPrefab = Resources.Load<GameObject>("UI/Inventory/Inventory");
        FaderPrefab = Resources.Load<GameObject>("UI/Fader");   

    }


    public ItemData GetItemData(string name) {
        foreach (Dictionary<string, ItemData> itemDataDic in itemDataDics) {
            if (itemDataDic.ContainsKey(name)) {
                return itemDataDic[name];
            }
        }
        Debug.LogError("Item not found");
        return null;

    }


    public List<Room> GetRoomDic(int level) {
        if (level < 1 || level > 3) {
            Debug.LogError("Level out of range");
            return null;
        }

        return roomLists[level - 1];
    }

    public Room GetShrineRoom(int level) {
        if (level < 1 || level > 3) {
            Debug.LogError("Level out of range");
            return null;
        }

        return shrineRooms[level - 1];
    }

    public List<Enemy> GetEnemies(int level) {
        return enemiesLists[level - 1];
    }


}
