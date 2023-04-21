using UnityEngine;
using System;

public class ThrowTile : MonoBehaviour {

    [SerializeField] GameObject shadowSprite;
    [SerializeField] float damageHeight;

    public event Action OnDestroyed;

    public bool Destroyed { get; private set; }

    PlayerCharacter player;

    float height;
    float speed;
    int damage;


    void Awake() {
        player = FindObjectOfType<PlayerCharacter>();
        height = 10;
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        height -= Time.deltaTime * speed;

        shadowSprite.transform.position = new Vector3(transform.position.x, transform.position.y - height, transform.position.z);

        if (height <= 0) {
            Destroy(gameObject);
        }
    }

    public void Initialize(float speed, int damage, Sprite sprite) {
        this.speed = speed;
        this.damage = damage;
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (height > damageHeight) {
            return;
        }
        if (other.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter player)) {
            player.TakeDamage(damage);
            OnDestroyed?.Invoke();
            Destroyed = true;
            gameObject.SetActive(false);
        }
    }
}
