using System;
using UnityEngine;


public class Tentacle : MonoBehaviour, IDamageable {

    public event Action OnDeath;

    public int CurrentHealth { get; set; }


    public void TakeDamage(int damageDealt, DamageType damageType, float knockback = 1) {
        CurrentHealth -= damageDealt;
        if (CurrentHealth <= 0) {
            Die();
        }
    }


    void Die() {
        OnDeath?.Invoke();
        //Make sprite gray
    }
}
