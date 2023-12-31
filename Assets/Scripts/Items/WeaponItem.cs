using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

[Serializable]
public class WeaponItem : Item, IUpgradeable {

    public UpgradeableStat Damage { get; private set; }
    public UpgradeableStat Speed { get; private set; }
    public UpgradeableStat CriticalChance { get; private set; }


    [JsonConstructor]
    public WeaponItem(string itemName, UpgradeableStat damage, UpgradeableStat speed, UpgradeableStat criticalChance) : base(itemName) {
        Damage = damage;
        Speed = speed;
        CriticalChance = criticalChance;
    }

    public WeaponItem(string itemName, int damage, float speed, float criticalChance, int upgradeCostBase) : base(itemName) {
        WeaponData itemData = (WeaponData)ResourceManager.Instance.GetItemData(itemName);
        Damage = new UpgradeableStat("Damage", damage, upgradeCostBase, itemData.UpgradeValueMultiplier, itemData.UpgradeCostMultiplier);
        Speed = new UpgradeableStat("Speed", speed, upgradeCostBase, itemData.UpgradeValueMultiplier, itemData.UpgradeCostMultiplier);
        CriticalChance = new UpgradeableStat("Critical Chance", criticalChance, upgradeCostBase, itemData.UpgradeValueMultiplier, itemData.UpgradeCostMultiplier);
    }



    //TODO: format this to be part right justified
    public override string GetDescription() {
        string description = "";
        description += $"Damage: {Damage.CurrentValue.ToString("N", CultureInfo.CurrentCulture)} Lvl: {Damage.Level}\n";
        description += $"Speed: {Math.Round(Speed.CurrentValue, 2, MidpointRounding.AwayFromZero)} Lvl: {Speed.Level}\n";
        description += $"CriticalChance: {Math.Round(Math.Min(CriticalChance.CurrentValue, 100), 2, MidpointRounding.AwayFromZero)} Lvl: {CriticalChance.Level}\n";
        description += "\n";
        description += $"Scrap Value: {GetScrapValue()}\n";

        return description;
    }

    public List<UpgradeableStat> GetUpgradeableStats() {
        List<UpgradeableStat> stats = new List<UpgradeableStat>();
        stats.Add(Damage);
        stats.Add(Speed);
        stats.Add(CriticalChance);
        return stats;
    }

    public int GetScrapValue() {
        return (int)(Damage.CurrentValue + Speed.CurrentValue + (CriticalChance.CurrentValue / 10f)) * 30;
    }


}
