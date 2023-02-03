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



    void Awake() {
        controller = GetComponent<PlayerController>();
        controller.OnAttack += HandleAttack;
        EquipWeapon(weaponPrefab);
    }

    void HandleAttack() {

        Vector2 difference = Room.GetRoomOffset(controller.MousePos, transform.position);
        float angle = (int)Vector2.Angle(difference, Vector2.right);
        if (difference.y < 0) {
            angle = -angle;
        }
        Debug.Log(angle);
        Direction direction = Utilities.GetDirectionFromAngle(angle);
        currentWeapon.Use(direction);
    }

    void EquipWeapon(Weapon weapon) {
        currentWeapon = Instantiate(weapon, Vector3.zero, Quaternion.identity, weaponHolder.transform);
        currentWeapon.Initialize(true);
    }

}

