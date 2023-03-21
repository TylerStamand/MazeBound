using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuManager : MonoBehaviour {
    [SerializeField] Button loadButton;
    [SerializeField] Button newButton;

    public event Action OnLoadPressed;
    public event Action OnNewPressed;

    void Awake() {
        loadButton.onClick.AddListener(() => OnLoadPressed?.Invoke());
        newButton.onClick.AddListener(() => OnNewPressed?.Invoke());
    }

    void OnDestroy() {
        loadButton.onClick.RemoveAllListeners();
        newButton.onClick.RemoveAllListeners();
    }
}
