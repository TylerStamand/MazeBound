using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : ICanLevel {
    //Stats
    public float BaseDefense { get; private set; }

    public int Level { get; private set; }

    //Formula for how to scale to level
    public float CurrentDefense { get => BaseDefense; }

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
