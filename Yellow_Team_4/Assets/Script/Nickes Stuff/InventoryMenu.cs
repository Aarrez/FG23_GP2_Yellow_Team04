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
    public Material kayakMat;
    public Material playerMat;
    private Kayak kayak;

    private List<GameObject> spawnedPlayerIcons;
    private List<GameObject> spawnedKayakIcons;

    public void Initialize()
    {
        InventorySystem.instance.newItem += UpdateMenu;
        // spawnedIcons = new List<GameObject>();
        spawnedPlayerIcons = new List<GameObject>();
        spawnedKayakIcons = new List<GameObject>();
        UpdateMenu();
        for (int i = 1; i < spawnedPlayerIcons.Count; i++)
        {
            // if (spawnedIcons[i].GetComponent<IvDataHolder>().refData.type == ItemType.KayakTexture)
            // {
            //     //kayakMat = spawnedIcons[i].GetComponent<IvDataHolder>().refData.material;
            // }
            // if (spawnedIcons[i].GetComponent<IvDataHolder>().refData.type == ItemType.PlayerTexture)
            // {
            //     //playerMat = spawnedIcons[i].GetComponent<IvDataHolder>().refData.material;
            // }
            if (spawnedPlayerIcons[i] != null) {
                spawnedPlayerIcons[i].transform.Find("Sprite").gameObject.SetActive(false);
            }            
        }
        for (int i = 1; i < spawnedKayakIcons.Count; i++)
        {
            // if (spawnedIcons[i].GetComponent<IvDataHolder>().refData.type == ItemType.KayakTexture)
            // {
            //     //kayakMat = spawnedIcons[i].GetComponent<IvDataHolder>().refData.material;
            // }
            // if (spawnedIcons[i].GetComponent<IvDataHolder>().refData.type == ItemType.PlayerTexture)
            // {
            //     //playerMat = spawnedIcons[i].GetComponent<IvDataHolder>().refData.material;
            // }
            if (spawnedKayakIcons[i] != null) {
                spawnedKayakIcons[i].transform.Find("Sprite").gameObject.SetActive(false);
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
        int idx = 0;
        foreach (InventoryItemData item in InventorySystem.instance.kayakinventory)
        {
            var icon = AddInventorySlot(item, kayakIvMenuHolder.transform);                        
            if (idx > 0) icon.transform.Find("Sprite").gameObject.SetActive(false);
            idx++;
        }
        for (int i = 0; i < playerIvMenuHolder.transform.childCount; i++)
        {
            Destroy(playerIvMenuHolder.transform.GetChild(i).GameObject());
        }
        idx = 0;
        foreach (InventoryItemData item in InventorySystem.instance.playerinventory)
        {
            var icon = AddInventorySlot(item, playerIvMenuHolder.transform);
            if (idx > 0) icon.transform.Find("Sprite").gameObject.SetActive(false);
            idx++;
        }
    }

    GameObject AddInventorySlot(InventoryItemData item, Transform parent)
    {
        //create the menu icon
        GameObject icon = Instantiate((GameObject)Resources.Load("Item"), parent);
        var im = icon.transform.GetChild(0);
        var txt = icon.transform.GetChild(1);
        TextMeshProUGUI text = txt.GetComponent<TextMeshProUGUI>();
        text.text = item.name;
        icon.GetComponent<IvDataHolder>().refData = item;
        icon.GetComponent<Button>().onClick.AddListener(ApplyCustomization);
        if (item.type == ItemType.KayakTexture) {
            spawnedKayakIcons.Add(icon);
        }
        else if (item.type == ItemType.PlayerTexture) {
            spawnedPlayerIcons.Add(icon);
        }
        // spawnedIcons.Add(icon);
        return icon;
    }

    public void ApplyCustomization()
    {
        if (spawnedKayakIcons.Contains(EventSystem.current.currentSelectedGameObject))
        {
            for (int i = 0; i < spawnedKayakIcons.Count; i++)
            {
                if (spawnedKayakIcons[i] == EventSystem.current.currentSelectedGameObject)
                {
                    spawnedKayakIcons[i].transform.Find("Sprite").gameObject.SetActive(true);
                    var data = spawnedKayakIcons[i].GetComponent<IvDataHolder>().refData;
                    if (data.type == ItemType.KayakTexture)
                    {
                        kayakMat = data.material;
                    }
                    else if (data.type == ItemType.PlayerTexture)
                    {
                        playerMat = data.material;
                    }
                } else {
                    if (spawnedKayakIcons[i] != null) {
                        spawnedKayakIcons[i].transform.Find("Sprite").gameObject.SetActive(false);
                    }
                }            
            }
        }

        if (spawnedPlayerIcons.Contains(EventSystem.current.currentSelectedGameObject))
        {
            for (int i = 0; i < spawnedPlayerIcons.Count; i++)
            {
                if (spawnedPlayerIcons[i] == EventSystem.current.currentSelectedGameObject)
                {
                    spawnedPlayerIcons[i].transform.Find("Sprite").gameObject.SetActive(true);
                    var data = spawnedPlayerIcons[i].GetComponent<IvDataHolder>().refData;
                    if (data.type == ItemType.KayakTexture)
                    {
                        kayakMat = data.material;
                    }
                    else if (data.type == ItemType.PlayerTexture)
                    {
                        playerMat = data.material;
                    }
                } else {
                    if (spawnedPlayerIcons[i] != null) {
                        spawnedPlayerIcons[i].transform.Find("Sprite").gameObject.SetActive(false);
                    }
                }            
            }
        }
    }
}
