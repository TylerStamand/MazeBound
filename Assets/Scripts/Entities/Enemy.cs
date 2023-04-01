using System;
using System.Collections;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable {

    [SerializeField] WeaponData weaponData;
    [SerializeField] GameObject weaponHolder;
    
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
    protected bool inKnockback;
    protected float scale;


    protected virtual void Awake() {

        collider = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    protected virtual void OnDestroy() {
        rigidbody.DOKill();
    }


    protected virtual void FixedUpdate() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
        if (colliders.Length > 0) {
            GameObject target = colliders[0].gameObject;
        }

        if (!inKnockback) {
            Move();

        }
    }

    public void Initialize(float scale, int health) {
        this.scale = scale;
        CurrentHealth = health;
        EquipWeapon();
    }


    public void TakeDamage(int damageDealt, DamageType damageType, float knockback) {
        if (inKnockback) return;
        CurrentHealth -= damageDealt;
        if (knockback > 0) {
            StartCoroutine(Knockback(knockback));
        }

        if (CurrentHealth <= 0) {
            OnDie?.Invoke(this);
            Destroy(gameObject);
        }
    }


    void EquipWeapon() {
        WeaponItem weaponItem = (WeaponItem)weaponData.CreateItem(scale);
        currentWeapon = Instantiate(weaponData.WeaponPrefab, weaponHolder.transform);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.Initialize(false, (int)(weaponData.Damage.GetRandomValue() * scale) + weaponData.Damage.MinValue,
            (float)(Math.Truncate(weaponData.Speed.GetRandomValue() * 100 * scale) / 100) + weaponData.Speed.MinValue,
            (float)(Math.Truncate(weaponData.CriticalChance.GetRandomValue() * 100 * scale) / 100) + weaponData.CriticalChance.MinValue);
    }

    void Move() {

        Collider2D collider = Physics2D.OverlapCircle(transform.position, alertRadius, LayerMask.GetMask(new string[] { "Player" }));
        if (collider != null) {
            target = collider.GetComponentInParent<PlayerCharacter>();

            if (Vector2.Distance(target.transform.position, transform.position) > stopDistance) {
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
                    spriteRenderer.flipX = differenceInPosition.x < 0;
                } else
                    anim.SetBool("isMoving", false);

            } else {
                //Fix logic in here, enemy will attack before moving to player because it is technically in alert radius
                Attack();
                anim.SetBool("isMoving", false);

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

    IEnumerator Knockback(float knockback) {
        inKnockback = true;
        Vector3 knockBackDirection = (transform.position - target.transform.position).normalized;

        //Add the knockback force to the rigidbody
        rigidbody.AddForce(new Vector2(knockBackDirection.x, knockBackDirection.y) * knockback, ForceMode2D.Impulse);
        rigidbody.AddForce(Vector2.up * knockback, ForceMode2D.Impulse);
        rigidbody.gravityScale = 1;

        //Wait for the knockback to finish
        yield return new WaitForSeconds((2 * knockback) / -Physics2D.gravity.y);

        //Reset the rigidbody back to normal
        rigidbody.velocity = Vector2.zero;
        rigidbody.gravityScale = 0;
        inKnockback = false;
    }

    void Attack() {
        currentWeapon.Use(Utilities.DirectionFromVector2(target.transform.position - transform.position));
    }

}
