using UnityEngine;
using PresistentData;

public class UserDataManager : MonoBehaviour
{
    private UserPresistentData UPD;
    private UserData userData;
    [SerializeField] private string userName = "Aaron";
    [SerializeField] private float[] levelData;
    [SerializeField] private int currency = 50;
    [SerializeField] private GameObject slot;
    private GameObject inslot;
    private TMPro.TMP_Text tmpText;
    

    private void Awake()
    {
        userData.userName = userName;
        userData.levelData = levelData;
        userData.currency = currency;
        string path = Application.persistentDataPath + "/UserData.json";
        UPD = new UserPresistentData(path);
        
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            UPD.SaveData(userData);
            inslot = Instantiate(slot, transform);
            tmpText = inslot.GetComponent<TMPro.TMP_Text>();
            string text = $"Username: {userData.userName} \n" +
                          $"Currency: {userData.currency.ToString()}";
            tmpText.text = text;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UPD.ClearJsonFile();
            UPD.SaveData(UPD.defaultData);
            if (!inslot) return;
            string text = $"Username: {UPD.defaultData.userName} \n" +
                          $"Currency: {UPD.defaultData.currency.ToString()}";
            tmpText.text = text;

        }
    }
#endif
}
