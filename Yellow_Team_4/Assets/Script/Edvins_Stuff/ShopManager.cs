using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int coins;
    public int stars;
    public TMP_Text coinUI;
    public TMP_Text starUI;
    public TMP_Text starCostUI;

    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;

    public GameObject[] lockedScreens;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        }
        coinUI.text = "Coins: " + coins.ToString();
        starUI.text = "Stars: " + stars.ToString();
        LoadPanels();
        CheckPurchaseable();
        DeactivateLockScreen();
    }

    public void AddCoins()
    {
        coins++;
        coinUI.text = "Coins: " + coins.ToString();
        CheckPurchaseable();
    }

    public void CheckPurchaseable()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            if (coins >= shopItemsSO[i].baseCost)
            {
                myPurchaseBtns[i].interactable = true;
            }
            else
            {
                myPurchaseBtns[i].interactable = false;
            }
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if (coins >= shopItemsSO[btnNo].baseCost)
        {
            coins = coins - shopItemsSO[btnNo].baseCost;
            coinUI.text = "Coins: " + coins.ToString();
            CheckPurchaseable();
        }
    }

    public void LoadPanels()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanels[i].titleTxt.text = shopItemsSO[i].title;
            shopPanels[i].descriptionTxt.text = shopItemsSO[i].description;
            shopPanels[i].costTxt.text = "Cost: " + shopItemsSO[i].baseCost;
            shopPanels[i].starsAmountTxt.text = "Star Cost: " + shopItemsSO[i].starsAmount;
        }
    }

    public void DeactivateLockScreen()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            if (stars >= shopItemsSO[i].starsAmount)
            {
                lockedScreens[i].SetActive(false);
            }
        }
    }
}
