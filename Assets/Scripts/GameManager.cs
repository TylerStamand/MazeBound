using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GameState {
    Hub,
    Maze
}

class GameSaveData {
    public GameState GameState;

    public int PuzzlePiecesCollected;

    //These should be new game defaults
    public GameSaveData() {

        GameState = GameState.Hub;
        PuzzlePiecesCollected = 0;
    }


}

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public GameState CurrentGameState { get; private set; }
    public int PuzzlePiecesCollected { get; private set; }

    string SaveDataID = "GameSaveData";

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        SaveManager.Instance.OnSave += Save;
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
        SaveManager.Instance.SetData(SaveDataID, gameSaveData);
        SaveManager.Instance.Save();
        SceneManager.LoadSceneAsync("Hub");
        CurrentGameState = GameState.Hub;
    }

    void Save() {
        Debug.Log("Saving Game Manager");
        GameSaveData gameSaveData = new GameSaveData() {
            GameState = CurrentGameState,
            PuzzlePiecesCollected = PuzzlePiecesCollected
        };
        SaveManager.Instance.SetData(SaveDataID, gameSaveData);
    }


}
