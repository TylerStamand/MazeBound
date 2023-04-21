using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using NaughtyAttributes;

public enum GameState {
    Hub,
    Maze,
    Fade,
    Boss
}

class GameSaveData {
    public bool[] PuzzlePiecesCollected;
    public bool IsGuy;
    public bool IntroCompleted;

    //These should be new game defaults
    public GameSaveData() {
        PuzzlePiecesCollected = new bool[3];
        IsGuy = true;
        IntroCompleted = false;
    }


}

public class GameManager : MonoBehaviour {

    [SerializeField] bool overridePuzzlePieceCount = false;
    [SerializeField] int puzzlePieceOverride = -1;


    [Header("Maze Scenes")]
    [Scene]
    [SerializeField] string maze1;
    [Scene]
    [SerializeField] string maze2;
    [Scene]
    [SerializeField] string maze3;

    [Header("Hub Scenes")]
    [Scene]
    [SerializeField] string hub0;
    [Scene]
    [SerializeField] string hub1;
    [Scene]
    [SerializeField] string hub2;
    [Scene]
    [SerializeField] string hub3;

    [Scene]
    [SerializeField] string bossScene;



    public static GameManager Instance { get; private set; }

    public event Action OnSceneChange;
    public event Action<float> OnVolumeChange;


    public GameState CurrentGameState { get; private set; }
    public bool[] PuzzlePiecesCollected { get; private set; } = new bool[3];
    public bool IsGuy { get; private set; } = true;
    public bool IntroCompleted { get; private set; } = false;



    public int PuzzlePiecesCollectedCount {
        get {

            if (overridePuzzlePieceCount) return puzzlePieceOverride;

            int count = 0;
            foreach (bool b in PuzzlePiecesCollected) {
                if (b) count++;
            }
            return count;
        }
    }

    string SaveDataID = "GameSaveData";


    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }



    }


    void Start() {
        LoadMenuManager loadMenu = Instantiate(ResourceManager.Instance.LoadMenuPrefab).GetComponent<LoadMenuManager>();
        loadMenu.OnLoadPressed += () => { Destroy(loadMenu.gameObject); LoadGame(); };
        loadMenu.OnNewPressed += () => { Destroy(loadMenu.gameObject); NewGame(); };

    }

    void LoadGame() {
        Debug.Log("Load Game");
        bool dataFound = SaveManager.Instance.Load();
        if (!dataFound) {
            Debug.Log("No save data found");
            Debug.Log("Starting new game");
            NewGame();
            return;
        }

        GameSaveData gameSaveData = SaveManager.Instance.GetData<GameSaveData>(SaveDataID);
        if (gameSaveData == null) {
            Debug.Log("No game state save data found");
            Debug.Log("Starting new game");
            NewGame();
        }


        PuzzlePiecesCollected = gameSaveData.PuzzlePiecesCollected;
        IsGuy = gameSaveData.IsGuy;
        IntroCompleted = gameSaveData.IntroCompleted;
        Debug.Log("Current Puzzle Pieces Collected: " + PuzzlePiecesCollectedCount);

        LoadHub();


    }

    void NewGame() {
        Debug.Log("New Game");
        GameSaveData gameSaveData = new GameSaveData();

        //This wipes the file and saves a new gamemanager save data
        SaveManager.Instance.SetData(SaveDataID, gameSaveData);
        SaveManager.Instance.Save();
        LoadCharacterSelect();
    }

    void Save() {
        Debug.Log("Saving Game Manager");
        GameSaveData gameSaveData = new GameSaveData() {
            PuzzlePiecesCollected = PuzzlePiecesCollected,
            IsGuy = IsGuy
        };
        SaveManager.Instance.SetData(SaveDataID, gameSaveData);
    }

    public void Quit() {
        Debug.Log("Quitting Game");
        Save();
        SaveManager.Instance.Save();
        Application.Quit();
    }

    public void LoadCharacterSelect() {
        CharacterSelectMenu charSelect = Instantiate(ResourceManager.Instance.CharacterSelectPrefab).GetComponent<CharacterSelectMenu>();
        charSelect.OnCharacterSelect += (bool isGuy) => {
            this.IsGuy = isGuy;
            LoadHub();
            Destroy(charSelect.gameObject);
        };
    }

    public void LoadHub() {




        string sceneName;
        switch (PuzzlePiecesCollectedCount) {
            case -1:
                sceneName = hub0;
                break;
            case 0:
                sceneName = hub1;
                break;
            case 1:
                sceneName = hub2;
                break;
            case 2:
                sceneName = hub3;
                break;
            default:
                sceneName = hub3;
                break;
        }
        Debug.Log("Loading Hub Scene: " + sceneName);
        OnSceneChange?.Invoke();
        SceneManager.LoadSceneAsync(sceneName).completed += (AsyncOperation obj) => {
            CurrentGameState = GameState.Hub;
        };


    }

    public void LoadMaze(int mazeIndex) {

        string sceneName;
        switch (mazeIndex) {
            case 1:
                sceneName = maze1;
                break;
            case 2:
                sceneName = maze2;
                break;
            case 3:
                sceneName = maze3;
                break;
            default:
                Debug.LogError("Invalid maze index");
                return;
        }

        OnSceneChange?.Invoke();
        SceneManager.LoadSceneAsync(sceneName).completed += (AsyncOperation obj) => {
            CurrentGameState = GameState.Maze;
        };
    }

    public void LoadBoss() {

        if (PuzzlePiecesCollectedCount != 3) {
            Debug.LogError("Cannot load boss scene without all puzzle pieces");
            return;
        }

        OnSceneChange?.Invoke();

        SceneFader fader = Instantiate(ResourceManager.Instance.FaderPrefab).GetComponent<SceneFader>();
        Debug.Log("Setting Fader");
        StartCoroutine(fader.FadeAndLoadScene(bossScene));
        fader.OnFadeComplete += () => {
            CurrentGameState = GameState.Boss;
        };
        CurrentGameState = GameState.Fade;
    }

    public void SetPuzzlePieceCollected(int level) {
        Debug.Log("Puzzle Piece Collected " + level);
        PuzzlePiecesCollected[level - 1] = true;
    }


    public void SetVolume(float volume) {
        PlayerPrefs.SetFloat("Volume", volume);
        OnVolumeChange?.Invoke(volume);
    }

    public float GetVolume() {
        return PlayerPrefs.GetFloat("Volume", .5f);
    }

    public void SetCharacter(bool isGuy) {
        this.IsGuy = isGuy;
    }

    public void SetIntroCompleted() {
        IntroCompleted = true;
    }


}
