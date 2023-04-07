using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Collections;
using System;

public class InventoryUIController : MonoBehaviour {
    [SerializeField] GameObject inventorySlotsParent;

    [SerializeField] MouseFollower heldUIItemPrefab;
    [SerializeField] GameObject mainPanel;
    [SerializeField] Slot slotPrefab;

    [SerializeField] Slot scrapSlot;

    [Header("Equip Slots")]
    [SerializeField] Slot weaponSlot;
    [SerializeField] Slot headSlot;
    [SerializeField] Slot chestSlot;
    [SerializeField] Slot legSlot;
    [SerializeField] Slot bootSlot;

    Inventory inventory;

    Item weapon;
    List<Slot> slots;

    protected List<Item> items;
    protected PlayerCharacter playerCharacter;
    protected Item currentHeldItem;
    protected MouseFollower currentHeldUIItem;


    protected virtual void Awake() {
        items = new List<Item>();
        slots = inventorySlotsParent.GetComponentsInChildren<Slot>().ToList();
        playerCharacter = GameObject.FindObjectOfType<PlayerCharacter>();
        inventory = playerCharacter.Inventory;
        currentHeldUIItem = null;


        //Setup Armor Slots here
        weaponSlot.OnLeftClick += HandleWeaponSlotClick;

        //Incase other inventories dont have scrap slots
        if (scrapSlot != null) {
            scrapSlot.OnLeftClick += HandleScrapSlotClick;
        }

        if (inventory != null) {
            PopulateItemList();
            Display(slots, items, HandleSlotLeftClick, HandleSlotRightClick, inventorySlotsParent.transform);
        } else {
            Debug.Log("Player does not have an inventory, can not display Inventory");
        }

    }

    protected virtual void OnDestroy() {
        if (inventory != null) {

            //Deals with exiting inventory with item in hand
            if (currentHeldUIItem != null) {
                for (int i = 0; i < Inventory.InventorySize; i++) {
                    if (items[i] == null) {
                        items[i] = currentHeldItem;
                        break;
                    }
                }
            }

            SetInventoryOrder();
        }

    }


    protected virtual void PopulateItemList() {
        items.Clear();

        Debug.Log("Populating List");
        // Debug.Log("Items in info list " + inventory.GetItemInfoList().Count);
        foreach (Item item in inventory.GetItemList()) {
            items.Add(item);
        }
        weapon = inventory.CurrentWeapon;
    }

    protected void Display(List<Slot> slots, List<Item> items, Action<Slot> slotLeftClickHandler, Action<Slot> slotRightClickHandler, Transform parent) {
        Debug.Log("Displaying Items");
        foreach (Slot slot in slots) {
            Destroy(slot.gameObject);
        }

        slots.Clear();

        for (int i = 0; i < Inventory.InventorySize; i++) {
            Slot slot = Instantiate(slotPrefab, parent);

            slot.OnLeftClick += slotLeftClickHandler;
            slot.OnRightClick += slotRightClickHandler;

            slots.Add(slot);

            Item item = items[i];
            if (item != null) {
                slot.SetItem(item);
            }
        }

        weaponSlot.SetItem(weapon);
    }

    void HandleSlotRightClick(Slot slot) {
        Debug.Log("Handle slot right click");
        Item slotItem = slot.Item;

        IConsumable consumable = slotItem as IConsumable;
        if (consumable != null) {
            if(consumable.Consume(playerCharacter)) {
                slotItem.Quantity--;
                if (slotItem.Quantity <= 0) {
                    items[slots.IndexOf(slot)] = null;
                }

            }
        }



        Display(slots, items, HandleSlotLeftClick, HandleSlotRightClick, inventorySlotsParent.transform);
    }

    void HandleSlotLeftClick(Slot slot) {
        Debug.Log("Handle slot click");
        Item slotItem = slot.Item;

        //Removes the old item info from the slot
        if (currentHeldUIItem == null) {
            items[items.IndexOf(slotItem)] = null;
        }
        //Changes the item slot to the item currently held
        else {
            items[slots.IndexOf(slot)] = currentHeldItem;
            Destroy(currentHeldUIItem.gameObject);
        }

        //If the slot held an item, create a held item
        if (slotItem != null) {
            currentHeldItem = slotItem;
            currentHeldUIItem = CreateHeldUIItem(slot);
        }

        //Otherwise set the held item to null
        else {
            currentHeldUIItem = null;
            currentHeldItem = null;
        }

        Display(slots, items, HandleSlotLeftClick, HandleSlotRightClick, inventorySlotsParent.transform);
    }





    void HandleWeaponSlotClick(Slot slot) {

        //Prevents the player from unequipping their last weapon
        // if (currentHeldUIItem == null) return;

        //Item is not a weapon

        Item slotItem = slot.Item;

        if (currentHeldItem == null) {
            weapon = null;
        } else {
            ItemData itemData = ResourceManager.Instance.GetItemData(currentHeldItem.ItemName);
            if (itemData.ItemType == ItemType.Weapon) {
                weapon = currentHeldItem;
                Destroy(currentHeldUIItem.gameObject);
            }
        }

        if (slotItem != null) {
            currentHeldItem = slotItem;
            currentHeldUIItem = CreateHeldUIItem(slot);
        } else {
            currentHeldUIItem = null;
            currentHeldItem = null;
        }



        //slot.SetItem(weapon);

        SetInventoryOrder();

        inventory.SetWeapon((WeaponItem)weapon);

        Display(slots, items, HandleSlotLeftClick, HandleSlotRightClick, inventorySlotsParent.transform);
    }

    void HandleScrapSlotClick(Slot slot) {
        if (currentHeldUIItem == null) return;


        ItemData itemData = ResourceManager.Instance.GetItemData(currentHeldItem.ItemName);

        //Cant scrap your last weapon
        if (itemData.ItemType == ItemType.Weapon && !ItemsContainWeapon()) return;


        //Item is not a weapon or armor
        if (itemData.ItemType != ItemType.Weapon && itemData.ItemType != ItemType.Armor) return;


        Destroy(currentHeldUIItem.gameObject);
        currentHeldUIItem = null;

        WeaponItem weaponItem = (WeaponItem)currentHeldItem;
        playerCharacter.AddWeaponScraps(weaponItem.GetScrapValue());

        currentHeldItem = null;
    }

    protected MouseFollower CreateHeldUIItem(Slot slot) {
        MouseFollower UIItem = Instantiate(heldUIItemPrefab, mainPanel.transform, false);
        UIItem.transform.SetAsLastSibling();
        UIItem.GetComponent<RectTransform>().position = slot.GetComponent<RectTransform>().position;
        UIItem.GetComponent<Image>().sprite = ResourceManager.Instance.GetItemData(slot.Item.ItemName).Sprite;


        return UIItem;
    }


    void SetInventoryOrder() {
        Debug.Log("Setting Inventory Order");
        inventory.SetItems(items);
    }

    bool ItemsContainWeapon() {
        if (weapon != null) return true;

        foreach (Item item in items) {
            if (item != null) {
                ItemData itemData = ResourceManager.Instance.GetItemData(item.ItemName);
                if (itemData.ItemType == ItemType.Weapon) {
                    return true;
                }
            }
        }
        return false;
    }

}
