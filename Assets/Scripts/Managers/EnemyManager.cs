using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance { get; private set; }

    public event Action<IDamageable> EnemyAdded;
    public event Action<IDamageable> EnemyRemoved;
    public event Action EnemiesCleared;

    [SerializeField] List<IDamageable> enemies = new List<IDamageable>();

    void Awake() {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
        }
    }

    public void AddEnemy(IDamageable enemy) {
        Debug.Log("Enemy Added");
        enemies.Add(enemy);
        EnemyAdded?.Invoke(enemy);
        enemy.OnDeath += RemoveEnemy;
    }

    public void RemoveEnemy(IDamageable enemy) {
        enemies.Remove(enemy);
        EnemyRemoved?.Invoke(enemy);
    }

    public void ClearEnemies() {
        enemies.Clear();
    }

    public List<IDamageable> GetEnemies() {
        return enemies;
    }
}
