using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.AdaptivePerformance.VisualScripting;

public class FinishScreen : MonoBehaviour
{
    public Image[] stars;
    public Image[] smallStars;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI[] collectedCurrencyText;
    public TextMeshProUGUI finalCurrencyText;
    public Transform leadboardParent;
    public Sprite[] trophySprites;
    [SerializeField] private FinishGoal goalScript;
    public bool goalScriptInScene;

    private Animator animator;
    public void Initialize()
    {
        GameManager.onGameStateChanged += GameManagerOnonGameStateChanged;
        animator = GetComponent<Animator>();
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

            foreach (var star in stars) {
                star?.gameObject.SetActive(false);
            }

            foreach (var star in smallStars) {
                star?.gameObject.SetActive(false);
            }              
        }
        else if (state == GameManager.gameState.finishState) {
            if (goalScript != null) {
                for (int i = 0; i < goalScript.starsEarned; i++)
                {
                    stars[i]?.gameObject.SetActive(true);
                    smallStars[i]?.gameObject.SetActive(true);
                } 
                // update collected currency
                foreach(var cct in collectedCurrencyText) {
                    cct.text = goalScript.coinsEarned.ToString();
                }
                // update final currency
                finalCurrencyText.text = goalScript.totalCurrencyEarned.ToString();

                // update time
                var measuredTimeInSeconds = goalScript.measuredTime;
                var t = TimeSpan.FromSeconds(measuredTimeInSeconds);
                timeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", 
                    t.Hours, 
                    t.Minutes, 
                    t.Seconds);

                animator.SetBool("isEnter", true);

                var currentLevel = SceneManager.GetActiveScene().buildIndex;
                var levelStats = UserDataManager.upd.GetUserData().LevelData[currentLevel];
                var orderedLevelStats = levelStats.OrderByDescending(x=> x.Starts).OrderBy( x=> x.Time ).ToList();
                var idx = 0;
                for(int i=0; i<leadboardParent.childCount; i++) {
                    if (i < orderedLevelStats.Count()) {
                        var ol = orderedLevelStats[i];
                        var orls = leadboardParent.GetChild(i);
                        orls.gameObject.SetActive(true);                    
                        var im = orls.transform.Find("Image").GetComponent<Image>();
                        if (idx < 3) {
                            im.enabled = true;
                            im.sprite = trophySprites[idx];
                            im.preserveAspect = true;
                        } else {
                            im.enabled = false;
                        }
                        orls.transform.Find("Image/Rank").GetComponent<TextMeshProUGUI>().text = (idx + 1).ToString();                    
                        orls.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = UserDataManager.upd.GetUserData().userName + (idx + 1).ToString();
                        var hst = TimeSpan.FromSeconds(ol.Time);
                        orls.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = string.Format("{0:D2}:{1:D2}:{2:D2}", 
                        hst.Hours, 
                        hst.Minutes, 
                        hst.Seconds);
                        idx++;                                                                                                  
                    }
                    else {
                        leadboardParent.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        } else {
            animator.SetBool("isEnter", false);
        }        
    }
}
