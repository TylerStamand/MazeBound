using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {

    public event Action<IDamageable> OnDeath;
    public event Action<IDamageable, int> OnHealthChange;
    //Add second parameter for damage type and a third for knockback
    public void TakeDamage(int damageDealt, DamageType damageType, float knockback = 1);
    public int GetMaxHealth();
}
