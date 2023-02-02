using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour, IDamageable {

    [field: SerializeField] public float MaxHealth { get; private set; }

    public float CurrentHealth { get; private set; }
    //Active Weapon
    // [SerializeField] Weapon 
    //Drops
    //A controller depending on enemy type
    //Ie: Range, melee


    public void Awake() {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damageDealt) {
        CurrentHealth -= damageDealt;

        if (CurrentHealth <= 0) {
            Destroy(gameObject);
        }
    }


}
