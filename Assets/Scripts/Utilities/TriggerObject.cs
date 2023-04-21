using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerObject : MonoBehaviour {

    public event Action OnTriggerEnter;
    public event Action<GameObject> OnTriggerEnterAll;

    bool isTriggered = false;


    void OnTriggerEnter2D(Collider2D other) {
        OnTriggerEnterAll?.Invoke(other.gameObject);
        if (isTriggered) return;
        if (other.GetComponent<PlayerCharacter>()) {
            isTriggered = true;
            OnTriggerEnter?.Invoke();
        }

    }
}
