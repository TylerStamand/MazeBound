using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player Stats
//Health
//Defense



//Eventually Add inventory to hold weapon and other items

public class PlayerCharacter : MonoBehaviour {
    [SerializeField] Weapon weapon;
    [SerializeField] GameObject weaponHolder;
    PlayerController controller;


    //4-Directional
    Direction currentDirection;

    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnAttack += HandleAttack;
    }

    void HandleAttack() {
        weapon.Use(currentDirection);
    }

    void EquipWeapon(Weapon weapon) {
        Instantiate(weapon, Vector3.zero, Quaternion.identity, weaponHolder.transform);
    }

}

