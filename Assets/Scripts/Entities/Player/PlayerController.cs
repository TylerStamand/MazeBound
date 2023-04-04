using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;


/// <summary>
/// Maps player input to player actions
/// </summary>
public class PlayerController : MonoBehaviour {


    [SerializeField] float moveSpeed;
    [SerializeField] float interactDistance;


    public event Action OnLeftClick;
    public event Action OnRightClick;
    public event Action OnToggleMenu;
    public event Action<IInteractable> OnInteract;
    public event Action OnInventory;

    public Vector2 ScreenMousePos { get; private set; }
    public Vector2 WorldMousePos { get; private set; }
    new Rigidbody2D rigidbody;
    Animator animator;
    Vector2 inputVector;
    Direction direction;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ScreenMousePos = new Vector2();
        WorldMousePos = new Vector2();
        inputVector = new Vector2();
        direction = Direction.North;
    }

    void Update() {


        rigidbody.velocity = inputVector * moveSpeed;


        if (inputVector.x > 0) {
            direction = Direction.East;
        } else if (inputVector.x < 0) {
            direction = Direction.West;
        } else if (inputVector.y > 0) {
            direction = Direction.North;
        } else if (inputVector.y < 0) {
            direction = Direction.South;
        }


        //Update Player Direction
        UpdateAnimations();
    }

    void OnDestroy() {
        StopAllCoroutines();
    }



    public void OnMove(InputAction.CallbackContext context) {
        inputVector = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context) {
        ScreenMousePos = context.ReadValue<Vector2>();
        if (Camera.main != null)
            WorldMousePos = Camera.main.ScreenToWorldPoint(ScreenMousePos);
    }

    public void RightClick(InputAction.CallbackContext context) {
        if (context.performed) {
            OnRightClick?.Invoke();
        }
    }

    public void LeftClick(InputAction.CallbackContext context) {
        if (context.performed) {
            OnLeftClick?.Invoke();
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

            Vector2 directionVector = Utilities.GetDirectionVectorFromDirection(direction);

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionVector, interactDistance);
            if (hits != null) {
                foreach (RaycastHit2D hit in hits.ToList()) {
                    Debug.Log("Hit " + hit.collider.gameObject.name);
                    IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null) {
                        OnInteract?.Invoke(interactable);
                        return;
                    }

                }
            }

        }
    }

    public void ExitMenu(InputAction.CallbackContext context) {
        if (context.performed) {
            OnToggleMenu?.Invoke();
        }
    }



    void UpdateAnimations() {
        if (inputVector.x == 0 && inputVector.y == 0)
            animator.SetBool("isMoving", false);
        else {
            Vector2 directionVector = Utilities.GetDirectionVectorFromDirection(direction);
            animator.SetFloat("x", directionVector.x);
            animator.SetFloat("y", directionVector.y);
            animator.SetBool("isMoving", true);
        }
    }

}
