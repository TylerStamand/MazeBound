using System;
using NaughtyAttributes;
using UnityEngine;



[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ItemData {

    [field: SerializeField] public Weapon WeaponPrefab { get; private set; }

    [field: SerializeField] public WeaponType WeaponType { get; private set; }

    [field: SerializeField] public MinMaxInt Damage { get; private set; }
    [field: SerializeField] public MinMaxFloat Speed { get; private set; }
    [field: SerializeField] public MinMaxFloat CriticalChance { get; private set; }
    [field: SerializeField] public int UpgradeCostBase { get; private set; }
    [field: SerializeField] public float UpgradeCostBaseMultiplier { get; private set; }
    [field: SerializeField] public float UpgradeValueMultiplier { get; private set; }
    [field: SerializeField] public float UpgradeCostMultiplier { get; private set; }


    public override Item CreateItem(float scale) {
        Debug.Log("Creating WeaponItem with scale: " + scale);
        return new WeaponItem(Name,
        (int)((Damage.GetRandomValue() - Damage.MinValue) * scale) + Damage.MinValue,
        (float)(Math.Truncate((Speed.GetRandomValue() - Damage.MinValue) * 100 * scale) / 100) + Speed.MinValue,
        (float)(Math.Truncate((CriticalChance.GetRandomValue() - CriticalChance.MinValue) * 100 * scale) / 100) + CriticalChance.MinValue,
        UpgradeCostBase + (int)(UpgradeCostBase * UpgradeCostBaseMultiplier * scale - 1));
    }
}


public enum WeaponType {
    Sword, Spear, Hammer
}