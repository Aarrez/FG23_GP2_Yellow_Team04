using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PresistentData
{
    [Serializable]
    public struct UserData
    {
        public string userName;
        public int currency;
        public float[] levelData;
    }

    public class UserPresistentData
    {
        private byte[] jsonBytes = new byte[128];
        private string dataPath;
        public UserData defaultData;
        
        public UserPresistentData(string dataPath)
        {
            this.dataPath = dataPath;
            if (File.Exists(this.dataPath))
            {
                defaultData.userName = "newUser";
                defaultData.currency = 0;
                defaultData.levelData = new[] { 0.0f };
                GetUserData();
            }
        }

        public void SaveData(UserData uData)
        {
            string json = JsonConvert.SerializeObject(uData as object);
            jsonBytes = Encoding.UTF8.GetBytes(json);
            using (FileStream fs = File.Open(dataPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                AddText(fs, json);
            }
        }

        private void SaveDefaultData(FileStream fs ,UserData uData)
        {
            string json = JsonConvert.SerializeObject(uData as object);
            jsonBytes = Encoding.UTF8.GetBytes(json);
            AddText(fs, json);
            
        }
        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
        
        public UserData GetUserData()
        {
            string line;
            using (FileStream fs = File.Open(dataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                UTF8Encoding temp = new UTF8Encoding(true);
                int readLen;
                readLen = fs.Read(jsonBytes, 0, jsonBytes.Length);

                if (readLen <= jsonBytes.Length / 2)
                {
                    SaveDefaultData(fs, defaultData);
                    readLen = fs.Read(jsonBytes, 0, jsonBytes.Length);
                }
                    
                line = temp.GetString(jsonBytes, 0, readLen);
            }

            if (line.Length > 0)
            {
                UserData convertedData = JsonConvert.DeserializeObject<UserData>(line);
                return convertedData;
            }

            return defaultData;
        }

        public UserData ChangeUserData(UserData uData)
        {
            SaveData(uData);
            return uData;
        }
        public UserData ChangeUserData(string userName)
        {
            UserData dataToChange = GetUserData();
            dataToChange.userName = userName;
            SaveData(dataToChange);
            return dataToChange;
        }
        public UserData ChangeUserData(float[] levelData)
        {
            UserData dataToChange = GetUserData();
            dataToChange.levelData = levelData;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData ChangeUserData(int currency)
        {
            UserData dataToChange = GetUserData();
            dataToChange.currency = currency;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public void ClearJsonFile()
        {
            using (FileStream fs = File.Open(dataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                lock (fs)
                {
                    fs.SetLength(0);
                }
            }
        }
    }
}
