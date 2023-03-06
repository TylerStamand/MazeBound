using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnRates", menuName = "ScriptableObjects/SpawnRates")]

public class SpawnRates : ScriptableObject {
    public List<ItemSpawnRate> ItemSpawnRates = new List<ItemSpawnRate>();
    public List<EnemySpawnRate> EnemySpawnRates = new List<EnemySpawnRate>();

    [System.Serializable]
    public class EnemySpawnRate {
        public Enemy enemy;
        [CurveRange(0, 0, 1, 1)]
        public AnimationCurve spawnRateCurve;
    }

    [System.Serializable]
    public class ItemSpawnRate {
        public ItemData itemData;
        [CurveRange(0, 0, 1, 1)]
        public AnimationCurve spawnRateCurve;
    }
}
