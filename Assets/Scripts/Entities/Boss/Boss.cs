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
    [SerializeField] AudioClip bossDie;

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

    [Header("Transforming")]
    [SerializeField] float moveSpeed;
    [SerializeField] float hightForShadow;
    [SerializeField] GameObject shadow;
    [SerializeField] Sprite bossSprite;
    [SerializeField] Sprite guideSprite;
    [SerializeField] GameObject bossStartPosition;

    [Header("Walls")]
    [SerializeField] BossWall topRightWall;
    [SerializeField] BossWall topLeftWall;
    [SerializeField] BossWall bottomLeftWall;
    [SerializeField] BossWall bottomRightWall;


    [Header("Phase 1")]
    [Label("Time Between Tile Attacks")][SerializeField] int timeBetweenTileAttacks1;
    [Label("Tile Speed")][SerializeField] float tileSpeed1;
    [Label("Number Of Enemies")][SerializeField] int numberOfEnemies1;
    [Label("Speed Of Walls")][SerializeField] float wallSpeed1;

    [Header("Phase 2")]
    [Label("Time Between Tile Attacks")][SerializeField] int timeBetweenTileAttacks2;
    [Label("Tile Speed")][SerializeField] float tileSpeed2;
    [Label("Number Of Enemies")][SerializeField] int numberOfEnemies2;
    [Label("Speed Of Walls")][SerializeField] float wallSpeed2;

    [Header("Phase 3")]
    [Label("Time Between Tile Attacks")][SerializeField] int timeBetweenTileAttacks3;
    [Label("Tile Speed")][SerializeField] float tileSpeed3;
    [Label("Number Of Enemies")][SerializeField] int numberOfEnemies3;
    [Label("Speed Of Walls")][SerializeField] float wallSpeed3;


    PlayerCharacter playerCharacter;
    Animator animator;

    bool tileAttack = false;
    bool wallAttack = false;
    bool enemyAttack = false;

    List<GameObject> enemySpawns;
    List<int> timeBetweenTileAttackList = new List<int>();
    List<float> tileSpeedList = new List<float>();
    List<float> wallSpeedList = new List<float>();
    List<int> numberOfEnemiesList = new List<int>();

    List<Enemy> enemies = new List<Enemy>();

    int phase = 0;
    bool dead;

    Sequence hoverSequence;

    void Awake() {
        hoverSequence = DOTween.Sequence();
        hoverSequence.Append(transform.DOMove(new Vector3(bossStartPosition.transform.position.x - 2, bossStartPosition.transform.position.y), 2f));
        hoverSequence.Append(transform.DOMove(new Vector3(bossStartPosition.transform.position.x + 2, bossStartPosition.transform.position.y), 2f));
        hoverSequence.SetLoops(-1, LoopType.Yoyo);
        hoverSequence.Pause();

        animator = GetComponent<Animator>();
        animator.enabled = false;

        shadow.SetActive(false);
        shadow.transform.localPosition = Vector3.zero;


        dead = false;
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

        numberOfEnemiesList.Add(numberOfEnemies1);
        numberOfEnemiesList.Add(numberOfEnemies2);
        numberOfEnemiesList.Add(numberOfEnemies3);
    }
    void Start() {
        foreach (Tentacle tentacle in tentacles) {
            tentacle.Initialize(tentacleHealth);
            tentacle.gameObject.SetActive(false);
            tentacle.OnDeath += HandleTentacleDeath;
            EnemyManager.Instance.AddEnemy(tentacle);

        }

        playerCharacter = FindObjectOfType<PlayerCharacter>();
    }



    public void StartBossFight() {
        //Dialog
        Debug.Log("Starting boss dialog");
        DialogManager dialogManager = ShowDialog(bossIntroDialog, playerCharacter);
        if (dialogManager != null) {
            dialogManager.OnDialogComplete += (x) => {
                StartCoroutine(BossFight());
            };
        }
    }

    IEnumerator BossFight() {
        Debug.Log("Starting boss fight");
        yield return StartCoroutine(Transform(bossSprite));
        yield return StartCoroutine(MoveUp());
        yield return StartCoroutine(AttackCycle());
    }


    IEnumerator MoveUp() {
        shadow.SetActive(true);
        Debug.Log("Moving to start position");
        transform.DOMove(bossStartPosition.transform.position, moveSpeed);
        shadow.transform.DOLocalMove(new Vector3(0, -hightForShadow, 1), moveSpeed);
        while ((Vector2)transform.position != (Vector2)bossStartPosition.transform.position) {

            yield return null;
        }

        hoverSequence.Play();
        Debug.Log("Done moving to start position");
    }


    IEnumerator MoveDown() {
        Debug.Log("Moving to end position");
        hoverSequence.Kill();
        shadow.transform.DOLocalMove(new Vector3(0, 0, 1), moveSpeed);
        while (shadow.transform.localPosition.y != 0) {
            yield return null;
        }
        shadow.SetActive(false);
        Debug.Log("Done moving to end position");
    }



    IEnumerator AttackCycle() {
        while (true) {

            yield return new WaitForSeconds(timeBetweenAttackSwitch);
            //Deactivate tentacles
            foreach (Tentacle tentacle in tentacles) {
                tentacle.gameObject.SetActive(false);
            }

            Debug.Log("Starting attack cycle");

            yield return new WaitForSeconds(timeBetweenAttackSwitch);
            //Enemy Attack
            enemyAttack = true;
            StartCoroutine(EnemyAttack(phase));
            while (enemyAttack) {
                yield return null;
            }

            // yield return new WaitForSeconds(timeBetweenAttackSwitch);

            // //Wall Attack
            // wallAttack = true;
            // StartCoroutine(WallAttack(phase));
            // while (wallAttack) {
            //     yield return null;
            // }

            yield return new WaitForSeconds(timeBetweenAttackSwitch);

            //Activate tentacles
            foreach (Tentacle tentacle in tentacles) {
                tentacle.gameObject.SetActive(true);
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
        Debug.Log("Starting tile attack");



        float startTime = Time.time;
        float lastThrowTime = 0;
        ThrowTile lastTile = null;
        while (Time.time - startTime < timeForTileAttack) {
            Debug.Log("Dead: " + dead);
            if (dead) yield break;

            if (Time.time - lastThrowTime >= timeBetweenTileAttackList[phase]) {
                lastThrowTime = Time.time;
                lastTile = CreateTile();
                lastTile.OnDestroyed += () => {
                    Destroy(lastTile.gameObject);
                    lastTile = null;
                };
            }
            yield return null;
        }

        while (lastTile != null) {
            yield return null;

        }



        tileAttack = false;


    }

    IEnumerator WallAttack(int phase) {
        Debug.Log("Starting wall attack");
        wallAttack = true;

        StartCoroutine(topLeftWall.CloseWall(wallSpeedList[phase]));
        StartCoroutine(topRightWall.CloseWall(wallSpeedList[phase]));
        StartCoroutine(bottomLeftWall.CloseWall(wallSpeedList[phase]));
        StartCoroutine(bottomRightWall.CloseWall(wallSpeedList[phase]));


        while (!topLeftWall.Completed || !topRightWall.Completed || !bottomLeftWall.Completed || !bottomRightWall.Completed)
            yield return null;

        wallAttack = false;
    }

    IEnumerator EnemyAttack(int phase) {
        Debug.Log("Starting enemy attack");
        enemyAttack = true;
        enemies.Clear();
        List<GameObject> spawns = new List<GameObject>(enemySpawns);
        for (int i = 0; i < numberOfEnemiesList[phase]; i++) {
            int index = Random.Range(0, spawns.Count);
            GameObject spawn = spawns[index];
            spawns.RemoveAt(index);
            int enemyIndex = Random.Range(0, spawnRates.EnemySpawnRates.Count);
            SpawnRates.EnemySpawnRate enemySpawnRate = spawnRates.EnemySpawnRates[enemyIndex];
            Enemy enemy = Instantiate(enemySpawnRate.enemyData.EnemyPrefab, spawn.transform.position, Quaternion.identity);
            enemy.Initialize(enemySpawnRate.enemyData);
            enemy.OnDeath += HandleEnemyDeath;
            enemies.Add(enemy);
            EnemyManager.Instance.AddEnemy(enemy);
        }

        while (enemies.Count > 0) {
            yield return null;
        }
        enemyAttack = false;
    }

    ThrowTile CreateTile() {

        //Add pulling tile from tilemap and adding the empty collider tile
        ThrowTile throwTile = Instantiate(throwTilePrefab, transform.position, Quaternion.identity);
        throwTile.Initialize(tileSpeedList[phase], tileAttackDamage, GetRandomTileSprite());
        return throwTile;
    }

    void HandleEnemyDeath(IDamageable enemy) {

        if (enemy is Enemy e)
            enemies.Remove(e);

    }

    void HandleTentacleDeath(IDamageable tentacle) {

        phase++;
        if (phase == 3) {
            StartCoroutine(Die());
        } else {
            StopAllCoroutines();
            StartCoroutine(AttackCycle());
        }
    }


    IEnumerator Die() {
        dead = true;

        //Reset everything boss has done
        FindObjectsOfType<Enemy>().ToList().ForEach(e => Destroy(e));
        FindObjectsOfType<ThrowTile>().ToList().ForEach(e => Destroy(e));
        topRightWall.ResetToStartPosition();
        topLeftWall.ResetToStartPosition();
        bottomLeftWall.ResetToStartPosition();
        bottomRightWall.ResetToStartPosition();

        if (bossDie != null)
            AudioSource.PlayClipAtPoint(bossDie, transform.position, GameManager.Instance.GetVolume());


        yield return new WaitForSeconds(3f);

        yield return StartCoroutine(MoveDown());

        yield return Transform(guideSprite);

        DialogManager dialogManager = ShowDialog(bossDefeatedDialog, playerCharacter);
        dialogManager.OnDialogComplete += (x) => {
            StartCoroutine(BossManager.Instance.HandleBossFightOver());
        };

    }


    IEnumerator Transform(Sprite sprite) {
        SceneFader fader = Instantiate(ResourceManager.Instance.FaderPrefab).GetComponent<SceneFader>();
        yield return StartCoroutine(fader.Fade(SceneFader.FadeDirection.In));

        animator.enabled = !animator.enabled;
        GetComponent<SpriteRenderer>().sprite = sprite;

        yield return StartCoroutine(fader.Fade(SceneFader.FadeDirection.Out));
        Destroy(fader.gameObject);
        yield return new WaitForSeconds(3f);
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
