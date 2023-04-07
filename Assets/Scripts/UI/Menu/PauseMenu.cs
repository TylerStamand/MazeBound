using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    [SerializeField] Slider volumeSlider;

    void Awake() {
        volumeSlider.value = GameManager.Instance.GetVolume();
        volumeSlider.onValueChanged.AddListener(delegate { GameManager.Instance.SetVolume(volumeSlider.value); });
    }


}
