using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField]private int coins;
    [SerializeField] private int stars;
    public TMP_Text coinUI;
    public TMP_Text starUI;

    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;
    [SerializeField] private MainMenu mainmenu;

    public GameObject[] lockedScreens;
    // Start is called before the first frame update
    private void Awake()
    {
        coins = new int();
        stars = new int();
        GameManager.onGameStateChanged += GiveCoins;
    }

    void Start()
    {
        coins = mainmenu.totalcoins;
        stars = mainmenu.totalstars;
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanelsGO[i].SetActive(true);
        }
        coinUI.text = coins.ToString();
        starUI.text = stars.ToString();
        LoadPanels();
        CheckPurchaseable();
        DeactivateLockScreen();
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

            for (int j = 0; j < shopItemsSO.Length; j++)
            {
                if (InventorySystem.instance.kayakinventory.Contains(shopItemsSO[i].LinkedItemData))
                {
                    myPurchaseBtns[i].interactable = false;
                }   
            }

        }
        DeactivateLockScreen();
    }

    public void PurchaseItem(int btnNo)
    {
        if (coins >= shopItemsSO[btnNo].baseCost)
        {
            var tempUpd = UserDataManager.upd.GetUserData();
            tempUpd.CurrentCurrency -= shopItemsSO[btnNo].baseCost;
            UserDataManager.upd.ChangeUserData(tempUpd.CurrentCurrency);
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
            else
            {
                lockedScreens[i].SetActive(true);
            }
        }
    }

    
    void GiveCoins(GameManager.gameState state)
    {
        if (state == GameManager.gameState.storeState)
        {
            CheckPurchaseable();   
        }
    }
    

    private void Update()
    {
        coins = mainmenu.totalcoins;
        stars = mainmenu.totalstars;
        coinUI.text = coins.ToString();
        starUI.text = stars.ToString();
    }
}
