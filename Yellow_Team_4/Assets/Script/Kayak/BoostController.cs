using UnityEngine;
using TestData;
public class BoostController : MonoBehaviour, IKayakEntity
{
    private float boostTimer;
    private bool boosting;
    public TestData.TestData playerStats;  // Assign this in the Unity Editor
    private Kayak kayak;

    public void Initialize(Kayak kayak)
    {
        this.kayak = kayak as Kayak; 
        CheckPlayerStats();
    }

    public void OnUpdate(float dt)
    {
        UpdateBoostTimer();
    }

    public void OnFixedUpdate(float dt) {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered: " + other.tag); 
        if (other.CompareTag("BoostAreaTest"))
        {
            Debug.Log("Boost area detected!");
            StartSpeedBoost();
            
        }
    }

    public bool IsActive()
    {
        return boosting;
    }

    public TestData.TestData GetPlayerStats()
    {
        return playerStats;
    }

    private void CheckPlayerStats()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is null in BoostController");
        }
    }

    private void UpdateBoostTimer()
    {
        if (boosting)
        {
            boostTimer -= Time.deltaTime;
            Debug.Log(boostTimer);

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
        if (playerStats != null && kayak.rb != null)
        {
            Vector3 boostDirection = transform.forward.normalized;
            Vector3 boostForce = boostDirection * playerStats.boostForce;

            kayak.rb.AddForce(boostForce, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogError("PlayerStats or Rigidbody is null in BoostController");
        }
    }
}
