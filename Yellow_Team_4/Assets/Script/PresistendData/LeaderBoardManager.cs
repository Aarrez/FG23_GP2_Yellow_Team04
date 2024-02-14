using System;
using System.Collections.Generic;
using GlobalStructs;
using PresistentData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    public static Func<string> GetName;
    public static UnityAction<LevelCompleteStats> LeaderboardLoaded;
    
    [SerializeField] private GameObject slot;
    private LevelCompleteStats leaderBoardData;
    private Button[] buttons;
    private Button send, read, clear;
    private Toggle completedLevel;
    private string playerName;
    private int activeBuildIndex;
    
    private void Awake()
    {
        activeBuildIndex = SceneManager.GetActiveScene().buildIndex;
    }
    
    private void OnEnable()
    {
        ReadLeaderBoard();
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddDefaultData();
            ReadLeaderBoard();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            RemoveDefaults();
        }
    }
#endif
    
    private void ReadLeaderBoard()
    {
        List<LevelCompleteStats> dataset;
        try
        {
            dataset =
                UserDataManager.upd.GetUserData().LevelData[activeBuildIndex];
        }
        catch (Exception e)
        {
            UserDataManager.InstatiateUpd();
            dataset =
                UserDataManager.upd.GetUserData().LevelData[1];
        }
       
        LeaderboardLoaded?.Invoke(dataset[0]);
        playerName = GetName?.Invoke();
        var childComp = transform.GetComponentsInChildren<TMPro.TMP_Text>();
        for (int i = 0; i < childComp.Length; i++)
        {
            if (i >= dataset.Count)
            {
                childComp[i].text = "";
                continue;
            }
                
            string text = $"{playerName}, Stars: {((int)dataset[i].Starts).ToString()}, Time: {dataset[i].Time.ToString()}";
            childComp[i].text = text;
        }
    }

    private void AddDefaultData()
    {
        UserDataManager.upd.ChangeUserData(1, UserDataManager.upd.defaultData.LevelData[1][0]);
    }

    private void RemoveDefaults()
    {
        var d = UserDataManager.upd.GetUserData();
        foreach (var levelScore in d.LevelData)
        {
            for (int i = 0; i < levelScore.Value.Count; i++)
            {
                if (levelScore.Value[i].Time <= 1)
                {
                    levelScore.Value.Remove(levelScore.Value[i]);
                }
            }
        }
            
        
        UserDataManager.upd.SaveData(d);
                    
    }

    private void AddLeaderBoardData(LevelCompleteStats levelStats)
    {
        UserDataManager.upd.ChangeUserData(activeBuildIndex, levelStats);
    }
}
