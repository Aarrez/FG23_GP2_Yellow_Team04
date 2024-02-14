using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using GlobalStructs;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySystem : MonoBehaviour
{
   public List<InventoryItemData> kayakinventory = new();
   public List<InventoryItemData> playerinventory = new();
   public static InventorySystem instance;
   public event Action newItem;
   public AssetData assetData;

   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
      DontDestroyOnLoad(instance);
   }

   void Start() {
      var udata = UserDataManager.upd.GetUserData();
      Add(assetData.GetKayakInventoryData("2"));
      
      Add(assetData.GetPlayerInventoryData("1"));
      Add(assetData.GetPlayerInventoryData("3"));
      Add(assetData.GetPlayerInventoryData("5"));

      foreach(var kayakData in udata.kayakInventory) {         
         Add(assetData.GetKayakInventoryData(kayakData.id));
      }
      foreach(var playerData in udata.playerInventory) {         
         Add(assetData.GetPlayerInventoryData(playerData.id));
      }                     
   }

   public void Add(InventoryItemData refData)
   {
      var udata = UserDataManager.upd.GetUserData();
      if (refData.type == ItemType.KayakTexture)
      {
         var kayakData = udata.kayakInventory.Find(x => x.id == refData.id);
         if (kayakData == null) {            
            SerializeableItem tempItem = new();
            tempItem.type = refData.type;
            tempItem.material = refData.material.name;
            tempItem.name = refData.name;
            tempItem.id = refData.id;
            UserDataManager.upd.ChangeUserData(kayakInventory: tempItem);
         }
         if (kayakinventory.Find(x => x.id == refData.id) == null) {
            kayakinventory.Add(refData);
         }         
      }
      else if (refData.type == ItemType.PlayerTexture)
      {
         var playerData = udata.playerInventory.Find(x => x.id == refData.id);
         if (playerData == null) {
            SerializeableItem tempItem = new();
            tempItem.type = refData.type;
            tempItem.material = refData.material.name;
            tempItem.name = refData.name;
            tempItem.id = refData.id;
            UserDataManager.upd.ChangeUserData(playerInventory: tempItem);
         }
         if (playerinventory.Find(x => x.id == refData.id) == null) {
            playerinventory.Add(refData);
         } 
      }
      newItem?.Invoke();
   }

   public void DestroySelf()
   {
      Destroy(EventSystem.current.currentSelectedGameObject);
   }
}

[Serializable]
public class InventoryItem
{
   public InventoryItemData data { get; private set; }

   public InventoryItem(InventoryItemData source)
   {
      data = source;
   }
}
