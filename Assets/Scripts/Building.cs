using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour {

    [field: SerializeField] public Tilemap Tilemap;
    [field: SerializeField] public Vector2Int North { get; private set; }
    [field: SerializeField] public Vector2Int South { get; private set; }
    [field: SerializeField] public Vector2Int East { get; private set; }
    [field: SerializeField] public Vector2Int West { get; private set; }

    void Awake() {
    }

    public static Vector2 GetBuildingOffset(Vector2 a, Vector2 b) {
        return new Vector2(a.x - b.x, a.y - b.y);
    }

    public Vector3Int GetConnectionPosition(Direction direction) {
        if (direction == Direction.North) {
            return (Vector3Int)North;
        } else if (direction == Direction.East) {
            return (Vector3Int)East;
        } else if (direction == Direction.South) {
            return (Vector3Int)South;
        } else {
            return (Vector3Int)West;
        }
    }
}
