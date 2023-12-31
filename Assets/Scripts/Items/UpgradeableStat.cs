using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class UpgradeableStat {
    public static int MaxLevel = 5;

    public string Name { get; private set; }
    public int Level { get; private set; }
    public float BaseValue { get; private set; }
    public float CurrentValue { get; private set; }
    public int UpgradeCost { get; private set; }
    public int UpgradeCostBase { get; private set; }
    public float UpgradeCostMultiplier { get; private set; }
    public float UpgradeValueMultiplier { get; private set; }

    public event Action OnUpgrade;

    [JsonConstructor]
    public UpgradeableStat(string name, int level, float baseValue, float currentValue, int upgradeCost, int upgradeCostBase, float upgradeCostMultiplier, float upgradeValueMultiplier) {
        Name = name;
        Level = level;
        BaseValue = baseValue;
        CurrentValue = currentValue;
        UpgradeCost = upgradeCost;
        UpgradeCostBase = upgradeCostBase;
        UpgradeCostMultiplier = upgradeCostMultiplier;
        UpgradeValueMultiplier = upgradeValueMultiplier;
    }

    public UpgradeableStat(string name, float baseValue, int upgradeCostBase, float upgradeValueMultiplier, float upgradeCostMultiplier) {
        Name = name;
        Level = 1;
        BaseValue = baseValue;
        CurrentValue = baseValue;
        UpgradeCostBase = upgradeCostBase;
        UpgradeCost = upgradeCostBase;
        UpgradeCostMultiplier = upgradeCostMultiplier;
        UpgradeValueMultiplier = upgradeValueMultiplier;
    }

    public bool Upgrade() {
        if (Level >= MaxLevel) {
            return false;
        }

        Level++;
        if (Name.Equals("Critical Chance")) {
            CurrentValue += BaseValue * (UpgradeValueMultiplier / 5);
        } else {
            CurrentValue += BaseValue * UpgradeValueMultiplier;
        }
        UpgradeCost += (int)(UpgradeCostBase * UpgradeCostMultiplier);
        OnUpgrade?.Invoke();
        return true;
    }
}
