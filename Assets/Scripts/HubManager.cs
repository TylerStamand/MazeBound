using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class HubSaveData {

    public HubSaveData() {
        //Set new game defaults
    }
}

public class HubManager : MonoBehaviour {

    [SerializeField] GameObject playerSpawn;
    [SerializeField] TriggerObject dungeon1Trigger;
    [SerializeField] TriggerObject dungeon2Trigger;
    [SerializeField] TriggerObject dungeon3Trigger;

    public static HubManager Instance { get; private set; }

    HubSaveData hubSaveData;




    void Awake() {

        //Singleton
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }


        //Sets up maze entrance triggers
        dungeon1Trigger.OnTriggerEnter += () => {
            SceneManager.LoadSceneAsync("Prototype", LoadSceneMode.Single);
        };

        //Load player
        Instantiate(ResourceManager.Instance.PlayerPrefab, playerSpawn.transform.position, Quaternion.identity);


    }

    void Start() {

        //Load Hub data and anything inside the scene
        Load();

        //Show only NPCs that have been found
        foreach (NPC npc in FindObjectsOfType<NPC>()) {
            npc.gameObject.SetActive(npc.InHub);
        }

    }

    public void Load() {

        Debug.Log("Loading Hub");
        //Load Hub data
        hubSaveData = SaveManager.Instance.GetData<HubSaveData>("Hub");
        if (hubSaveData == null) {
            hubSaveData = new HubSaveData();
        }

        //Load anything inside the scene
        List<ISaveLoad> saveLoadObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveLoad>().ToList();
        Debug.Log("Loading " + saveLoadObjects.Count + " objects");
        foreach (ISaveLoad saveLoadObject in saveLoadObjects) {
            saveLoadObject.Load();
        }
    }

    public void Save() {
        SaveManager.Instance.SetData("Hub", hubSaveData);
        List<ISaveLoad> saveLoadObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveLoad>().ToList();
        foreach (ISaveLoad saveLoadObject in saveLoadObjects) {
            saveLoadObject.Save();
        }
    }

}
