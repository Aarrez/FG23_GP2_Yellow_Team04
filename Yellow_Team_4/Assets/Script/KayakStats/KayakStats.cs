using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KayakConfiguration", menuName = "Kayak Configuration", order = 0)]
public class KayakConfiguration : ScriptableObject
{
    [Header("Speed")]
    public float maxHorizontalVelocity = 5;
    public float maxVerticalVelocity = 5;
    public float maxLateralTorque = 5;
}
