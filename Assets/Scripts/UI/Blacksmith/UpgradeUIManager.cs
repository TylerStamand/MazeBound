using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UpgradeUIManager : InventoryUIController {
    [SerializeField] TextMeshProUGUI scrapsText;
    [SerializeField] GameObject upgradeBarsParent;
    [SerializeField] UpgradeBarUI upgradeBarUIPrefab;

    [SerializeField] Slot upgradeSlot;

    protected override void Awake() {
        base.Awake();
        scrapsText.text = "Scraps: " + playerCharacter.WeaponScraps;
        upgradeSlot.OnLeftClick += HandleUpgradeSlotClick;

        foreach (Transform child in upgradeBarsParent.transform) {
            Destroy(child.gameObject);
        }
    }

    protected override void OnDestroy() {
        upgradeSlot.OnLeftClick -= HandleUpgradeSlotClick;
        if (currentHeldUIItem != null) {
            for (int i = 0; i < Inventory.InventorySize; i++) {
                if (items[i] == null) {
                    items[i] = currentHeldItem;
                    break;
                }
            }
        }


        //Wait to call base.OnDestroy() because it calls SetInventory 
        base.OnDestroy();

    }

    void HandleUpgradeSlotClick(Slot slot) {
        if (currentHeldItem != null) {
            if (currentHeldItem is IUpgradeable) {
                slot.SetItem(currentHeldItem);
                DisplayUpgradeBars(currentHeldItem as IUpgradeable);
            }
        }
        if (slot.Item != null) {
            currentHeldItem = slot.Item;
        }
    }

    void DisplayUpgradeBars(IUpgradeable upgradeableItem) {
        foreach (Transform child in upgradeBarsParent.transform) {
            Destroy(child.gameObject);
        }
        foreach (UpgradeableStat upgrade in upgradeableItem.GetUpgradeableStats()) {
            UpgradeBarUI upgradeBarUI = Instantiate(upgradeBarUIPrefab, upgradeBarsParent.transform);
            upgradeBarUI.Initialize(upgrade);
        }
    }
}
