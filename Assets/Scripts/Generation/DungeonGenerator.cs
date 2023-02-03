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
    [SerializeField] Tilemap hallTilemap;
    [SerializeField] Room initalRoomPrefab;
    [SerializeField] LayerMask buildingLayer;
    [SerializeField] Room vertHallPrefab;
    [SerializeField] Room horzHallPrefab;
    [SerializeField] int hallRadius;
    [SerializeField] int minimumRoomDistance;

    [Header("Hallway Sprites")]
    [SerializeField] Tile testHallTile;
    [SerializeField] Tile leftVertical;
    [SerializeField] Tile rightVertical;
    [SerializeField] Tile bottomHorizontal;
    [SerializeField] Tile topHorizontal;
    [SerializeField] Tile LeftExterior;
    [SerializeField] Tile LeftInterior;
    [SerializeField] Tile RightExterior;
    [SerializeField] Tile RightInterior;



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
    Room SpawnBuilding(Room baseBuilding, Room addedBuildingPrefab, Direction direction) {
        if (baseBuilding == null || addedBuildingPrefab == null) return null;

        Room spawnedAddition;

        BoundsInt baseBounds = baseBuilding.FloorTileSet.cellBounds;
        BoundsInt addBounds = addedBuildingPrefab.FloorTileSet.cellBounds;

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
        Vector3 additionCenterWorld = baseBuilding.FloorTileSet.GetCellCenterWorld(additionCellCenterPoint);
        additionCenterWorld.y -= .5f;
        additionCenterWorld.x -= .5f;
        spawnedAddition = Instantiate(addedBuildingPrefab, additionCenterWorld, Quaternion.identity, dungeonGrid.transform);

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
        Vector3Int hallStart = hallTilemap.WorldToCell(baseBuilding.FloorTileSet.CellToWorld(baseConnectionPoint));
        //second needs to be in reference 
        Vector3Int hallEnd = hallTilemap.WorldToCell(spawnedAddition.FloorTileSet.CellToWorld(additionConnectionPoint));


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

    //Needs to be in hallmap coordinates
    void DrawHallway(Vector3Int start, Vector3Int end, Direction direction) {
        bool[] activeLanes = new bool[2 * hallRadius + 1];
        for (int i = 0; i < activeLanes.Length; i++) {
            activeLanes[i] = true;
        }

        Vector3Int current = start;
        Vector3Int difference = new Vector3Int(end.x - start.x, end.y - start.y);

        int xInc = end.x < start.x ? -1 : 1;
        int yInc = end.y < start.y ? -1 : 1;

        if (xInc < 0) {
            current.x--;
            end.x--;
        } else if (yInc < 0) {
            current.y--;
            end.y--;
        }

        if (direction == Direction.North || direction == Direction.South) {
            //Check Backwards
            //This outer for loop is a safety incase a tile is never hit as well as a way to walk backwards
            for (int i = 0; i < 10; i++) {
                int offset = -hallRadius - 1;

                for (int j = 0; j < activeLanes.Length; j++) {
                    offset++;
                    //Check if one of the lanes collided with the room
                    if (!activeLanes[j]) continue;

                    Vector3Int currentOffset = new Vector3Int(current.x + offset, current.y - yInc * i);
                    Vector3 worldCellPos = hallTilemap.CellToWorld(currentOffset);
                    //IDK WHY THE OFFSET FOR THIS IS DIFFERENT THAN THE OTHER ONE, LOOK AT THIS LATER
                    worldCellPos = new Vector2(worldCellPos.x - .5f + 1, worldCellPos.y + .5f);
                    Collider2D collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);

                    if (collider != null) {
                        if (collider.gameObject.name == "FloorTile") {
                            activeLanes[j] = false;
                            continue;
                        } else {
                            Tilemap collidedTilemap = collider.GetComponent<Tilemap>();
                            collidedTilemap.SetTile(collidedTilemap.WorldToCell(worldCellPos), null);

                            //Do a second check for collision a tile ahead to determine if it should put down a corner
                            worldCellPos = new Vector2(worldCellPos.x, worldCellPos.y - yInc);
                            collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);
                            if (collider != null && collider.name == "FloorTile") {
                                Debug.Log($"Detected Floor tile {worldCellPos}");
                                if (j == 0) {
                                    if (Utilities.GetOppDirection(direction) == Direction.North)
                                        hallTilemap.SetTile(currentOffset, LeftExterior);
                                    else {
                                        hallTilemap.SetTile(currentOffset, topHorizontal);
                                    }

                                } else if (j == activeLanes.Length - 1) {
                                    if (Utilities.GetOppDirection(direction) == Direction.North)
                                        hallTilemap.SetTile(currentOffset, RightExterior);
                                    else {
                                        hallTilemap.SetTile(currentOffset, topHorizontal);
                                    }
                                } else
                                    hallTilemap.SetTile(currentOffset, testHallTile);

                                continue;

                            }

                        }
                    }

                    if (j == 0)
                        hallTilemap.SetTile(currentOffset, leftVertical);
                    else if (j == activeLanes.Length - 1)
                        hallTilemap.SetTile(currentOffset, rightVertical);
                    else
                        hallTilemap.SetTile(currentOffset, testHallTile);

                }
            }

            for (int i = 0; i < activeLanes.Length; i++) {
                activeLanes[i] = true;
            }

            //Check Forwards


            for (; current.y != end.y - difference.y / 2; current.y += yInc) {
                int offset = -hallRadius - 1;
                for (int i = 0; i < activeLanes.Length; i++) {
                    offset++;
                    //Check if one of the lanes collided with the room
                    if (!activeLanes[i]) continue;

                    Vector3Int currentOffset = new Vector3Int(current.x + offset, current.y);

                    if (i == 0)
                        hallTilemap.SetTile(currentOffset, leftVertical);
                    else if (i == activeLanes.Length - 1)
                        hallTilemap.SetTile(currentOffset, rightVertical);
                    else
                        hallTilemap.SetTile(currentOffset, testHallTile);
                }

            }

            for (; current.x != end.x; current.x += xInc) {
                int offset = -hallRadius - 1;
                for (int i = 0; i < activeLanes.Length; i++) {
                    offset++;
                    //Check if one of the lanes collided with the room
                    if (!activeLanes[i]) continue;

                    Vector3Int currentOffset = new Vector3Int(current.x, current.y + offset);
                    if (i == 0)
                        hallTilemap.SetTile(currentOffset, bottomHorizontal);
                    else if (i == activeLanes.Length - 1)
                        hallTilemap.SetTile(currentOffset, topHorizontal);
                    else
                        hallTilemap.SetTile(currentOffset, testHallTile);
                }

            }

            for (; current.y != end.y + (yInc * 5); current.y += yInc) {
                //Minus one to let me add one to it before it checks active lanes so it updates regardless
                int offset = -hallRadius - 1;
                for (int i = 0; i < activeLanes.Length; i++) {
                    offset++;
                    //Check if one of the lanes collided with the room
                    if (!activeLanes[i]) continue;

                    Vector3 worldCellPos = hallTilemap.CellToWorld(new Vector3Int(current.x + offset, current.y));
                    //IDK WHY THE OFFSET FOR THIS IS DIFFERENT THAN THE OTHER ONE, LOOK AT THIS LATER
                    worldCellPos = new Vector2(worldCellPos.x - .5f + 1, worldCellPos.y + .5f);
                    Collider2D collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);

                    Vector3Int currentOffset = new Vector3Int(current.x + offset, current.y);

                    if (collider != null) {
                        if (collider.gameObject.name == "FloorTile") {
                            activeLanes[i] = false;
                            continue;

                        } else {

                            //Sets the ends of the halls with corners
                            Tilemap collidedTilemap = collider.GetComponent<Tilemap>();
                            collidedTilemap.SetTile(collidedTilemap.WorldToCell(worldCellPos), null);

                            //Do a second check for collision a tile ahead to determine if it should put down a corner
                            worldCellPos = new Vector2(worldCellPos.x, worldCellPos.y + yInc);
                            collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);
                            if (collider != null && collider.name == "FloorTile") {
                                Debug.Log($"Detected Floor tile {worldCellPos}");
                                if (i == 0) {
                                    if (direction == Direction.North)
                                        hallTilemap.SetTile(currentOffset, LeftExterior);
                                    else {
                                        hallTilemap.SetTile(currentOffset, topHorizontal);
                                    }

                                } else if (i == activeLanes.Length - 1) {
                                    if (direction == Direction.North)
                                        hallTilemap.SetTile(currentOffset, RightExterior);
                                    else {
                                        hallTilemap.SetTile(currentOffset, topHorizontal);
                                    }
                                } else
                                    hallTilemap.SetTile(currentOffset, testHallTile);

                                continue;

                            }

                        }
                    }
                    if (i == 0)
                        hallTilemap.SetTile(currentOffset, leftVertical);
                    else if (i == activeLanes.Length - 1)
                        hallTilemap.SetTile(currentOffset, rightVertical);
                    else
                        hallTilemap.SetTile(currentOffset, testHallTile);

                }
            }

        }
        if (direction == Direction.East || direction == Direction.West) {
            //Check Backwords
            //This outer for loop is a safety incase a tile is never hit as well as a way to walk backwards
            for (int i = 0; i < 10; i++) {
                int offset = -hallRadius - 1;

                for (int j = 0; j < activeLanes.Length; j++) {
                    offset++;
                    //Check if one of the lanes collided with the room
                    if (!activeLanes[j]) continue;

                    Vector3Int currentOffset = new Vector3Int(current.x - xInc * i, current.y - offset);
                    Vector3 worldCellPos = hallTilemap.CellToWorld(currentOffset);
                    //IDK WHY THE OFFSET FOR THIS IS DIFFERENT THAN THE OTHER ONE, LOOK AT THIS LATER
                    worldCellPos = new Vector2(worldCellPos.x - .5f + 1, worldCellPos.y + .5f);
                    Collider2D collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);

                    if (collider != null) {
                        if (collider.gameObject.name == "FloorTile") {
                            activeLanes[j] = false;
                            continue;
                        } else {
                            Tilemap collidedTilemap = collider.GetComponent<Tilemap>();
                            collidedTilemap.SetTile(collidedTilemap.WorldToCell(worldCellPos), null);

                            //Do a second check for collision a tile ahead to determine if it should put down a corner
                            worldCellPos = new Vector2(worldCellPos.x - xInc, worldCellPos.y);
                            collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);
                            if (collider != null && collider.name == "FloorTile") {
                                Debug.Log($"Detected Floor tile {worldCellPos}");
                                if (j == 0) {
                                    if (Utilities.GetOppDirection(direction) == Direction.East)
                                        hallTilemap.SetTile(currentOffset, topHorizontal);
                                    else {
                                        hallTilemap.SetTile(currentOffset, topHorizontal);
                                    }

                                } else if (j == activeLanes.Length - 1) {
                                    if (Utilities.GetOppDirection(direction) == Direction.East)
                                        hallTilemap.SetTile(currentOffset, LeftExterior);
                                    else {
                                        hallTilemap.SetTile(currentOffset, RightExterior);
                                    }
                                } else
                                    hallTilemap.SetTile(currentOffset, testHallTile);

                                continue;
                            }
                        }
                    }
                    if (j == 0)
                        hallTilemap.SetTile(currentOffset, topHorizontal);
                    else if (j == activeLanes.Length - 1)
                        hallTilemap.SetTile(currentOffset, bottomHorizontal);
                    else
                        hallTilemap.SetTile(currentOffset, testHallTile);
                }
            }

            for (int i = 0; i < activeLanes.Length; i++) {
                activeLanes[i] = true;
            }

            //Check Forwards


            for (; current.x != end.x - difference.x / 2; current.x += xInc) {
                int offset = -hallRadius - 1;
                for (int i = 0; i < activeLanes.Length; i++) {
                    offset++;
                    //Check if one of the lanes collided with the room
                    if (!activeLanes[i]) continue;

                    Vector3Int currentOffset = new Vector3Int(current.x, current.y + offset);
                    if (i == 0)
                        hallTilemap.SetTile(currentOffset, bottomHorizontal);
                    else if (i == activeLanes.Length - 1)
                        hallTilemap.SetTile(currentOffset, topHorizontal);
                    else
                        hallTilemap.SetTile(currentOffset, testHallTile);
                }

            }

            for (; current.y != end.y; current.y += yInc) {
                int offset = -hallRadius - 1;
                for (int i = 0; i < activeLanes.Length; i++) {
                    offset++;
                    //Check if one of the lanes collided with the room
                    if (!activeLanes[i]) continue;

                    Vector3Int currentOffset = new Vector3Int(current.x + offset, current.y);
                    hallTilemap.SetTile(currentOffset, testHallTile);
                }

            }

            //Add xInc * 5 to ensure the path goes as deep as needed into the Room
            for (; current.x != end.x + (xInc * 5); current.x += xInc) {
                //Minus one to let me add one to it before it checks active lanes so it updates regardless
                int offset = -hallRadius - 1;
                for (int i = 0; i < activeLanes.Length; i++) {
                    offset++;
                    //Check if one of the lanes collided with the room
                    if (!activeLanes[i]) continue;

                    Vector3 worldCellPos = hallTilemap.CellToWorld(new Vector3Int(current.x, current.y + offset));
                    //IDK WHY THE OFFSET FOR THIS IS DIFFERENT THAN THE OTHER ONE, LOOK AT THIS LATER
                    worldCellPos = new Vector2(worldCellPos.x - .5f + 1, worldCellPos.y + .5f);
                    Collider2D collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);
                    Vector3Int currentOffset = new Vector3Int(current.x, current.y + offset);
                    if (collider != null) {
                        if (collider.gameObject.name == "FloorTile") {
                            activeLanes[i] = false;

                            continue;
                        } else {
                            Tilemap collidedTilemap = collider.GetComponent<Tilemap>();
                            collidedTilemap.SetTile(collidedTilemap.WorldToCell(worldCellPos), null);

                            //Do a second check for collision a tile ahead to determine if it should put down a corner
                            worldCellPos = new Vector2(worldCellPos.x + xInc, worldCellPos.y);
                            collider = Physics2D.OverlapBox(worldCellPos, new Vector2(0.5f, 0.5f), buildingLayer);
                            if (collider != null && collider.name == "FloorTile") {
                                Debug.Log($"Detected Floor tile {worldCellPos}");
                                if (i == 0) {
                                    if (direction == Direction.East)
                                        hallTilemap.SetTile(currentOffset, LeftExterior);
                                    else {
                                        hallTilemap.SetTile(currentOffset, RightExterior);
                                    }

                                } else if (i == activeLanes.Length - 1) {
                                    if (direction == Direction.East)
                                        hallTilemap.SetTile(currentOffset, topHorizontal);
                                    else {
                                        hallTilemap.SetTile(currentOffset, topHorizontal);
                                    }
                                } else
                                    hallTilemap.SetTile(currentOffset, testHallTile);

                                continue;

                            }
                        }
                    }

                    if (i == 0)
                        hallTilemap.SetTile(currentOffset, bottomHorizontal);
                    else if (i == activeLanes.Length - 1)
                        hallTilemap.SetTile(currentOffset, topHorizontal);
                    else
                        hallTilemap.SetTile(currentOffset, testHallTile);




                }
            }
        }

    }


}



