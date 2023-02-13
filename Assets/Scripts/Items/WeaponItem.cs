using System;
using System.Globalization;

[Serializable]
public class WeaponItem : Item {

    public new WeaponData ItemData { get; private set; }

    //Eventually set these to formulas to take in level as a modifier
    public float Damage { get => BaseDamage; }
    public float CoolDown { get => BaseCoolDown; }
    public float CriticalChance { get => BaseCriticalChance; }

    //I cant remember why I declared fields for these, probably has something to do with serialization
    public float BaseDamage { get => baseDamage; private set => baseDamage = value; }
    public float BaseCoolDown { get => baseCoolDown; private set => baseCoolDown = value; }
    public float BaseCriticalChance { get => baseCriticalChance; private set => baseCriticalChance = value; }

    public int DamageLevel { get; private set; }
    public int CoolDownLevel { get; private set; }
    public int CriticalChanceLevel { get; private set; }


    private float baseDamage;
    private float baseCoolDown;
    private float baseCriticalChance;


    public WeaponItem(WeaponData itemData, float damage, float coolDown, float criticalChance) : base(itemData) {
        BaseDamage = damage;
        BaseCoolDown = coolDown;
        BaseCriticalChance = criticalChance;
        ItemData = itemData;
    }


    //TODO: format this to be part right justified
    public override string GetDescription() {
        string description = $" Damage: {Damage.ToString("N", CultureInfo.CurrentCulture)} Lvl: {DamageLevel}\n";
        description += $"CoolDown: {CoolDown} Lvl: {CoolDownLevel}\n";
        description += $"CriticalChange: {CriticalChance} Lvl: {CriticalChanceLevel}\n";

        return description;
    }


}
