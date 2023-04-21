using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HUDController : MonoBehaviour {
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI scrapText;

    PlayerCharacter playerCharacter;

    void Awake() {
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        playerCharacter.OnHealthChange += HandleHealthChange;
        playerCharacter.OnWeaponScrapChange += HandleWeaponScrapChange;
        HandleHealthChange(playerCharacter, playerCharacter.CurrentHealth);
        HandleWeaponScrapChange(playerCharacter.WeaponScraps);
    }

    void Destroy() {
        playerCharacter.OnHealthChange -= HandleHealthChange;
        playerCharacter.OnWeaponScrapChange -= HandleWeaponScrapChange;
    }

    void HandleHealthChange(IDamageable damageable, int health) {
        healthText.text = "Health: " + health;
    }

    void HandleWeaponScrapChange(int scrap) {
        scrapText.text = "Scrap: " + scrap;
    }
}
