using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace PresistentData
{
    [Serializable]
    public struct UserData<TClassKayackInventory, TClassPlayerInventory, TClassVolumeSettings, TStructLevel> 
        where TClassVolumeSettings : class, new()
        where TClassPlayerInventory : class
        where TClassKayackInventory : class
        where TStructLevel : struct
    {
        public string userName;
        public Dictionary<int, List<TStructLevel>> LevelData;
        public List<TClassPlayerInventory> playerInventory;
        public List<TClassKayackInventory> kayakInventory;
        public TClassVolumeSettings volumeSettings;
        public int CurrentCurrency;
    }

    public class UserPresistentData<TClassKayakInventory, TClassPlayerInventory, TClassVolumeSettings, TStructLevel> 
        where TClassVolumeSettings : class, new()
        where TClassKayakInventory : class
        where TClassPlayerInventory : class
        where TStructLevel : struct
    {
        private string dataPath;
        public UserData<TClassKayakInventory, TClassPlayerInventory, TClassVolumeSettings, TStructLevel> defaultData;
        
        public UserPresistentData(string dataPath)
        {
            this.dataPath = dataPath;
            defaultData.userName = "newUser";
            defaultData.LevelData = new ();
            defaultData.volumeSettings = new TClassVolumeSettings();
            defaultData.kayakInventory = new List<TClassKayakInventory>();
            defaultData.playerInventory = new List<TClassPlayerInventory>();
            Dictionary<int, List<TStructLevel>> levelData = new Dictionary<int, List<TStructLevel>>();
            List<TStructLevel> a = new List<TStructLevel>();
            a.Add(new TStructLevel());
            levelData.Add(1, a);
            defaultData.LevelData = levelData;
            GetUserData();
        }
        
        public UserPresistentData(string dataPath, UserData<TClassKayakInventory, TClassPlayerInventory, TClassVolumeSettings, TStructLevel> defaultData)
        {
            CheckForNullValues(defaultData);
            this.dataPath = dataPath;
            if (File.Exists(this.dataPath))
            {
                this.defaultData = defaultData;
                GetUserData();
            }
        }
        
        private void CheckForNullValues(UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> data)
        {
            if (data.userName == null)
                throw new NullReferenceException("The name has not been set");
            if (data.kayakInventory == null)
                throw new NullReferenceException("Inventory has no data");
        }

        private void ReplaceNullValuse(UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> data)
        {
            if (data.kayakInventory == null)
                data.kayakInventory = defaultData.kayakInventory;

            if (data.userName == null)
                data.userName = defaultData.userName;
        }

        public void SaveData(UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> uData)
        {
            ReplaceNullValuse(uData);
            string json = JsonConvert.SerializeObject(uData as object);
            using (FileStream fs = File.Open(dataPath, FileMode.Create, FileAccess.Write))
            {
                AddText(fs, json);
            }
        }

        public void SaveDefaultData()
        {
            string json = JsonConvert.SerializeObject(defaultData as object);
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
        
        public UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> GetUserData()
        {
            if (!File.Exists(dataPath))
                SaveDefaultData();
            
            string line = File.ReadAllTextAsync(dataPath).Result;
            if (line.Length > 1)
            {
                UserData<TClassKayakInventory,TClassPlayerInventory,TClassVolumeSettings, TStructLevel> convertedData = 
                    JsonConvert.DeserializeObject<UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel>>(line);
                return convertedData;
            }
            SaveDefaultData();
            return defaultData;
        }

        public UserData<TClassKayakInventory,TClassPlayerInventory,TClassVolumeSettings, TStructLevel> 
            ChangeUserData(UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> uData)
        {
            SaveData(uData);
            return uData;
        }
        public UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> ChangeUserData(string userName)
        {
            UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> dataToChange = GetUserData();
            dataToChange.userName = userName;
            SaveData(dataToChange);
            return dataToChange;
        }
        public UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> ChangeUserData(int level, TStructLevel levelData)
        {
            UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> dataToChange = GetUserData();
            if (dataToChange.LevelData.ContainsKey(level))
            {
                dataToChange.LevelData[level].Add(levelData);
            }
            else
            {
                var a = new List<TStructLevel>();
                a.Add(levelData);
                dataToChange.LevelData.Add(level, a);
            }
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData<TClassKayakInventory,TClassPlayerInventory,TClassVolumeSettings, TStructLevel> ChangeUserData(TClassVolumeSettings settings)
        {
            UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> dataToChange = GetUserData();
            dataToChange.volumeSettings = settings;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> 
            ChangeUserData(TClassKayakInventory kayakInventory)
        {
            UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> dataToChange = GetUserData();
            dataToChange.kayakInventory.Add(kayakInventory);
            SaveData(dataToChange);
            return dataToChange;
        }
        public UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> 
            ChangeUserData(List<TClassKayakInventory> kayakInventory)
        {
            UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> dataToChange = GetUserData();
            dataToChange.kayakInventory = kayakInventory;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> 
            ChangeUserData(TClassPlayerInventory playerInventory)
        {
            UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> dataToChange = GetUserData();
            
            dataToChange.playerInventory.Add(playerInventory);
            SaveData(dataToChange);
            return dataToChange;
        }
        public UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> 
            ChangeUserData(List<TClassPlayerInventory> playerInventory)
        {
            UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> dataToChange = GetUserData();

            dataToChange.playerInventory = playerInventory;
            SaveData(dataToChange);
            return dataToChange;
        }

        public UserData<TClassKayakInventory, TClassPlayerInventory, TClassVolumeSettings, TStructLevel> ChangeUserData
            (int currentCurrency)
        {
            UserData<TClassKayakInventory,TClassPlayerInventory, TClassVolumeSettings, TStructLevel> dataToChange = GetUserData();
            dataToChange.CurrentCurrency = currentCurrency;
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
