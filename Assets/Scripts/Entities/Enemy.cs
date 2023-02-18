using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable {

    [SerializeField] WeaponData weaponData;
    [SerializeField] GameObject weaponHolder;

    [field: Header("Stats")]
    [field: SerializeField] public float MaxHealth { get; private set; }

    [Header("Movement")]
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float alertRadius = 1;
    [SerializeField] float stopDistance = 2;
    [SerializeField] ContactFilter2D contactFilter;

    public float CurrentHealth { get; private set; }

    public Action<Enemy> OnDie;


    protected new Collider2D collider;
    protected new Rigidbody2D rigidbody;
    protected SpriteRenderer spriteRenderer;
    protected PlayerCharacter target;
    protected Weapon currentWeapon;
    protected Animator anim;


    protected virtual void Awake() {
        CurrentHealth = MaxHealth;

        collider = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentWeapon = Instantiate(weaponData.WeaponPrefab, weaponHolder.transform);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.Initialize(false, weaponData.Damage.GetRandomValue(), weaponData.CoolDown.GetRandomValue(), weaponData.CriticalChance.GetRandomValue());
    }




    protected virtual void FixedUpdate() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
        if (colliders.Length > 0) {
            GameObject target = colliders[0].gameObject;
            if ((target.transform.position.x - transform.position.x) >= 0) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }
        }
        Move();
    }

    public void TakeDamage(int damageDealt) {
        CurrentHealth -= damageDealt;

        if (CurrentHealth <= 0) {
            OnDie?.Invoke(this);
            Destroy(gameObject);
        }
    }

    void Move() {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
        if (collider != null) {
            target = collider.GetComponent<PlayerCharacter>();

            if (Vector2.Distance(target.transform.position, transform.position) >= stopDistance) {
                Vector2 positionToMoveTowards = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed);
                Vector2 differenceInPosition = new Vector2(positionToMoveTowards.x - transform.position.x, positionToMoveTowards.y - transform.position.y);


                RaycastHit2D[] results = new RaycastHit2D[1];

                //Casts the collider to position to see if it can move without collisions
                int numOfHits = collider.Cast(differenceInPosition.normalized, contactFilter, results, moveSpeed);

                if (numOfHits == 0) {
                    rigidbody.MovePosition(transform.position + (Vector3)differenceInPosition);
                    anim.SetBool("isMoving", true);
                    anim.SetFloat("x", differenceInPosition.x);
                    anim.SetFloat("y", differenceInPosition.y);

                } else {
                    anim.SetBool("isMoving", false);
                }

            } else {
                currentWeapon.Use(Utilities.DirectionFromVector2(target.transform.position - transform.position));
            }
        } else {
            target = null;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alertRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }


}
