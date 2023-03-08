using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotionItem : Item, IConsumable {

    public int HealPoints { get; private set; }

    public HealingPotionItem(ItemData itemData, int healPoints) : base(itemData) {
        HealPoints = healPoints;
    }

    public new HealingPotionData ItemData { get; private set; }

    public void Consume(PlayerCharacter playerCharacter) {
        playerCharacter.Heal(HealPoints);
    }

    public override string GetDescription() {
        return "Heals " + HealPoints + " HP";
    }
}
