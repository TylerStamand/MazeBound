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
    [SerializeField] Dialog MazeAfterEncounterDialog;
    [SerializeField] Dialog HubFirstEncounterDialog;
    [SerializeField] Dialog[] PuzzlePieceDialog = new Dialog[3];
    [SerializeField] List<Dialog> GeneralDialog = new List<Dialog>();

    [field: SerializeField] public bool IsGuide { get; private set; } = false;
    [SerializeField] bool isVendor = false;
    [ShowIf("isVendor")]
    [SerializeField] GameObject shop;

    public event Action<NPC> OnFound;

    public bool InHub => npcState.InHub;
    public bool MazeEncounterComplete => npcState.MazeEncounterComplete;

    NPCSaveData npcState = new NPCSaveData();


    public virtual void Interact(PlayerCharacter playerCharacter) {
        DialogManager dialogManager;

        if (!npcState.MazeEncounterComplete) {
            OnFound?.Invoke(this);
            if (MazeEncounterDialog != null) {
                //Create the dialog manager and set the dialog
                dialogManager = ShowDialog(MazeEncounterDialog, playerCharacter);
                if (dialogManager != null) {
                    dialogManager.OnDialogComplete += (x) => {
                        npcState.MazeEncounterComplete = true;
                        Save();
                    };
                }

                //End early, so as not to show any shop if there is one
                return;
            }


        }

        //If the NPC is not in hub after first encounter, show a dialog to say they'll go to the hub
        if (GameManager.Instance.CurrentGameState != GameState.Hub) {
            dialogManager = ShowDialog(MazeAfterEncounterDialog, playerCharacter);
            if (dialogManager != null) {
                dialogManager.OnDialogComplete += (x) => {
                    npcState.MazeEncounterComplete = true;
                    Save();
                };
            }
            return;
        }

        //Otherwise, show Dialog like normal
        else {

            //Hub first encounter
            if (!npcState.HubFirstEncounterComplete && HubFirstEncounterDialog != null) {

                //Create the dialog manager and set the dialog
                dialogManager = ShowDialog(HubFirstEncounterDialog, playerCharacter);
                if (dialogManager != null)
                    dialogManager.OnDialogComplete += (x) => {
                        npcState.HubFirstEncounterComplete = true;
                        Save();
                    };
            }

            //Puzzle Piece Dialog
            // else if (playerCharacter.PuzzlePiecesCollected < 3) {
            //     Instantiate(dialogManagerPrefab).GetComponent<DialogManager>().SetDialog(PuzzlePieceDialog[playerCharacter.PuzzlePiecesCollected]);
            // }

            //General Dialog
            else {
                if (GeneralDialog.Count == 0) return;
                dialogManager = ShowDialog(GeneralDialog[UnityEngine.Random.Range(0, GeneralDialog.Count)], playerCharacter);
            }
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
        Debug.Log("Saving NPC: " + Name);
        SaveManager.Instance.SetData(Name, npcState);
    }

    public void Load() {
        Debug.Log("Loading NPC: " + Name);
        npcState = SaveManager.Instance.GetData<NPCSaveData>(Name);
        if (npcState == null) {
            Debug.Log("No save data found for NPC: " + Name);
            npcState = new NPCSaveData();
        }
    }
}
