using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour, IInteractable {

    public event Action<Shrine> OnInteract;

    bool activated = false;


    public void Interact(PlayerCharacter playerCharacter) {
        if (!activated) {
            activated = true;
            ShowActivated();
            OnInteract?.Invoke(this);
        }
    }

    public void ShowActivated() {
        Debug.Log("Showing activated");
    }

    public void ShowPuzzlePiece() {
        Debug.Log("Showing puzzle piece");
    }
}
