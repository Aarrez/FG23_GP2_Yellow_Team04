using System;
using System.Collections;
using System.Collections.Generic;
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
            GameManager.instance.UpdateGameState(GameManager.gameState.finishState);
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
        }
    }
}
