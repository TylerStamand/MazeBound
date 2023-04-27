using System;
using System.Collections;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable {


    [SerializeField] GameObject weaponHolder;

    [SerializeField] AudioClip deathSound;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float alertRadius = 1;
    [SerializeField] float attackRadius = 1;
    [SerializeField] float stopDistance = 2;
    [SerializeField] ContactFilter2D contactFilter;

    public int CurrentHealth { get; private set; }
    public string Name => enemyData.EnemyName;

    public event Action<IDamageable> OnDeath;
    public event Action<IDamageable, int> OnHealthChange;


    protected new Collider2D collider;
    protected new Rigidbody2D rigidbody;
    protected SpriteRenderer spriteRenderer;
    protected PlayerCharacter target;
    protected Weapon currentWeapon;
    protected Animator anim;
    protected bool inKnockback;

    protected EnemyData enemyData;


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

            if (colliders[0].GetComponent<PlayerCharacter>() != null) {
                target = colliders[0].GetComponent<PlayerCharacter>();

            }
        }

        if (!inKnockback) {
            Move();

        }
    }

    public void Initialize(EnemyData enemyData) {
        this.enemyData = enemyData;
        CurrentHealth = enemyData.Health;
        EquipWeapon();
    }


    public void TakeDamage(int damageDealt, DamageType damageType, float knockback) {
        if (inKnockback) return;
        Debug.Log($"{gameObject.name} took {damageDealt} damage");
        CurrentHealth -= damageDealt;
        OnHealthChange?.Invoke(this, CurrentHealth);
        if (knockback > 0) {
            StartCoroutine(Knockback(knockback));
        }

        if (CurrentHealth <= 0) {
            if (deathSound != null)
                AudioSource.PlayClipAtPoint(deathSound, transform.position, GameManager.Instance.GetVolume());
            OnDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }


    void EquipWeapon() {
        currentWeapon = Instantiate(enemyData.WeaponPrefab, weaponHolder.transform);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.Initialize(false, enemyData.Damage, enemyData.AttackSpeed, Math.Min(enemyData.CriticalChance, 100));
    }


    //Collider that alerts enemy
    //collider is then checked every frame to see if it is within stop distance
    //if it is, the enemy moves towards the player
    //if not, set collider to null
    //if collider is not null, check if within attack distance
    //if yes attack

    void Move() {


        if (target != null) {
            float distance = Vector2.Distance(target.transform.position, transform.position);
            if (distance <= attackRadius) {
                anim.SetBool("isMoving", false);
                Attack();
                return;
            }

            if (distance <= stopDistance) {
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
                target = null;
                anim.SetBool("isMoving", false);

            }
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
        Vector3 knockBackDirection;
        if (target != null) {
            knockBackDirection = (transform.position - target.transform.position).normalized;

        } else {
            knockBackDirection = Vector3.zero;
        }

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
        Direction directionToAttack = Utilities.DirectionFromVector2(target.transform.position - transform.position);
        Debug.Log(directionToAttack);
        currentWeapon.Use(directionToAttack);
    }

    public int GetMaxHealth() {
        return enemyData.Health;
    }
}
