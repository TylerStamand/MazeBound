using UnityEngine.Tilemaps;
using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Building))]
public class Room : MonoBehaviour {

    [field: SerializeField] public RoomRarity Rarity { get; private set; }

    [SerializeField] bool CompleteRoom;

    public event Action<Room> OnRoomCompletion;

    public int RoomLevel { get; private set; }

    bool playerInRoom;
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

    public void Initialize(int roomLevel) {
        RoomLevel = roomLevel;
    }



    void OnTriggerEnter2D(Collider2D collider) {
        PlayerCharacter player = collider.GetComponent<PlayerCharacter>();
        if (player != null) {
            playerInRoom = true;

        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        PlayerCharacter player = collider.GetComponent<PlayerCharacter>();
        if (player != null) {
            playerInRoom = false;
        }
    }

}
