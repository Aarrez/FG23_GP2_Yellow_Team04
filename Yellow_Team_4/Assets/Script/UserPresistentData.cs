using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace PresistentData
{
    [Serializable]
    public struct UserData<TStructInventory, TStructSettings> 
        where TStructSettings : struct
        where TStructInventory : struct
    {
        public string userName;
        public int currency;
        public float[] levelData;
        public TStructInventory inventory;
        public TStructSettings playerSettings;
    }

    public class UserPresistentData<TStructInventory, TEnumSettings> 
        where TEnumSettings : struct
        where TStructInventory : struct
    {
        private string dataPath;
        public UserData<TStructInventory, TEnumSettings> defaultData;
        
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

        public void SaveData(UserData<TStructInventory, TEnumSettings> uData)
        {
            string json = JsonConvert.SerializeObject(uData as object);
            using (FileStream fs = File.Open(dataPath, FileMode.Create, FileAccess.Write))
            {
                AddText(fs, json);
            }
        }

        private void SaveDefaultData(UserData<TStructInventory, TEnumSettings> uData)
        {
            string json = JsonConvert.SerializeObject(uData as object);
            using (FileStream fs = File.Open(dataPath, FileMode.Create, FileAccess.Write))
            {
                AddText(fs, json);
            }
            
        }
        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
        
        public UserData<TStructInventory, TEnumSettings> GetUserData()
        {
            string line = File.ReadAllTextAsync(dataPath).Result;
            if (line.Length > 1)
            {
                UserData<TStructInventory, TEnumSettings> convertedData = JsonConvert.DeserializeObject<UserData<TStructInventory, TEnumSettings>>(line);
                return convertedData;
            }
            SaveDefaultData(defaultData);
            return defaultData;
        }

        public UserData<TStructInventory, TEnumSettings> ChangeUserData(UserData<TStructInventory, TEnumSettings> uData)
        {
            SaveData(uData);
            return uData;
        }
        public UserData<TStructInventory, TEnumSettings> ChangeUserData(string userName)
        {
            UserData<TStructInventory, TEnumSettings> dataToChange = GetUserData();
            dataToChange.userName = userName;
            SaveData(dataToChange);
            return dataToChange;
        }
        public UserData<TStructInventory, TEnumSettings> ChangeUserData(float[] levelData)
        {
            UserData<TStructInventory, TEnumSettings> dataToChange = GetUserData();
            dataToChange.levelData = levelData;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData<TStructInventory, TEnumSettings> ChangeUserData(int currency)
        {
            UserData<TStructInventory, TEnumSettings> dataToChange = GetUserData();
            dataToChange.currency = currency;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData<TStructInventory, TEnumSettings> ChangeUserData(TEnumSettings kayak)
        {
            UserData<TStructInventory, TEnumSettings> dataToChange = GetUserData();
            dataToChange.playerSettings = kayak;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData<TStructInventory, TEnumSettings> ChangeUserData(TStructInventory inventory)
        {
            UserData<TStructInventory, TEnumSettings> dataToChange = GetUserData();
            dataToChange.inventory = inventory;
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
