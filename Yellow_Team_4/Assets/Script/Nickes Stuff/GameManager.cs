using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public gameState state;
    public static event Action<gameState> onGameStateChanged;
    [SerializeField] private VideoPlayer loadingAnim;
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
                Time.timeScale = 1;
                break;
            case gameState.settingsState:
                break;
            case gameState.leaderboardState:
                break;
            case gameState.customizationState:
                break;
            case gameState.playerCustomizationState:
                break;
            case gameState.kayakCustomizationState:
                break;
            case gameState.storeState:
                break;
            case gameState.levelSelectionState:
                break;
            case gameState.loadingState:
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
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }
    public enum gameState
    {
        mainmenuState,
        settingsState,
        leaderboardState,
        customizationState,
        playerCustomizationState,
        kayakCustomizationState,
        storeState,
        levelSelectionState,
        loadingState,
        readyState,
        racingState,
        pauseState,
        pauseSettingState,
        finishState,
    }

    IEnumerator delayedFpsOptim()
    {
        yield return new WaitForSeconds(0.5f);
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }
}
