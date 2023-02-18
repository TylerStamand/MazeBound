using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



//TODO: ADD A BETTER WAY TO MANAGE STATE

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

    GameObject currentMenu;

    bool inventoryOn;

    Weapon weaponInstance;

    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnAttack += HandleAttack;
        controller.OnInventory += HandleInventory;
        controller.OnExitMenu += ExitMenu;
        controller.OnInteract += HandleInteract;
        Inventory = new Inventory();
        Inventory.OnWeaponChange += HandleWeaponChange;
        WeaponItem starterWeapon = (WeaponItem)weaponData.CreateItem();
        Inventory.SetWeapon(starterWeapon);

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
        weaponInstance.Initialize(true, weaponItem.Damage, weaponItem.Speed, weaponItem.CriticalChance);

    }

    void HandleInventory() {
        if (currentMenu == null) {
            ShowMenu(inventoryUIPrefab);
        }

    }

    void HandleInteract(IInteractable interactable) {
        interactable.Interact(this);
    }


    void EquipWeapon(Weapon weapon) {

        //Add some way of setting stats

    }

    void Die() {
        OnDie?.Invoke();
        SceneManager.LoadScene("Prototype");
    }

    public GameObject ShowMenu(GameObject menuPrefab) {
        if (currentMenu != null) {
            return null;
        }
        GameObject menu = Instantiate(menuPrefab);
        currentMenu = menu;
        return menu;
    }

    void ExitMenu() {
        if (currentMenu == null) {
            return;
        }

        Debug.Log("Exiting Menu");
        Destroy(currentMenu);
        currentMenu = null;
    }



}

