using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnRates", menuName = "ScriptableObjects/SpawnRates")]

public class SpawnRates : ScriptableObject {
    public List<ItemSpawnRate> ItemSpawnRates = new List<ItemSpawnRate>();
    public List<EnemySpawnRate> EnemySpawnRates = new List<EnemySpawnRate>();
    
    [System.Serializable]
    public class EnemySpawnRate {
        public Enemy enemy;
        public AnimationCurve spawnRateCurve;
    }
    
    [System.Serializable]
    public class ItemSpawnRate {
        public ItemData itemData;
        public AnimationCurve spawnRateCurve;
    }
}
