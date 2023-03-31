using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject {
    public string EnemyName;
    public int MaxHealth;

    [Range(0, 1)]
    public float Scale;

    public Enemy EnemyPrefab;

}
