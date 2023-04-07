using UnityEngine;
using UnityEngine.UI;

public class HubPauseMenu : MonoBehaviour {

    [SerializeField] Button QuitButton;

    void Awake() {
        QuitButton.onClick.AddListener(() => {

            AudioSource.PlayClipAtPoint(ResourceManager.Instance.ButtonClickSound, Camera.main.transform.position, GameManager.Instance.GetVolume());
            GameManager.Instance.Quit();
        });
    }
}
