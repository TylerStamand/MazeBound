using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : Item, IUpgradeable {
    public ArmorItem(string itemName) : base(itemName) {

    }

    public enum ArmorPiece {
        Head, Chest, Leg, Boot
    }

    //Stats
    public int BaseDefense { get; private set; }

    public int Level { get; private set; }

    public ArmorPiece Piece { get; private set; }

    //Formula for how to scale to level
    public int CurrentDefense { get => BaseDefense; }

    public List<UpgradeableStat> GetUpgradeableStats()
    {
        throw new System.NotImplementedException();
    }
}
