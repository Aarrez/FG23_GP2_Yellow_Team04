using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMeasurement : MonoBehaviour
{
    public float finalMeasuredTime;
    private bool hasStarted;

    private void Awake()
    {
        GameManager.onGameStateChanged += GameManagerOnonGameSateChanged;
    }
    private void GameManagerOnonGameSateChanged(GameManager.gameState state)
    {
        if (state == GameManager.gameState.racingState)
        {
            hasStarted = true;
        }
        if (state == GameManager.gameState.finishState)
        {
            hasStarted = false;
            Debug.Log(finalMeasuredTime);
        }
        if (state == GameManager.gameState.mainmenuState)
        {
            hasStarted = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (hasStarted)
        {
            finalMeasuredTime = Time.timeSinceLevelLoad;
        }
    }
}
