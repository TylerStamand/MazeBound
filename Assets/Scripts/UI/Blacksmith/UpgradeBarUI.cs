using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBarUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI statNameLevelText;
    [SerializeField] TextMeshProUGUI statCostText;
    [SerializeField] Button upgradeButton;

    UpgradeableStat stat;

    public void Initialize(UpgradeableStat stat) {
        this.stat = stat;
        upgradeButton.onClick.AddListener(() => stat.Upgrade());
        stat.OnUpgrade += PopulateFields;
        PopulateFields();
    }
    void PopulateFields() {
        statNameLevelText.text = $"{stat.Name} ({stat.Level}/{UpgradeableStat.MaxLevel})  Cost: {stat.UpgradeCost}";
        statCostText.text = stat.UpgradeCost.ToString();
    }

}
