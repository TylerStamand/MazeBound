using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager {



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

    private ResourceManager() {
        foreach (RoomRarity rarity in Enum.GetValues(typeof(RoomRarity))) {
            roomDataDic.Add(rarity, new List<Room>());
        }
        AssembleResources();
    }

    private void AssembleResources() {
        Debug.Log("Assembling Resources");
        List<Room> roomPrefabList = Resources.LoadAll<Room>("Prefabs/Rooms").ToList();
        Debug.Log($"Rooms Found: {roomPrefabList.Count}");
        foreach (Room room in roomPrefabList) {
            roomDataDic[room.Rarity].Add(room);
        }
    }

    public Dictionary<RoomRarity, List<Room>> GetRoomDic() {
        return roomDataDic;
    }
}
