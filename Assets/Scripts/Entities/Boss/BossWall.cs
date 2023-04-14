using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossWall : MonoBehaviour {
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Direction direction;

    public bool Completed { get; private set; }

    bool closing;
    public IEnumerator CloseWall(float speed) {
        closing = true;
        Completed = false;
        float startX = transform.position.x;

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

        if (direction == Direction.East) {
            while (transform.position.x > startX) {
                transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, 0);
                yield return null;
            }
        } else {
            while (transform.position.x < startX) {
                transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, 0);
                yield return null;
            }
        }
        transform.position = new Vector3(startX, transform.position.y, 0);

        Completed = true;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (TryGetComponent<BossWall>(out BossWall wall)) {
            closing = false;
        }
    }

}
