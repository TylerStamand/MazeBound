using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollower : MonoBehaviour {
    [SerializeField] Vector2 offset;

    Camera mainCamera;
    Vector3 mask = new Vector3(1, 1, 0);
    RectTransform rectTransform;
    Vector3 newPosition;

    void Awake() {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();

    }

    void Update() {
        newPosition = new Vector3(newPosition.x + offset.x, newPosition.y + offset.y, 0);

        rectTransform.position = newPosition;

    }

    public void OnMouseMove(InputAction.CallbackContext context) {
        newPosition = context.ReadValue<Vector2>();

    }
}
