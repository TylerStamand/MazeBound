using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour {

    [SerializeField] Slider healthBar;
    [SerializeField] Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] TextMeshProUGUI healthText;

    RectTransform rectTransform;

    IDamageable damageable;
    new Camera camera;

    public void Initialize(IDamageable damageable) {
        this.damageable = damageable;
        damageable.OnHealthChange += UpdateHealthBar;
        UpdateHealthBar(damageable, damageable.GetMaxHealth());
        rectTransform = GetComponent<RectTransform>();
        transform.position = ((MonoBehaviour)damageable).transform.position + Vector3.up * 2;

    }

    void Update() {
        if (camera == null) {
            camera = Camera.main;
        } else {
            SetPosition();

            Debug.Log(((MonoBehaviour)damageable).isActiveAndEnabled);
            if (((MonoBehaviour)damageable).isActiveAndEnabled && healthBar.isActiveAndEnabled == false) {
                healthBar.gameObject.SetActive(true);
            } else if (((MonoBehaviour)damageable).isActiveAndEnabled == false && healthBar.isActiveAndEnabled) {
                healthBar.gameObject.SetActive(false);

            }
        }
    }

    void SetPosition() {
        Vector3 targetPosition = ((MonoBehaviour)damageable).transform.position;


        // Convert the world position to screen position
        Vector3 screenPosition = camera.WorldToScreenPoint(targetPosition);

        // Add the offset in screen space
        screenPosition += offset;

        // Set the UI element's position
        rectTransform.position = screenPosition;
    }

    public void UpdateHealthBar(IDamageable damageable, int newHeath) {
        healthBar.value = newHeath / (float)damageable.GetMaxHealth();
        if (damageable is Enemy e) {
            healthText.text = $"{e.Name}: {newHeath}/{damageable.GetMaxHealth()}";
        } else {
            healthText.text = $"Tentacle: {newHeath}/{damageable.GetMaxHealth()}";
        }

    }




}
