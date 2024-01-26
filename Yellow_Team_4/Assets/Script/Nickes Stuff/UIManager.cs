using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject readyScreen;
    [SerializeField] private GameObject raceScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject finishScreen;
    
    private void Awake()
    {
        GameManager.onGameStateChanged += GameManagerOnonGameStateChanged;
    }
    
    private void GameManagerOnonGameStateChanged(GameManager.gameState state)
    {
        readyScreen.SetActive(state == GameManager.gameState.readyState);
        raceScreen.SetActive(state == GameManager.gameState.racingState);
        pauseScreen.SetActive(state == GameManager.gameState.pauseState);
        finishScreen.SetActive(state == GameManager.gameState.finishState);
    }
}
