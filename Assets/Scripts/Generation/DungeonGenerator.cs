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

    //Change this to spawnrates in the future
    [SerializeField] ItemData itemForChest;

    [SerializeField] Grid dungeonGrid;
    [SerializeField] Tilemap hallTilemapFloors;
    [SerializeField] Tilemap hallTilemapWalls;
    [SerializeField] Room initalRoomPrefab;
    [SerializeField] LayerMask buildingLayer;
    [SerializeField] int hallRadius;
    [SerializeField] int minimumRoomDistance;

    [Header("Hallway Sprites")]
    [SerializeField] Tile floorTile;
    [SerializeField] Tile leftVertical;
    [SerializeField] Tile rightVertical;
    [SerializeField] Tile bottomHorizontal;
    [SerializeField] Tile topHorizontal;
    [SerializeField] Tile topHorizontalWall;
    [SerializeField] Tile LeftExterior;
    [SerializeField] Tile LeftInterior;
    [SerializeField] Tile RightExterior;
    [SerializeField] Tile RightInterior;



    public static DungeonGenerator Instance { get; private set; }

    public static int MaxRoomScale { get; private set; } = 15;

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
        room.Initialize(0, itemForChest);
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
    Room SpawnRoom(Room baseRoom, Room addedRoomPrefab, Direction direction) {
        if (baseRoom == null || addedRoomPrefab == null) return null;

        Room spawnedAddition;

        BoundsInt baseBounds = baseRoom.FloorTileSet.cellBounds;
        BoundsInt addBounds = addedRoomPrefab.FloorTileSet.cellBounds;

        Vector3Int baseConnectionPoint = GetConnectionPoint(baseBounds, direction);
        Vector3Int additionConnectionPoint = GetConnectionPoint(addBounds, Utilities.GetOppDirection(direction));
        Vector3Int additionCellCenterPoint;

        if (direction == Direction.North) {
            additionCellCenterPoint = new Vector3Int(baseConnectionPoint.x - additionConnectionPoint.x, baseConnectionPoint.y + minimumRoomDistance - additionConnectionPoint.y);
        } else if (direction == Direction.South) {
            additionCellCenterPoint = new Vector3Int(baseConnectionPoint.x - additionConnectionPoint.x, baseConnectionPoint.y - minimumRoomDistance - additionConnectionPoint.y);
        } else if (direction == Direction.East) {
            additionCellCenterPoint = new Vector3Int(baseConnectionPoint.x + minimumRoomDistance - additionConnectionPoint.x, baseConnectionPoint.y - additionConnectionPoint.y);
        } else {
            additionCellCenterPoint = new Vector3Int(baseConnectionPoint.x - minimumRoomDistance - additionConnectionPoint.x, baseConnectionPoint.y - additionConnectionPoint.y);
        }

        //This cell coordinates based on the tilemap of the current base Room, not of the original spawned Room. Meaning they are all relative 
        Vector3 additionCenterWorld = baseRoom.FloorTileSet.GetCellCenterWorld(additionCellCenterPoint);
        additionCenterWorld.y -= .5f;
        additionCenterWorld.x -= .5f;
        spawnedAddition = Instantiate(addedRoomPrefab, additionCenterWorld, Quaternion.identity, dungeonGrid.transform);

        //Turns colliders off to avoid messing with collision detection of already existing room
        foreach (Collider2D collider2D in spawnedAddition.GetComponentsInChildren<Collider2D>()) {
            collider2D.enabled = false;
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
                        Vector2 worldCellPos = new Vector2(cellPos.x - .5f + 1, cellPos.y - .5f);
                        Collider2D collider = Physics2D.OverlapBox(worldCellPos, Vector2.one, buildingLayer);


                        //Room already exists where the new one is to be placed
                        if (collider != null && !tilemaps.Contains(collider.GetComponent<Tilemap>())) {
                            // Debug.Log($"Collided with {collider.name} at {worldCellPos}");

                            // Debug.Log($"Destroy {spawnedAddition.name}");
                            Destroy(spawnedAddition.gameObject);


                            //Draw a hallway to the collided room
                            Vector3Int collidedConnectionPoint = GetConnectionPoint(collider.GetComponentInParent<Room>().FloorTileSet.cellBounds, Utilities.GetOppDirection(direction));
                            //DrawHallway(baseBuilding.Tilemap, baseConnectionPoint, collidedConnectionPoint);
                            return null;
                        }
                    }
                }
            }
        }

        //Turns colliders back on for new room
        foreach (Collider2D collider2D in spawnedAddition.GetComponentsInChildren<Collider2D>()) {
            collider2D.enabled = true;
        }

        //Connect base with new addition
        Vector3Int hallStart = hallTilemapFloors.WorldToCell(baseRoom.FloorTileSet.CellToWorld(baseConnectionPoint));
        //second needs to be in reference 
        Vector3Int hallEnd = hallTilemapFloors.WorldToCell(spawnedAddition.FloorTileSet.CellToWorld(additionConnectionPoint));


        //LOOK FOR REASON THIS IS HAPPENING
        if (direction == Direction.North || direction == Direction.South) {
            hallStart.x--;
            hallEnd.x--;
        }
        if (direction == Direction.East || direction == Direction.West) {
            hallStart.y--;
            hallEnd.y--;
        }

        DrawHallway(hallStart, hallEnd, direction);

        return spawnedAddition;




    }



    void HandleRoomCompletion(Room room) {

        Room roomSpawn;
        Room roomBuilding = room.GetComponent<Room>();

        foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
            Room roomBuildingPrefab = GetNextRoom(room.RoomLevel + 1).GetComponent<Room>();


            roomSpawn = SpawnRoom(roomBuilding, roomBuildingPrefab, direction);
            if (roomSpawn != null) {
                SetupRoom(roomSpawn.GetComponent<Room>(), room.RoomLevel + 1);
            }
        }

    }


    void SetupRoom(Room room, int roomLevel) {
        room.GetComponent<Room>().OnRoomCompletion += HandleRoomCompletion;
        room.Initialize(roomLevel, itemForChest);
    }


    Room GetNextRoom(int roomLevel) {
        List<Room> commonRooms = roomDic[RoomRarity.Common];
        return commonRooms[Random.Range(0, commonRooms.Count)];
    }


    //Change this in the future to get more varied spawns
    Vector3Int GetConnectionPoint(BoundsInt bounds, Direction direction) {

        if (direction == Direction.North) {
            return new Vector3Int(bounds.xMax - bounds.size.x / 2, bounds.yMax);

        } else if (direction == Direction.South) {
            return new Vector3Int(bounds.xMax - bounds.size.x / 2, bounds.yMin);
        } else if (direction == Direction.East) {
            return new Vector3Int(bounds.xMax, bounds.yMax - bounds.size.y / 2);
        } else {
            return new Vector3Int(bounds.xMin, bounds.yMax - bounds.size.y / 2);

        }
    }


    void DrawHallway(Vector3Int start, Vector3Int end, Direction direction) {

        int xInc = end.x < start.x ? -1 : 1;
        int yInc = end.y < start.y ? -1 : 1;

        //Not totally sure why this is needed but it is
        if (xInc < 0) {
            start.x--;
            end.x--;
        } else if (yInc < 0) {
            start.y--;
            end.y--;
        }

        //Offset for where the hallway should start drawing forwards and backwards
        if (direction == Direction.North) {
            start.y += 3;
        } else if (direction == Direction.East) {
            start.x++;
        } else if (direction == Direction.South) {
            start.y--;
        } else if (direction == Direction.West) {
            start.x--;
        }

        //Draw backwards
        if (direction == Direction.North || direction == Direction.South)
            InitializeDrawing(Utilities.GetOppDirection(direction), start, xInc, -yInc);
        else
            InitializeDrawing(Utilities.GetOppDirection(direction), start, -xInc, yInc);



        //Draw forwards
        InitializeDrawing(direction, start, xInc, yInc);


    }


    void InitializeDrawing(Direction direction, Vector3Int start, int xInc, int yInc) {
        if (direction == Direction.North) {
            Draw(direction, start, new Vector2Int(0, yInc), LeftExterior, RightExterior, leftVertical, rightVertical);
        } else if (direction == Direction.South) {
            Draw(direction, start, new Vector2Int(0, yInc), LeftInterior, RightInterior, leftVertical, rightVertical);
        } else if (direction == Direction.East) {
            Draw(direction, start, new Vector2Int(xInc, 0), RightExterior, topHorizontal, bottomHorizontal, topHorizontal);
        } else if (direction == Direction.West) {
            Draw(direction, start, new Vector2Int(xInc, 0), LeftExterior, topHorizontal, bottomHorizontal, topHorizontal);
        }
    }

    void Draw(
        Direction direction, Vector3Int start, Vector2Int inc,
        Tile leftOrBottomCorner, Tile rightOrTopCorner, Tile leftOrBottomWall,
        Tile rightOrTopWall
        ) {

        Debug.Log("Direction: " + direction);

        bool[] activeLanes = new bool[2 * hallRadius + 1];
        for (int i = 0; i < activeLanes.Length; i++) {
            activeLanes[i] = true;
        }

        Vector3Int current = start;

        for (int i = 0; i < 20; i++) {

            Debug.Log("I: " + i);

            //Minus one so it can increment at start of loop
            int laneOffset = -hallRadius - 1;

            for (int laneIndex = 0; laneIndex < activeLanes.Length; laneIndex++) {
                Debug.Log("LaneIndex: " + laneIndex);

                laneOffset++;
                //Check if one of the lanes collided with the room
                if (!activeLanes[laneIndex]) continue;

                Vector3Int currentPosition = new Vector3Int();
                if (direction == Direction.East || direction == Direction.West)
                    currentPosition = new Vector3Int(current.x, current.y + laneOffset);
                else if (direction == Direction.North || direction == Direction.South)
                    currentPosition = new Vector3Int(current.x + laneOffset, current.y);


                //Check for tile collision by using the world position
                Vector3 worldCellPos = hallTilemapFloors.CellToWorld(currentPosition);
                //Centers the world position to the center of the tile
                worldCellPos = new Vector2(worldCellPos.x + .5f, worldCellPos.y + .5f);

                Collider2D collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), 0, buildingLayer);

                Tilemap collidedTilemap = collider?.GetComponent<Tilemap>();
                if (collidedTilemap != null) {

                    //Checks if hallway collided with room floors
                    if (collider.gameObject.name == "FloorTile") {
                        //Stops the lane from drawing anymore and skips the rest of the loop so it does not draw over the floor tile it hit
                        activeLanes[laneIndex] = false;
                        continue;
                    } else {

                        //Checks if hallway sides collided with room walls and stops them
                        //This is mostly for the tops of rooms so that the two high room walls will remain intact
                        if (laneIndex == 0 || laneIndex == activeLanes.Length - 1) {
                            activeLanes[laneIndex] = false;
                        }

                        //Removes the room tile
                        collidedTilemap.SetTile(collidedTilemap.WorldToCell(worldCellPos), null);

                        //Do a second check for collision a tile ahead to determine if it should put down a corner
                        worldCellPos = new Vector2(worldCellPos.x + inc.x, worldCellPos.y + inc.y);
                        collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);

                        //If it hit a floor tile, then it draws a corner tile and skips the rest of the loop
                        if (collider != null && collider.name == "FloorTile") {
                            if (laneIndex == 0) {
                                hallTilemapWalls.SetTile(currentPosition, leftOrBottomCorner);
                            } else if (laneIndex == activeLanes.Length - 1) {
                                if (direction == Direction.East || direction == Direction.West) {
                                    //This is for the two high walls for the top side of horizontal hallways
                                    hallTilemapWalls.SetTile(new Vector3Int(currentPosition.x, currentPosition.y), topHorizontalWall);
                                    hallTilemapWalls.SetTile(new Vector3Int(currentPosition.x, currentPosition.y + 1), topHorizontalWall);
                                    hallTilemapWalls.SetTile(new Vector3Int(currentPosition.x, currentPosition.y + 2), rightOrTopCorner);
                                } else {
                                    hallTilemapWalls.SetTile(currentPosition, rightOrTopCorner);
                                }
                            } else
                                hallTilemapFloors.SetTile(currentPosition, floorTile);
                            continue;

                        }

                    }
                }

                //If we get here, then either a side wall or floor tile needs to be drawn 
                if (laneIndex == 0)
                    hallTilemapWalls.SetTile(currentPosition, leftOrBottomWall);
                else if (laneIndex == activeLanes.Length - 1) {
                    if (direction == Direction.East || direction == Direction.West) {
                        //This is for the two high walls for the top side of horizontal hallways
                        hallTilemapWalls.SetTile(new Vector3Int(currentPosition.x, currentPosition.y), topHorizontalWall);
                        hallTilemapWalls.SetTile(new Vector3Int(currentPosition.x, currentPosition.y + 1), topHorizontalWall);
                        hallTilemapWalls.SetTile(new Vector3Int(currentPosition.x, currentPosition.y + 2), rightOrTopWall);
                    } else {
                        hallTilemapWalls.SetTile(currentPosition, rightOrTopWall);
                    }
                } else
                    hallTilemapFloors.SetTile(currentPosition, floorTile);

            }

            current.y += inc.y;
            current.x += inc.x;
        }

    }

}


