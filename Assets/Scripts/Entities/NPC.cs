using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable {
    [field: SerializeField] public string Name { get; private set; }
    [SerializeField] GameObject dialogManagerPrefab;
    [SerializeField] Dialog MazeEncounterDialog;
    [SerializeField] Dialog HubFirstEncounterDialog;
    [SerializeField] Dialog[] PuzzlePieceDialog = new Dialog[3];
    [SerializeField] List<Dialog> GeneralDialog = new List<Dialog>();

    bool mazeEncounterComplete;
    bool hubFirstEncounterComplete;


    public virtual void Interact(PlayerCharacter playerCharacter) {
        DialogManager dialogManager = Instantiate(dialogManagerPrefab).GetComponent<DialogManager>();
        if (!mazeEncounterComplete) {
            dialogManager.OnDialogComplete += (x) => mazeEncounterComplete = true;
            dialogManager.SetDialog(MazeEncounterDialog, Name);
        } else if (!hubFirstEncounterComplete) {
            dialogManager.OnDialogComplete += (x) => hubFirstEncounterComplete = true;
            dialogManager.SetDialog(HubFirstEncounterDialog, Name);
        }
          // else if (playerCharacter.PuzzlePiecesCollected < 3) {
          //     Instantiate(dialogManagerPrefab).GetComponent<DialogManager>().SetDialog(PuzzlePieceDialog[playerCharacter.PuzzlePiecesCollected]);
          // }
          else {
            Instantiate(dialogManagerPrefab).GetComponent<DialogManager>().SetDialog(GeneralDialog[UnityEngine.Random.Range(0, GeneralDialog.Count)], Name);
        }

    }


    void OnValidate() {
        if (PuzzlePieceDialog.Length > 3) {
            Debug.Log("Only 3 puzzle piece dialogs are allowed");
            Array.Resize(ref PuzzlePieceDialog, 3);
        }
    }
}
