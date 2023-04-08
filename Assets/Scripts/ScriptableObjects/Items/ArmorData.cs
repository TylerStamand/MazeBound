using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmorData", menuName = "Items/ArmorData")]
public class ArmorData : ItemData {
    [field: SerializeField] public ArmorItem.ArmorPiece Piece { get; private set; }
    [field: SerializeField] public MinMaxInt Health { get; private set; }
    [field: SerializeField] public MinMaxInt Defense { get; private set; }

    [field: SerializeField] public int UpgradeCostBase { get; private set; }
    [field: SerializeField] public float UpgradeCostBaseMultiplier { get; private set; }
    [field: SerializeField] public float UpgradeValueMultiplier { get; private set; }
    [field: SerializeField] public float UpgradeCostMultiplier { get; private set; }


    public override Item CreateItem(float scale) {
        Debug.Log("Creating ArmorItem with scale: " + scale);
        return new ArmorItem(Name,
        (int)((Defense.GetRandomValue() - Defense.MinValue) * scale) + Defense.MinValue,
        (int)((Health.GetRandomValue() - Health.MinValue) * scale) + Health.MinValue,
        UpgradeCostBase + (int)(UpgradeCostBase * UpgradeCostBaseMultiplier * scale - 1));
    }
}
