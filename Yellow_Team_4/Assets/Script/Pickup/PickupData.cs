using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickupData", menuName = "Data/PickupData", order = 2)]

public class PickupData : ScriptableObject
{
    [SerializeField] public GameObject Coin;
}
