using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    public static int InventorySize = 35;

    public event Action<Item> OnItemAdded;
    public event Action<WeaponItem> OnWeaponChange;

    Dictionary<string, Item> itemLookup { get; set; } = new Dictionary<string, Item>();
    List<Item> items;

    //Will probably need events to show change for HUD depending on implementation

    public WeaponItem CurrentWeapon { get; set; }

    public Armor Head { get; set; }
    public Armor Chest { get; set; }
    public Armor Leg { get; set; }
    public Armor Boots { get; set; }

    public List<Potion> Potions { get; set; }


    public bool IsFull => itemLookup.Count == InventorySize;

    public float GetDefenseFromArmor() {
        return 0;
    }

    public void RemoveItem(string itemID) {
        Item itemToRemove = itemLookup[itemID];
        itemLookup.Remove(itemID);
        int indexOfItemToRemove = items.IndexOf(itemToRemove);
        items[indexOfItemToRemove] = null;


    }

    public void AddItem(Item item) {
        itemLookup.TryAdd(item.ItemID, item);


        for (int i = 0; i < InventorySize; i++) {
            if (items[i] == null) {
                items[i] = item;
                Debug.Log("Item Added Successfully");
                break;
            }
        }
    }

    public Item GetItem(string itemID) {
        return itemLookup[itemID];
    }

    public List<Item> GetItemList() {
        return items;
    }


    public void SetWeapon(string itemID) {
        Debug.Log("Setting Weapon");
        Item item = itemLookup[itemID];

        if (item == null) {
            Debug.LogWarning("Trying to set weapon but item was not found in inventory");
        }

        WeaponItem newWeapon;
        if (item is WeaponItem) {
            newWeapon = (WeaponItem)item;
        } else {
            Debug.LogWarning("Item is not a weapon");
            return;
        }

        if (CurrentWeapon != null) {
            AddItem(CurrentWeapon);
        }

        //Removes item from item list but does not drop it from the lookup
        Item itemToRemove = itemLookup[itemID];
        if (items.IndexOf(itemToRemove) != -1) {
            items[items.IndexOf(itemToRemove)] = null;
        }

        CurrentWeapon = newWeapon;
        OnWeaponChange?.Invoke(newWeapon);

    }

    public void SetItemOrder(int itemIndexFrom, int itemIndexTo) {
        Item itemTemp = items[itemIndexTo];
        items[itemIndexTo] = items[itemIndexFrom];
        items[itemIndexFrom] = itemTemp;

    }




}
