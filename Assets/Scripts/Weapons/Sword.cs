using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Sword : Weapon {


    void OnTriggerEnter2D(Collider2D collider) {

            IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {

                //Keeps Enemies from hitting each other
                // if (!playerWeapon && collider.GetComponent<Enemy>() != null)
                //     return;

                damageable.TakeDamage(BaseDamage);
            }
    }


    public override void Use(Direction direction) {
        spriteRenderer.enabled = true;
        collider.enabled = true;

        if (transform.parent != null) {

            //Gets Angle from direction, then subtracts 90 degrees to make it a wider rotation
            transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction) - new Vector3(0, 0, 90);
            transform.parent.DORotate(new Vector3(0, 0, transform.parent.eulerAngles.z + 179), .3f).onComplete +=
                () => {
                    transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction);
                    spriteRenderer.enabled = false;
                    collider.enabled = false;
                };
        } else {
            Debug.LogWarning("Weapon parent is not assigned, will not animate");
        }

    
    }

       

}
