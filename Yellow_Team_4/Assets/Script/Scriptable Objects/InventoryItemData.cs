using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ItemType
{
    KayakTexture,
    PlayerTexture
}
[CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    public string id;
    public string name;
    public ItemType type;
    public Sprite icon;
    public Material material;
}
