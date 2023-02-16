using System;
using UnityEngine;



[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ItemData {
    public Weapon WeaponPrefab;

    public WeaponType WeaponType;
    public MinMaxInt Damage;
    public MinMaxFloat CoolDown;
    public MinMaxFloat CriticalChance;

    public override Item CreateItem() {
        return new WeaponItem(this,
        Damage.GetRandomValue(),
        (float)Math.Truncate(CoolDown.GetRandomValue() * 100) / 100,
        (float)Math.Truncate(CriticalChance.GetRandomValue() * 100) / 100);
    }
}


public enum WeaponType {
    Sword, Dagger, Hammer
}