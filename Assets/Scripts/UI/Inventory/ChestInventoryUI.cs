using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class ChestInventoryUI : InventoryUIController {

    [SerializeField] GameObject chestSlotsParent;


    List<Slot> chestSlots = new List<Slot>();
    List<Item> chestItems = new List<Item>();
    Chest chest;

    protected override void Awake() {
        base.Awake();
        chestSlots = chestSlotsParent.GetComponentsInChildren<Slot>().ToList();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        chest.Items.Clear();
        foreach (Item item in chestItems) {
            chest.Items.Add(item);
        }
    }


    public void SetChest(Chest chest) {
        this.chest = chest;
        PopulateItemList();
        Display(chestSlots, chestItems, HandleSlotLeftClick, HandleSlotRightClick, chestSlotsParent.transform);
    }

    //If there is time, fix this so that it is not a copy paste of the other method fromm InventoryUIController
    void HandleSlotRightClick(Slot slot) {
        Debug.Log("Handle slot right click");
        Item slotItem = slot.Item;


        IConsumable consumable = slotItem as IConsumable;
        if (consumable != null) {
            if (consumable.Consume(playerCharacter)) {
                slotItem.Quantity--;
                if (slotItem.Quantity <= 0) {
                    chestItems[chestSlots.IndexOf(slot)] = null;
                }

            }
        }



        Display(chestSlots, chestItems, HandleSlotLeftClick, HandleSlotRightClick, chestSlotsParent.transform);
    }

    void HandleSlotLeftClick(Slot slot) {
        Item slotItem = slot.Item;

        //Removes the old item info from the slot
        if (currentHeldUIItem == null) {
            chestItems[chestItems.IndexOf(slotItem)] = null;
        }
        //Changes the item slot to the item currently held
        else {
            chestItems[chestSlots.IndexOf(slot)] = currentHeldItem;
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

        Display(chestSlots, chestItems, HandleSlotLeftClick, HandleSlotRightClick, chestSlotsParent.transform);
    }

    protected override void PopulateItemList() {
        base.PopulateItemList();
        if (chest != null) {
            foreach (Item item in chest.Items) {
                chestItems.Add(item);
            }

        }
    }


}
