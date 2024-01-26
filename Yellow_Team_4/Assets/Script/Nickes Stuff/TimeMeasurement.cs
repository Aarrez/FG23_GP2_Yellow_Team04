using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMeasurement : MonoBehaviour
{
    private float currentMeasuredTime;
    public float finalMeasuredTime;
    private float startTime;
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
            startTime = Time.timeSinceLevelLoad;
        }

        if (state == GameManager.gameState.finishState)
        {
            finalMeasuredTime = currentMeasuredTime;
            Debug.Log(finalMeasuredTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (hasStarted)
        {
            currentMeasuredTime = Time.timeSinceLevelLoad - startTime;
        }
    }
}
