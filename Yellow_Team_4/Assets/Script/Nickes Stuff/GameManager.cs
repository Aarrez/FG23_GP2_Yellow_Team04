using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public gameState state;
    public static event Action<gameState> onGameStateChanged; 
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
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        UpdateGameState(gameState.mainmenuState);
        StartCoroutine(delayedFpsOptim());
    }
    public void UpdateGameState(gameState newState)
    {
        state = newState;

        switch (newState)
        {
            case gameState.mainmenuState:
                break;
            case gameState.settingsState:
                break;
            case gameState.leaderboardState:
                break;
            case gameState.customizationState:
                break;
            case gameState.storeState:
                break;
            case gameState.levelSelectionState:
                break;
            case gameState.readyState:
                Time.timeScale = 0;
                break;
            case gameState.racingState:
                Time.timeScale = 1;
                break;
            case gameState.pauseState:
                Time.timeScale = 0;
                break;
            case gameState.pauseSettingState:
                break;
            case gameState.finishState:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        onGameStateChanged.Invoke(newState);
    }
    public enum gameState
    {
        mainmenuState,
        settingsState,
        leaderboardState,
        customizationState,
        storeState,
        levelSelectionState,
        readyState,
        racingState,
        pauseState,
        pauseSettingState,
        finishState,
    }

    IEnumerator delayedFpsOptim()
    {
        yield return new WaitForSeconds(0.1f);
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }
}
