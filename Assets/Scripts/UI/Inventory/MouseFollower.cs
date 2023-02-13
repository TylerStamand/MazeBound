using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollower : MonoBehaviour {
    [SerializeField] Vector2 offset;

    RectTransform rectTransform;

    PlayerController playerController;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        playerController = FindObjectOfType<PlayerController>();
        rectTransform.position = new Vector3(playerController.ScreenMousePos.x + offset.x, playerController.ScreenMousePos.y + offset.y, 0);

    }

    void Update() {
        Debug.Log(playerController.ScreenMousePos);
        rectTransform.position = new Vector3(playerController.ScreenMousePos.x + offset.x, playerController.ScreenMousePos.y + offset.y, 0);


    }


}
