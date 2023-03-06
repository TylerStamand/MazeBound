using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable {

    public static int ChestSize = 15;

    public List<Item> Items { get; set; } = new List<Item>();

    private List<int> emptySlots = new List<int>();

    GameObject inventoryUI;
    void Awake() {
        for (int i = 0; i < ChestSize; i++) {
            Items.Add(null);
            emptySlots.Add(i);
        }
    }

    public void Interact(PlayerCharacter playerCharacter) {
        Debug.Log("Interacted");

        inventoryUI = playerCharacter.ShowMenu(ResourceManager.ChestInventoryPrefab);
        if (inventoryUI != null) {
            inventoryUI.GetComponentInChildren<ChestInventoryUI>().SetChest(this);
        }

    }

    /// <summary>
    /// Adds an item at a random spot in the chest
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item) {
        if (emptySlots.Count == 0) {
            Debug.Log("Chest is full");
            return;
        }

        int index = UnityEngine.Random.Range(0, emptySlots.Count);
        Items[emptySlots[index]] = item;
    }


}
