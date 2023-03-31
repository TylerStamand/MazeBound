using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum GameState {
    Hub,
    Maze
}

class GameSaveData {
    public GameState GameState;

    public bool[] PuzzlePiecesCollected = new bool[3];


    //These should be new game defaults
    public GameSaveData() {

        GameState = GameState.Hub;
        PuzzlePiecesCollected = new bool[3];
    }


}

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public event Action OnSceneChange;

    public GameState CurrentGameState { get; private set; }
    public bool[] PuzzlePiecesCollected { get; private set; }

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

        SceneManager.LoadSceneAsync("Hub");
        CurrentGameState = GameState.Hub;


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
            GameState = CurrentGameState,
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

        OnSceneChange?.Invoke();
        SceneManager.LoadSceneAsync("Hub").completed += (AsyncOperation obj) => {
            CurrentGameState = GameState.Hub;
        };


    }

    public void LoadMaze(int mazeIndex) {

        OnSceneChange?.Invoke();
        SceneManager.LoadSceneAsync("Prototype").completed += (AsyncOperation obj) => {
            CurrentGameState = GameState.Maze;
        };
    }

    public void SetPuzzlePieceCollected(int index) {
        PuzzlePiecesCollected[index] = true;
    }


}
