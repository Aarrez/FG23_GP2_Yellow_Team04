using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMeasurement : MonoBehaviour
{
    private float currentMeasuredTime;
    public float finalMeasuredTime;
    private float startTime;

    private void Awake()
    {
        GameManager.onGameStateChanged += GameManagerOnonGameSateChanged;
    }

    private void GameManagerOnonGameSateChanged(GameManager.gameState state)
    {
       
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentMeasuredTime = Time.timeSinceLevelLoad - startTime;
    }
}
