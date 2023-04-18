using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBarUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI statNameLevelText;
    [SerializeField] TextMeshProUGUI statCostText;
    [SerializeField] Button upgradeButton;

    [SerializeField] AudioClip upgradeSound;

    UpgradeableStat stat;
    PlayerCharacter playerCharacter;

    public void Initialize(UpgradeableStat stat) {
        this.stat = stat;
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        upgradeButton.onClick.AddListener(HandleButtonClick);
        stat.OnUpgrade += PopulateFields;

        PopulateFields();
    }
    void PopulateFields() {
        statNameLevelText.text = $"{stat.Name} ({stat.Level}/{UpgradeableStat.MaxLevel}) ";
        statCostText.text = $"Cost: {stat.UpgradeCost}";
    }

    void HandleButtonClick() {
        if (playerCharacter.WeaponScraps >= stat.UpgradeCost) {
            //Play upgrade sound
            if (stat.Upgrade()) {
                if (upgradeSound != null)
                    AudioSource.PlayClipAtPoint(upgradeSound, playerCharacter.transform.position, GameManager.Instance.GetVolume());
                playerCharacter.RemoveWeaponScraps(stat.UpgradeCost);
            }

        }
    }


}
