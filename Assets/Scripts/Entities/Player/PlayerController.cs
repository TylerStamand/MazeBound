using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour {


    [SerializeField] float moveSpeed;
    [SerializeField] float interactDistance;

    public event Action OnClick;
    public event Action OnExitMenu;
    public event Action<IInteractable> OnInteract;
    public event Action OnInventory;

    public Vector2 ScreenMousePos { get; private set; }
    public Direction CurrentDirection { get => Utilities.DirectionFromVector2(inputVector); }
    public Vector2 WorldMousePos { get; private set; }
    new Rigidbody2D rigidbody;
    Animator animator;
    Vector2 inputVector;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context) {
        inputVector = context.ReadValue<Vector2>();
        Debug.Log(transform.forward);

    }

    public void OnAim(InputAction.CallbackContext context) {
        ScreenMousePos = context.ReadValue<Vector2>();
        WorldMousePos = Camera.main.ScreenToWorldPoint(ScreenMousePos);
    }

    public void Click(InputAction.CallbackContext context) {
        if (context.performed) {
            OnClick?.Invoke();
        }
    }

    public void OnToggleInventory(InputAction.CallbackContext context) {
        if (context.performed) {
            Debug.Log("Toggle");
            OnInventory?.Invoke();
        }
    }

    public void Interact(InputAction.CallbackContext context) {
        if (context.performed) {
            Debug.Log("Trying to Interact");

            //Use tranform.right or tranform.up depending on the direction the player is facing
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.forward, interactDistance);
            if (hits != null) {
                foreach (RaycastHit2D hit in hits.ToList()) {
                    Debug.Log("Hit " + hit.collider.gameObject.name);
                    IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null) {
                        OnInteract?.Invoke(interactable);
                    }

                }
            }

        }
    }

    public void ExitMenu(InputAction.CallbackContext context) {
        OnExitMenu?.Invoke();
    }

    void Update() {
        rigidbody.velocity = inputVector * moveSpeed;
        //Update Player Direction
        UpdateAnimations();
    }

    void UpdateAnimations() {
        if (inputVector.x == 0 && inputVector.y == 0)
            animator.SetBool("isMoving", false);
        else {
            animator.SetFloat("x", inputVector.x);
            animator.SetFloat("y", inputVector.y);
            animator.SetBool("isMoving", true);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * interactDistance);
    }
}
