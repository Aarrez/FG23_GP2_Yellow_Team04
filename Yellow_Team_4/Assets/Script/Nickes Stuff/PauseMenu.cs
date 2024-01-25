using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //Pause menu script, mainly public voids here aswell
    [SerializeField] private GameObject pauseScreen;

    private void Update()
    {
        if (GameManager.instance.state == GameManager.gameState.pauseState)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
        }
        else if (GameManager.instance.state == GameManager.gameState.racingState)
        {
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
        }
    }

    public void PauseGame()
    {
       GameManager.instance.UpdateGameState(GameManager.gameState.pauseState);
    }

    public void Continue()
    {
        GameManager.instance.UpdateGameState(GameManager.gameState.racingState);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
