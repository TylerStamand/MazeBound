using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable {
    [field: SerializeField] public string Name { get; private set; }
    [SerializeField] Dialog MazeEncounterDialog;
    [SerializeField] Dialog HubFirstEncounterDialog;
    [SerializeField] Dialog[] PuzzlePieceDialog = new Dialog[3];
    [SerializeField] List<Dialog> GeneralDialog = new List<Dialog>();

    bool mazeEncounterComplete;
    bool hubFirstEncounterComplete;


    public virtual void Interact(PlayerCharacter playerCharacter) {

        if (playerCharacter == null) Debug.Log("Player null");
        if (ResourceManager.DialogManagerPrefab == null) Debug.Log("DialogManagerPrefab null");
        if (!mazeEncounterComplete && MazeEncounterDialog != null) {
            //Check if there is dialog to show
            if (MazeEncounterDialog == null) return;

            //Create the dialog manager and set the dialog
            DialogManager dialogManager = ShowDialog(MazeEncounterDialog, playerCharacter);
            if (dialogManager != null)
                dialogManager.OnDialogComplete += (x) => mazeEncounterComplete = true;


        } else if (!hubFirstEncounterComplete && HubFirstEncounterDialog != null) {
            //Check if there is dialog to show
            if (HubFirstEncounterDialog == null) return;

            //Create the dialog manager and set the dialog
            DialogManager dialogManager = ShowDialog(HubFirstEncounterDialog, playerCharacter);
            if (dialogManager != null)
                dialogManager.OnDialogComplete += (x) => hubFirstEncounterComplete = true;
        }
          // else if (playerCharacter.PuzzlePiecesCollected < 3) {
          //     Instantiate(dialogManagerPrefab).GetComponent<DialogManager>().SetDialog(PuzzlePieceDialog[playerCharacter.PuzzlePiecesCollected]);
          // }
          else {
            if (GeneralDialog.Count == 0) return;
            ShowDialog(GeneralDialog[UnityEngine.Random.Range(0, GeneralDialog.Count)], playerCharacter);

        }

    }


    void OnValidate() {
        if (PuzzlePieceDialog.Length > 3) {
            Debug.Log("Only 3 puzzle piece dialogs are allowed");
            Array.Resize(ref PuzzlePieceDialog, 3);
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
