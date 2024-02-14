using System;
using System.Collections;
using System.Collections.Generic;
using GlobalStructs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishGoal : MonoBehaviour
{
    public float[] starTimes = new float[3];
    public int starsEarned;
    public int totalCurrencyEarned;
    public int coinsEarned;
    public float measuredTime;
    [SerializeField] private FinishScreen starScript;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {            
            TimeMeasurement timeScript = other.gameObject.GetComponent<TimeMeasurement>();
            measuredTime = timeScript.finalMeasuredTime;
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
            coinsEarned = collectController.coins;
            totalCurrencyEarned = starsEarned * collectController.coins;            
            Debug.Log("Currency earned: " + totalCurrencyEarned);

            var paddleController = other.GetComponent<PaddleController>();
            paddleController.leftCharacterPaddleAnimator.SetBool("isCelebrate", true);
            paddleController.rightCharacterPaddleAnimator.SetBool("isCelebrate", true);            

            LevelCompleteStats stats = new LevelCompleteStats();
            stats.CurrencyEarned = totalCurrencyEarned;
            stats.Time = measuredTime;
            var tempUpd = UserDataManager.upd.GetUserData();
            tempUpd.CurrentCurrency += totalCurrencyEarned;
            UserDataManager.upd.ChangeUserData(tempUpd.CurrentCurrency);
            if (starsEarned == 0) stats.Starts = StarsEarned.Zero;
            if (starsEarned == 1) stats.Starts = StarsEarned.One;
            if (starsEarned == 2) stats.Starts = StarsEarned.Two;
            if (starsEarned == 3) stats.Starts = StarsEarned.Three;
            UserDataManager.LevelComplete?.Invoke(SceneManager.GetActiveScene().buildIndex, stats);          

            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateGameState(GameManager.gameState.finishState);
            }
        }
    }
}
