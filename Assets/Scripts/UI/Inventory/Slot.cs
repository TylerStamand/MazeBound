using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    [SerializeField] Image itemImageSlot;
    [SerializeField] Sprite itemSilhouette;
    [SerializeField] DescriptionBox DescriptionBoxPrefab;

    public bool ItemSet;
    public Item Item { get; private set; }

    public event Action<Slot> OnLeftClick;
    public event Action<Slot> OnRightClick;

    Canvas canvas;

    DescriptionBox currentDescriptionObject;

    ItemData itemData;

    void Awake() {
        canvas = GetComponentInParent<Canvas>();


        if (itemSilhouette != null && ItemSet == false) {
            itemImageSlot.sprite = itemSilhouette;
            itemImageSlot.color = new Color(1, 1, 1, 0.6f);
        }

    }

    void OnDestroy() {
        if (currentDescriptionObject != null) {
            Destroy(currentDescriptionObject.gameObject);
        }
    }

    public void SetItem(Item item) {
        Debug.Log("Got Item: " + item?.ItemName);
        if (item == null) {
            Debug.Log("Item is null");
            this.Item = null;
            itemData = null;
            ItemSet = false;

            if (itemSilhouette != null) {
                itemImageSlot.sprite = itemSilhouette;
                itemImageSlot.color = new Color(1, 1, 1, 0.6f);
            } else {
                itemImageSlot.sprite = null;
                itemImageSlot.color = Color.clear;
            }
            return;
        }

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
        currentDescriptionObject = Instantiate(DescriptionBoxPrefab, canvas.transform, false);
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
