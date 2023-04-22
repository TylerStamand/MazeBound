using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IInteractable {
    public void Interact(PlayerCharacter playerCharacter) {
        Debug.Log("Portal Interact");
        HubManager.Instance.OnPortalTriggered();
    }
}
