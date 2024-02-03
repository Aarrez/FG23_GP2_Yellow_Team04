using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

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
    public string[] StringLines = Array.Empty<string>();
    private string jsonPath;
    private List<byte[]> byteList = new();
    public Data[] DefaultData;
    private string dname = "Default";
    private int dscore = 12;
    private float dtime = 3.14f;
    private bool dcompletedLevel = false;

    public PresistentLeaderBoard(string jsonPath)
    {
        this.jsonPath = jsonPath;
        if (File.Exists(jsonPath))
            UpdateCurrentData();
        DefaultData = new Data[1];
        DefaultData[0].name = dname;
        DefaultData[0].score = dscore;
        DefaultData[0].time = dtime;
        DefaultData[0].completedLevel = dcompletedLevel;
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
        UpdateCurrentData();
    }

    public void SaveData(Data data)
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
            foreach (string line in StringLines)
            {
                AddText(fs, line);
                byteList.Add(Encoding.UTF8.GetBytes(line));
            }
        }
        StringLines = File.ReadAllLinesAsync(jsonPath).Result;
    }
    private void UpdateCurrentData()
    {
        byteList.Clear();
        using (FileStream fs = File.Open(jsonPath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            foreach (string line in StringLines)
            {
                AddText(fs, line);
                byteList.Add(Encoding.UTF8.GetBytes(line));
            }
        }
        StringLines = File.ReadAllLinesAsync(jsonPath).Result;
    }
    private static void AddText(FileStream fs, string value)
    {
        value += "\n";
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }
    public Data[] GetLeaderBorad()
    {
        StringLines = File.ReadAllLinesAsync(jsonPath).Result;
        for (int i = 0; i < StringLines.Length; i++)
        {
            if (StringLines[i].Length <= 0)
            {
                StringLines = StringLines.Where((val, idx) => idx !=  i).ToArray();
            }
        }
        if (StringLines.Length <= 0)
        {
            SaveData(DefaultData[0]);
        }
        Data[] convertedData = new Data[StringLines.Length];
        for (int i = 0; i < StringLines.Length; i++)
        {
            convertedData[i] = JsonConvert.DeserializeObject<Data>(StringLines[i]);
        }
        
        return convertedData;
    }

    public Data GetLineByName(string name)
    {
        int i;
        for (i = 0; i < StringLines.Length; i++)
        {
            if (StringLines[i].Contains(name))
            {
                break;
            }
        }
        Data temp = JsonConvert.DeserializeObject<Data>(StringLines[i]);
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
        for (i = 0; i < StringLines.Length; i++)
        {
            if (StringLines[i].Contains(playerName))
            {
                break;
            }
        }
        string temp = StringLines[i];
        StringLines = StringLines.Where((val, idx) => idx !=  i).ToArray();
        UpdateCurrentData();
        return temp;
    }
    }
}