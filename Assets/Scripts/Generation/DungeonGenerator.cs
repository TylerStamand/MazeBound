using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using NaughtyAttributes;

public enum Direction {
    North, South, East, West
}



public class DungeonGenerator : MonoBehaviour {
    [MinValue(1), MaxValue(3)]
    [SerializeField] int mazeLevel;

    [SerializeField] SpawnRates spawnRates;

    [Header("Shrine Settings")]
    [SerializeField] int shrineMinDistance;
    [SerializeField] float shrineSpawnRate;

    [Header("Room Settings")]
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


    public event Action<NPC> OnNPCFound;

    public event Action<int, Shrine> OnShrineFound;

    public Vector2 SpawnPoint {
        get {
            Vector3Int tileCenter = new Vector3Int(startRoom.FloorTileSet.cellBounds.x / 2, startRoom.FloorTileSet.cellBounds.y / 2);
            return startRoom.FloorTileSet.CellToWorld(tileCenter);
        }
    }

    Room startRoom;

    List<Room> roomList;

    Room shrineRoom;

    void Awake() {

        if (Instance == null) {
            Instance = this;
            Initialize();
        } else {
            Destroy(this);
        }

    }

    public void Initialize() {
        Debug.Log("Initializing Dungeon");

        Room room = Instantiate(initalRoomPrefab, Vector3.zero, Quaternion.identity, dungeonGrid.transform);
        room.Initialize(0, spawnRates);
        List<Tilemap> tilemaps = new List<Tilemap>(room.GetComponentsInChildren<Tilemap>());

        room.OnRoomCompletion += HandleRoomCompletion;
        room.OnNPCFound += (x) => OnNPCFound?.Invoke(x);
        roomList = ResourceManager.Instance.GetRoomDic(mazeLevel);
        shrineRoom = ResourceManager.Instance.GetShrineRoom(mazeLevel);
        startRoom = room;

    }




    //Pick an edge floor tile for each room
    //Find the center of the room within the world
    //Spawn the room and check if it conflicts with other rooms
    //If it does, destroy it and skip to the next room
    //Build connection to it


    Room SpawnRoom(Room baseRoom, Room addedRoomPrefab, Direction direction) {
        if (baseRoom == null || addedRoomPrefab == null) return null;

        Room spawnedAddition;

        BoundsInt baseBounds = baseRoom.FloorTileSet.cellBounds;
        BoundsInt addBounds = addedRoomPrefab.FloorTileSet.cellBounds;

        //1. Finds the connection point of the base room and the room to be added
        Vector3Int baseConnectionPoint = GetConnectionPoint(baseBounds, direction);
        Vector3Int additionConnectionPoint = GetConnectionPoint(addBounds, Utilities.GetOppDirection(direction));

        //2.1 Finds where the center of the room would be in the tilemap of the base room
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

        //2.2 This cell coordinates based on the tilemap of the current base Room, not of the original spawned Room. Meaning they are all relative 
        Vector3 additionCenterWorld = baseRoom.FloorTileSet.GetCellCenterWorld(additionCellCenterPoint);
        additionCenterWorld.y -= .5f;
        additionCenterWorld.x -= .5f;

        //3. Spawns the room
        spawnedAddition = Instantiate(addedRoomPrefab, additionCenterWorld, Quaternion.identity, dungeonGrid.transform);
        spawnedAddition.OnNPCFound += (x) => OnNPCFound?.Invoke(x);
        spawnedAddition.OnShrineFound += (x) => OnShrineFound?.Invoke(mazeLevel, x);


        //4. Turns colliders off to avoid messing with collision detection of already existing room
        foreach (Collider2D collider2D in spawnedAddition.GetComponentsInChildren<Collider2D>()) {
            collider2D.enabled = false;
        }
        //4.1 Check for already spawned buildings in the area it would be spawned
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
                        Room collidedRoom = collider?.GetComponentInParent<Room>();

                        //Room already exists where the new one is to be placed
                        if (collidedRoom != null && !tilemaps.Contains(collider.GetComponent<Tilemap>())) {

                            Destroy(spawnedAddition.gameObject);

                            //Draw a hallway to the collided room
                            Vector3Int collidedConnectionPoint = GetConnectionPoint(collidedRoom.FloorTileSet.cellBounds, Utilities.GetOppDirection(direction));
                            return null;
                        }
                    }
                }
            }
        }

        //4.2 Turns colliders back on for new room
        foreach (Collider2D collider2D in spawnedAddition.GetComponentsInChildren<Collider2D>()) {
            collider2D.enabled = true;
        }

        //Connect base with new addition
        Vector3Int hallStart = hallTilemapFloors.WorldToCell(baseRoom.FloorTileSet.CellToWorld(baseConnectionPoint));
        //second needs to be in reference 
        Vector3Int hallEnd = hallTilemapFloors.WorldToCell(spawnedAddition.FloorTileSet.CellToWorld(additionConnectionPoint));


        if (direction == Direction.North || direction == Direction.South) {
            hallStart.x--;
            hallEnd.x--;
        }
        if (direction == Direction.East || direction == Direction.West) {
            hallStart.y--;
            hallEnd.y--;
        }

        //5 Finally, draw the hallway
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
        room.Initialize(roomLevel, spawnRates);
    }


    Room GetNextRoom(int roomLevel) {
        Debug.Log("Room Level: " + roomLevel);
        //Checks if the player is deep enough in the dungeon to spawn a shrine
        if (GameManager.Instance != null) {

            //Prevents shrines from spawning if puzzle piece is already obtained
            if (roomLevel > shrineMinDistance && mazeLevel > GameManager.Instance.PuzzlePiecesCollectedCount) {
                //Chance to spawn a shrine
                if (Random.value < shrineSpawnRate) {
                    return shrineRoom;
                }
            }
        }

        //Otherwise spawn a normal room
        return roomList[Random.Range(0, roomList.Count)];
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
            Draw(direction, start, new Vector2Int(xInc, 0), LeftExterior, LeftInterior, bottomHorizontal, topHorizontal);
        } else if (direction == Direction.West) {
            Draw(direction, start, new Vector2Int(xInc, 0), RightExterior, RightInterior, bottomHorizontal, topHorizontal);
        }
    }

    void Draw(
        Direction direction, Vector3Int start, Vector2Int inc,
        Tile leftOrBottomCorner, Tile rightOrTopCorner, Tile leftOrBottomWall,
        Tile rightOrTopWall
        ) {


        bool[] activeLanes = new bool[2 * hallRadius + 1];
        for (int i = 0; i < activeLanes.Length; i++) {
            activeLanes[i] = true;
        }

        Vector3Int current = start;

        for (int i = 0; i < 20; i++) {

            //Minus one so it can increment at start of loop
            int laneOffset = -hallRadius - 1;

            for (int laneIndex = 0; laneIndex < activeLanes.Length; laneIndex++) {

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


                        if (direction != Direction.South) {
                            //Do a second check for collision a tile ahead to determine if it should put down a corner
                            worldCellPos = new Vector2(worldCellPos.x + inc.x, worldCellPos.y + inc.y);
                        } else {
                            //if south, go three tiles ahead to check for collision
                            worldCellPos = new Vector2(worldCellPos.x + inc.x, worldCellPos.y + inc.y - 2);
                        }

                        collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);

                        //If it hit a floor tile, then it draws a corner tile and skips the rest of the loop
                        if (collider != null && collider.name == "FloorTile") {
                            if (direction == Direction.East || direction == Direction.West) {
                            }
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


