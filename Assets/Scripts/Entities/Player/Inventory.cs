using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    public static int InventorySize = 15;

    public event Action<Item> OnItemAdded;
    public event Action<WeaponItem> OnWeaponChange;
    public event Action<ArmorItem> OnArmorChange;

    Dictionary<string, Item> itemLookup { get; set; } = new Dictionary<string, Item>();
    public List<Item> Items { get; private set; }

    //Will probably need events to show change for HUD depending on implementation

    public WeaponItem CurrentWeapon { get; private set; }

    public ArmorItem Head { get; private set; }
    public ArmorItem Chest { get; private set; }
    public ArmorItem Leg { get; private set; }
    public ArmorItem Boot { get; private set; }

    public List<Potion> Potions { get; set; }


    public bool IsFull => itemLookup.Count == InventorySize;

    public Inventory() {
        Items = new List<Item>();
        itemLookup = new Dictionary<string, Item>();
        for (int i = 0; i < InventorySize; i++) {
            Items.Add(null);
        }
    }


    public int GetDefenseFromArmor() {
        return Head.CurrentDefense
        + Chest.CurrentDefense
        + Leg.CurrentDefense
        + Boot.CurrentDefense;
    }

    public void RemoveItem(string itemID) {
        Item itemToRemove = itemLookup[itemID];
        itemLookup.Remove(itemID);
        int indexOfItemToRemove = Items.IndexOf(itemToRemove);
        Items[indexOfItemToRemove] = null;


    }

    public void AddItem(Item item) {
        itemLookup.TryAdd(item.ItemID, item);


        for (int i = 0; i < InventorySize; i++) {
            if (Items[i] == null) {
                Items[i] = item;
                Debug.Log("Item Added Successfully");
                break;
            }
        }
    }

    public Item GetItem(string itemID) {
        return itemLookup[itemID];
    }

    public List<Item> GetItemList() {
        return Items;
    }


    public void SetWeapon(WeaponItem newWeapon) {
        Debug.Log("Setting Weapon");

        Item temp = CurrentWeapon;
        EquipItem(ref temp, newWeapon);
        CurrentWeapon = (WeaponItem)temp;

        OnWeaponChange?.Invoke(newWeapon);

    }

    public void SetArmorPiece(ArmorItem armorItem) {
        Item temp;
        switch (armorItem.Piece) {
            case ArmorItem.ArmorPiece.Head:
                //Temp is because get/set cant be passed as ref
                temp = Head;
                EquipItem(ref temp, armorItem);
                Head = (ArmorItem)temp;
                break;
            case ArmorItem.ArmorPiece.Chest:
                temp = Chest;
                EquipItem(ref temp, armorItem);
                Chest = (ArmorItem)temp;
                break;
            case ArmorItem.ArmorPiece.Leg:
                temp = Leg;
                EquipItem(ref temp, armorItem);
                Leg = (ArmorItem)temp;
                break;
            case ArmorItem.ArmorPiece.Boot:
                temp = Boot;
                EquipItem(ref temp, armorItem);
                Boot = (ArmorItem)temp;
                break;
        }

        Debug.Log("Armor Set");
        OnArmorChange?.Invoke(armorItem);
    }

    public void SetItems(List<Item> items) {
        this.Items.Clear();
        foreach (Item item in items) {
            this.Items.Add(item);
        }



    }

    void EquipItem(ref Item equippedItem, Item itemToSet) {
        if (equippedItem != null) {
            AddItem(equippedItem);
        }

        //Removes item from item list but does not drop it from the lookup
        if (Items.IndexOf(itemToSet) != -1) {
            Items[Items.IndexOf(itemToSet)] = null;
        }
        equippedItem = itemToSet;
    }


}
