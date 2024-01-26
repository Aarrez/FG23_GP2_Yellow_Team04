using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ready : MonoBehaviour
{
    private int readyPlayers = 0;
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
