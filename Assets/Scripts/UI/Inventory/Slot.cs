using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] Image itemImageSlot;
    [SerializeField] DescriptionBox DescriptionBoxPrefab;

    public bool ItemSet;
    public Item Item { get; private set; }

    public event Action<Slot> OnLeftClick;
    public event Action<Slot> OnRightClick;

    Canvas canvas;

    DescriptionBox currentDescriptionObject;

    ItemData itemData;

    void Awake() {
        // if (TryGetComponent<Button>(out Button button)) {
        //     button.onClick.AddListener(HandleButtonClick);
        // }
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
        if (item == null) {
            this.Item = null;
            itemData = null;
            itemImageSlot.sprite = null;
            itemImageSlot.color = Color.clear;
            ItemSet = false;
            return;
        };

        this.Item = item;
        itemData = ResourceManager.Instance.GetItemData(item.ItemName);
        Sprite itemSprite = itemData.Sprite;
        itemImageSlot.sprite = itemSprite;
        itemImageSlot.color = Color.white;

        ItemSet = true;
    }

    void HandleButtonClick() {
        OnLeftClick?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (Item == null) return;
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
        currentDescriptionObject = Instantiate(DescriptionBoxPrefab);
        currentDescriptionObject.transform.SetParent(canvas.transform);
        currentDescriptionObject.transform.SetAsLastSibling();
        currentDescriptionObject.Title.text = itemData.Name;
        currentDescriptionObject.Description.text = Item.GetDescription();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            OnLeftClick?.Invoke(this);
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            OnRightClick?.Invoke(this);
        }
    }
}
