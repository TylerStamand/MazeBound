using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Spear : Weapon {

    [SerializeField] float moveDistance = 2;

    void OnTriggerEnter2D(Collider2D collider) {
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null) {

            // Keeps Enemies from hitting each other
            if (!playerWeapon && collider.GetComponent<Enemy>() != null)
                return;

            damageable.TakeDamage(Damage, DamageType.Default, knockBack);
        }
    }


    //Needs a way to prevent the sword from being spammed, or a way to reset sword to top when player presses too fast
    public override bool Use(Direction direction) {

        if (!base.Use(direction)) return false;

        spriteRenderer.enabled = true;
        collider.enabled = true;
        inUse = true;

        //Gets Angle from direction, then subtracts 90 degrees to make it a wider rotation
        transform.parent.DOKill();
        transform.parent.eulerAngles = Utilities.GetAngleFromDirection(direction);
        transform.parent.localPosition = Vector3.zero;


        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.parent.DOLocalMove(Utilities.GetDirectionVectorFromDirection(direction) * moveDistance, 1 / Speed).SetEase(Ease.OutQuad));
        sequence.Append(transform.parent.DOLocalMove(Vector3.zero, 1 / Speed).SetEase(Ease.InQuad));

        sequence.onComplete +=
            () => {
                Debug.Log("Spear Complete");
                transform.parent.localPosition = Vector3.zero;
                spriteRenderer.enabled = false;
                collider.enabled = false;
                inUse = false;
            };
        sequence.onUpdate +=
            () => {
                if (sequence.Elapsed() > 1 / Speed) {
                    collider.enabled = false;
                }
            };
        Debug.Log("Spear Sequence Playing");
        Debug.Log(Speed);
        sequence.Play();
        return true;
    }


}