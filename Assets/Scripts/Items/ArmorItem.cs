using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ArmorItem : Item, IUpgradeable {

    public ArmorPiece Piece { get; private set; }
    public UpgradeableStat Defense { get; private set; }
    public UpgradeableStat Health { get; private set; }

    [JsonConstructor]
    public ArmorItem(string itemName, UpgradeableStat defense, UpgradeableStat health) : base(itemName) {
        Defense = defense;
        Health = health;
    }

    public ArmorItem(string itemName, int defense, int health, int upgradeCostBase) : base(itemName) {
        ArmorData itemData = (ArmorData)ResourceManager.Instance.GetItemData(itemName);
        Defense = new UpgradeableStat("Defense", defense, upgradeCostBase, itemData.UpgradeValueMultiplier, itemData.UpgradeCostMultiplier);
        Health = new UpgradeableStat("Health", health, upgradeCostBase, itemData.UpgradeValueMultiplier, itemData.UpgradeCostMultiplier);
    }

    public enum ArmorPiece {
        Head, Chest,  Boot
    }

    public List<UpgradeableStat> GetUpgradeableStats() {
        throw new System.NotImplementedException();
    }
}
