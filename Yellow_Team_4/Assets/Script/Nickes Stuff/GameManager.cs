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
        instance = this;
    }
    private void Start()
    {
        UpdateGameState(gameState.readyState);
    }

    public void UpdateGameState(gameState newState)
    {
        state = newState;

        switch (newState)
        {
            case gameState.readyState:
                Time.timeScale = 0;
                break;
            case gameState.racingState:
                Time.timeScale = 1;
                break;
            case gameState.pauseState:
                Time.timeScale = 0;
                break;
            case gameState.finishState:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        onGameStateChanged?.Invoke(newState);
    }
    public enum gameState
    {
        readyState,
        racingState,
        pauseState,
        finishState,
    }
}
