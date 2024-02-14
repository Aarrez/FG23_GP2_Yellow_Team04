using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GlobalStructs;
using Newtonsoft.Json;

namespace PresistentData
{
    [Serializable]
    public class DeprecatedPresistentLeaderBoard
    {
    public string[] StringLines = Array.Empty<string>();
    private string jsonPath;
    private List<byte[]> byteList = new();
    public LevelCompleteStats[] DefaultData;
    private int dscore = 12;
    private float dtime = 3.14f;

    public DeprecatedPresistentLeaderBoard(string jsonPath)
    {
        this.jsonPath = jsonPath;
        if (File.Exists(jsonPath))
            UpdateCurrentData();
        DefaultData = new LevelCompleteStats[1];
        DefaultData[0].Starts = 0;
        DefaultData[0].Time = dtime;
        DefaultData[0].CurrencyEarned = 0;
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

    public void SaveData(LevelCompleteStats leaderBoardData)
    {
        object p = leaderBoardData as object;
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
    public LevelCompleteStats[] GetLeaderBoard()
    {
        if (!File.Exists(jsonPath))
        {
            SaveData(DefaultData[0]);
        }
        StringLines = File.ReadAllLinesAsync(jsonPath).Result;
        for (int i = 0; i < StringLines.Length; i++)
        {
            if (StringLines[i].Length <= 0)
            {
                StringLines = StringLines.Where((_, idx) => idx !=  i).ToArray();
            }
        }
        if (StringLines.Length <= 0)
        {
            SaveData(DefaultData[0]);
        }
        LevelCompleteStats[] convertedData = new LevelCompleteStats[StringLines.Length];
        for (int i = 0; i < StringLines.Length; i++)
        {
            convertedData[i] = JsonConvert.DeserializeObject<LevelCompleteStats>(StringLines[i]);
        }
        
        return convertedData;
    }
    }
}