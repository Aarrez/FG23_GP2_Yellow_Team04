using System;
using UnityEngine;

public enum ItemType
{
    KayakTexture,
    PlayerTexture
}
[CreateAssetMenu(menuName = "Inventory Item Data")]
[Serializable]
public class InventoryItemData : ScriptableObject
{
    public string id;
    public string name;
    public ItemType type;
    public Material material;
}
