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
    public static UnityAction<InventoryItem> AddInvetoryItem;
    public static UnityAction<string> ChangeName;

    [SerializeField] private int levelAmount = 5;
    
    private UserPresistentData<InventoryItem, PlayerSettings, LevelCompleteStats> upd;
    
    private void Awake()
    {
        string path = Application.persistentDataPath + "/UserData.json";
        upd = new UserPresistentData<InventoryItem, PlayerSettings, LevelCompleteStats>(path, levelAmount);
    }

    private void OnEnable()
    {
        LevelComplete += CompletedLevel;
        GetSavedVolume += GetSavedVolumeMethod;
        SetSavedVolume += ChangeSavedVolumeMethod;
        ChangeName += ChangeTheUserName;
        AddInvetoryItem += item => upd.ChangeUserData(item);
        LeaderBoardManager.GetName += () => upd.GetUserData().userName;
    }
    
    private void OnDisable()
    {
        LevelComplete -= CompletedLevel;
        GetSavedVolume -= GetSavedVolumeMethod;
        SetSavedVolume -= ChangeSavedVolumeMethod;
        ChangeName -= ChangeTheUserName;
        AddInvetoryItem -= item => upd.ChangeUserData(item);
        LeaderBoardManager.GetName -= () => upd.GetUserData().userName;
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
        userdata.playerSettings.soundVolume = sv;
        upd.ChangeUserData(userdata);
    }

    private SoundVolume GetSavedVolumeMethod()
    {
        var userdata = upd.GetUserData();
        return userdata.playerSettings.soundVolume;
    }

    public void ChangeTheUserName(string userName)
    {
        upd.ChangeUserData(userName);
    }
}


