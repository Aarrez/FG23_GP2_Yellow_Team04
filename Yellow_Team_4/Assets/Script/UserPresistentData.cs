using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor.AddressableAssets.HostingServices;
using Object = UnityEngine.Object;

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
        private byte[] jsonBytes;
        private string dataPath;
        
        public UserPresistentData(string dataPath)
        {
            this.dataPath = dataPath;
            if (File.Exists(this.dataPath))
                GetUserData();
        }

        public void SaveData(UserData uData)
        {
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }
            string json = JsonConvert.SerializeObject(uData as object);
            jsonBytes = Encoding.UTF8.GetBytes(json);
            using (FileStream fs = File.Open(dataPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                AddText(fs, json);
            }
        }
        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
        
        public UserData GetUserData()
        {
            if (!File.Exists(dataPath))
                throw new NullReferenceException("There is no file to get data from");
            
            string line = "";
            using (FileStream fs = File.Open(dataPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                UTF8Encoding temp = new UTF8Encoding(true);
                int readLen;
                while ((readLen = fs.Read(jsonBytes, 0, jsonBytes.Length)) > 0)
                {
                    line = temp.GetString(jsonBytes, 0, readLen);
                }
            }

            if (line.Length > 0)
            {
                UserData convertedData = JsonConvert.DeserializeObject<UserData>(line);
                return convertedData;
            }

            throw new Exception("File does not contain any data");
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
