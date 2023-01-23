using UnityEngine.Tilemaps;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class Room : MonoBehaviour {

  

    Vector2Int origin; //bottom-left vertex

    bool playerInRoom;

    new TilemapCollider2D collider;
    Tilemap tilemap;


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
