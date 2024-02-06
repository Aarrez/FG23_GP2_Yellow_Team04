using System;
using System.Collections.Generic;
using System.Linq;
using PresistentData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    public static UnityAction<string, int, float, bool> SaveLeaderboardData;
    public static Func<string> GetName; 
    
    [SerializeField] private GameObject slot;
    private List<GameObject> slotsInstantiated = new();
    private Transform slotContainer;
    private LeaderBoardData leaderBoardData;
    private Button[] buttons;
    private Button send, read, clear;
    private PresistentLeaderBoard leaderBoard;
    private TMPro.TMP_InputField inputName;
    private Toggle completedLevel;
    
    private void Awake()
    {
        string jsonPath = Application.persistentDataPath + "/ScoreBoard.json";
        leaderBoard = new PresistentLeaderBoard(jsonPath);
        slotContainer = transform;
        inputName = GetComponentInChildren<TMPro.TMP_InputField>();
    }
    
    private void OnEnable()
    {
        ReadLeaderBoard();
        SaveLeaderboardData += AddLeaderBoardData;
    }

    private void OnDisable()
    {
        SaveLeaderboardData -= AddLeaderBoardData;
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
            ClearLeaderBoard();
            ReadLeaderBoard();
        }
    }
    private void AddDefaultData()
    {
        leaderBoard.SaveData(leaderBoard.DefaultData[0]);
    }
    
    public void ClearLeaderBoard()
    {
        leaderBoard.ClearJsonFile();
    }
#endif

    private void ClearCurrentLeaderBorad()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        slotsInstantiated.Clear();
    }
    
    private void ReadLeaderBoard()
    {
        ClearCurrentLeaderBorad();
        
        LeaderBoardData[] dataset = leaderBoard.GetLeaderBoard();
        foreach (LeaderBoardData d in dataset)
        {
            slotsInstantiated.Add(Instantiate(slot, slotContainer));
            string text = $"Player: {d.name}, Score: {d.score.ToString()}, Time: {d.time.ToString()}" ;
            slotsInstantiated.Last().GetComponentInChildren<TMPro.TMP_Text>().text = text;
        }
    }

    private void AddLeaderBoardData(string name, int score, float time, bool completedLevel)
    {
        LeaderBoardData lLeaderBoardData = new LeaderBoardData();
        lLeaderBoardData.name = name;
        lLeaderBoardData.score = score;
        lLeaderBoardData.time = time;
        lLeaderBoardData.completedLevel = completedLevel;
        leaderBoard.SaveData(lLeaderBoardData);
    }

    private LeaderBoardData GetPlayerData(string playerName)
    {
        return leaderBoard.GetLineByName(playerName);
    }
}
