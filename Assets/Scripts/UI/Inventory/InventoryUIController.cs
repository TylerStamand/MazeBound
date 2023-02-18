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

    [Header("Equip Slots")]
    [SerializeField] Slot weaponSlot;
    [SerializeField] Slot headSlot;
    [SerializeField] Slot chestSlot;
    [SerializeField] Slot legSlot;
    [SerializeField] Slot bootSlot;

    PlayerCharacter playerCharacter;
    Inventory inventory;

    List<Item> items;
    Item weapon;
    List<Slot> slots;

    protected Item currentHeldItem;
    protected MouseFollower currentHeldUIItem;


    protected virtual void Awake() {
        items = new List<Item>();
        slots = inventorySlotsParent.GetComponentsInChildren<Slot>().ToList();
        playerCharacter = GameObject.FindObjectOfType<PlayerCharacter>();
        inventory = playerCharacter.Inventory;
        currentHeldUIItem = null;


        weaponSlot.OnClick += HandleWeaponSlotClick;
        //Setup Armor Slots here

        if (inventory != null) {
            PopulateItemList();
            Display(slots, items, HandleSlotClick, inventorySlotsParent.transform);
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
                //Or Drop it
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

    protected void Display(List<Slot> slots, List<Item> items, Action<Slot> slotClickHandler, Transform parent) {
        Debug.Log("Displaying Items");
        foreach (Slot slot in slots) {
            Destroy(slot.gameObject);
        }

        slots.Clear();

        for (int i = 0; i < Inventory.InventorySize; i++) {
            Slot slot = Instantiate(slotPrefab, parent);

            slot.OnClick += slotClickHandler;

            slots.Add(slot);

            Item item = items[i];
            if (item != null) {
                Debug.Log("Item Does not equal null");
                slot.SetItem(item);
            }
        }

        //Maybe move this 
        if (weapon != null) {
            weaponSlot.SetItem(weapon);
        }
    }


    void HandleSlotClick(Slot slot) {
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

        Display(slots, items, HandleSlotClick, inventorySlotsParent.transform);
    }





    void HandleWeaponSlotClick(Slot slot) {
        //Debug.Log("Handling Weapon Slot Click");
        if (currentHeldUIItem == null) return;

        //Item is not a weapon
        if (currentHeldItem.ItemData.ItemType != ItemType.Weapon) return;

        Item oldWeapon = slot.Item;

        Destroy(currentHeldUIItem.gameObject);
        currentHeldUIItem = null;

        //Checks if the weapon slot had an item in it
        if (oldWeapon != null) {
            currentHeldUIItem = CreateHeldUIItem(slot);
        }

        weapon = currentHeldItem;


        currentHeldItem = oldWeapon;

        slot.SetItem(weapon);

        SetInventoryOrder();

        inventory.SetWeapon((WeaponItem)weapon);

        Display(slots, items, HandleSlotClick, inventorySlotsParent.transform);
    }

    protected MouseFollower CreateHeldUIItem(Slot slot) {
        MouseFollower UIItem = Instantiate(heldUIItemPrefab);
        UIItem.transform.SetParent(mainPanel.transform);
        UIItem.transform.SetAsLastSibling();
        UIItem.GetComponent<RectTransform>().position = slot.GetComponent<RectTransform>().position;
        UIItem.GetComponent<Image>().sprite = slot.Item.ItemData.Sprite;


        return UIItem;
    }


    void SetInventoryOrder() {
        List<string> itemIDs = new List<string>();
        foreach (Item item in items) {

            if (item == null)
                itemIDs.Add(null);
            else
                itemIDs.Add(item.ItemID);


        }
        inventory.SetItemOrder(itemIDs);
    }

}
