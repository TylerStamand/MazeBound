using System;
using System.Collections;


[Serializable]
public class Item {

    public string ItemName;

    public string ItemID { get; private set; }


    public Item(string itemName) {
        ItemID = Guid.NewGuid().ToString();
        ItemName = itemName;
    }

    public virtual string GetDescription() {
        return $"This is {ItemName} with the id {ItemID}";
    }

}