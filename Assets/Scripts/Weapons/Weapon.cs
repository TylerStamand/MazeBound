using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

//Weapon Stats
//NEeds a way to set them according to parameters
//Attack
//Critical change (Defense doesnt count)

public abstract class Weapon : MonoBehaviour {

    [SerializeField] protected float AnimationLength = 0.3f;

    [field: Header("Stats")]
    [field: SerializeField] public float CoolDown { get; private set; }
    [field: SerializeField] public float BaseDamage { get; private set; }

    protected float timeLastUsed;


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

    void Awake() {
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

    //Handle CoolDown and stats to determine if use works
    public virtual bool Use(Direction direction) {
        if (!initialized) {
            Debug.LogError("Weapon not initialized, please initialize before using weapon");
            return false;
        }

        if (inUse || timeLastUsed + CoolDown > Time.time) {
            return false;
        }

        timeLastUsed = Time.time;
        return true;
    }


    public virtual void Initialize(bool playerWeapon) {
        if (initialized) return;

        this.playerWeapon = playerWeapon;
        //Set stats and other stuff

        initialized = true;
    }
}
