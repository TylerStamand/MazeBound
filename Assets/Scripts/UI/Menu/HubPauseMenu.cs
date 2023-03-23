using UnityEngine;
using UnityEngine.UI;

public class HubPauseMenu : MonoBehaviour {

    [SerializeField] Button QuitButton;

    void Awake() {
        QuitButton.onClick.AddListener(() => {
            GameManager.Instance.Quit();
        });
    }
}
