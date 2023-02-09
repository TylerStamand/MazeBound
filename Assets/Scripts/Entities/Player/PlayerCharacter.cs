using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player Stats
//Health
//Defense



//Eventually Add inventory to hold weapon and other items

public class PlayerCharacter : MonoBehaviour, IDamageable {
    [SerializeField] WeaponData weaponPrefab;
    [SerializeField] GameObject weaponHolder;
    [SerializeField] GameObject inventoryUIPrefab;
    PlayerController controller;

    public Inventory Inventory { get; private set; }
    GameObject inventoryUI;

    bool inventoryOn;

    Weapon weaponInstance;

    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnAttack += HandleAttack;
        controller.OnInventory += HandleInventory;
        Inventory = new Inventory();
        Inventory.OnWeaponChange += HandleWeaponChange;
        HandleWeaponChange((WeaponItem)weaponPrefab.CreateItem());

    }

    void HandleAttack() {

        Vector2 difference = Room.GetRoomOffset(controller.MousePos, transform.position);
        float angle = (int)Vector2.Angle(difference, Vector2.right);
        if (difference.y < 0) {
            angle = -angle;
        }
        Direction direction = Utilities.GetDirectionFromAngle(angle);
        weaponInstance.Use(direction);
    }

    void HandleWeaponChange(WeaponItem weaponItem) {

        weaponInstance = Instantiate(weaponItem.ItemData.WeaponPrefab, Vector3.zero, Quaternion.identity, weaponHolder.transform);
        weaponInstance.Initialize(true, weaponItem.Damage, weaponItem.CoolDown, weaponItem.CriticalChance);
        Inventory.AddItem(weaponItem);
    }

    void HandleInventory() {
        if (!inventoryOn) {
            inventoryUI = Instantiate(inventoryUIPrefab);
            inventoryOn = true;
        } else {
            Destroy(inventoryUI);
            inventoryOn = false;

        }

    }

    void EquipWeapon(Weapon weapon) {

        //Add some way of setting stats

    }

    public void TakeDamage(float damageDealt) {

    }
}

