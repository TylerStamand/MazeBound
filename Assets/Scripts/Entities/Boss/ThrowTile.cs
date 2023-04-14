using UnityEngine;
using System;

public class ThrowTile : MonoBehaviour {

    public event Action OnDestroyed;

    PlayerCharacter player;

    float speed;
    int damage;

    void Awake() {
        player = FindObjectOfType<PlayerCharacter>();
    }


    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    public void Initialize(float speed, int damage) {
        this.speed = speed;
        this.damage = damage;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (TryGetComponent<PlayerCharacter>(out PlayerCharacter player)) {
            player.TakeDamage(damage);
            OnDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
