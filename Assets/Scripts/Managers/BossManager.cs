using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {
    [SerializeField] GameObject playerSpawn;
    [SerializeField] Dialog npcsLeaveDialog;
    [SerializeField] Dialog canLeaveDialog;
    [SerializeField] Dialog portalDialog;

    public static BossManager Instance { get; private set; }

    bool IsBossFightActive { get; set; }
    bool portalTriggered = false;

    void Awake() {
        //Singleton
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        IsBossFightActive = true;
        PlayerCharacter playerCharacter = Instantiate(ResourceManager.Instance.PlayerPrefab, playerSpawn.transform.position, Quaternion.identity).GetComponent<PlayerCharacter>();
        playerCharacter.Load();


    }

    void Start() {


        StartCoroutine(HandleBossStart());

    }
    IEnumerator HandleBossStart() {
        Debug.Log("Handle boss start");
        while (GameManager.Instance.CurrentGameState != GameState.Boss) {

            yield return null;

        }

        DialogManager dialogManager = ShowDialog(npcsLeaveDialog, FindObjectOfType<PlayerCharacter>(), "---");
        if (dialogManager != null) {
            dialogManager.OnDialogComplete += (x) => {

                Debug.Log("Starting boss fight");
                FindObjectOfType<Boss>().StartBossFight();
            };
        }



    }

    DialogManager ShowDialog(Dialog dialog, PlayerCharacter playerCharacter, string name) {
        DialogManager dialogManager = playerCharacter.ShowMenu(ResourceManager.Instance.DialogManagerPrefab, false)?.GetComponent<DialogManager>();
        if (dialogManager == null) return null;
        dialogManager.SetDialog(dialog, name);
        dialogManager.OnDialogComplete += (x) => {
            Destroy(dialogManager.gameObject);
            playerCharacter.ExitMenu();
        };
        return dialogManager;
    }

    public IEnumerator HandleBossFightOver() {
        IsBossFightActive = false;
        yield return new WaitForSeconds(1);
        DialogManager dialogManager = ShowDialog(canLeaveDialog, FindObjectOfType<PlayerCharacter>(), "---");
    }

    public IEnumerator HandlePortalTriggered() {
        if (IsBossFightActive) yield break;
        if (portalTriggered) yield break;

        portalTriggered = true;

        yield return new WaitForSeconds(1);

        DialogManager dialogManager = ShowDialog(portalDialog, FindObjectOfType<PlayerCharacter>(), "---");
        if (dialogManager != null) {
            dialogManager.OnDialogComplete += (x) => {

                GameManager.Instance.LoadEndScreen();
            };
        }

    }


}
