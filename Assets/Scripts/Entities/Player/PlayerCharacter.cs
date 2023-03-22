using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;



class PlayerSaveData {
    public int WeaponScraps;
}


public class PlayerCharacter : MonoBehaviour, IDamageable, ISaveLoad {

    [SerializeField] bool canDie = true;
    [field: SerializeField] public int CurrentHealth { get; private set; }

    [Required]
    [SerializeField] WeaponData weaponData;
    [SerializeField] GameObject weaponHolder;
    [SerializeField] GameObject inventoryUIPrefab;
    [SerializeField] GameObject inventoryChestUIPrefab;
    [SerializeField] GameObject dialogManagerPrefab;
    [SerializeField] GameObject aimArrow;


    public event Action OnExitMenu;
    public event Action<int> OnHealthChange;
    public event Action OnDie;

    public int WeaponScraps { get; private set; }
    public int BaseHealth { get; private set; }
    public int Defense { get => Inventory.GetDefenseFromArmor(); }

    PlayerController controller;

    public Inventory Inventory { get; private set; }

    GameObject currentMenu = null;
    bool canExitMenu;

    Direction currentDirection;
    Weapon weaponInstance;

    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnLeftClick += HandleAttack;
        controller.OnInventory += HandleInventory;
        controller.OnExitMenu += ExitMenu;
        controller.OnInteract += HandleInteract;
        Inventory = new Inventory();
        Inventory.OnWeaponChange += SpawnWeapon;
        WeaponItem starterWeapon = (WeaponItem)weaponData.CreateItem(1);
        Inventory.SetWeapon(starterWeapon);
        BaseHealth = CurrentHealth;
    }

    void Update() {
        UpdateDirection();
        aimArrow.transform.eulerAngles = Utilities.GetAngleFromDirection(currentDirection);

    }


    public void TakeDamage(int damageDealt, DamageType damageType, float knockback = 1) {
        //TODO: Add damage type

        CurrentHealth -= damageDealt;
        OnHealthChange?.Invoke(CurrentHealth);
        if (CurrentHealth <= 0 && canDie) {
            Die();
        }
    }

    public void Heal(int healAmount) {
        CurrentHealth += healAmount;
        if (CurrentHealth > BaseHealth) {
            CurrentHealth = BaseHealth;
        }
        OnHealthChange?.Invoke(CurrentHealth);
    }



    /// <summary>
    /// This is used to show a menu, and will return null if a menu is already open
    /// </summary>
    /// <param name="menuPrefab"></param>
    /// <returns></returns>
    public GameObject ShowMenu(GameObject menuPrefab, bool canExit = true) {

        //Return if there is already a menu being displayed
        if (currentMenu != null) {
            ExitMenu();
        }

        GameObject menu = Instantiate(menuPrefab);
        currentMenu = menu;
        canExitMenu = canExit;
        Debug.Log(menu.name);
        return menu;
    }


    /// <summary>
    /// This is used to exit the current menu on the player
    /// </summary>
    public void ExitMenu() {
        //Return if there is no menu
        if (currentMenu == null) {
            return;
        }
        //Return if the menu cannot be exited by using esc
        if (!canExitMenu) {
            return;
        }

        Debug.Log("Exiting Menu");
        Destroy(currentMenu);
        currentMenu = null;

        Save();
        SaveManager.Instance.Save();
    }



    void HandleAttack() {
        //Return if there is no weapon
        if (weaponInstance == null) {
            Debug.Log("No weapon equipped");
            return;
        }
        if (currentMenu != null) {
            return;
        }

        weaponInstance.Use(currentDirection);
    }


    /// <summary>
    /// Spawns the weapon as a gameobject and initializes it based on the corresponding weapon item
    /// </summary>
    /// <param name="weaponItem"></param>
    void SpawnWeapon(WeaponItem weaponItem) {
        if (weaponInstance != null) {
            Destroy(weaponInstance.gameObject);
        }

        WeaponData itemData = (WeaponData)ResourceManager.Instance.GetItemData(weaponItem.ItemName);
        weaponInstance = Instantiate(itemData.WeaponPrefab, weaponHolder.transform);
        weaponHolder.transform.localPosition = Vector3.zero;
        weaponHolder.transform.localRotation = Quaternion.identity;
        weaponInstance.transform.localPosition = Vector3.zero;
        weaponInstance.transform.localRotation = Quaternion.identity;
        weaponInstance.Initialize(true, (int)weaponItem.Damage.CurrentValue, weaponItem.Speed.CurrentValue, weaponItem.CriticalChance.CurrentValue);

    }

    //Handles the inventory button being pressed
    void HandleInventory() {
        if (currentMenu == null) {
            ShowMenu(inventoryUIPrefab);
        }

    }

    void HandleInteract(IInteractable interactable) {
        interactable.Interact(this);
    }


    void Die() {
        OnDie?.Invoke();
        SceneManager.LoadScene("Prototype");
    }



    /// <summary>
    /// Updates the direction the mouse is in relation to the player
    /// </summary>
    void UpdateDirection() {
        Vector2 difference = controller.WorldMousePos.Difference(transform.position);
        float angle = (int)Vector2.Angle(difference, Vector2.right);
        if (difference.y < 0) {
            angle = -angle;
        }
        currentDirection = Utilities.GetDirectionFromAngle(angle);
    }

    public void Save() {
        Debug.Log("Saving Player");
        SaveManager.Instance.SetData("Player", new PlayerSaveData() { WeaponScraps = WeaponScraps });
    }

    public void Load() {
        Debug.Log("Loading Player");
        PlayerSaveData data = SaveManager.Instance.GetData<PlayerSaveData>("Player");
        if (data != null) {
            WeaponScraps = data.WeaponScraps;
            Inventory.Load();
            if (Inventory.CurrentWeapon != null) {
                SpawnWeapon(Inventory.CurrentWeapon);
            }
        }
    }
}

