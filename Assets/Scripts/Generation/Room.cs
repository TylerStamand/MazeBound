using UnityEngine.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Room : MonoBehaviour {

    [field: SerializeField] public RoomRarity Rarity { get; private set; }
    [field: SerializeField] public Tilemap FloorTileSet { get; private set; }

    [SerializeField] bool CompleteRoom;
    [SerializeField] bool TrapRoom;
    [SerializeField] List<GameObject> enemySpawnLocations;

    public event Action<Room> OnRoomCompletion;

    public int RoomLevel { get; private set; }

    List<Enemy> roomEnemies;
    bool playerEnteredRoom;
    int EnemiesLeft;

    public static Vector2 GetRoomOffset(Vector2 a, Vector2 b) {
        return new Vector2(a.x - b.x, a.y - b.y);
    }

    void Awake() {
        roomEnemies = new List<Enemy>();
        CompleteRoom = false;

    }

    void Update() {
        if (CompleteRoom) {
            OnRoomCompletion?.Invoke(this);
            CompleteRoom = false;
        }
    }

    public void Initialize(int roomLevel) {
        RoomLevel = roomLevel;
        List<Enemy> enemyPrefabs = ResourceManager.Instance.GetEnemies();
        foreach (GameObject enemySpawn in enemySpawnLocations) {
            Enemy enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
            Enemy enemy = Instantiate(enemyPrefab, enemySpawn.transform.position, Quaternion.identity);
            enemy.enabled = false;
            roomEnemies.Add(enemy);
        }



    }



    void OnTriggerEnter2D(Collider2D collider) {
        PlayerCharacter player = collider.GetComponent<PlayerCharacter>();
        if (player != null && !playerEnteredRoom) {
            playerEnteredRoom = true;
            if (!TrapRoom) {
                OnRoomCompletion?.Invoke(this);
            }
            foreach (Enemy enemy in roomEnemies) {
                enemy.enabled = true;
            }

        }
    }






}

// Chest tile Link
// https://opengameart.org/content/treasure-chests-32x32
