using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetData", menuName = "Data/AssetData", order = 2)]
public class AssetData : ScriptableObject
{
    [SerializeField] private InventoryItemData[] masterKayakData;
    [SerializeField] private InventoryItemData[] masterPlayerData;

    public InventoryItemData GetKayakInventoryData(string id) {
        InventoryItemData inventoryItemData = masterKayakData.First(x=> x.id == id);
        return inventoryItemData;
    }
    public InventoryItemData GetPlayerInventoryData(string id) {
        InventoryItemData inventoryItemData = masterPlayerData.First(x=> x.id == id);
        return inventoryItemData;
    }
}
