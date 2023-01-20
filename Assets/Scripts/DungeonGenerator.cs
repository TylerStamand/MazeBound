using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {
    // [SerializeField] PlayerCharacter playerObject;

    [SerializeField] Grid dungeonGrid;
    [SerializeField] Room roomPrefab;

    void Awake() {
        Debug.Log("Creating room");
        Room room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, dungeonGrid.transform);
        room.Initialize(Vector2Int.zero);
    }


    void Update() {

    }
}
