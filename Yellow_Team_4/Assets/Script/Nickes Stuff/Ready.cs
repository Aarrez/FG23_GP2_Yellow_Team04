using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ready : MonoBehaviour
{
    private bool isReady;
    private int readyPlayers = 0;
    private int buttonsPressed;
    [SerializeField] private GameObject[] disabledButtons = new GameObject[2];
    private void Awake()
    {
        GameManager.onGameStateChanged += GameManagerOnonGameStateChanged;
    }
    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameManagerOnonGameStateChanged;
    }
    private void GameManagerOnonGameStateChanged(GameManager.gameState state)
    {
        if (state == GameManager.gameState.readyState)
        {
            foreach (GameObject button in disabledButtons)
            {
                button.SetActive(true);
            }
        }
    }

    public void ReadyUp(bool isPressed)
    {
        isReady = isPressed;
        if (isReady)
        {
            buttonsPressed++;
        }
        else
        {
            buttonsPressed--;
        }
    }
    private void Update()
    {
        if (isReady)
        {
            if (buttonsPressed >= 2)
            {
                GameManager.instance.UpdateGameState(GameManager.gameState.racingState);
                buttonsPressed = 0;
            }
        }

        if (Input.GetKeyDown((KeyCode.Space)))
        {
            isReady = true;
            buttonsPressed = 2;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isReady = false;
            buttonsPressed = 0;
        }
    }
}
