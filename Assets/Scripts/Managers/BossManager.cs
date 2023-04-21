using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {
    [SerializeField] GameObject playerSpawn;

    void Awake() {

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

        Debug.Log("Waiting");
        yield return new WaitForSeconds(2f);
        Debug.Log("Starting boss fight");
        FindObjectOfType<Boss>().StartBossFight();
    }
}
