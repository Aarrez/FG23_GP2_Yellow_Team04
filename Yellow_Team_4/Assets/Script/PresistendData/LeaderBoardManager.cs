using System;
using System.Collections.Generic;
using System.Linq;
using GlobalStructs;
using PresistentData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    public static UnityAction<LevelCompleteStats> SaveLeaderboardData;
    public static Func<string> GetName; 
    
    [SerializeField] private GameObject slot;
    private List<GameObject> slotsInstantiated = new();
    private LevelCompleteStats leaderBoardData;
    private Button[] buttons;
    private Button send, read, clear;
    private PresistentLeaderBoard leaderBoard;
    private TMPro.TMP_InputField inputName;
    private Toggle completedLevel;

    private RectTransform contentRect;
    private float slotHeight;
    
    private void Awake()
    {
        string jsonPath = Application.persistentDataPath + "/ScoreBoard.json";
        leaderBoard = new PresistentLeaderBoard(jsonPath);
        inputName = GetComponentInChildren<TMPro.TMP_InputField>();
        contentRect = transform.GetComponent<RectTransform>();
        //WTF TODO Find out why it gives 350 instead of 50;
        slotHeight = GetComponent<RectTransform>().rect.size.y - 300.0f;
        slotHeight /= 2;
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
        float totalHeight = 0;
        LevelCompleteStats[] dataset = leaderBoard.GetLeaderBoard();
        foreach (var d in dataset)
        {
            totalHeight += slotHeight;
            slotsInstantiated.Add(Instantiate(slot, transform));
            string text = $"Player: {GetName.Invoke()}, Stars: {((int)d.Starts).ToString()}, Time: {d.Time.ToString()}" ;
            slotsInstantiated.Last().GetComponentInChildren<TMPro.TMP_Text>().text = text;
        }
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
    }

    private void AddLeaderBoardData(LevelCompleteStats levelStats)
    {
        leaderBoard.SaveData(levelStats);
    }
}
