using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : IIntractable
{
    [field: SerializeField] public string Name { get; private set; }
    

    public void Interact(PlayerCharacter playerCharacter)
    {
        throw new System.NotImplementedException();
    }
}
