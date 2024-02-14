using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject leaderboardScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject customizationScreen;
    [SerializeField] private GameObject playerCustomization;
    [SerializeField] private GameObject kayakCustomization;
    [SerializeField] private GameObject storeScreen;
    [SerializeField] private GameObject levelScreen;
    [SerializeField] private GameObject readyScreen;
    [SerializeField] private GameObject raceScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject pauseSettingsScreen;
    [SerializeField] private GameObject finishScreen;
    public static UIManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameManager.onGameStateChanged += GameManagerOnonGameStateChanged;            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {
        FindObjectOfType<MainMenu>().Initialize();
        FindObjectOfType<Ready>().Initialize();
        FindObjectOfType<InventoryMenu>().Initialize();
        FindObjectOfType<FinishScreen>().Initialize();
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameManagerOnonGameStateChanged;
    }

    public void EnableScreen(GameObject enableObject)
    {
        enableObject.SetActive(true);
    }

    public void DisableScreen(GameObject disableObject)
    {
        disableObject.SetActive(false);
    }

    private void GameManagerOnonGameStateChanged(GameManager.gameState state)
    {
        mainMenuScreen.SetActive(state == GameManager.gameState.mainmenuState);
        settingsScreen.SetActive(state == GameManager.gameState.settingsState);
        leaderboardScreen.SetActive(state == GameManager.gameState.leaderboardState);
        customizationScreen.SetActive(state == GameManager.gameState.customizationState);
        playerCustomization.SetActive(state == GameManager.gameState.playerCustomizationState);
        kayakCustomization.SetActive(state == GameManager.gameState.kayakCustomizationState);
        storeScreen.SetActive(state == GameManager.gameState.storeState);
        levelScreen.SetActive(state == GameManager.gameState.levelSelectionState);
        loadingScreen.SetActive(state == GameManager.gameState.loadingState);
        readyScreen.SetActive(state == GameManager.gameState.readyState);
        raceScreen.SetActive(state == GameManager.gameState.racingState);
        pauseScreen.SetActive(state == GameManager.gameState.pauseState);
        pauseSettingsScreen.SetActive(state == GameManager.gameState.pauseSettingState);
        finishScreen.SetActive(state == GameManager.gameState.finishState);
    }
}
