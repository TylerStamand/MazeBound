using System;
using System.Collections.Generic;
using System.Globalization;

[Serializable]
public class WeaponItem : Item, IUpgradeable {

    public new WeaponData ItemData { get; private set; }

    public UpgradeableStat Damage { get; private set; }
    public UpgradeableStat Speed { get; private set; }
    public UpgradeableStat CriticalChance { get; private set; }


    private int baseDamage;
    private float baseSpeed;
    private float baseCriticalChance;


    public WeaponItem(WeaponData itemData, int damage, float speed, float criticalChance, int upgradeCostBase) : base(itemData) {
        Damage = new UpgradeableStat("Damage", damage, upgradeCostBase, itemData.UpgradeValueMultiplier, itemData.UpgradeCostMultiplier);
        Speed = new UpgradeableStat("Speed", speed, upgradeCostBase, itemData.UpgradeValueMultiplier, itemData.UpgradeCostMultiplier);
        CriticalChance = new UpgradeableStat("Critical Chance", criticalChance, upgradeCostBase, itemData.UpgradeValueMultiplier, itemData.UpgradeCostMultiplier);
        ItemData = itemData;
    }


    //TODO: format this to be part right justified
    public override string GetDescription() {
        string description = "";
        description += $"Damage: {Damage.CurrentValue.ToString("N", CultureInfo.CurrentCulture)} Lvl: {Damage.Level}\n";
        description += $"Speed: {Speed.CurrentValue} Lvl: {Speed.Level}\n";
        description += $"CriticalChance: {CriticalChance.CurrentValue} Lvl: {CriticalChance.Level}\n";

        return description;
    }

    public List<UpgradeableStat> GetUpgradeableStats()
    {
        List<UpgradeableStat> stats = new List<UpgradeableStat>();
        stats.Add(Damage);
        stats.Add(Speed);
        stats.Add(CriticalChance);
        return stats;
    }


}
