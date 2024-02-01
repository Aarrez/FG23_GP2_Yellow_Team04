using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    public Image[] stars;
    [SerializeField] private Sprite fullStar;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private FinishGoal goalScript;
    public bool goalScriptInScene;

    private void Awake()
    {
        GameManager.onGameStateChanged += GameManagerOnonGameStateChanged;
    }

    private void GameManagerOnonGameStateChanged(GameManager.gameState state)
    {
        if (state == GameManager.gameState.racingState)
        {
            goalScript = GameObject.FindObjectOfType<FinishGoal>();
            if (goalScript == null)
            {
                goalScriptInScene = false;
            }
            else
            {
                goalScriptInScene = true;
            }
            
            for (int i = 0; i < 3; i++)
            {
                stars[i].sprite = emptyStar;
            }
        }
    }
    private void Update()
    {
        if (goalScriptInScene)
        {
            for (int i = 0; i < goalScript.starsEarned; i++)
            {
                stars[i].sprite = fullStar;
            } 
        }
    }

    public void DelGoalScript()
    {
        goalScriptInScene = false;
    }
}
