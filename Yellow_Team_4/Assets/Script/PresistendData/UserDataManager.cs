using System;
using UnityEngine;
using PresistentData;
using GlobalStructs;
using UnityEngine.Events;

public class UserDataManager : MonoBehaviour
{
    public static UnityAction<int, LevelCompleteStats> LevelComplete;
    public static Func<SoundVolume> GetSavedVolume;
    public static Action<SoundVolume> SetSavedVolume;
    public static UnityAction<string> ChangeName;
    public static UnityAction InstantiateData;

    [SerializeField] private string defaultNameFallback = "newUser";
    
    public static UserPresistentData<SerializeableItem, SerializeableItem, SoundVolume, LevelCompleteStats> upd;

    private void Awake()
    {
        InstatiateUpd();
    }

    public static void InstatiateUpd()
    {
        if (upd != null) return;
        string path = Application.persistentDataPath + "/UserData.json";
        upd = new UserPresistentData<SerializeableItem, SerializeableItem, SoundVolume, LevelCompleteStats>(path);
    }

    private void OnEnable()
    {
        LeaderBoardManager.GetName += () =>
        {
            if (upd == null)
            {
                return defaultNameFallback;
            }
            return upd.defaultData.userName;
        };
        LevelComplete += CompletedLevel;
        GetSavedVolume += GetSavedVolumeMethod;
        SetSavedVolume += ChangeSavedVolumeMethod;
        ChangeName += ChangeTheUserName;
    }
    
    private void OnDisable()
    {
        LeaderBoardManager.GetName -= () => upd.GetUserData().userName;
        LevelComplete -= CompletedLevel;
        GetSavedVolume -= GetSavedVolumeMethod;
        SetSavedVolume -= ChangeSavedVolumeMethod;
        ChangeName -= ChangeTheUserName;
        
    }


/*#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            upd.ClearJsonFile();
            upd.SaveData(upd.defaultData);
            if (!inslot) return;
            string text = $"Username: {upd.defaultData.userName} \n" +
                          $"Currency: {upd.defaultData.currency.ToString()}";
            tmpText.text = text;

        }
    }
#endif*/
    private void CompletedLevel(int level, LevelCompleteStats lcs)
    {
        upd.ChangeUserData(level, lcs);
    }

    private void ChangeSavedVolumeMethod(SoundVolume sv)
    {
        var userdata = upd.GetUserData();
        userdata.volumeSettings = sv;
        upd.ChangeUserData(userdata);
    }

    private SoundVolume GetSavedVolumeMethod()
    {
        var userdata = upd.GetUserData();
        return userdata.volumeSettings;
    }

    public void ChangeTheUserName(string userName)
    {
        upd.ChangeUserData(userName);
    }
}


