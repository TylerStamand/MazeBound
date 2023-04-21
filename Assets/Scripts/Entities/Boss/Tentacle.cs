using System;
using UnityEngine;


public class Tentacle : MonoBehaviour, IDamageable {

    public event Action OnDeath;

    public int CurrentHealth { get; set; }

    bool dead;

    void Awake() {
        dead = false;
    }

    public void TakeDamage(int damageDealt, DamageType damageType, float knockback = 1) {
        if (dead) {
            return;
        }

        CurrentHealth -= damageDealt;
        if (CurrentHealth <= 0) {
            dead = true;
            Die();
        }
    }


    void Die() {
        OnDeath?.Invoke();
        GetComponent<SpriteRenderer>().color = Color.gray;
        //Make sprite gray
    }
}
