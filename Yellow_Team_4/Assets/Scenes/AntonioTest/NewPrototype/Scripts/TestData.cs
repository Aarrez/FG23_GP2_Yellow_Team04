using System.Collections;
using System.Collections.Generic;


namespace TestData
{
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "TestStat", menuName = "Data/TestStat", order = 2)]
    public class TestData : ScriptableObject
    {
        [Header("Movement")]
        [Range(0.1f, 100.0f)] public float maxSpeed;
        [Range(0.1f, 100.0f)] public float speedBoost;

        [Header("Boosting")]
        [Range(0.1f, 100.0f)] public float boostForce;
        [Range(0.1f, 100.0f)] public float boostDuration;
        
        [Header("Test")]
        [Range(0.1f, 100.0f)] public float paddleStrength;
        [Range(0.1f, 100.0f)] public float rotationStrength;
        [Range(0.1f, 100.0f)] public float lateralStrength;
    }
}
