using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour {
    [SerializeField] private float volumeMultiplier = 1f;
    AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();

        if (GameManager.Instance != null) {
            GameManager.Instance.OnVolumeChange += HandleVolumeChange;
            HandleVolumeChange(GameManager.Instance.GetVolume());
        }

    }

    void OnDestroy() {
        if (GameManager.Instance != null) {
            GameManager.Instance.OnVolumeChange -= HandleVolumeChange;
        }
    }

    void HandleVolumeChange(float volume) {
        audioSource.volume = volume * volumeMultiplier;
    }
}
