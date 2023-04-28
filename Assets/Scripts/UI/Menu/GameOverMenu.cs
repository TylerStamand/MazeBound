using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour {
    [SerializeField] Button button;

    void Awake() {
        button.onClick.AddListener(HandleButtonClicked);
    }

    void HandleButtonClicked() {
        GameManager.Instance.LoadMenu();
    }
}
