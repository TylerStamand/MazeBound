using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossWall : MonoBehaviour {
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] TriggerObject edge;
    [SerializeField] Direction direction;

    public bool Completed { get; private set; }
    bool closing;
    Vector2 startPosition;

    void Awake() {
        startPosition = transform.position;
        edge.OnTriggerEnterAll += HandleTrigger;
    }

    public IEnumerator CloseWall(float speed) {
        closing = true;
        Completed = false;
        float startX = startPosition.x;

        while (closing) {
            if (direction == Direction.East) {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, 0);
                yield return null;
            }
            if (direction == Direction.West) {
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, 0);
                yield return null;
            }

        }

        Debug.Log("Closing wall");
        if (direction == Direction.East) {
            while (transform.position.x > startX) {
                Debug.Log($"StartX: {startX}, CurrentX: {transform.position.x}");
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, 0);
                yield return null;
            }
        } else {
            while (transform.position.x < startX) {
                Debug.Log($"StartX: {startX}, CurrentX: {transform.position.x}");

                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, 0);
                yield return null;
            }
        }
        transform.position = new Vector3(startX, transform.position.y, 0);

        Completed = true;
    }

    void OnCollisionEnter2D(Collision2D other) {

    }

    void HandleTrigger(GameObject other) {
        Debug.Log("Collision" + other.gameObject.name);
        if (other.gameObject.TryGetComponent<BossWall>(out BossWall wall)) {
            closing = false;
        }
    }

    public void ResetToStartPosition() {
        transform.position = startPosition;
    }

}
