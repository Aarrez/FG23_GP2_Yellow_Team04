using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "shopMenu", menuName = "Scriptable objects/New Shop item", order = 1)]
public class ShopItemSO : ScriptableObject
{
    public string title;
    public string description;
    public int baseCost;
    public int starsAmount;
    public InventoryItemData LinkedItemData;
}
