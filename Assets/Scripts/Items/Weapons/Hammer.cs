using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Weapon {
    [SerializeField] GameObject colliderObject;
    [SerializeField] float offset;

    Animator animator;

    protected override void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();

    }


    public override bool Use(Direction direction) {
        if (!base.Use(direction)) return false;


        //Stops any previous animations from playing so the animation end function isn't called during a new animation
        StopAllCoroutines();

        if (useSound != null)
            AudioSource.PlayClipAtPoint(useSound, transform.position, GameManager.Instance.GetVolume());

        inUse = true;

        //Turns on the sprite
        spriteRenderer.enabled = true;

        Vector2 directionVector = Utilities.GetDirectionVectorFromDirection(direction);


        //set collider position and rotation
        colliderObject.transform.localPosition = directionVector * offset;
        colliderObject.transform.eulerAngles = Utilities.GetAngleFromDirection(direction);

        //Sets the attack animation, then subsequently calls OnAttackAnimationEnd
        if (direction == Direction.North) {
            StartCoroutine(SetAttackAnimation("HammerUp"));
        } else if (direction == Direction.South) {
            StartCoroutine(SetAttackAnimation("HammerDown"));
        } else if (direction == Direction.East) {
            StartCoroutine(SetAttackAnimation("HammerRight"));
        } else if (direction == Direction.West) {
            StartCoroutine(SetAttackAnimation("HammerLeft"));
        }

        return true;
    }

    IEnumerator SetAttackAnimation(string animationName) {
        animator.Play(animationName);

        //This sets the multiplier for the animation using the speed stat
        animator.SetFloat("Speed", Speed);

        //This yield is to skip a cycle so the sprite gets a chance to display before the animation can end
        //This is mostly on fast animations 
        yield return null;

        //Checks when the animation ends and goes to done state
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Done") != true)
            yield return null;

        //This is called when the animation is done
        OnAttackAnimationEnd();

    }

    void OnAttackAnimationEnd() {
        collider.enabled = true;


        List<Collider2D> colliders = new List<Collider2D>();

        //This uses the special half circle collider to check for collisions
        Physics2D.OverlapCollider(collider, new ContactFilter2D().NoFilter(), colliders);

        //Turns off the sprite and collider
        collider.enabled = false;
        spriteRenderer.enabled = false;

        inUse = false;

        bool hit = false;
        foreach (Collider2D collider in colliders) {
            IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

            // Keeps Enemies from hitting each other
            if (!playerWeapon && collider.GetComponent<Enemy>() != null)
                continue;

            if (damageable != null) {

                // Keeps Enemies from hitting each other
                if (!playerWeapon && collider.GetComponent<Enemy>() != null)
                    continue;

                damageable.TakeDamage(GetDamage(), DamageType.Default, knockBack);
                hit = true;
            }


        }

        //Plays the hit sound if it hit something
        if (hitSound != null && hit) {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, GameManager.Instance.GetVolume());
        }
    }

}
