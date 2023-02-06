using UnityEngine;



[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ItemData {
    public Weapon WeaponPrefab;

    public WeaponType WeaponType;
    public MinMaxFloat Damage;
    public MinMaxFloat CoolDown;
    public MinMaxFloat CriticalChance;

    public override Item CreateItem() {
        return new WeaponItem(this, Damage.GetRandomValue(), CoolDown.GetRandomValue(), CriticalChance.GetRandomValue());
    }
}


public enum WeaponType {
    Sword, Dagger, Hammer
}