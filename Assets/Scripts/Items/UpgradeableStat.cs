using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public void Upgrade() {
        if (Level >= MaxLevel) {
            return;
        }

        Level++;
        CurrentValue += BaseValue * UpgradeValueMultiplier;
        UpgradeCost += (int)(UpgradeCostBase * UpgradeCostMultiplier);
        OnUpgrade?.Invoke();
    }
}
