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

    PlayerController playerController;

    void Awake() {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update() {
        Debug.Log(playerController.ScreenMousePos);
        rectTransform.position = new Vector3(playerController.ScreenMousePos.x + offset.x, playerController.ScreenMousePos.y + offset.y, 0);


    }


}
