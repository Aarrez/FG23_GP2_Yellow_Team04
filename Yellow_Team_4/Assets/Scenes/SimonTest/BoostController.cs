using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataandStats;
public class BoostController : MonoBehaviour
{
    private float boostTimer;
    private bool boosting;
    private Boat boat;
    public PlayerStats playerStats;  // Assign this in the Unity Editor

    void Start()
    {
        boat = GetComponent<Boat>();
        CheckPlayerStats();        
    }

    void Update()
    {
        UpdateBoostTimer();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered: " + other.tag); 
        if (other.CompareTag("boostarea"))
        {
            Debug.Log("Boost area detected!");
            StartSpeedBoost();
            
        }
    }

    public bool IsActive()
    {
        return boosting;
    }

    public PlayerStats GetPlayerStats()
    {
        return playerStats;
    }

    private void CheckPlayerStats()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is null in BoostController");
        }
        boostTimer = playerStats.boostDuration;
    }

    private void UpdateBoostTimer()
    {
        if (boosting)
        {
            boostTimer -= Time.deltaTime;
            Debug.Log(boostTimer);
            Vector3 boostDirection = transform.forward.normalized;
            Vector3 boostForce = boostDirection * playerStats.boostForce;
            boat.ExternalVelocity = Vector3.Lerp(playerStats.initialConstantVelocity, boostForce, (1 - playerStats.boostDecay.Evaluate(Mathf.InverseLerp(0, playerStats.boostDuration,boostTimer))));

            if (boostTimer <= 0f)
            {
                boosting = false;
            }
        }
    }

    private void StartSpeedBoost()
    {
        if (playerStats != null)
        {
            boosting = true;
            boostTimer = playerStats.boostDuration;
            ApplyBoostForce();
        }
        else
        {
            Debug.LogError("PlayerStats is null in BoostController");
        }
    }

    private void ApplyBoostForce()
    {
        if (playerStats != null && boat != null)
        {
            Debug.Log("apply boost force");
            Vector3 boostDirection = transform.forward.normalized;
            Vector3 boostForce = boostDirection * playerStats.boostForce;

            // boat.AddForce(boostForce, ForceMode.VelocityChange);
            boat.ExternalVelocity = boostForce;
        }
        else
        {
            Debug.LogError("PlayerStats or Rigidbody is null in BoostController");
        }
    }
}
