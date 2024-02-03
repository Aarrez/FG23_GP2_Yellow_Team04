using System.Collections.Generic;
using System.Linq;
using PresistentData;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    
    [SerializeField] private GameObject slot;
    private List<GameObject> slotsInstantiated = new();
    private Transform slotContainer;
    private Data data;
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
        
        Data[] dataset = leaderBoard.GetLeaderBorad();
        foreach (Data d in dataset)
        {
            slotsInstantiated.Add(Instantiate(slot, slotContainer));
            string text = $"Player: {d.name}, Score: {d.score.ToString()}, Time: {d.time.ToString()}" ;
            slotsInstantiated.Last().GetComponentInChildren<TMPro.TMP_Text>().text = text;
        }
    }
    private void AddPlayerToLeaderBoard()
    {
        if (data.name == null)
        {
            Debug.LogError("Name Needed");
            return;
        }
        leaderBoard.SaveData(data);
        inputName.text = "";
    }

    private Data GetPlayerData(string playerName)
    {
        return leaderBoard.GetLineByName(playerName);
    }

    private void AddPlayerName(string name)
    {
        data.name = name;
    }

   
}
