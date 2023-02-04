using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanLevel {

    public int GetCurrentLevel();
    public int GetMaxLevel();
    public bool CanLevel();
    public void IncreaseLevel();
}
