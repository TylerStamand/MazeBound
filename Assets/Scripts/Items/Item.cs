using System;
using System.Collections;


[Serializable]
public class Item {

    public string ItemID { get; private set; }

    public string ItemName { get; private set; }

    public int Quantity { get; set; }

    public Item(string itemName) {
        ItemID = Guid.NewGuid().ToString();
        ItemName = itemName;
        Quantity = 1;
    }

    public virtual string GetDescription() {
        return $"Description";
    }

    public void AddQuantity(int quantity) {
        ItemData itemData = ResourceManager.Instance.GetItemData(ItemName);
        if (itemData.StackAble && Quantity + quantity <= itemData.MaxStackSize)
            Quantity += quantity;
    }

}