using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //Pause menu script, mainly public voids here aswell
    public void PauseGame()
    {
       GameManager.instance.UpdateGameState(GameManager.gameState.pauseState);
    }

    public void Continue()
    {
        GameManager.instance.UpdateGameState(GameManager.gameState.racingState);
    }

    public void SettingsMenu()
    {
        GameManager.instance.UpdateGameState(GameManager.gameState.pauseSettingState);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        GameManager.instance.UpdateGameState(GameManager.gameState.mainmenuState);
    }

    public void Restart()
    {
        Scene currentscene;
        currentscene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentscene.buildIndex);
        GameManager.instance.UpdateGameState(GameManager.gameState.readyState);
    }
}
