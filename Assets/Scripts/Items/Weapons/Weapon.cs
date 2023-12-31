using DG.Tweening;
using UnityEngine;



public enum DamageType {
    Default
}

public abstract class Weapon : MonoBehaviour {

    [SerializeField] protected float knockBack = 5;
    [SerializeField] protected AudioClip hitSound;
    [SerializeField] protected AudioClip useSound;

    public float Speed { get; private set; }
    public int Damage { get; private set; }
    public float CriticalChange { get; private set; }


    protected SpriteRenderer spriteRenderer;
    protected new Collider2D collider;
    protected bool playerWeapon;
    protected bool initialized;
    protected bool inUse;

    void OnValidate() {

        if (GetComponentInChildren<SpriteRenderer>() == null) {
            Debug.LogWarning("No SpriteRenderer for " + gameObject.name + " found");
        }
    }

    protected virtual void Awake() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        collider = GetComponentInChildren<Collider2D>();
        initialized = false;
        inUse = false;

        spriteRenderer.enabled = false;
        collider.enabled = false;
    }


    void OnDestroy() {
        transform.parent.DOKill();
    }

    //Handle Speed and stats to determine if use works
    public virtual bool Use(Direction direction) {
        if (!initialized) {
            Debug.LogError("Weapon not initialized, please initialize before using weapon");
            return false;
        }

        if (inUse) {
            return false;
        }

        return true;
    }


    public virtual void Initialize(bool playerWeapon, int damage, float speed, float criticalChance) {
        this.playerWeapon = playerWeapon;
        Damage = damage;
        Speed = speed;
        CriticalChange = criticalChance;

        initialized = true;
    }

    protected int GetDamage() {
        if (CriticalChange > Random.Range(0, 100)) {
            return (int)(Damage * 1.5f);
        }
        return Damage;
    }
}
