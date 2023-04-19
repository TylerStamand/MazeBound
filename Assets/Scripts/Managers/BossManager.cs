using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour {
    void Awake() {
        StartCoroutine(HandleBossStart());
    }

    IEnumerator HandleBossStart() {
        while (GameManager.Instance.CurrentGameState != GameState.Boss) {

            yield return null;

        }

        yield return new WaitForSeconds(2f);
        FindObjectOfType<Boss>().StartBossFight();
    }
}
