using System;
using UnityEngine;
using PresistentData;
using GlobalStructs;
using UnityEngine.Events;

public class UserDataManager : MonoBehaviour
{
    public static UnityAction<LevelCompleteStats> LevelComplete;
    public static Func<SoundVolume> GetSavedVolume;
    public static Action<SoundVolume> SetSavedVolume;
    
    private UserPresistentData<Inventory, PlayerSettings> upd;
    private GameObject inslot;
    private TMPro.TMP_Text tmpText;
    

    private void Awake()
    {
        string path = Application.persistentDataPath + "/UserData.json";
        upd = new UserPresistentData<Inventory, PlayerSettings>(path);
    }

    private void OnEnable()
    {
        LevelComplete += CompletedLevel;
        GetSavedVolume += GetSavedVolumeMethod;
        SetSavedVolume += ChangeSavedVolumeMethod;
        LeaderBoardManager.GetName += () => upd.GetUserData().userName;
    }
    
    private void OnDisable()
    {
        LevelComplete -= CompletedLevel;
        GetSavedVolume -= GetSavedVolumeMethod;
        SetSavedVolume -= ChangeSavedVolumeMethod;
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
    private void CompletedLevel(LevelCompleteStats lcs)
    {
        var data = upd.GetUserData();
        data.currency = lcs.CurrencyEarned;
        if (data.levelData.Length <= lcs.Level)
        {
            data.levelData = new float[lcs.Level];
            data.levelData[lcs.Level - 1] = lcs.Time;
        }
        else
            data.levelData[lcs.Level - 1] = lcs.Time;

        upd.ChangeUserData(data);
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
}


