using System;
using System.Collections.Generic;
using UnityEngine;

//Player Stats
//Health
//Defense



//Eventually Add inventory to hold weapon and other items

public class PlayerCharacter : MonoBehaviour, IDamageable {


    [field: SerializeField] public int CurrentHealth { get; private set; }

    [SerializeField] WeaponData weaponData;
    [SerializeField] GameObject weaponHolder;
    [SerializeField] GameObject inventoryUIPrefab;

    public event Action<int> OnHealthChange;

    public int BaseHealth { get; private set; }
    public int Defense { get => Inventory.GetDefenseFromArmor(); }

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
        HandleWeaponChange((WeaponItem)weaponData.CreateItem());

    }

    void HandleAttack() {

        Vector2 difference = Room.GetRoomOffset(controller.WorldMousePos, transform.position);
        float angle = (int)Vector2.Angle(difference, Vector2.right);
        if (difference.y < 0) {
            angle = -angle;
        }
        Direction direction = Utilities.GetDirectionFromAngle(angle);
        weaponInstance.Use(direction);
    }

    void HandleWeaponChange(WeaponItem weaponItem) {
        if (weaponInstance != null) {
            Destroy(weaponInstance.gameObject);
        }
        weaponInstance = Instantiate(weaponItem.ItemData.WeaponPrefab, weaponHolder.transform);
        weaponInstance.transform.localPosition = Vector3.zero;
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

