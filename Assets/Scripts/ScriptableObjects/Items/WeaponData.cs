using System;
using UnityEngine;



[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ItemData {
    //TODO: Change visibility to match ItemData

    [field: SerializeField] public Weapon WeaponPrefab { get; private set; }

    [field: SerializeField] public WeaponType WeaponType { get; private set; }
    [field: SerializeField] public MinMaxInt Damage { get; private set; }
    [field: SerializeField] public MinMaxFloat Speed { get; private set; }
    [field: SerializeField] public MinMaxFloat CriticalChance { get; private set; }

    public override Item CreateItem(float scale) {
        return new WeaponItem(this,
        (int)(Damage.GetRandomValue() * scale) + Damage.MinValue,
        (float)(Math.Truncate(Speed.GetRandomValue() * 100 * scale) / 100) + Speed.MinValue,
        (float)(Math.Truncate(CriticalChance.GetRandomValue() * 100 * scale) / 100) + CriticalChance.MinValue
        );
    }
}


public enum WeaponType {
    Sword, Spear, Hammer
}