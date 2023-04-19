using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class Boss : MonoBehaviour {


    [SerializeField] List<Tentacle> tentacles;


    [Header("Boss Dialog")]
    [SerializeField] Dialog bossIntroDialog;
    [SerializeField] Dialog bossDefeatedDialog;

    [Header("Boss Settings")]
    [SerializeField] int tentacleHealth;
    [SerializeField] int tileAttackDamage;
    [SerializeField] int wallAttackDamage;
    [SerializeField] int timeBetweenAttackSwitch;
    [SerializeField] int timeForTileAttack;
    [SerializeField] SpawnRates spawnRates;


    [Header("Tile Attack")]
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap floorTilemapPrefab;
    [SerializeField] Sprite emptyTileSprite;
    [SerializeField] ThrowTile throwTilePrefab;
    [SerializeField] float tileSpeed;

    [Header("Transforming")]
    [SerializeField] Image screenFlashImage;
    [SerializeField] float transformTime;
    [SerializeField] Sprite bossSprite;
    [SerializeField] Sprite guideSprite;
    [SerializeField] Vector2 bossStartPosition;
    [SerializeField] Vector2 bossEndPosition;

    [Header("Walls")]
    [SerializeField] BossWall topRightWall;
    [SerializeField] BossWall topLeftWall;
    [SerializeField] BossWall bottomLeftWall;
    [SerializeField] BossWall bottomRightWall;


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


    PlayerCharacter playerCharacter;

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
        playerCharacter = FindObjectOfType<PlayerCharacter>();

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

    void Update() {
        //Move left and right DOTWEEN
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOMove(new Vector3(bossStartPosition.x - 2, bossStartPosition.y), 2f));
        mySequence.Append(transform.DOMove(new Vector3(bossStartPosition.x + 2, bossStartPosition.y), 2f));
        mySequence.SetLoops(-1, LoopType.Yoyo);
        mySequence.Play();

    }


    public void StartBossFight() {
        //Dialog
        DialogManager dialogManager = ShowDialog(bossIntroDialog, playerCharacter);
        if (dialogManager != null) {
            dialogManager.OnDialogComplete += (x) => {

                StartCoroutine(Transform(bossSprite));
                StartCoroutine(BossFight());
            };
        }
    }

    IEnumerator BossFight() {
        yield return StartCoroutine(MoveToStartPosition());
        yield return StartCoroutine(AttackCycle());
    }

    IEnumerator MoveToStartPosition() {
        while ((Vector2)transform.position != bossStartPosition) {
            transform.position = Vector3.MoveTowards(transform.position, bossStartPosition, 1f * Time.deltaTime);
            yield return null;
        }
    }


    IEnumerator AttackCycle() {
        while (true) {

            //Enemy Attack
            enemyAttack = true;
            StartCoroutine(EnemyAttack(phase));
            while (enemyAttack) {
                yield return null;
            }

            //Wall Attack
            wallAttack = true;
            StartCoroutine(WallAttack(phase));
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


        while (!topLeftWall.Completed || !topRightWall.Completed || !bottomLeftWall.Completed || !bottomRightWall.Completed)
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
        throwTile.Initialize(tileAttackDamage, tileSpeedList[phase], GetRandomTileSprite());
        return throwTile;
    }

    void HandleEnemyDeath(Enemy enemy) {

        enemies.Remove(enemy);

    }

    void HandleTentacleDeath() {
        phase++;
        if (phase == 3) {
            Die();
        }
    }


    IEnumerator Die() {

        StopAllCoroutines();

        //Reset everything boss has done
        FindObjectsOfType<Enemy>().ToList().ForEach(e => Destroy(e));
        FindObjectsOfType<ThrowTile>().ToList().ForEach(e => Destroy(e));
        topRightWall.ResetToStartPosition();
        topLeftWall.ResetToStartPosition();
        bottomLeftWall.ResetToStartPosition();
        bottomRightWall.ResetToStartPosition();

        yield return new WaitForSeconds(3f);

        Transform(guideSprite);

    }


    IEnumerator Transform(Sprite sprite) {
        float startTime = Time.time;
        while (Time.time - startTime < transformTime) {
            float t = (Time.time - startTime) / transformTime;
            screenFlashImage.color = new Color(1, 1, 1, t);
            yield return null;
        }
        screenFlashImage.color = new Color(1, 1, 1, 1);



        GetComponent<SpriteRenderer>().sprite = sprite;

        startTime = Time.time;
        while (Time.time - startTime < transformTime) {
            float t = (Time.time - startTime) / transformTime;
            screenFlashImage.color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }
        screenFlashImage.color = new Color(1, 1, 1, 0);

    }

    Vector3Int GetRandomTilePosition() {
        BoundsInt bounds = floorTilemap.cellBounds;
        int randomX = Random.Range(bounds.min.x, bounds.max.x);
        int randomY = Random.Range(bounds.min.y, bounds.max.y);
        int randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3Int(randomX, randomY, randomZ);
    }

    Sprite GetRandomTileSprite() {
        Vector3Int randomTilePosition = GetRandomTilePosition();
        Tile tile = floorTilemap.GetTile<Tile>(randomTilePosition);
        Sprite tileSprite = tile.sprite;
        // tile.sprite = emptyTileSprite;
        // floorTilemap.RefreshTile(randomTilePosition);
        return tileSprite;

    }


    DialogManager ShowDialog(Dialog dialog, PlayerCharacter playerCharacter) {
        DialogManager dialogManager = playerCharacter.ShowMenu(ResourceManager.Instance.DialogManagerPrefab, false)?.GetComponent<DialogManager>();
        if (dialogManager == null) return null;
        dialogManager.SetDialog(dialog, "Guide");
        dialogManager.OnDialogComplete += (x) => {
            Destroy(dialogManager.gameObject);
            playerCharacter.ExitMenu();
        };
        return dialogManager;
    }


}
