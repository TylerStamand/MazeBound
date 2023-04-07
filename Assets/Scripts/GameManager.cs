using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using NaughtyAttributes;

public enum GameState {
    Hub,
    Maze
}

class GameSaveData {
    public bool[] PuzzlePiecesCollected = new bool[3];


    //These should be new game defaults
    public GameSaveData() {
        PuzzlePiecesCollected = new bool[3];
    }


}

public class GameManager : MonoBehaviour {

    public static float Volume = .5f;

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



    public static GameManager Instance { get; private set; }

    public event Action OnSceneChange;

    public GameState CurrentGameState { get; private set; }
    public bool[] PuzzlePiecesCollected { get; private set; } = new bool[3];

    public int PuzzlePiecesCollectedCount {
        get {
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
        loadMenu.OnLoadPressed += LoadGame;
        loadMenu.OnNewPressed += NewGame;
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
        Debug.Log("Current Puzzle Pieces Collected: " + PuzzlePiecesCollectedCount);

        LoadHub();


    }

    void NewGame() {
        Debug.Log("New Game");
        GameSaveData gameSaveData = new GameSaveData();

        //This wipes the file and saves a new gamemanager save data
        SaveManager.Instance.SetData(SaveDataID, gameSaveData);
        SaveManager.Instance.Save();
        LoadHub();
    }

    void Save() {
        Debug.Log("Saving Game Manager");
        GameSaveData gameSaveData = new GameSaveData() {
            PuzzlePiecesCollected = PuzzlePiecesCollected
        };
        SaveManager.Instance.SetData(SaveDataID, gameSaveData);
    }

    public void Quit() {
        Debug.Log("Quitting Game");
        Save();
        SaveManager.Instance.Save();
        Application.Quit();
    }

    public void LoadHub() {

        string sceneName;
        switch (PuzzlePiecesCollectedCount) {
            case 0:
                sceneName = hub0;
                break;
            case 1:
                sceneName = hub1;
                break;
            case 2:
                sceneName = hub2;
                break;
            case 3:
                sceneName = hub3;
                break;
            default:
                Debug.LogError("Invalid hub index");
                return;
        }
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

    public void SetPuzzlePieceCollected(int level) {
        Debug.Log("Puzzle Piece Collected " + level);
        PuzzlePiecesCollected[level - 1] = true;
    }


    public void SetVolume(float volume) {
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public float GetVolume() {
        return PlayerPrefs.GetFloat("Volume", .5f);
    }


}
