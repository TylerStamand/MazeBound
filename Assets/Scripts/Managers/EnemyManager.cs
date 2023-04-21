using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public static EnemyManager Instance { get; private set; }

    public event Action<Enemy> EnemyAdded;
    public event Action<Enemy> EnemyRemoved;
    public event Action EnemiesCleared;

    [SerializeField] List<Enemy> enemies = new List<Enemy>();

    void Awake() {
        if (Instance == null)
            Instance = this;
        else {
            Destroy(gameObject);
        }
    }

    public void AddEnemy(Enemy enemy) {
        enemies.Add(enemy);
        EnemyAdded?.Invoke(enemy);
        enemy.OnDeath += RemoveEnemy;
    }

    public void RemoveEnemy(IDamageable enemy) {
        enemies.Remove((Enemy)enemy);
        EnemyRemoved?.Invoke((Enemy)enemy);
    }

    public void ClearEnemies() {
        enemies.Clear();
    }

    public List<Enemy> GetEnemies() {
        return enemies;
    }
}
