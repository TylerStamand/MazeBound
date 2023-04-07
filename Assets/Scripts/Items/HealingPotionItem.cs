using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotionItem : Item, IConsumable {

    public int HealPoints { get; private set; }

    public HealingPotionItem(string itemName, int healPoints) : base(itemName) {
        HealPoints = healPoints;
    }

    public bool Consume(PlayerCharacter playerCharacter) {
        if (playerCharacter.CurrentHealth == playerCharacter.BaseHealth) {
            return false;
        }
        playerCharacter.Heal(HealPoints);
        return true;
    }

    public override string GetDescription() {
        return "Heals " + HealPoints + " HP";
    }
}
