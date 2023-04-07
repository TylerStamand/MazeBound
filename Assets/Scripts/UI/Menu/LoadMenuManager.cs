using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour {
    [SerializeField] Button loadButton;
    [SerializeField] Button newButton;

    public event Action OnLoadPressed;
    public event Action OnNewPressed;

    void Awake() {
        loadButton.onClick.AddListener(() => {
            AudioSource.PlayClipAtPoint(ResourceManager.Instance.ButtonClickSound, Camera.main.transform.position, GameManager.Instance.GetVolume());
            OnLoadPressed?.Invoke();
        });
        newButton.onClick.AddListener(() => {
            AudioSource.PlayClipAtPoint(ResourceManager.Instance.ButtonClickSound, Camera.main.transform.position, GameManager.Instance.GetVolume());
            OnNewPressed?.Invoke();
        });
    }

    void OnDestroy() {
        loadButton.onClick.RemoveAllListeners();
        newButton.onClick.RemoveAllListeners();
    }
}
