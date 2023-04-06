using UnityEngine.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Room : MonoBehaviour {

    public static readonly float ChestSpawnRate = .5f;
    public static readonly float EnemySpawnRate = .5f;


    [field: SerializeField] public Tilemap FloorTileSet { get; private set; }

    [SerializeField] bool CompleteRoom;
    [SerializeField] List<GameObject> enemySpawnLocations;

    public event Action<Room> OnRoomCompletion;
    public event Action<NPC> OnNPCFound;
    public event Action<Shrine> OnShrineFound;

    public int RoomLevel { get; private set; }

    List<Enemy> roomEnemies;
    bool playerEnteredRoom;
    int EnemiesLeft;



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

    public void Initialize(int roomLevel, SpawnRates spawnRates) {

        RoomLevel = roomLevel;

        //Spawns the chests for the room
        InitializeChests(spawnRates);


        //Prevents Enemies from spawning in base room
        if (roomLevel == 0) return;

        InitializeEnemies(spawnRates);
        //Spawns the enemies for the room


        InitializeNPCs();

        InitializeShrine();

    }


    void OnTriggerEnter2D(Collider2D collider) {
        PlayerCharacter player = collider.GetComponentInParent<PlayerCharacter>();
        if (player != null && !playerEnteredRoom) {
            playerEnteredRoom = true;
            OnRoomCompletion?.Invoke(this);
            foreach (Enemy enemy in roomEnemies) {
                enemy.enabled = true;
            }

        }
    }

    void InitializeEnemies(SpawnRates spawnRates) {


        foreach (GameObject enemySpawn in enemySpawnLocations) {
            //Decides if the enemy will spawn
            if (UnityEngine.Random.Range(0, 1f) > .5) continue;

            float totalProbability = 0;
            float cumulativeProbablity = 0;


            //Gets the total probability of all enemies in the spawnRates object
            spawnRates.EnemySpawnRates.ForEach(x => totalProbability += x.spawnRateCurve.Evaluate(MathF.Min((float)RoomLevel, DungeonGenerator.MaxRoomScale)));


            float randomValue = UnityEngine.Random.Range(0, totalProbability);

            foreach (SpawnRates.EnemySpawnRate enemySpawnRate in spawnRates.EnemySpawnRates) {
                cumulativeProbablity += enemySpawnRate.spawnRateCurve.Evaluate(MathF.Min((float)RoomLevel, DungeonGenerator.MaxRoomScale));

                //Does this enemy get a chance to spawn
                if (cumulativeProbablity >= randomValue) {
                    //Spawn the enemy
                    Enemy enemy = Instantiate(enemySpawnRate.enemyData.EnemyPrefab, enemySpawn.transform.position, Quaternion.identity);
                    enemy.Initialize(enemySpawnRate.enemyData);
                    enemy.enabled = false;
                    roomEnemies.Add(enemy);
                }

            }
        }
    }

    void InitializeChests(SpawnRates spawnRates) {
        foreach (Chest chest in GetComponentsInChildren<Chest>()) {
            //Decides if the chest will spawn
            if (UnityEngine.Random.Range(0f, 1f) > .5) {
                //If chestSpawned, give it items

                foreach (SpawnRates.ItemSpawnRate itemSpawnRate in spawnRates.ItemSpawnRates) {
                    float chance = itemSpawnRate.spawnRateCurve.Evaluate(MathF.Min((float)RoomLevel, DungeonGenerator.MaxRoomScale));

                    //Does this item get a chance to spawn
                    if (chance > UnityEngine.Random.Range(0f, 1f)) {
                        //Spawn the item
                        chest.AddItem(itemSpawnRate.itemData.CreateItem((float)RoomLevel / DungeonGenerator.MaxRoomScale));
                    }

                }
            } else Destroy(chest.gameObject);
        }
    }

    void InitializeNPCs() {

        //This checks if the npc in a room is already in the hub, if it is, it destroys it
        foreach (NPC npc in FindObjectsOfType<NPC>()) {
            npc.Load();
            if (npc.MazeEncounterComplete) Destroy(npc.gameObject);
            else {
                //Adds a listener that will destroy the npc if it is found and its not the one actually found
                npc.OnFound += (x) => OnNPCFound?.Invoke(x);
            }
        }
    }

    void InitializeShrine() {
        Shrine shrine = GetComponentInChildren<Shrine>();
        if (shrine != null) {
            shrine.OnInteract += (x) => OnShrineFound?.Invoke(x);
        }
    }


}

// Chest tile Link
// https://opengameart.org/content/treasure-chests-32x32
