using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeUIManager : InventoryUIController {
    [SerializeField] TextMeshProUGUI scrapsText;

    override void Awake() {
        base.Awake();
        scrapsText.text = "Scraps: " + playerCharacter.WeaponScraps;
    }
}
