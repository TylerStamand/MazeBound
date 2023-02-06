using UnityEngine;



[CreateAssetMenu(fileName ="WeaponData", menuName = "ScriptableObjects/WeaponData" )]
public class WeaponData : ItemData
{
    public Weapon WeaponPrefab;
    
    public WeaponType WeaponType;
    public MinMaxFloat Damage;
    public MinMaxFloat CoolDown;
    public MinMaxFloat ProjectileSpeed;

    public override Item CreateItem() {
        return new WeaponItem(Name, Damage.GetRandomValue(), CoolDown.GetRandomValue(), ProjectileSpeed.GetRandomValue());
    }
}


public enum WeaponType {
    Sword, Projectile
}