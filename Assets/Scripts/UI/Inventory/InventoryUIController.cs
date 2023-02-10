using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class InventoryUIController : MonoBehaviour {
    [SerializeField] GameObject inventorySlotsParent;
    [SerializeField] Slot weaponSlot;
    [SerializeField] Slot slotPrefab;
    [SerializeField] MouseFollower heldUIItemPrefab;

    // [SerializeField] GameObject backPanel;
    [SerializeField] GameObject mainPanel;

    PlayerCharacter playerCharacter;
    Inventory inventory;

    List<Item> items;
    Item weapon;
    List<Slot> slots;

    Item currentHeldItem;
    MouseFollower currentHeldUIItem;


    void Awake() {
        items = new List<Item>();
        slots = inventorySlotsParent.GetComponentsInChildren<Slot>().ToList();
        playerCharacter = GameObject.FindObjectOfType<PlayerCharacter>();
        inventory = playerCharacter.Inventory;
        currentHeldUIItem = null;
        weaponSlot.OnClick += HandleWeaponSlotClick;


        if (inventory != null) {


            inventory.OnItemAdded += AddItemToList;
            inventory.OnWeaponChange += HandleWeaponItemUpdate;


            PopulateItemList();
            Display();
        } else {
            Debug.Log("Player does not have an inventory, can not display Inventory");
        }

    }

    void OnDestroy() {
        if (inventory != null) {
            inventory.OnItemAdded -= AddItemToList;
            inventory.OnWeaponChange -= HandleWeaponItemUpdate;

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


    void PopulateItemList() {
        items.Clear();

        Debug.Log("Populating List");
        // Debug.Log("Items in info list " + inventory.GetItemInfoList().Count);
        foreach (Item item in inventory.GetItemList()) {

            items.Add(item);

        }
        Debug.Log(items.Count);
        // Debug.Log(items[0].ItemID.Value.ToString());
        weapon = inventory.CurrentWeapon;
    }

    void Display() {
        Debug.Log("Displaying Items");
        foreach (Slot slot in slots) {
            Destroy(slot.gameObject);
        }

        slots.Clear();



        for (int i = 0; i < Inventory.InventorySize; i++) {
            Slot slot = Instantiate(slotPrefab, inventorySlotsParent.transform);

            slot.OnClick += HandleSlotClick;

            slots.Add(slot);

            Item item = items[i];
            if (item != null) {
                Debug.Log("Item Does not equal null");
                slot.SetItem(item);
            }
        }


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

        Display();


    }

    // void HandleItemDrop() {
    //     if (currentHeldUIItem != null) {
    //         playerControllerServer.DropItemServerRpc(currentHeldItemInfo);
    //         Destroy(currentHeldUIItem.gameObject);
    //         currentHeldUIItem = null;
    //         currentHeldItemInfo = new ItemInfo();
    //         backPanel.GetComponent<Button>().onClick.RemoveListener(HandleItemDrop);
    //     }

    // }


    void HandleWeaponSlotClick(Slot slot) {
        //Debug.Log("Handling Weapon Slot Click");
        if (currentHeldUIItem == null) return;

        // ItemData itemData = ResourceManager.Instance.GetItemData(currentHeldItemInfo.Name.Value.ToString());

        //Item is not a weapon
        if (currentHeldItem.ItemData.ItemType != ItemType.Weapon) return;

        Item oldWeapon = slot.Item;

        Destroy(currentHeldUIItem.gameObject);
        currentHeldUIItem = null;

        //Checks if the weapon slot had an item in it
        if (oldWeapon != null) {
            currentHeldUIItem = CreateHeldUIItem(slot);
        }

        Item newWeapon = currentHeldItem;


        currentHeldItem = oldWeapon;

        slot.SetItem(newWeapon);

        SetInventoryOrder();

        inventory.SetWeapon(newWeapon);

    }

    MouseFollower CreateHeldUIItem(Slot slot) {
        MouseFollower UIItem = Instantiate(heldUIItemPrefab);
        // SceneManager.MoveGameObjectToScene(UIItem.gameObject, SceneManager.GetSceneByName("Inventory"));
        UIItem.transform.SetParent(mainPanel.transform);
        UIItem.transform.SetAsLastSibling();
        UIItem.GetComponent<RectTransform>().position = slot.GetComponent<RectTransform>().position;
        UIItem.GetComponent<Image>().sprite = slot.Item.ItemData.Sprite;


        return UIItem;
    }

    void AddItemToList(Item item) {

        //Debug.Log("Added Item To List");
        if (items.IndexOf(item) != -1) return;
        if (item.ItemID.ToString() == currentHeldItem.ItemID.ToString()) return;
        for (int i = 0; i < Inventory.InventorySize; i++) {
            if (items[i].ItemID.Count() == 0) {
                items[i] = item;
                break;
            }
        }
        Display();
    }


    void HandleWeaponItemUpdate(Item weapon) {
        //Debug.Log("Handling Weapon Item Update");
        this.weapon = weapon;
        Display();
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
