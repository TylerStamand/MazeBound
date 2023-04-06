using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject {
    [field: Header("Enemy Properties")]
    [field: SerializeField] public string EnemyName { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public Enemy EnemyPrefab { get; private set; }

    [field: Header("Weapon Properties")]
    [field: SerializeField] public Weapon WeaponPrefab { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public float CriticalChance { get; private set; }


    //Possibly add range/movement properties

}
