using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectMenu : MonoBehaviour {
    [SerializeField] Button girlButton;
    [SerializeField] Button guyButton;

    public event Action<bool> OnCharacterSelect;

    void Awake() {
        girlButton.onClick.AddListener(() => {
            AudioSource.PlayClipAtPoint(ResourceManager.Instance.ButtonClickSound, Camera.main.transform.position, GameManager.Instance.GetVolume());
            OnCharacterSelect?.Invoke(false);
        });

        guyButton.onClick.AddListener(() => {
            AudioSource.PlayClipAtPoint(ResourceManager.Instance.ButtonClickSound, Camera.main.transform.position, GameManager.Instance.GetVolume());
            OnCharacterSelect?.Invoke(true);
        });
    }

    
}
