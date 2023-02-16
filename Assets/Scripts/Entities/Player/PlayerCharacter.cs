using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Player Stats
//Health
//Defense



//Eventually Add inventory to hold weapon and other items

public class PlayerCharacter : MonoBehaviour, IDamageable {


    [field: SerializeField] public int CurrentHealth { get; private set; }

    [SerializeField] WeaponData weaponData;
    [SerializeField] GameObject weaponHolder;
    [SerializeField] GameObject inventoryUIPrefab;
    [SerializeField] GameObject inventoryChestUIPrefab;

    public event Action OnExitMenu;
    public event Action<int> OnHealthChange;
    public event Action OnDie;

    public int BaseHealth { get; private set; }
    public int Defense { get => Inventory.GetDefenseFromArmor(); }

    PlayerController controller;

    public Inventory Inventory { get; private set; }

    GameObject inventoryUI;
    GameObject inventoryChestUI;

    bool inventoryOn;

    Weapon weaponInstance;

    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnAttack += HandleAttack;
        controller.OnInventory += HandleInventory;
        controller.OnExitMenu += () => OnExitMenu?.Invoke();
        controller.OnInteract += HandleInteract;
        Inventory = new Inventory();
        Inventory.OnWeaponChange += HandleWeaponChange;
        HandleWeaponChange((WeaponItem)weaponData.CreateItem());

    }


    public void TakeDamage(int damageDealt) {
        CurrentHealth -= damageDealt;
        OnHealthChange?.Invoke(CurrentHealth);
        if (CurrentHealth <= 0) {
            Die();
        }
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

    void HandleInteract(IInteractable interactable) {
        interactable.Interact(this);
    }

    public void HandleOpeningChest(List<Item> items) {

        Debug.Log(items);
    }

    void EquipWeapon(Weapon weapon) {

        //Add some way of setting stats

    }

    void Die() {
        OnDie?.Invoke();
        SceneManager.LoadScene("Prototype");
    }

    void ExitMenu() {

    }



}

