using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour {


    [SerializeField] float moveSpeed;

    public event Action OnAttack;
    public event Action OnInventory;

    public Vector2 ScreenMousePos { get; private set; }
    public Direction CurrentDirection { get => Utilities.DirectionFromVector2(inputVector); }
    public Vector2 WorldMousePos { get; private set; }
    new Rigidbody2D rigidbody;
    Vector2 inputVector;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        inputVector = context.ReadValue<Vector2>();

    }

    public void OnAim(InputAction.CallbackContext context) {
        ScreenMousePos = context.ReadValue<Vector2>();
        WorldMousePos = Camera.main.ScreenToWorldPoint(ScreenMousePos);
    }

    public void OnFire(InputAction.CallbackContext context) {
        if (context.performed) {
            OnAttack?.Invoke();
        }
    }

    public void OnToggleInventory(InputAction.CallbackContext context) {
        if (context.performed) {
            Debug.Log("Toggle");
            OnInventory?.Invoke();
        }
    }

    void Update() {
        rigidbody.velocity = inputVector * moveSpeed;

    }
}
