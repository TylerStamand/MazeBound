using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

class NPCSaveData {
    public bool MazeEncounterComplete;
    public bool HubFirstEncounterComplete;
    public bool InHub;

    //new game defaults
    public NPCSaveData() {
        MazeEncounterComplete = false;
        HubFirstEncounterComplete = false;
        InHub = false;
    }
}

public class NPC : MonoBehaviour, IInteractable, ISaveLoad {
    [field: SerializeField] public string Name { get; private set; }
    [SerializeField] Dialog MazeEncounterDialog;
    [SerializeField] Dialog HubFirstEncounterDialog;
    [SerializeField] Dialog[] PuzzlePieceDialog = new Dialog[3];
    [SerializeField] List<Dialog> GeneralDialog = new List<Dialog>();

    [SerializeField] bool isVendor = false;
    [ShowIf("isVendor")]
    [SerializeField] GameObject shop;

    public bool InHub => npcState.InHub;

    NPCSaveData npcState = new NPCSaveData();


    // void Awake() {
    //     //This is the default if you just instantiate an npc without loading from save data
    // }

    public virtual void Interact(PlayerCharacter playerCharacter) {
        DialogManager dialogManager;

        if (!npcState.MazeEncounterComplete && MazeEncounterDialog != null) {

            //Create the dialog manager and set the dialog
            dialogManager = ShowDialog(MazeEncounterDialog, playerCharacter);
            if (dialogManager != null)
                dialogManager.OnDialogComplete += (x) => npcState.MazeEncounterComplete = true;

            //End early, so as not to show any shop if there is one
            return;
        }

        if (!npcState.HubFirstEncounterComplete && HubFirstEncounterDialog != null) {
            //Check if there is dialog to show
            if (HubFirstEncounterDialog == null) return;

            //Create the dialog manager and set the dialog
            dialogManager = ShowDialog(HubFirstEncounterDialog, playerCharacter);
            if (dialogManager != null)
                dialogManager.OnDialogComplete += (x) => npcState.HubFirstEncounterComplete = true;
        }
        // else if (playerCharacter.PuzzlePiecesCollected < 3) {
        //     Instantiate(dialogManagerPrefab).GetComponent<DialogManager>().SetDialog(PuzzlePieceDialog[playerCharacter.PuzzlePiecesCollected]);
        // }
        else {
            if (GeneralDialog.Count == 0) return;
            dialogManager = ShowDialog(GeneralDialog[UnityEngine.Random.Range(0, GeneralDialog.Count)], playerCharacter);
        }

        if (dialogManager == null) return;

        dialogManager.OnDialogComplete += (x) => {
            if (isVendor) {
                playerCharacter.ShowMenu(shop, true);
            }
        };
    }

    void OnValidate() {
        if (PuzzlePieceDialog.Length > 3) {
            Debug.Log("Only 3 puzzle piece dialogs are allowed");
            Array.Resize(ref PuzzlePieceDialog, 3);
        }
    }

    void OnDestroy() {
        if (npcState.HubFirstEncounterComplete) {
            npcState.InHub = true;
            SaveManager.Instance.SetData(Name, npcState);
        }
    }

    DialogManager ShowDialog(Dialog dialog, PlayerCharacter playerCharacter) {
        DialogManager dialogManager = playerCharacter.ShowMenu(ResourceManager.Instance.DialogManagerPrefab, false)?.GetComponent<DialogManager>();
        if (dialogManager == null) return null;
        dialogManager.SetDialog(dialog, Name);
        dialogManager.OnDialogComplete += (x) => {
            Destroy(dialogManager.gameObject);
            playerCharacter.ExitMenu();
        };
        return dialogManager;
    }

    public void Save() {
        SaveManager.Instance.SetData(Name, npcState);
    }

    public void Load() {
        npcState = SaveManager.Instance.GetData<NPCSaveData>(Name);
        if (npcState == null) {
            npcState = new NPCSaveData();
        }
    }
}
