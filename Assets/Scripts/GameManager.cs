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

    int PuzzlePiecesCollected;

    //These should be new game defaults
    public GameSaveData() {

        GameState = GameState.Hub;
        PuzzlePiecesCollected = 0;
    }


}

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public GameState CurrentGameState { get; private set; }


    GameSaveData gameSaveData;

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

        gameSaveData = SaveManager.Instance.GetData<GameSaveData>("GameSaveData");
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
        gameSaveData = new GameSaveData();
        SaveManager.Instance.SetData("GameSaveData", gameSaveData);
        SaveManager.Instance.Save();
        SceneManager.LoadSceneAsync("Hub");
        CurrentGameState = GameState.Hub;
    }


}
