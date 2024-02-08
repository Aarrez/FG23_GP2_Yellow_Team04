using System;
using System.Collections;
using System.Collections.Generic;
using GlobalStructs;
using UnityEngine;

public class FinishGoal : MonoBehaviour
{
    public float[] starTimes = new float[3];
    public int starsEarned;
    [SerializeField] private Stars starScript;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateGameState(GameManager.gameState.finishState);
            }
            TimeMeasurement timeScript = other.gameObject.GetComponent<TimeMeasurement>();
            float measuredTime = timeScript.finalMeasuredTime;
            for (int i = 0; i < 3; i++)
            {
                int comparison = measuredTime.CompareTo(starTimes[i]);
                if (comparison <= 0)
                {
                    starsEarned++;
                }
            }
            Debug.Log("You have earned "+starsEarned+" stars!");
            // calculate currency earned here
            var collectController = other.GetComponent<CollectController>();
            var totalCurrencyEarned = starsEarned * collectController.coins;
            Debug.Log("Currency earned: " + totalCurrencyEarned);

            LevelCompleteStats stats = new LevelCompleteStats();
            stats.CurrencyEarned = totalCurrencyEarned;
            stats.Time = measuredTime;
            if (starsEarned == 0) stats.Starts = StarsEarned.Zero;
            if (starsEarned == 1) stats.Starts = StarsEarned.One;
            if (starsEarned == 2) stats.Starts = StarsEarned.Two;
            if (starsEarned == 3) stats.Starts = StarsEarned.Three;
            UserDataManager.LevelComplete?.Invoke(0, stats);          
        }
    }
}
