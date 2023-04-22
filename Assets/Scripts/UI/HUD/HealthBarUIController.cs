using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUIController : MonoBehaviour {
    [SerializeField] HealthBarUI healthBarPrefab;

    Dictionary<IDamageable, HealthBarUI> healthBarDict = new Dictionary<IDamageable, HealthBarUI>();

    EnemyManager enemyManager;

    void Awake() {

        enemyManager = FindObjectOfType<EnemyManager>();
        enemyManager.EnemyAdded += Subscribe;
        enemyManager.EnemyRemoved += Unsubscribe;
        enemyManager.GetEnemies().ForEach(Subscribe);

    }


    public void Subscribe(IDamageable damageable) {
        Debug.Log("Subscribing");
        if (healthBarDict.ContainsKey(damageable)) {
            return;
        }

        HealthBarUI healthBar = Instantiate(healthBarPrefab, transform);
        healthBar.Initialize(damageable);
        healthBarDict.Add(damageable, healthBar);


    }

    public void Unsubscribe(IDamageable damageable) {
        if (healthBarDict.ContainsKey(damageable)) {
            Destroy(healthBarDict[damageable].gameObject);
            healthBarDict.Remove(damageable);
        }
    }
}
