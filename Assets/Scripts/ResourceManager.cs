using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// A class that manages the loading of resources from the Resources folder
/// </summary>
public class ResourceManager {

    public static GameObject DialogManagerPrefab { get; private set; }
    public static GameObject ChestInventoryPrefab { get; private set; }

    static private ResourceManager instance;
    public static ResourceManager Instance {
        get {
            if (instance == null) {
                instance = new ResourceManager();
            }
            return instance;
        }
    }

    private Dictionary<RoomRarity, List<Room>> roomDataDic = new Dictionary<RoomRarity, List<Room>>();
    private Dictionary<string, ItemData> itemDataDic;
    private List<Enemy> enemiesList = new List<Enemy>();

    private ResourceManager() {
        foreach (RoomRarity rarity in Enum.GetValues(typeof(RoomRarity))) {
            roomDataDic.Add(rarity, new List<Room>());
        }

        AssembleResources();
    }

    /// <summary>
    /// This loads certain resources in the Resource folder and makes them easily available to classes that need them 
    /// </summary>
    private void AssembleResources() {
        Debug.Log("Assembling Resources");

        //Load room prefabs
        List<Room> roomPrefabList = Resources.LoadAll<Room>("Rooms").ToList();
        Debug.Log($"Rooms Found: {roomPrefabList.Count}");
        foreach (Room room in roomPrefabList) {
            roomDataDic[room.Rarity].Add(room);
        }

        //load enemy prefabs
        enemiesList = Resources.LoadAll<Enemy>("Entities/Enemies").ToList();
        Debug.Log($"Enemies Found: {enemiesList.Count}");

        //load ui prefabs
        DialogManagerPrefab = Resources.Load<GameObject>("UI/Dialog/DialogManager");
        ChestInventoryPrefab = Resources.Load<GameObject>("UI/Inventory/ChestInventory");

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


    public Dictionary<RoomRarity, List<Room>> GetRoomDic() {
        return roomDataDic;
    }

    public List<Enemy> GetEnemies() {
        return enemiesList;
    }


}
