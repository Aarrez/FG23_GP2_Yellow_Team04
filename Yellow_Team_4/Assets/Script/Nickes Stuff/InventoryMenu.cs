using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = System.Object;
using TMPro;

public class InventoryMenu : MonoBehaviour
{
    [SerializeField] private GameObject kayakIvMenuHolder;
    [SerializeField] private GameObject playerIvMenuHolder;
    private AssetData inventoryData;
    private List<GameObject> spawnedIcons;
    public Material kayakMat;
    public Material playerMat;
    private Kayak kayak;
    public void Initialize()
    {
        InventorySystem.instance.newItem += UpdateMenu;
        spawnedIcons = new List<GameObject>();
        UpdateMenu();
        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            if (spawnedIcons[i].GetComponent<IvDataHolder>().refData.type == ItemType.KayakTexture)
            {
                //kayakMat = spawnedIcons[i].GetComponent<IvDataHolder>().refData.material;
            }
            if (spawnedIcons[i].GetComponent<IvDataHolder>().refData.type == ItemType.PlayerTexture)
            {
                //playerMat = spawnedIcons[i].GetComponent<IvDataHolder>().refData.material;
            }
        }
    }

    private void OnDestroy()
    {
        InventorySystem.instance.newItem -= UpdateMenu;
    }

    public void UpdateMenu()
    {
        if (kayakIvMenuHolder == null) return;
        
        for (int i = 0; i < kayakIvMenuHolder.transform.childCount; i++)
        {
            Destroy(kayakIvMenuHolder.transform.GetChild(i).GameObject());
        }
        foreach (InventoryItemData item in InventorySystem.instance.kayakinventory)
        {
            AddInventorySlot(item, kayakIvMenuHolder.transform);
        }
        for (int i = 0; i < playerIvMenuHolder.transform.childCount; i++)
        {
            Destroy(playerIvMenuHolder.transform.GetChild(i).GameObject());
        }
        foreach (InventoryItemData item in InventorySystem.instance.playerinventory)
        {
            AddInventorySlot(item, playerIvMenuHolder.transform);
        }
    }

    void AddInventorySlot(InventoryItemData item, Transform parent)
    {
        //create the menu icon
        GameObject icon = Instantiate((GameObject)Resources.Load("Item"), parent);
        var im = icon.transform.GetChild(0);
        var txt = icon.transform.GetChild(1);
        TextMeshProUGUI text = txt.GetComponent<TextMeshProUGUI>();
        text.text = item.name;
        icon.GetComponent<IvDataHolder>().refData = item;
        icon.GetComponent<Button>().onClick.AddListener(ApplyCustomization);
        spawnedIcons.Add(icon);
    }

    public void ApplyCustomization()
    {
        if (spawnedIcons.Contains(EventSystem.current.currentSelectedGameObject))
        {
            for (int i = 0; i < spawnedIcons.Count; i++)
            {
                if (spawnedIcons[i] == EventSystem.current.currentSelectedGameObject)
                {
                    var data = spawnedIcons[i].GetComponent<IvDataHolder>().refData;
                    if (data.type == ItemType.KayakTexture)
                    {
                        Debug.Log("Changing Kayak Texture");
                        kayakMat = data.material;
                    }
                    else if (data.type == ItemType.PlayerTexture)
                    {
                        Debug.Log("Changing Player Texture");
                        //playerMat = data.material;
                    }
                }
            }
        }
    }
}
