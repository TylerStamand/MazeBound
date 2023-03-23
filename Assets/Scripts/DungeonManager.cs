using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : MonoBehaviour {
    public static DungeonManager Instance { get; private set; }

    PlayerCharacter player;

    void Awake() {
        //Singleton
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        GameManager.Instance.OnSceneChange += Save;
        DungeonGenerator.Instance.OnNPCFound += HandleNPCFound;
        FindObjectsOfType<PlayerCharacter>().ToList().ForEach(x => Destroy(x.gameObject));

    }

    void OnDestroy() {
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
