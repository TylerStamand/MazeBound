using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotionItem : Item, IConsumable {

    public int HealPoints { get; private set; }

    public HealingPotionItem(string itemName, int healPoints) : base(itemName) {
        HealPoints = healPoints;
    }
    
    public void Consume(PlayerCharacter playerCharacter) {
        playerCharacter.Heal(HealPoints);
    }

    public override string GetDescription() {
        return "Heals " + HealPoints + " HP";
    }
}
