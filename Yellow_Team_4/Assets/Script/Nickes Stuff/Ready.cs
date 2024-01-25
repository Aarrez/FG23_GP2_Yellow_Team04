using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ready : MonoBehaviour
{
    [SerializeField] private GameObject readyScreen;
    [SerializeField] private GameObject raceScreen;
    private int readyPlayers = 0;
    void Update()
    {
        if (GameManager.instance.state == GameManager.gameState.readyState)
        {
            Time.timeScale = 0;
            readyScreen.SetActive(true);
            raceScreen.SetActive(false);
        }
        else if (GameManager.instance.state == GameManager.gameState.racingState)
        {
            Time.timeScale = 1;
            readyScreen.SetActive(false);
            raceScreen.SetActive(true);
        }
    }

    public void ReadyUp()
    {
        readyPlayers++;
        EventSystem.current.currentSelectedGameObject.SetActive(false);
        if (readyPlayers >= 2)
        {
            GameManager.instance.UpdateGameState(GameManager.gameState.racingState);
            //Start game
        }
    }
}
