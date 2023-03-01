using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    //Add second parameter for damage type and a third for knockback
    public void TakeDamage(int damageDealt, DamageType damageType, float knockback = 1);

}
