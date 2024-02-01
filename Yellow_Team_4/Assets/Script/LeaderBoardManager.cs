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
        buttons = GetComponentsInChildren<Button>();
        slotContainer = GetComponentInChildren<VerticalLayoutGroup>().transform;
        foreach (Button button in buttons)
        {
            switch (button.gameObject.name)
            {
                case "SendData":
                    send = button;
                    break;
                case "ReadData":
                    read = button;
                    break;
                case "ClearData":
                    clear = button;
                    break;
            }
        }
        inputName = GetComponentInChildren<TMPro.TMP_InputField>();
    }

    private void OnEnable()
    {
        send.onClick.AddListener(AddPlayerToLeaderBoard);
        read.onClick.AddListener(ReadLeaderBoard);
        clear.onClick.AddListener(ClearLeaderBoard);
        inputName.onDeselect.AddListener(AddPlayerName);
        inputName.onSubmit.AddListener(AddPlayerName);
    }

    private void ReadLeaderBoard()
    {
        if(slotsInstantiated.Count <= 0)
            foreach (GameObject s in slotsInstantiated)
                Destroy(s);
        
        Data[] dataset = leaderBoard.GetLeaderBorad(ref leaderBoard.stringLines);
        foreach (Data d in dataset)
        {
            if (slotContainer.childCount > 5) return;
            slotsInstantiated.Add(Instantiate(slot, slotContainer));
            string text = $"Player: {d.name}, Score: {d.score.ToString()}, Time: {d.time.ToString()}" ;
            slotsInstantiated.Last().GetComponentInChildren<TMPro.TMP_Text>().text = text;
        }
    }

    private void OnDisable()
    {
        send.onClick.RemoveAllListeners();
        read.onClick.RemoveAllListeners();
        clear.onClick.RemoveAllListeners();
        inputName.onSubmit.RemoveAllListeners();
        inputName.onDeselect.RemoveAllListeners();
    }
    private void AddPlayerToLeaderBoard()
    {
        if (data.name == null)
        {
            Debug.LogError("Name Needed");
            return;
        }
        leaderBoard.SavePartialData(data);
        inputName.text = "";
    }

    private Data GetPlayerData(string playerName)
    {
        return leaderBoard.GetLineByName(playerName, ref leaderBoard.stringLines);
    }

    private void AddPlayerName(string name)
    {
        data.name = name;
    }

    private void ClearLeaderBoard()
    {
        leaderBoard.ClearJsonFile();
        foreach (GameObject s in slotsInstantiated)
        {
            Destroy(s);
        }
    }
}
