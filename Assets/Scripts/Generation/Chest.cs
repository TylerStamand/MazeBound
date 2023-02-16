using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable {
    [SerializeField] GameObject chestUIPrefab;

    public static int ChestSize = 15;

    public List<Item> Items { get; set; } = new List<Item>();

    GameObject inventoryUI;
    void Awake() {
        for (int i = 0; i < ChestSize; i++) {
            Items.Add(null);
        }
    }

    public void Interact(PlayerCharacter playerCharacter) {
        Debug.Log("Interacted");

        inventoryUI = playerCharacter.ShowMenu(chestUIPrefab);
        if(inventoryUI != null) {
            inventoryUI.GetComponentInChildren<ChestInventoryUI>().SetChest(this);
        }

    }


}
