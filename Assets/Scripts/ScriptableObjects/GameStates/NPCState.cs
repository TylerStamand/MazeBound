using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/NPCState")]
public class NPCState : ScriptableObject {
    public bool MazeEncounterComplete;
    public bool HubFirstEncounterComplete;
    public bool InHub;

    public void Reset() {
        MazeEncounterComplete = false;
        HubFirstEncounterComplete = false;
        InHub = false;
    }
}
