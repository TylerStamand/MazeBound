using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButtons : MonoBehaviour {
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    public event Action<bool> OnChoiceMade;

    void Awake() {
        yesButton.onClick.AddListener(() => OnChoiceMade?.Invoke(true));
        noButton.onClick.AddListener(() => OnChoiceMade?.Invoke(false));
    }
}
