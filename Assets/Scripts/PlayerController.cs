using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour {


    [SerializeField] float moveSpeed;

    public event Action OnAttack;

    new Rigidbody2D rigidbody;
    Vector2 inputVector;
    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        inputVector = context.ReadValue<Vector2>();

    }

    public void OnFire(InputAction.CallbackContext context) {
        if (context.performed)
            OnAttack?.Invoke();
    }

    void Update() {
        rigidbody.velocity = inputVector * moveSpeed;

    }
}
