using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Collections;

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
    [SerializeField] TriggerObject portalTrigger;

    [Header("Portal Stuff")]
    [SerializeField] Tilemap deactivatedPortal;
    [SerializeField] Tilemap activatedPortal;
    [SerializeField] Dialog portalDialog;
    public static HubManager Instance { get; private set; }

    HubSaveData hubSaveData;




    void Awake() {

        //Singleton
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        GameManager.Instance.OnSceneChange += Save;

        //Sets up maze entrance triggers

        if (dungeon1Trigger != null)
            dungeon1Trigger.OnTriggerEnter += () => {
                GameManager.Instance.LoadMaze(1);
            };
        if (dungeon2Trigger != null)
            dungeon2Trigger.OnTriggerEnter += () => {
                GameManager.Instance.LoadMaze(2);
            };

        if (dungeon3Trigger != null)
            dungeon3Trigger.OnTriggerEnter += () => {
                GameManager.Instance.LoadMaze(3);
            };

        if (portalTrigger != null)
            portalTrigger.OnTriggerEnter += () => {
                GameManager.Instance.LoadBoss();
            };

        //Load player
        Instantiate(ResourceManager.Instance.PlayerPrefab, playerSpawn.transform.position, Quaternion.identity);


    }

    void OnDestroy() {
        GameManager.Instance.OnSceneChange -= Save;
    }

    void Start() {

        //Load Hub data and anything inside the scene
        Load();

        //Show only NPCs that have been found
        foreach (NPC npc in FindObjectsOfType<NPC>()) {
            npc.gameObject.SetActive(npc.MazeEncounterComplete);
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

    IEnumerator HandlePortalTriggered() {
        if (GameManager.Instance.PuzzlePiecesCollectedCount != 3) yield break;
        deactivatedPortal.enabled = false;
        activatedPortal.enabled = true;
        yield return new WaitForSeconds(2);

        DialogManager dialogManager = ShowDialog(portalDialog, FindObjectOfType<PlayerCharacter>());
        if (dialogManager != null) {
            dialogManager.OnDialogComplete += (x) => {

                GameManager.Instance.LoadBoss();
            };
        }

    }

    DialogManager ShowDialog(Dialog dialog, PlayerCharacter playerCharacter) {
        DialogManager dialogManager = playerCharacter.ShowMenu(ResourceManager.Instance.DialogManagerPrefab, false)?.GetComponent<DialogManager>();
        if (dialogManager == null) return null;
        dialogManager.SetDialog(dialog, "---");
        dialogManager.OnDialogComplete += (x) => {
            Destroy(dialogManager.gameObject);
            playerCharacter.ExitMenu();
        };
        return dialogManager;
    }

}
