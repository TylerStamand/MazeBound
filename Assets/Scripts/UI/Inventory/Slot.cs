using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] Image itemImageSlot;
    [SerializeField] DescriptionBox DescriptionBoxPrefab;

    public bool ItemSet;
    public Item Item { get; private set; }

    public event Action<Slot> OnClick;

    Canvas canvas;

    DescriptionBox currentDescriptionObject;


    void Awake() {
        if (TryGetComponent<Button>(out Button button)) {
            Debug.Log("setting button click");
            button.onClick.AddListener(HandleButtonClick);
        }
        ItemSet = false;

        canvas = GetComponentInParent<Canvas>();
    }

    void OnDestroy() {
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
    }

    public void SetItem(Item item) {
        Debug.Log("Got Item: " + item);
        if (item == null) return;

        Debug.Log("Setting Item");
        this.Item = item;
        Sprite itemSprite = item.ItemData.Sprite;
        itemImageSlot.sprite = itemSprite;
        itemImageSlot.color = Color.white;

        ItemSet = true;
    }

    void HandleButtonClick() {
        Debug.Log("Handling button click");
        OnClick?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (Item.ItemID.Equals("")) return;
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
        currentDescriptionObject = Instantiate(DescriptionBoxPrefab);
        currentDescriptionObject.transform.SetParent(canvas.transform);
        currentDescriptionObject.transform.SetAsLastSibling();
        currentDescriptionObject.Title.text = Item.ItemData.Name;
        currentDescriptionObject.Description.text = Item.ItemData.Description;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
    }
}
