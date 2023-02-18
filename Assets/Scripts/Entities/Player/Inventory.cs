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
    List<Item> items;

    //Will probably need events to show change for HUD depending on implementation

    public WeaponItem CurrentWeapon { get; private set; }

    public ArmorItem Head { get; private set; }
    public ArmorItem Chest { get; private set; }
    public ArmorItem Leg { get; private set; }
    public ArmorItem Boot { get; private set; }

    public List<Potion> Potions { get; set; }


    public bool IsFull => itemLookup.Count == InventorySize;

    public Inventory() {
        items = new List<Item>();
        itemLookup = new Dictionary<string, Item>();
        for (int i = 0; i < InventorySize; i++) {
            items.Add(null);
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

    public void SetItemOrder(List<string> itemIDs) {
        items.Clear();
        foreach (String id in itemIDs) {
            if (id != null) {
                itemLookup.TryGetValue(id, out Item item);
                items.Add(item);
            } else {

                items.Add(null);
            }
        }

    }

    void EquipItem(ref Item equippedItem, Item itemToSet) {
        if (equippedItem != null) {
            AddItem(equippedItem);
        }

        //Removes item from item list but does not drop it from the lookup
        if (items.IndexOf(itemToSet) != -1) {
            items[items.IndexOf(itemToSet)] = null;
        }
        equippedItem = itemToSet;
    }


}
