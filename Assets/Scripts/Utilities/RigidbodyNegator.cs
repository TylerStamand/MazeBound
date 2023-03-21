using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyNegator : MonoBehaviour
{
    [SerializeField] Collider2D kinematicRigidbodyCollider;
    [SerializeField] Collider2D dynamicRigidbodyCollider;

    void Awake() {
        Physics2D.IgnoreCollision(kinematicRigidbodyCollider, dynamicRigidbodyCollider, true);
    }
}
