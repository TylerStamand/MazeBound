using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

public enum Direction {
    North, South, East, West
}


public class DungeonGenerator : MonoBehaviour {
    // [SerializeField] PlayerCharacter playerObject;

    [SerializeField] Grid dungeonGrid;
    [SerializeField] Room roomPrefab;
    [SerializeField] LayerMask buildingLayer;
    [SerializeField] Building vertHallPrefab;
    [SerializeField] Building horzHallPrefab;


    public static DungeonGenerator Instance { get; private set; }


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
        Room room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, dungeonGrid.transform);

        List<Tilemap> tilemaps = new List<Tilemap>(room.GetComponentsInChildren<Tilemap>());

        room.OnRoomCompletion += HandleRoomCompletion;
    }

    Building SpawnBuilding(Building baseBuilding, Building addedBuildingPrefab, Direction direction) {
        if (baseBuilding == null || addedBuildingPrefab == null) return null;

        Building spawnedAddition = null;
        Vector2 baseConnection = baseBuilding.Tilemap.GetCellCenterWorld((Vector3Int)baseBuilding.GetConnectionPosition(direction));

        Vector2 addedBuildingCenter = baseBuilding.Tilemap.GetCellCenterWorld(Vector3Int.zero);


        Vector2 addedBuildingConnectionWorldPos = baseBuilding.Tilemap.GetCellCenterWorld(addedBuildingPrefab.GetConnectionPosition(Utilities.GetOppDirection(direction)));
        Vector2 diff = Building.GetBuildingOffset(addedBuildingCenter, addedBuildingConnectionWorldPos);

        if (direction == Direction.North) {
            spawnedAddition = Instantiate(addedBuildingPrefab, new Vector3(baseConnection.x + diff.x - .5f, baseConnection.y + diff.y + .5f, 0), Quaternion.identity, dungeonGrid.transform);
        } else if (direction == Direction.South) {
            spawnedAddition = Instantiate(addedBuildingPrefab, new Vector3(baseConnection.x + diff.x - .5f, baseConnection.y + diff.y - 1.5f, 0), Quaternion.identity, dungeonGrid.transform);
        } else if (direction == Direction.East) {
            spawnedAddition = Instantiate(addedBuildingPrefab, new Vector3(baseConnection.x + diff.x + .5f, baseConnection.y + diff.y - .5f, 0), Quaternion.identity, dungeonGrid.transform);
        } else {
            spawnedAddition = Instantiate(addedBuildingPrefab, new Vector3(baseConnection.x + diff.x - 1.5f, baseConnection.y + diff.y - .5f, 0), Quaternion.identity, dungeonGrid.transform);
        }

        //Check for already spawned buildings in the area it would be spawned
        List<Tilemap> tilemaps = new List<Tilemap>(spawnedAddition.GetComponentsInChildren<Tilemap>());
        foreach (Tilemap tilemap in tilemaps) {

            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            for (int x = 0; x < bounds.size.x; x++) {
                for (int y = 0; y < bounds.size.y; y++) {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null) {

                        Vector2 cellPos = tilemap.CellToWorld(new Vector3Int(x + bounds.xMin, y + bounds.yMin));
                        Vector2 topRight = new Vector2(cellPos.x + .5f, cellPos.y + .5f);
                        Vector2 bottomLeft = new Vector2(cellPos.x + .5f, cellPos.y + .5f);
                        Collider2D collider = Physics2D.OverlapArea(bottomLeft, topRight, buildingLayer);
                        if (collider != null && !tilemaps.Contains(collider.GetComponent<Tilemap>())) {
                            Debug.Log($"Collided with {collider.name} at {topRight} and {bottomLeft}");
                            Debug.Log($"Destroy {spawnedAddition.name}");
                            Destroy(spawnedAddition.gameObject);
                            return null;
                        }

                    }
                }
            }

        }
        return spawnedAddition;




    }

    void HandleRoomCompletion(Room room) {
        Building hall;
        Building roomSpawn;
        Building roomBuilding = room.GetComponent<Building>();
        Building roomBuildingPrefab = roomPrefab.GetComponent<Building>();
        hall = SpawnBuilding(roomBuilding, horzHallPrefab, Direction.East);
        roomSpawn = SpawnBuilding(hall, roomBuildingPrefab, Direction.East);
        if (roomSpawn != null)
            roomSpawn.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;

        hall = SpawnBuilding(roomBuilding, horzHallPrefab, Direction.West);
        roomSpawn = SpawnBuilding(hall, roomBuildingPrefab, Direction.West);
        if (roomSpawn != null)
            roomSpawn.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;
        hall = SpawnBuilding(roomBuilding, vertHallPrefab, Direction.North);
        roomSpawn = SpawnBuilding(hall, roomBuildingPrefab, Direction.North);
        if (roomSpawn != null)
            roomSpawn.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;

        hall = SpawnBuilding(roomBuilding, vertHallPrefab, Direction.South);
        roomSpawn = SpawnBuilding(hall, roomBuildingPrefab, Direction.South);
        if (roomSpawn != null)
            roomSpawn.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;



    }
}