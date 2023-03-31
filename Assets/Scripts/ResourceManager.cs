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

    static private ResourceManager instance;
    public static ResourceManager Instance {
        get {
            if (instance == null) {
                instance = new ResourceManager();
            }
            return instance;
        }
    }

    private List<Room> roomList1 = new List<Room>();
    private Room shrineRoom1;
    private Dictionary<string, ItemData> itemDataDic;
    private List<Enemy> enemiesList = new List<Enemy>();

    private ResourceManager() {


        AssembleResources();
    }

    /// <summary>
    /// This loads certain resources in the Resource folder and makes them easily available to classes that need them 
    /// </summary>
    private void AssembleResources() {
        Debug.Log("Assembling Resources");

        //Load player prefab
        PlayerPrefab = Resources.Load<GameObject>("Entities/PlayerPrefab");

        //Load room prefabs
        List<Room> roomPrefabList = Resources.LoadAll<Room>("Rooms").ToList();
        Debug.Log($"Rooms Found: {roomPrefabList.Count}");
        foreach (Room room in roomPrefabList) {

            if (room.name != "Shrine Room")
                roomList1.Add(room);
            else
                shrineRoom1 = room;
        }

        //load enemy prefabs
        enemiesList = Resources.LoadAll<Enemy>("Entities/Enemies").ToList();
        Debug.Log($"Enemies Found: {enemiesList.Count}");

        //load ui prefabs
        DialogManagerPrefab = Resources.Load<GameObject>("UI/Dialog/DialogManager");
        ChestInventoryPrefab = Resources.Load<GameObject>("UI/Inventory/ChestInventory");
        LoadMenuPrefab = Resources.Load<GameObject>("UI/Menu/LoadMenu");
        MazePauseMenuPrefab = Resources.Load<GameObject>("UI/Menu/MazePauseMenu");
        HubPauseMenuPrefab = Resources.Load<GameObject>("UI/Menu/HubPauseMenu");
        InventoryPrefab = Resources.Load<GameObject>("UI/Inventory/Inventory");

        //load item data
        List<ItemData> itemDataList = Resources.LoadAll<ItemData>("Items").ToList();
        itemDataDic = itemDataList.ToDictionary(r => {
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


    public ItemData GetItemData(string name) {
        itemDataDic.TryGetValue(name, out ItemData value);
        return value;

    }


    public List<Room> GetRoomDic(int level) {
        return roomList1;
    }

    public Room GetShrineRoom(int level) {
        return shrineRoom1;
    }

    public List<Enemy> GetEnemies() {
        return enemiesList;
    }


}
