using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class InventorySystem : MonoBehaviour
{
   public List<InventoryItemData> kayakinventory;
   public List<InventoryItemData> playerinventory;
   public static InventorySystem instance;
   public event Action newItem;

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

   public void Add(InventoryItemData refData)
   {
      if (refData.type == ItemType.KayakTexture)
      {
         kayakinventory.Add(refData);
      }
      else if (refData.type == ItemType.PlayerTexture)
      {
         playerinventory.Add(refData);
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
