using System;
using System.Collections;


[Serializable]
public class Item {

    public string ItemID { get; private set; }

    public ItemData ItemData { get; private set; }

    public Item(ItemData itemData) {
        ItemID = Guid.NewGuid().ToString();
        ItemData = itemData;
    }

    public virtual string GetDescription() {
        return $"Description";
    }

}