using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazePauseMenu : MonoBehaviour {
    [SerializeField] Button HubButton;

    void Awake() {
        HubButton.onClick.AddListener(() => {
            GameManager.Instance.LoadHub();
        });
    }
}
