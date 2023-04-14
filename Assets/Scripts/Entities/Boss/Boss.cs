using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;
using System.Collections;

public class Boss : MonoBehaviour {
    [SerializeField] List<Tentacle> tentacles;
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] ThrowTile throwTilePrefab;
    [SerializeField] SpawnRates spawnRates;

    [Header("Walls")]
    [SerializeField] BossWall topLeftWall;
    [SerializeField] BossWall topRightWall;
    [SerializeField] BossWall bottomLeftWall;
    [SerializeField] BossWall bottomRightWall;

    [Header("Boss Settings")]
    [SerializeField] int tentacleHealth;
    [SerializeField] int tileAttackDamage;
    [SerializeField] int wallAttackDamage;
    [SerializeField] int timeBetweenAttackSwitch;
    [SerializeField] int timeForTileAttack;


    [Header("Phase 1")]
    [Label("Time Between Tile Attacks")][SerializeField] int timeBetweenTileAttacks1;
    [Label("Tile Speed")][SerializeField] int tileSpeed1;
    [Label("Number Of Enemies")][SerializeField] int numberOfEnemies1;
    [Label("Speed Of Walls")][SerializeField] int wallSpeed1;

    [Header("Phase 2")]
    [Label("Time Between Tile Attacks")][SerializeField] int timeBetweenTileAttacks2;
    [Label("Tile Speed")][SerializeField] int tileSpeed2;
    [Label("Number Of Enemies")][SerializeField] int numberOfEnemies2;
    [Label("Speed Of Walls")][SerializeField] int wallSpeed2;

    [Header("Phase 3")]
    [Label("Time Between Tile Attacks")][SerializeField] int timeBetweenTileAttacks3;
    [Label("Tile Speed")][SerializeField] int tileSpeed3;
    [Label("Number Of Enemies")][SerializeField] int numberOfEnemies3;
    [Label("Speed Of Walls")][SerializeField] int wallSpeed3;



    bool tileAttack = false;
    bool wallAttack = false;
    bool enemyAttack = false;

    List<GameObject> enemySpawns;
    List<int> timeBetweenTileAttackList = new List<int>();
    List<int> tileSpeedList = new List<int>();
    List<int> wallSpeedList = new List<int>();

    List<Enemy> enemies = new List<Enemy>();

    int phase = 0;

    void Awake() {
        foreach (Tentacle tentacle in tentacles) {
            tentacle.CurrentHealth = tentacleHealth;
            tentacle.enabled = false;
            tentacle.OnDeath += HandleTentacleDeath;
        }

        enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn").ToList();

        //Initialize phase variables
        timeBetweenTileAttackList.Add(timeBetweenTileAttacks1);
        timeBetweenTileAttackList.Add(timeBetweenTileAttacks2);
        timeBetweenTileAttackList.Add(timeBetweenTileAttacks3);

        tileSpeedList.Add(tileSpeed1);
        tileSpeedList.Add(tileSpeed2);
        tileSpeedList.Add(tileSpeed3);

        wallSpeedList.Add(wallSpeed1);
        wallSpeedList.Add(wallSpeed2);
        wallSpeedList.Add(wallSpeed3);

    }


    IEnumerator AttackCycle() {
        while (true) {

            //Enemy Attack
            enemyAttack = true;
            EnemyAttack(phase);
            while (enemyAttack) {
                yield return null;
            }

            //Wall Attack
            wallAttack = true;
            WallAttack(phase);
            while (wallAttack) {
                yield return null;
            }

            //Tile Attack
            tileAttack = true;
            StartCoroutine(TileAttack(phase));
            while (tileAttack) {
                yield return null;
            }
        }
    }


    void HandleTentacleDeath() {
        phase++;
        if (phase == 3) {
            //Do something
        }
    }

    IEnumerator TileAttack(int phase) {
        int startTime = (int)Time.time;
        ThrowTile lastTile = null;
        while (Time.time - startTime < timeForTileAttack) {
            if (Time.time - startTime % timeBetweenTileAttackList[phase] == 0) {
                lastTile = CreateTile();
            }
            yield return null;
        }
        lastTile.OnDestroyed += () => tileAttack = false;

    }

    IEnumerator WallAttack(int phase) {
        wallAttack = true;

        topLeftWall.CloseWall(wallSpeedList[phase]);
        topRightWall.CloseWall(wallSpeedList[phase]);
        bottomLeftWall.CloseWall(wallSpeedList[phase]);
        bottomRightWall.CloseWall(wallSpeedList[phase]);


        while (topLeftWall.Completed || topRightWall.Completed || bottomLeftWall.Completed || bottomRightWall.Completed)
            yield return null;

        wallAttack = false;
    }

    IEnumerator EnemyAttack(int phase) {
        enemyAttack = true;
        enemies.Clear();
        List<GameObject> spawns = new List<GameObject>(enemySpawns);
        for (int i = 0; i < numberOfEnemies1; i++) {
            int index = Random.Range(0, spawns.Count);
            GameObject spawn = spawns[index];
            spawns.RemoveAt(index);
            int enemyIndex = Random.Range(0, spawnRates.EnemySpawnRates.Count);
            SpawnRates.EnemySpawnRate enemySpawnRate = spawnRates.EnemySpawnRates[enemyIndex];
            Enemy enemy = Instantiate(enemySpawnRate.enemyData.EnemyPrefab, spawn.transform.position, Quaternion.identity);
            enemy.Initialize(enemySpawnRate.enemyData);
            enemy.OnDie += HandleEnemyDeath;
            enemies.Add(enemy);
        }

        while (enemies.Count > 0) {
            yield return null;
        }
        enemyAttack = false;
    }

    ThrowTile CreateTile() {

        //Add pulling tile from tilemap and adding the empty collider tile

        ThrowTile throwTile = Instantiate(throwTilePrefab, transform.position, Quaternion.identity);
        throwTile.Initialize(tileAttackDamage, tileSpeedList[phase]);
        return throwTile;
    }

    void HandleEnemyDeath(Enemy enemy) {

        enemies.Remove(enemy);

    }
}
