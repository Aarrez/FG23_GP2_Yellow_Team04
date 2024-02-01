using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace PresistentData
{
    
    [Serializable]
public struct Data
{ 
    public string name;
    public int score;
    public float time;
    public bool completedLevel;
}

public class PresistentLeaderBoard
{
    public string[] stringLines;
    private string jsonPath;
    private List<byte[]> byteList = new();


    public PresistentLeaderBoard(string jsonPath)
    {
        this.jsonPath = jsonPath;
        if (File.Exists(jsonPath))
            UpdateJsonDataList(ref stringLines);
    }
    public void ClearJsonFile()
    {
        byteList.Clear();
        using (FileStream fs = File.Open(jsonPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            lock (fs)
            {
                fs.SetLength(0);
            }
        }
        UpdateJsonDataList(ref stringLines);
        UpdateCurrentData();
    }

    public void SaveCompleteData(string playerName, int score, float time, bool completedLevel)
    {
        Data playerData;
        playerData.name = playerName;
        playerData.score = score;
        playerData.time = time;
        playerData.completedLevel = completedLevel;
        object p = playerData as object;
        string json = JsonConvert.SerializeObject(p);
        byteList.Clear();
        UpdateJsonDataList(ref stringLines);
        UpdateCurrentData(json);
    }

    public void SavePartialData(Data data)
    {
        object p = data as object;
        string json = JsonConvert.SerializeObject(p);
        byteList.Clear();
        UpdateCurrentData(json);
    }

    private void UpdateCurrentData(string json)
    {
        using (FileStream fs = File.Open(jsonPath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            AddText(fs, json);
            byteList.Add(Encoding.UTF8.GetBytes(json));
            foreach (string line in stringLines)
            {
                AddText(fs, line);
                byteList.Add(Encoding.UTF8.GetBytes(line));
            }
        }
        UpdateJsonDataList(ref stringLines);
    }
    private void UpdateCurrentData()
    {
        byteList.Clear();
        using (FileStream fs = File.Open(jsonPath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            foreach (string line in stringLines)
            {
                AddText(fs, line);
                byteList.Add(Encoding.UTF8.GetBytes(line));
            }
        }
        UpdateJsonDataList(ref stringLines);
    }
    private static void AddText(FileStream fs, string value)
    {
        value += "\n";
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }
    

    private void UpdateJsonDataList(ref string[] stringLines)
    {
        stringLines = File.ReadAllLinesAsync(jsonPath).Result;
    }
    
    public Data[] GetLeaderBorad(ref string[] stringLines)
    {
        UpdateJsonDataList(ref stringLines);
        Data[] convertedData = new Data[stringLines.Length];

        for (int i = 0; i < stringLines.Length; i++)
        {
            convertedData[i] = JsonConvert.DeserializeObject<Data>(stringLines[i]);
        }
        
        return convertedData;
    }

    public Data GetLineByName(string name, ref string[] stringLines)
    {
        int i;
        for (i = 0; i < stringLines.Length; i++)
        {
            if (stringLines[i].Contains(name))
            {
                break;
            }
        }
        Data temp = JsonConvert.DeserializeObject<Data>(stringLines[i]);
        return temp;
    }

    /// <summary>
    /// Removes the first instance of the playerName given from the json file
    /// </summary>
    /// <param name="playerName">Player to remove</param>
    /// <returns></returns>
    public string RemoveLineByName(string playerName)
    {
        int i;
        for (i = 0; i < stringLines.Length; i++)
        {
            if (stringLines[i].Contains(playerName))
            {
                break;
            }
        }
        string temp = stringLines[i];
        stringLines = stringLines.Where((val, idx) => idx !=  i).ToArray();
        UpdateCurrentData();
        return temp;
    }
}
}