using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public enum Direction {
    North, South, East, West
}

public enum RoomRarity {
    Common
}


public class DungeonGenerator : MonoBehaviour {
    // [SerializeField] PlayerCharacter playerObject;

    [SerializeField] Grid dungeonGrid;
    [SerializeField] Tile testHallTile;
    [SerializeField] Room initalRoomPrefab;
    [SerializeField] LayerMask buildingLayer;
    [SerializeField] Building vertHallPrefab;
    [SerializeField] Building horzHallPrefab;
    [SerializeField] int minimumRoomDistance;

    public static DungeonGenerator Instance { get; private set; }

    public Dictionary<RoomRarity, List<Room>> roomDic;

    void Awake() {

        if (Instance == null) {
            Instance = this;
            Initialize();
        } else {
            Destroy(this);
        }

    }

    void Initialize() {
        Debug.Log("Initializing Dungeon");
        Room room = Instantiate(initalRoomPrefab, Vector3.zero, Quaternion.identity, dungeonGrid.transform);

        List<Tilemap> tilemaps = new List<Tilemap>(room.GetComponentsInChildren<Tilemap>());

        room.OnRoomCompletion += HandleRoomCompletion;


        roomDic = ResourceManager.Instance.GetRoomDic();
    }



    //try each side of room
    //pick an edge floor tile
    //check if room is within an area of floor tile in the direction chosen
    //Build connection to it
    //decide on a room to spawn
    //decide on center of that room 
    //make minimum distance for center 5 + half the length of the room in the direction
    //make the other axis a random value from floor tile +- length of room in that axis
    //Check if spawning the room conflicts with already placed rooms
    //If not, Build connection to it
    Building SpawnBuilding(Building baseBuilding, Building addedBuildingPrefab, Direction direction) {
        if (baseBuilding == null || addedBuildingPrefab == null) return null;

        Building spawnedAddition = null;

        BoundsInt baseBounds = baseBuilding.Tilemap.cellBounds;
        BoundsInt addBounds = addedBuildingPrefab.Tilemap.cellBounds;

        if (direction == Direction.North) {
            // Debug.Log($"Add Bounds Y Diff: {addBounds.size.y}");
            // Debug.Log($"Add Bounds Y Other Meth: {addBounds.yMax - addBounds.yMin}");
            Vector3Int basePoint = new Vector3Int(Random.Range(baseBounds.xMin, baseBounds.xMax), baseBounds.yMax - 1);
            // Debug.Log($"Base Bounds: {baseBounds}");
            // Debug.Log($"Base Point: {basePoint}");
            Vector3Int additionCenter = new Vector3Int(basePoint.x, basePoint.y + minimumRoomDistance + Random.Range((addBounds.size.y) / 2, addBounds.size.y));
            // Debug.Log($"Addition Center: {additionCenter}");
            Vector3 additionCenterWorld = baseBuilding.Tilemap.GetCellCenterWorld(additionCenter);

            // Debug.Log($"Addition Center World: {additionCenterWorld}");


            Vector2 addedBuildingCenter = baseBuilding.Tilemap.GetCellCenterWorld(Vector3Int.zero);
            Vector3 diff = Building.GetBuildingOffset(addedBuildingCenter, additionCenterWorld);
            additionCenterWorld.y += .5f;
            additionCenterWorld.x -= .5f;
            // Debug.Log($"Diff: {diff}");
            spawnedAddition = Instantiate(addedBuildingPrefab, additionCenterWorld - diff, Quaternion.identity, dungeonGrid.transform);
        } else if (direction == Direction.South) {
            Vector3Int basePoint = new Vector3Int(Random.Range(baseBounds.xMin, baseBounds.xMax), baseBounds.yMin);
            Debug.Log($"Base Point: {basePoint}");
            Vector3Int additionCenter = new Vector3Int(basePoint.x, basePoint.y - minimumRoomDistance - Random.Range((addBounds.size.y) / 2, addBounds.size.y));
            Debug.Log($"Addition Center: {additionCenter}");
            Vector3 additionCenterWorld = baseBuilding.Tilemap.GetCellCenterWorld(additionCenter);
            Debug.Log($"Addition Center World: {additionCenterWorld}");

            Vector2 addedBuildingCenter = baseBuilding.Tilemap.GetCellCenterWorld(Vector3Int.zero);
            Vector3 diff = Building.GetBuildingOffset(addedBuildingCenter, additionCenterWorld);
            Debug.Log($"Diff: {diff}");
            additionCenterWorld.y += .5f;
            additionCenterWorld.x -= 1.5f;
            spawnedAddition = Instantiate(addedBuildingPrefab, additionCenterWorld, Quaternion.identity, dungeonGrid.transform);

        } else if (direction == Direction.East) {
            // Vector3Int basePoint = new Vector3Int(Random.Range(baseBounds.xMin, baseBounds.xMax), baseBounds.yMax - 1);
            // Vector3Int additionCenter = new Vector3Int(basePoint.x, basePoint.y + minimumRoomDistance + Random.Range((addBounds.size.y) / 2, addBounds.size.y));
            // Vector3 additionCenterWorld = baseBuilding.Tilemap.GetCellCenterWorld(additionCenter);
            // Vector2 addedBuildingCenter = baseBuilding.Tilemap.GetCellCenterWorld(Vector3Int.zero);
            // Vector3 diff = Building.GetBuildingOffset(addedBuildingCenter, additionCenterWorld);
            // additionCenterWorld.y -= .5f;
            // additionCenterWorld.x -= .5f;
            // spawnedAddition = Instantiate(addedBuildingPrefab, additionCenterWorld - diff, Quaternion.identity, dungeonGrid.transform);
        } else {
            // Vector3Int basePoint = new Vector3Int(Random.Range(baseBounds.xMin, baseBounds.xMax), baseBounds.yMax - 1);
            // Vector3Int additionCenter = new Vector3Int(basePoint.x, basePoint.y + minimumRoomDistance + Random.Range((addBounds.size.y) / 2, addBounds.size.y));
            // Vector3 additionCenterWorld = baseBuilding.Tilemap.GetCellCenterWorld(additionCenter);
            // Vector2 addedBuildingCenter = baseBuilding.Tilemap.GetCellCenterWorld(Vector3Int.zero);
            // Vector3 diff = Building.GetBuildingOffset(addedBuildingCenter, additionCenterWorld);
            // additionCenterWorld.y -= 1.5f;
            // additionCenterWorld.x -= .5f;
            // spawnedAddition = Instantiate(addedBuildingPrefab, additionCenterWorld - diff, Quaternion.identity, dungeonGrid.transform);
        }

        //Check for already spawned buildings in the area it would be spawned
        // List<Tilemap> tilemaps = new List<Tilemap>(spawnedAddition.GetComponentsInChildren<Tilemap>());
        // foreach (Tilemap tilemap in tilemaps) {

        //     BoundsInt bounds = tilemap.cellBounds;
        //     TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        //     for (int x = 0; x < bounds.size.x; x++) {
        //         for (int y = 0; y < bounds.size.y; y++) {
        //             TileBase tile = allTiles[x + y * bounds.size.x];
        //             if (tile != null) {

        //                 Vector2 cellPos = tilemap.CellToWorld(new Vector3Int(x + bounds.xMin, y + bounds.yMin));
        //                 Vector2 topRight = new Vector2(cellPos.x + .5f, cellPos.y + .5f);
        //                 Vector2 bottomLeft = new Vector2(cellPos.x + .5f, cellPos.y + .5f);
        //                 Collider2D collider = Physics2D.OverlapArea(bottomLeft, topRight, buildingLayer);
        //                 if (collider != null && !tilemaps.Contains(collider.GetComponent<Tilemap>())) {
        //                     Debug.Log($"Collided with {collider.name} at {topRight} and {bottomLeft}");
        //                     Debug.Log($"Destroy {spawnedAddition.name}");
        //                     Destroy(spawnedAddition.gameObject);
        //                     return null;
        //                 }
        //             }
        //         }
        //     }
        // }
        return spawnedAddition;




    }

    void HandleRoomCompletion(Room room) {
        Building hall;
        Building roomSpawn;
        Building roomBuilding = room.GetComponent<Building>();

        foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
            Building roomBuildingPrefab = GetNextRoom(room.RoomLevel + 1).GetComponent<Building>();
            // if (direction == Direction.East || direction == Direction.West) {
            //     hall = SpawnBuilding(roomBuilding, horzHallPrefab, direction);
            // } else {
            //     hall = SpawnBuilding(roomBuilding, vertHallPrefab, direction);
            // }

            roomSpawn = SpawnBuilding(roomBuilding, roomBuildingPrefab, direction);
            if (roomSpawn != null) {
                SetupRoom(roomSpawn.GetComponent<Room>(), room.RoomLevel + 1);
            }
        }

    }


    void SetupRoom(Room room, int roomLevel) {
        room.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;
        room.Initialize(roomLevel);
    }


    Room GetNextRoom(int roomLevel) {
        List<Room> commonRooms = roomDic[RoomRarity.Common];
        return commonRooms[Random.Range(0, commonRooms.Count)];
    }
}

