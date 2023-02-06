using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player Stats
//Health
//Defense



//Eventually Add inventory to hold weapon and other items

public class PlayerCharacter : MonoBehaviour, IDamageable {
    [SerializeField] Weapon weaponPrefab;
    [SerializeField] GameObject weaponHolder;
    PlayerController controller;

    Inventory inventory;

    Weapon weaponInstance;

    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnAttack += HandleAttack;
        inventory = new Inventory();
        inventory.OnWeaponChange += HandleWeaponChange;
        EquipWeapon(weaponPrefab);

    }

    void HandleAttack() {

        Vector2 difference = Room.GetRoomOffset(controller.MousePos, transform.position);
        float angle = (int)Vector2.Angle(difference, Vector2.right);
        if (difference.y < 0) {
            angle = -angle;
        }
        Debug.Log(angle);
        Direction direction = Utilities.GetDirectionFromAngle(angle);
        weaponInstance.Use(direction);
    }

    void HandleWeaponChange(WeaponItem weaponItem) {
        weaponInstance = Instantiate(weaponItem.ItemData.WeaponPrefab, Vector3.zero, Quaternion.identity, weaponHolder.transform);
        weaponInstance.Initialize(true, weaponItem.Damage, weaponItem.CoolDown, weaponItem.CriticalChance);
    }

    void EquipWeapon(Weapon weapon) {

        //Add some way of setting stats

    }

    public void TakeDamage(float damageDealt) {

    }
}

