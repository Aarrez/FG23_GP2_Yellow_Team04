using UnityEngine;
using PresistentData;

public class UserDataManager : MonoBehaviour
{
    private UserPresistentData UPD;
    private PresistentData.UserData userData;
    [SerializeField] private string userName = "Aaron";
    [SerializeField] private float[] levelData;
    [SerializeField] private int currency = 50;
    

    private void Awake()
    {
        userData.userName = userName;
        userData.levelData = levelData;
        userData.currency = currency;
        string path = Application.persistentDataPath + "/UserData.json";
        UPD = new UserPresistentData(path);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            UPD.SaveData(userData);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PresistentData.UserData data;
            data = UPD.GetUserData();
            Debug.Log($"Name: {data.userName} \n" +
                      $"Level/time: {data.levelData.Length.ToString()}/{data.levelData[0].ToString()} \n" +
                      $"Currency: {data.currency.ToString()}");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            
        }
    }
}
