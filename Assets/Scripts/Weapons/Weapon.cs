using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Weapon Stats
//NEeds a way to set them according to parameters
//Attack
//Critical change (Defense doesnt count)

public abstract class Weapon : MonoBehaviour {

    [field: SerializeField] public float CoolDown { get; private set; }
    [field: SerializeField] public float BaseDamage { get; private set; }

    protected float timeLastUsed;


    protected SpriteRenderer spriteRenderer;
    protected new Collider2D collider;

    bool initialized;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        initialized = false;
    }

    //Handle CoolDown and stats to determine if use works
    public abstract void Use(Direction direction);

    public virtual void Initialize() {
        if (initialized) return;

        //Set stats and other stuff

        initialized = true;
    }
}
