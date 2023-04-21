using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealingPotion", menuName = "Items/HealingPotion")]
public class HealingPotionData : ItemData {
    [field: SerializeField] public MinMaxInt HealPoints { get; private set; }

    public override Item CreateItem(float scale) {
        return new HealingPotionItem(Name,(int)(Math.Truncate((HealPoints.GetRandomValue() - HealPoints.MinValue) * 100 * scale) / 100) + HealPoints.MinValue);
    }
}
