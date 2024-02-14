using System.Collections.Generic;
using GlobalStructs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CurrentLevelStats : MonoBehaviour
{
    public static UnityAction<LevelCompleteStats> GetStats;

    [SerializeField] private GameObject[] Stars;
    
    private List<RawImage> starImage = new();

    [SerializeField] private TMPro.TMP_Text[] CurrencyTexts;
    [SerializeField] private TMPro.TMP_Text timeText;


    private void Awake()
    {
        foreach (var stars in Stars)
        {
            var imgArray = stars.transform.GetComponentsInChildren<RawImage>();
            foreach (var img in imgArray)
            {
                starImage.Add(img);
            }
        }
    }

    private void OnEnable()
    {
        LeaderBoardManager.LeaderboardLoaded += UpdateStats;
    }

    private void OnDisable()
    {
        LeaderBoardManager.LeaderboardLoaded -= UpdateStats;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LevelCompleteStats stats;
            stats.Starts = StarsEarned.Three;
            stats.CurrencyEarned = 10;
            stats.Time = 3.14f;
            UpdateStats(stats);
        }
    }
    private void UpdateStats(LevelCompleteStats stats)
    {
        
        for (int i = 0; i < starImage.Count/2; i++)
        {
            if (i <= (int)stats.Starts -1)
            {
                starImage[i].color = Color.yellow;
                starImage[i + starImage.Count/2].color = Color.yellow;
            }
        }

        foreach (var text in CurrencyTexts)
        {
            text.text = stats.CurrencyEarned.ToString();
        }

        timeText.text = stats.Time.ToString();
    }
}
