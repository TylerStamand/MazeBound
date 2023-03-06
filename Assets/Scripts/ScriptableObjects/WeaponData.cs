using System;
using UnityEngine;



[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ItemData {
    public Weapon WeaponPrefab;

    public WeaponType WeaponType;
    public MinMaxInt Damage;
    public MinMaxFloat Speed;
    public MinMaxFloat CriticalChance;

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