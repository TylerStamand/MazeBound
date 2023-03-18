using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable {
    [field: SerializeField] public string Name { get; private set; }
    [SerializeField] NPCState npcState;
    [SerializeField] Dialog MazeEncounterDialog;
    [SerializeField] Dialog HubFirstEncounterDialog;
    [SerializeField] Dialog[] PuzzlePieceDialog = new Dialog[3];
    [SerializeField] List<Dialog> GeneralDialog = new List<Dialog>();

    [SerializeField] bool isVendor = false;
    [ShowIf("isVendor")]
    [SerializeField] GameObject shop;


    //REMOVE THIS LATER
    void Awake() {
        npcState?.Reset();

    }

    public virtual void Interact(PlayerCharacter playerCharacter) {
        DialogManager dialogManager;

        if (playerCharacter == null) Debug.Log("Player null");
        if (!npcState.MazeEncounterComplete && MazeEncounterDialog != null) {
            //Check if there is dialog to show
            if (MazeEncounterDialog == null) return;

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
        }
    }

    DialogManager ShowDialog(Dialog dialog, PlayerCharacter playerCharacter) {
        DialogManager dialogManager = playerCharacter.ShowMenu(ResourceManager.DialogManagerPrefab, false)?.GetComponent<DialogManager>();
        if (dialogManager == null) return null;
        dialogManager.SetDialog(dialog, Name);
        dialogManager.OnDialogComplete += (x) => {
            Destroy(dialogManager.gameObject);
            playerCharacter.ExitMenu();
        };
        return dialogManager;
    }
}
