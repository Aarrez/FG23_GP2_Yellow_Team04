using System;
using System.Collections.Generic;
using GlobalStructs;
using PresistentData;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LeaderBoardManager : MonoBehaviour
{
    [SerializeField] private Sprite[] trophySprites;
    public static Func<string> GetName;
    public static UnityAction<LevelCompleteStats> LeaderboardLoaded;    
    private int activeBuildIndex = 1;
    
    private void OnEnable()
    {
        ReadLeaderBoard(activeBuildIndex);
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddDefaultData();
            ReadLeaderBoard(activeBuildIndex);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            RemoveDefaults();
        }
    }
#endif
    
    public void ReadLeaderBoard(int level)
    {
        if (UserDataManager.upd != null) {
            var levelStats = UserDataManager.upd.GetUserData().LevelData[level];
            var orderedLevelStats = levelStats.OrderByDescending(x=> x.Starts).OrderBy( x=> x.Time ).ToList();
            var idx = 0;
            for(int i=0; i<transform.childCount; i++) {
                if (i < orderedLevelStats.Count()) {
                    var ol = orderedLevelStats[i];
                    var orls = transform.GetChild(i);
                    orls.gameObject.SetActive(true);                    
                    var im = orls.transform.Find("Image").GetComponent<Image>();
                    if (idx < 3) {
                        im.enabled = true;
                        im.sprite = trophySprites[idx];
                        im.preserveAspect = true;
                    } else {
                        im.enabled = false;
                    }
                    orls.transform.Find("Image/Rank").GetComponent<TextMeshProUGUI>().text = (idx + 1).ToString();                    
                    orls.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = UserDataManager.upd.GetUserData().userName + (idx + 1).ToString();
                    var hst = TimeSpan.FromSeconds(ol.Time);
                    orls.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = string.Format("{0:D2}:{1:D2}:{2:D2}", 
                    hst.Hours, 
                    hst.Minutes, 
                    hst.Seconds);
                    idx++;                                                                                                  
                }
                else {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }               
    }

    private void AddDefaultData()
    {
        UserDataManager.upd.ChangeUserData(1, UserDataManager.upd.defaultData.LevelData[1][0]);
    }

    private void RemoveDefaults()
    {
        var d = UserDataManager.upd.GetUserData();
        
        foreach (var levelScore in d.LevelData)
        {
            if (levelScore.Value.Count <= 1) continue;
            for (int i = 0; i < levelScore.Value.Count; i++)
            {
                if (levelScore.Value[i].Time <= 1)
                {
                    levelScore.Value.Remove(levelScore.Value[i]);
                }
            }
        }
        UserDataManager.upd.SaveData(d);
                    
    }

    private void AddLeaderBoardData(LevelCompleteStats levelStats)
    {
        UserDataManager.upd.ChangeUserData(activeBuildIndex, levelStats);
    }
}
