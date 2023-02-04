using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    //Will probably need events to show change for HUD depending on implementation

    public Weapon CurrentWeapon { get; set; }

    public Armor Head { get; set; }
    public Armor Chest { get; set; }
    public Armor Leg { get; set; }
    public Armor Boots { get; set; }

    public List<Potion> Potions { get; set; }

    public float GetDefenseFromArmor() {
        return 0;
    }



}
