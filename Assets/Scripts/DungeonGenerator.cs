using UnityEngine.Tilemaps;
using UnityEngine;

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


    void Update() {

    }

    void Initialize() {
        Debug.Log("Initializing Dungeon");
        Room room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, dungeonGrid.transform);
        room.Initialize(Vector2Int.zero);
        Building roomBuilding = room.GetComponent<Building>();

        room.OnRoomCompletion += HandleRoomCompletion;

    }

    Building SpawnBuilding(Building baseBuilding, Building addedBuildingPrefab, Direction direction) {
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
        TilemapCollider2D tilemapCollider = spawnedAddition.GetComponent<TilemapCollider2D>();
        Vector2 bottomLeft = new Vector2(tilemapCollider.bounds.center.x - tilemapCollider.bounds.extents.x, tilemapCollider.bounds.center.y - tilemapCollider.bounds.extents.y);
        Debug.Log($"Bottom left: {bottomLeft}");
        Vector2 topRight = new Vector2 (tilemapCollider.bounds.center.x + tilemapCollider.bounds.extents.x, tilemapCollider.bounds.center.y + tilemapCollider.bounds.extents.y);
        Debug.Log($"Top Right: {topRight}");
        Collider2D collider = Physics2D.OverlapArea(bottomLeft, topRight, buildingLayer);
        if (collider != null) {
            Destroy(spawnedAddition.gameObject);
            return null;
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
            roomSpawn.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;
            hall = SpawnBuilding(roomBuilding, horzHallPrefab, Direction.West);
            roomSpawn = SpawnBuilding(hall, roomBuildingPrefab, Direction.West);
            roomSpawn.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;
            hall = SpawnBuilding(roomBuilding, vertHallPrefab, Direction.North);
            roomSpawn = SpawnBuilding(hall, roomBuildingPrefab, Direction.North);
            roomSpawn.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;
            
            hall = SpawnBuilding(roomBuilding, vertHallPrefab, Direction.South);
            roomSpawn = SpawnBuilding(hall, roomBuildingPrefab, Direction.South);
            roomSpawn.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;
           


    }
}