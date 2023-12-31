using System;
using UnityEngine;


public class Tentacle : MonoBehaviour, IDamageable {

    [SerializeField] AudioClip bossHurt;

    public event Action<IDamageable> OnDeath;
    public event Action<IDamageable, int> OnHealthChange;

    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }


    bool dead;

    void Awake() {
        dead = false;
    }

    public void Initialize(int maxHealth) {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damageDealt, DamageType damageType, float knockback = 1) {
        if (dead) {
            return;
        }

        if (bossHurt != null)
            AudioSource.PlayClipAtPoint(bossHurt, transform.position, GameManager.Instance.GetVolume());

        CurrentHealth -= damageDealt;
        OnHealthChange?.Invoke(this, CurrentHealth);

        if (CurrentHealth <= 0) {
            dead = true;
            Die();
        }
    }


    void Die() {
        OnDeath?.Invoke(this);
        GetComponent<SpriteRenderer>().color = Color.gray;
        GetComponent<Animator>().enabled = false;
        //Make sprite gray
    }

    public int GetMaxHealth() {
        return MaxHealth;
    }
}
