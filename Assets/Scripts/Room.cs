using UnityEngine.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Building))]
public class Room : MonoBehaviour {

    [SerializeField] bool CompleteRoom;

    public event Action<Room> OnRoomCompletion;


    Vector2Int origin; //bottom-left vertex
    bool playerInRoom;
    new TilemapCollider2D collider;
    Tilemap tilemap;
    int EnemiesLeft;

    void Awake() {
        CompleteRoom = false;

    }

    void Update() {
        if (CompleteRoom) {
            OnRoomCompletion?.Invoke(this);
            CompleteRoom = false;
        }
    }

    public void Initialize(Vector2Int origin) {
        this.origin = origin;
        //get world position of bottom middle cell, world position of top middle cell
        //Set position of center with the difference + the position of the top middle cell from parent room         
    }



    void OnTriggerEnter2D(Collider2D collider) {
        PlayerCharacter player = collider.GetComponent<PlayerCharacter>();
        if (player != null) {
            playerInRoom = true;
            Debug.Log($"Player entered room at {origin}");

        }
    }


    void OnTriggerExit2D(Collider2D collider) {
        PlayerCharacter player = collider.GetComponent<PlayerCharacter>();
        if (player != null) {
            playerInRoom = false;
            Debug.Log($"Player left room at {origin}");
        }
    }

}
