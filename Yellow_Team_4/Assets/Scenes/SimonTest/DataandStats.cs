using System.Collections;
using System.Collections.Generic;
namespace DataandStats
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Data/PlayerStats", order = 1)]
    public class PlayerStats : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] public Vector3 gravity = new Vector3(0, -5, 0);
        [SerializeField] public Vector3 initialConstantVelocity = new Vector3(0, 0, 0);
        [Range(0, 2f)][SerializeField] public float rateOfDirectionChange = 0.1f;            
        [Range(0, 1f)][SerializeField] public float rateOfRotationChange = 0.5f;
        [SerializeField] public float paddleStrength = 5;
        [SerializeField] public float maxSpeedHorizontal = 999;
        [SerializeField] public float maxSpeedVertical = 10;
        [Range(-1, 1)] [SerializeField] public float travelBounds = -1;

        [Header("Drag")]
        [SerializeField] public float minDragCoefficient= 0;
        [SerializeField] public float maxDragCoefficient = 2;

        [Header("Layer Masks")]        
        [SerializeField] public LayerMask hookMasks;

        [Header("Hook")]
        [Range(0, 1)][SerializeField] public float hookDirection;
        [SerializeField] public float hookStrength = 5;
        [SerializeField] public float hookPaddleStrength = 5;
        [SerializeField] public float hookRampUpMaxTime = 1;
        [SerializeField] public float unHookRadius = 1.3f; 
        [SerializeField] public AnimationCurve hookPowerRampUp; 

        [Header("Boosting")]
        [Range(0.1f, 100.0f)] public float boostForce;
        [Range(0.1f, 100.0f)] public float boostDuration;
        public AnimationCurve boostDecay;
    }
}
