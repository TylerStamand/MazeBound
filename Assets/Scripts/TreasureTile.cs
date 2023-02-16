using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Treasure Tile", menuName = "Tiles/Treasure Tile")]
public class TreasureTile : Tile, IIntractable {
    public List<Item> Items { get; set; }
    
    

    public void Interact(PlayerCharacter playerCharacter) {
        playerCharacter.HandleOpeningChest(Items);
    }

}
