using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {


    [SerializeField] float moveSpeed;

    new Rigidbody2D rigidbody;
    Vector2 inputVector;
    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        inputVector = context.ReadValue<Vector2>();

    }

    void Update() {
        rigidbody.velocity = inputVector * moveSpeed;

    }
}
