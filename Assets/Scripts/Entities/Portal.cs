using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IInteractable {
    public void Interact(PlayerCharacter playerCharacter) {
        Debug.Log("Portal Interact");
        if(HubManager.Instance != null)
            HubManager.Instance.OnPortalTriggered();
        else if(BossManager.Instance != null)
            StartCoroutine(BossManager.Instance.HandlePortalTriggered());
    }
}
