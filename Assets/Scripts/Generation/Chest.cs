using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IIntractable
{
    List<Item> items = new List<Item>();


    public void Interact(PlayerCharacter playerCharacter)
    {
        Debug.Log("Chest Interacted");
    }
}
