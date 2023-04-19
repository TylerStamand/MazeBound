using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DungeonManager : MonoBehaviour {
    public static DungeonManager Instance { get; private set; }

    int shrinesFound = 0;

    PlayerCharacter player;

    void Awake() {
        //Singleton
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnSceneChange += Save;
        else {
            Debug.LogError("GameManager not found");
        }


        DungeonGenerator.Instance.OnNPCFound += HandleNPCFound;
        DungeonGenerator.Instance.OnShrineFound += HandleShrineFound;
        FindObjectsOfType<PlayerCharacter>().ToList().ForEach(x => Destroy(x.gameObject));

    }

    void HandleShrineFound(int mazeLevel, Shrine shrine) {
        shrinesFound++;
        if (shrinesFound == 3) {
            if (GameManager.Instance == null) {
                Debug.LogError("GameManager not found");
                return;
            }
            GameManager.Instance.SetPuzzlePieceCollected(mazeLevel);
            shrine.ShowPuzzlePiece(mazeLevel);

        }
    }

    void OnDestroy() {
        if (GameManager.Instance != null)
            GameManager.Instance.OnSceneChange -= Save;
    }



    void Start() {
        //This gets the dungeon generator that is in the scene through its static instance
        //Maybe change to calling DungeonGenerator.Initialize() instead in this class to eventually set parameters 
        player = Instantiate(ResourceManager.Instance.PlayerPrefab, DungeonGenerator.Instance.SpawnPoint, Quaternion.identity).GetComponent<PlayerCharacter>();
        Load();

    }

    public void Load() {
        Debug.Log("Loading Maze");
        player.Load();
    }

    public void Save() {
        Debug.Log("Saving Maze");
        List<ISaveLoad> saveLoadObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveLoad>().ToList();
        foreach (ISaveLoad saveLoadObject in saveLoadObjects) {
            saveLoadObject.Save();
        }
    }

    void HandleNPCFound(NPC npc) {
        FindObjectsOfType<NPC>().ToList().ForEach(x => {
            if (x != npc && x.Name == npc.Name)
                Destroy(x.gameObject);

        });
    }

}
