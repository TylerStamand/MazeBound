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
        playerCharacter.OnWeaponScrapChange += HandleWeaponScrapsChange;
        upgradeSlot.OnLeftClick += HandleUpgradeSlotClick;

        HandleWeaponScrapsChange(playerCharacter.WeaponScraps);

        foreach (Transform child in upgradeBarsParent.transform) {
            Destroy(child.gameObject);
        }
    }

    protected override void OnDestroy() {
        upgradeSlot.OnLeftClick -= HandleUpgradeSlotClick;
        if (upgradeSlot.Item != null) {
            for (int i = 0; i < Inventory.InventorySize; i++) {
                if (items[i] == null) {
                    items[i] = upgradeSlot.Item;
                    break;
                }
            }
        }


        //Wait to call base.OnDestroy() because it calls SetInventory 
        base.OnDestroy();

    }

    void HandleWeaponScrapsChange(int weaponScraps) {
        scrapsText.text = "Scraps: " + weaponScraps;
    }

    void HandleUpgradeSlotClick(Slot slot) {
        Item oldHeldItem = null;

        //If holding item, destroy the UI of it and set currentHeldItem to null
        //Only if the item is of IUpgradeable
        if (currentHeldItem != null && currentHeldItem is IUpgradeable) {
            Destroy(currentHeldUIItem.gameObject);
            oldHeldItem = currentHeldItem;
            currentHeldItem = null;
        }

        //If slot has an item, create the UI of it and set currentHeldItem to it
        if (slot.Item != null) {
            currentHeldUIItem = CreateHeldUIItem(slot);
            currentHeldItem = slot.Item;
            foreach (Transform child in upgradeBarsParent.transform) {
                Destroy(child.gameObject);
            }
        }

        //set the slot to whatever was held before, which could be null
        slot.SetItem(oldHeldItem);

        //If it was not null, set the upgrade bars 
        if (slot.Item != null) {
            DisplayUpgradeBars(slot.Item as IUpgradeable);
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
