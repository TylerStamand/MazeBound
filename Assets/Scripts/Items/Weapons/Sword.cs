using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Sword : Weapon {


    void OnTriggerEnter2D(Collider2D collider) {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null) {

            // Keeps Enemies from hitting each other
            if (!playerWeapon && collider.GetComponent<Enemy>() != null)
                return;

            damageable.TakeDamage(GetDamage(), DamageType.Default, knockBack);
            if (hitSound != null)
                AudioSource.PlayClipAtPoint(hitSound, transform.position, GameManager.Instance.GetVolume());
        }
    }


    //Needs a way to prevent the sword from being spammed, or a way to reset sword to top when player presses too fast
    public override bool Use(Direction direction) {

        if (!base.Use(direction)) return false;

        if (useSound != null) {
            AudioSource.PlayClipAtPoint(useSound, transform.position, GameManager.Instance.GetVolume());
        }

        spriteRenderer.enabled = true;
        collider.enabled = true;
        inUse = true;

        //Gets Angle from direction, then subtracts 90 degrees to make it a wider rotation
        transform.parent.DOKill();
        transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction) - new Vector3(0, 0, 90);
        transform.parent.DORotate(new Vector3(0, 0, transform.parent.eulerAngles.z + 179), 1 / Speed).onComplete +=
            () => {
                transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction);
                spriteRenderer.enabled = false;
                collider.enabled = false;
                inUse = false;
            };

        return true;
    }



}
