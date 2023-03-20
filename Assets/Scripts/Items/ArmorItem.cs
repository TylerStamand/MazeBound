using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : Item, ICanLevel {
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

    public bool CanLevel() {
        throw new System.NotImplementedException();
    }

    public int GetCurrentLevel() {
        throw new System.NotImplementedException();
    }

    public int GetMaxLevel() {
        throw new System.NotImplementedException();
    }

    public void IncreaseLevel() {
        throw new System.NotImplementedException();
    }

}
