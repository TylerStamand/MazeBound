using System;
using System.Collections;


[Serializable]
public class Item {

    public string ItemID { get; private set; }

    public ItemData ItemData { get; private set; }

    public int Quantity { get; set; }

    public Item(ItemData itemData) {
        ItemID = Guid.NewGuid().ToString();
        ItemData = itemData;
        Quantity = 1;
    }

    public virtual string GetDescription() {
        return $"Description";
    }

    public void AddQuantity(int quantity) {
        if(ItemData.StackAble && Quantity + quantity <= ItemData.MaxStackSize)
        Quantity += quantity;
    }

}