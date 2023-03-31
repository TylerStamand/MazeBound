using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour, IInteractable {

    public event Action<Shrine> OnInteract;

    public void Interact(PlayerCharacter playerCharacter) {
        throw new System.NotImplementedException();
    }

    public void ShowActivated() {
        Debug.Log("Showing activated");
    }

    public void ShowPuzzlePiece() {
        Debug.Log("Showing puzzle piece");
    }
}
