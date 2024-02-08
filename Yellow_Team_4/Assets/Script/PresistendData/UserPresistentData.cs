using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PresistentData
{
    [Serializable]
    public struct UserData<TClassInventory, TStructSettings, TStructLevel> 
        where TStructSettings : struct
        where TClassInventory : class
        where TStructLevel : struct
    {
        public string userName;
        public TStructLevel[] levels;
        public List<TClassInventory> inventory;
        public TStructSettings playerSettings;
    }

    public class UserPresistentData<TClassInventory, TStructSettings, TStructLevel> 
        where TStructSettings : struct
        where TClassInventory : class
        where TStructLevel : struct
    {
        private string dataPath;
        public UserData<TClassInventory, TStructSettings, TStructLevel> defaultData;
        
        public UserPresistentData(string dataPath, int levelAmount)
        {
            this.dataPath = dataPath;
            if (File.Exists(this.dataPath))
            {
                defaultData.userName = "newUser";
                defaultData.levels = new TStructLevel[levelAmount];
                defaultData.inventory = new List<TClassInventory>();
                GetUserData();
            }
        }
        
        public UserPresistentData(string dataPath, int levelAmount, UserData<TClassInventory, TStructSettings, TStructLevel> defaultData)
        {
            CheckForNullValues(defaultData);
            this.dataPath = dataPath;
            if (File.Exists(this.dataPath))
            {
                this.defaultData = defaultData;
                GetUserData();
            }
        }
        
        private void CheckForNullValues(UserData<TClassInventory, TStructSettings, TStructLevel> data)
        {
            if (data.userName == null)
                throw new NullReferenceException("The name has not been set");
            if (data.inventory == null)
                throw new NullReferenceException("Inventory has no data");
            if (data.levels == null)
                throw new NullReferenceException("There is no Level data");
        }

        private void ReplaceNullValuse(UserData<TClassInventory, TStructSettings, TStructLevel> data)
        {
            if (data.levels == null)
                data.levels = defaultData.levels;

            if (data.inventory == null)
                data.inventory = defaultData.inventory;

            if (data.userName == null)
                data.userName = defaultData.userName;
        }

        public void SaveData(UserData<TClassInventory, TStructSettings, TStructLevel> uData)
        {
            ReplaceNullValuse(uData);
            string json = JsonConvert.SerializeObject(uData as object);
            using (FileStream fs = File.Open(dataPath, FileMode.Create, FileAccess.Write))
            {
                AddText(fs, json);
            }
        }

        private void SaveDefaultData(UserData<TClassInventory, TStructSettings, TStructLevel> uData)
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
        
        public UserData<TClassInventory, TStructSettings, TStructLevel> GetUserData()
        {
            if (!File.Exists(dataPath))
                SaveDefaultData(defaultData);
            
            string line = File.ReadAllTextAsync(dataPath).Result;
            if (line.Length > 1)
            {
                UserData<TClassInventory, TStructSettings, TStructLevel> convertedData = 
                    JsonConvert.DeserializeObject<UserData<TClassInventory, TStructSettings, TStructLevel>>(line);
                return convertedData;
            }
            return defaultData;
        }

        public UserData<TClassInventory, TStructSettings, TStructLevel> ChangeUserData(UserData<TClassInventory, TStructSettings, TStructLevel> uData)
        {
            SaveData(uData);
            return uData;
        }
        public UserData<TClassInventory, TStructSettings, TStructLevel> ChangeUserData(string userName)
        {
            UserData<TClassInventory, TStructSettings, TStructLevel> dataToChange = GetUserData();
            dataToChange.userName = userName;
            SaveData(dataToChange);
            return dataToChange;
        }
        public UserData<TClassInventory, TStructSettings, TStructLevel> ChangeUserData(int level, TStructLevel levelData)
        {
            UserData<TClassInventory, TStructSettings, TStructLevel> dataToChange = GetUserData();
            if (dataToChange.levels.Length < level)
            {
                Array.Resize(ref dataToChange.levels, level);
            }
            dataToChange.levels[level] = levelData;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData<TClassInventory, TStructSettings, TStructLevel> ChangeUserData(TStructSettings settings)
        {
            UserData<TClassInventory, TStructSettings, TStructLevel> dataToChange = GetUserData();
            dataToChange.playerSettings = settings;
            SaveData(dataToChange);
            return dataToChange;
        }
        
        public UserData<TClassInventory, TStructSettings, TStructLevel> ChangeUserData(TClassInventory inventory)
        {
            UserData<TClassInventory, TStructSettings, TStructLevel> dataToChange = GetUserData();
            foreach (var item in dataToChange.inventory)
            {
                if (item == inventory)
                {
                    return dataToChange;
                }
            }
            dataToChange.inventory.Add(inventory);
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
