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
    [SerializeField] GameObject dialogManagerPrefab;
    [SerializeField] GameObject aimArrow;


    public event Action OnExitMenu;
    public event Action<int> OnHealthChange;
    public event Action OnDie;

    public int BaseHealth { get; private set; }
    public int Defense { get => Inventory.GetDefenseFromArmor(); }

    PlayerController controller;

    public Inventory Inventory { get; private set; }

    GameObject currentMenu;


    bool inventoryOn;
    Direction currentDirection;
    Weapon weaponInstance;

    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnClick += HandleAttack;
        controller.OnInventory += HandleInventory;
        controller.OnExitMenu += ExitMenu;
        controller.OnInteract += HandleInteract;
        Inventory = new Inventory();
        Inventory.OnWeaponChange += HandleWeaponChange;
        WeaponItem starterWeapon = (WeaponItem)weaponData.CreateItem(0);
        Inventory.SetWeapon(starterWeapon);

    }

    void Update() {
        UpdateDirection();
        aimArrow.transform.eulerAngles = Utilities.GetAngleFromDirection(currentDirection);

    }


    public void TakeDamage(int damageDealt) {
        CurrentHealth -= damageDealt;
        OnHealthChange?.Invoke(CurrentHealth);
        if (CurrentHealth <= 0) {
            Die();
        }
    }


    void HandleAttack() {

        weaponInstance.Use(currentDirection);
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

    public GameObject ShowDialog(Dialog dialog, string name) {
        if (currentMenu != null) {
            return null;
        }
        currentMenu = Instantiate(dialogManagerPrefab);
        DialogManager dialogManager = currentMenu.GetComponent<DialogManager>();
        dialogManager.SetDialog(dialog, name);
        dialogManager.OnDialogComplete += (x) => {
            Destroy(dialogManager.gameObject);
            currentMenu = null;
        };
        return dialogManager.gameObject;
    }

    void ExitMenu() {
        if (currentMenu == null) {
            return;
        }

        Debug.Log("Exiting Menu");
        Destroy(currentMenu);
        currentMenu = null;
    }

    void UpdateDirection() {
        Vector2 difference = controller.WorldMousePos.Difference(transform.position);
        float angle = (int)Vector2.Angle(difference, Vector2.right);
        if (difference.y < 0) {
            angle = -angle;
        }
        currentDirection = Utilities.GetDirectionFromAngle(angle);
    }



}

