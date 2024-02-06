namespace SharedLibrary;

public class UserData<TStruct, TEnum>
where TStruct : struct
where TEnum : Enum
{
    public string UserName { get; set; }
    public int Currency { get; set; }
    public float[] LevelData { get; set; }
    public TStruct Inventory { get; set;  }
    public TEnum Kayak { get; set; }
}