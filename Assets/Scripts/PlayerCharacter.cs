using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player Stats
//Health
//Defense



//Eventually Add inventory to hold weapon and other items

public class PlayerCharacter : MonoBehaviour {
    [SerializeField] Weapon weaponPrefab;
    [SerializeField] GameObject weaponHolder;
    PlayerController controller;

    Weapon currentWeapon;


    //4-Directional
    Direction currentDirection = Direction.West;

    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnAttack += HandleAttack;
        EquipWeapon(weaponPrefab);
    }

    void HandleAttack() {
        currentWeapon.Use(currentDirection);
    }

    void EquipWeapon(Weapon weapon) {
        currentWeapon = Instantiate(weapon, Vector3.zero, Quaternion.identity, weaponHolder.transform);
        currentWeapon.Initialize(true);
    }

}

