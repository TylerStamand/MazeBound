using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerObject : MonoBehaviour {

    public event Action OnTriggerEnter;


    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<PlayerCharacter>()) {
            OnTriggerEnter?.Invoke();
        }
    }
}
