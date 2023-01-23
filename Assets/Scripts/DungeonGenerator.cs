using UnityEngine.Tilemaps;
using UnityEngine;

public enum Direction {
    North, South, East, West
}


public class DungeonGenerator : MonoBehaviour {
    // [SerializeField] PlayerCharacter playerObject;

    [SerializeField] Grid dungeonGrid;
    [SerializeField] Room roomPrefab;
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

        Vector2 roomNorthWorldPos = roomBuilding.Tilemap.GetCellCenterWorld((Vector3Int)roomBuilding.North);


        //Setup North hall/room
        Building nHall = SpawnBuilding(roomBuilding, vertHallPrefab, Direction.North);
        SpawnBuilding(nHall, roomBuilding, Direction.North);

        //Setup South hall/room
        Building sHall = SpawnBuilding(roomBuilding, vertHallPrefab, Direction.South);
        Building sRoom = SpawnBuilding(sHall, roomBuilding, Direction.South);
        Building ssHall = SpawnBuilding(sRoom, vertHallPrefab, Direction.South);
        Building ssRoom = SpawnBuilding(ssHall, roomBuilding, Direction.South);

        //Setup West hall/room
        Building wHall = SpawnBuilding(roomBuilding, horzHallPrefab, Direction.West);
        Building wRoom = SpawnBuilding(wHall, roomBuilding, Direction.West);


        //Setup East hall/room
        Building eHall = SpawnBuilding(roomBuilding, horzHallPrefab, Direction.East);
        Building eRoom = SpawnBuilding(eHall, roomBuilding, Direction.East);
    }

    Building SpawnBuilding(Building baseBuilding, Building addedBuildingPrefab, Direction direction) {
        Building spawnedAddition = null;
        Vector2 baseConnection = baseBuilding.Tilemap.GetCellCenterWorld((Vector3Int)baseBuilding.GetConnectionPosition(direction));

        Vector2 addedBuildingCenter = baseBuilding.Tilemap.GetCellCenterWorld(Vector3Int.zero);
        Debug.Log($"Center: {addedBuildingCenter}");
        Vector2 addedBuildingConnectionWorldPos = baseBuilding.Tilemap.GetCellCenterWorld(addedBuildingPrefab.GetConnectionPosition(Utilities.GetOppDirection(direction)));
        Debug.Log($"ConnectionPos: {addedBuildingPrefab.GetConnectionPosition(Utilities.GetOppDirection(direction))}");
        Debug.Log($"ConnectionWorldPos: {addedBuildingConnectionWorldPos}");

        Vector2 diff = Building.GetBuildingOffset(addedBuildingCenter, addedBuildingConnectionWorldPos);
        Debug.Log($"Diff: {diff}");
        if (direction == Direction.North) {
            spawnedAddition = Instantiate(addedBuildingPrefab, new Vector3(baseConnection.x + diff.x - .5f, baseConnection.y + diff.y + .5f, 0), Quaternion.identity, dungeonGrid.transform);
        } else if (direction == Direction.South) {
            spawnedAddition = Instantiate(addedBuildingPrefab, new Vector3(baseConnection.x + diff.x - .5f, baseConnection.y + diff.y - 1.5f, 0), Quaternion.identity, dungeonGrid.transform);
        } else if (direction == Direction.East) {
            spawnedAddition = Instantiate(addedBuildingPrefab, new Vector3(baseConnection.x + diff.x + .5f, baseConnection.y + diff.y - .5f, 0), Quaternion.identity, dungeonGrid.transform);
        } else {
            spawnedAddition = Instantiate(addedBuildingPrefab, new Vector3(baseConnection.x + diff.x - 1.5f, baseConnection.y + diff.y - .5f, 0), Quaternion.identity, dungeonGrid.transform);
        }

        return spawnedAddition;
    }
}