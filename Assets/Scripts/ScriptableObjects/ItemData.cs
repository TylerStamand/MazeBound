using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject {
    public static readonly string DefaultName = "Weapon Name";
    [field: SerializeField] public string Name { get; private set; } = DefaultName;
    [field: SerializeField] public ItemType ItemType { get; private set; }
    [field: SerializeField] public bool StackAble { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }

    public abstract Item CreateItem();
}

// Item Types enum


public enum ItemType {
    Weapon,
    Potion
}