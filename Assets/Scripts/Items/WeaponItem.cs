using System;
using System.Globalization;

[Serializable]
public class WeaponItem : Item {

    public new WeaponData ItemData { get; private set; }

    //Eventually set these to formulas to take in level as a modifier
    public int Damage { get => BaseDamage; }
    public float Speed { get => BaseSpeed; }
    public float CriticalChance { get => BaseCriticalChance; }

    //I cant remember why I declared fields for these, probably has something to do with serialization
    public int BaseDamage { get => baseDamage; private set => baseDamage = value; }
    public float BaseSpeed { get => baseSpeed; private set => baseSpeed = value; }
    public float BaseCriticalChance { get => baseCriticalChance; private set => baseCriticalChance = value; }

    public int DamageLevel { get; private set; }
    public int SpeedLevel { get; private set; }
    public int CriticalChanceLevel { get; private set; }


    private int baseDamage;
    private float baseSpeed;
    private float baseCriticalChance;


    public WeaponItem(WeaponData itemData, int damage, float Speed, float criticalChance) : base(itemData) {
        BaseDamage = damage;
        BaseSpeed = Speed;
        BaseCriticalChance = criticalChance;
        ItemData = itemData;
    }


    //TODO: format this to be part right justified
    public override string GetDescription() {
        string description = "";
        description += $"Damage: {Damage.ToString("N", CultureInfo.CurrentCulture)} Lvl: {DamageLevel}\n";
        description += $"Speed: {Speed} Lvl: {SpeedLevel}\n";
        description += $"CriticalChange: {CriticalChance} Lvl: {CriticalChanceLevel}\n";

        return description;
    }


}
