using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject leaderboardScreen;
    [SerializeField] private GameObject customizationScreen;
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
        }
        else
        {
            Destroy(gameObject);
        }
        GameManager.onGameStateChanged += GameManagerOnonGameStateChanged;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameManagerOnonGameStateChanged;
    }

    private void GameManagerOnonGameStateChanged(GameManager.gameState state)
    {
        mainMenuScreen.SetActive(state == GameManager.gameState.mainmenuState);
        settingsScreen.SetActive(state == GameManager.gameState.settingsState);
        leaderboardScreen.SetActive(state == GameManager.gameState.leaderboardState);
        customizationScreen.SetActive(state == GameManager.gameState.customizationState);
        storeScreen.SetActive(state == GameManager.gameState.storeState);
        levelScreen.SetActive(state == GameManager.gameState.levelSelectionState);
        readyScreen.SetActive(state == GameManager.gameState.readyState);
        raceScreen.SetActive(state == GameManager.gameState.racingState);
        pauseScreen.SetActive(state == GameManager.gameState.pauseState);
        pauseSettingsScreen.SetActive(state == GameManager.gameState.pauseSettingState);
        finishScreen.SetActive(state == GameManager.gameState.finishState);
    }
}
