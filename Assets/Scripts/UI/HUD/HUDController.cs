using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HUDController : MonoBehaviour {
    [SerializeField] TextMeshProUGUI healthText;

    PlayerCharacter playerCharacter;

    void Awake() {
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        playerCharacter.OnHealthChange += HandleHealthChange;
        HandleHealthChange(playerCharacter.CurrentHealth);
    }

    void Destroy() {
        playerCharacter.OnHealthChange -= HandleHealthChange;
    }

    void HandleHealthChange(int health) {
        healthText.text = "Health: " + health;
    }
}
