using UnityEngine;
using TestData;
using UnityEngine.Serialization;

public class BoostController : MonoBehaviour, IKayakEntity
{
    private float boostTimer;
    private bool boosting;
    public TestData.TestData playerStats;  // Assign this in the Unity Editor
    private Kayak kayak;
    private Vector3 boostDirection;
    private Vector3 cheatDirection;
    [SerializeField] private float cheatRadius = 5;
    [Range(0, 1)][SerializeField] private float cheatNudgeInfluence = 1;

    private Transform closestCheatObject;
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
        if (boosting) {
            ApplyBoostForce(dt);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BoostAreaTest"))
        {            
            StartSpeedBoost();
            boostDirection = other.transform.forward;
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
            
            var hits = Physics.OverlapSphere(transform.position, cheatRadius);
            float closestItem = float.MaxValue;
            closestCheatObject = null;
            cheatDirection = Vector3.zero;
            foreach (var hit in hits)
            {
                if (hit.CompareTag("COIN"))
                {
                    var dist = (hit.transform.position - transform.position).magnitude;
                    if (dist < closestItem)
                    {
                        closestCheatObject = hit.gameObject.transform;
                        closestItem = dist;
                    }
                }
            }
            
            if (closestCheatObject != null && closestCheatObject.gameObject.activeSelf)
            {
                cheatDirection = (closestCheatObject.transform.position - transform.position).normalized;
                if (Vector3.Dot(cheatDirection, boostDirection) < 0)
                {
                    cheatDirection = Vector3.zero;
                }
            }

            if (boostTimer <= 0f)
            {
                closestCheatObject = null;
                cheatDirection = Vector3.zero;
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
            ApplyBoostForce(Time.fixedDeltaTime);
        }
        else
        {
            Debug.LogError("PlayerStats is null in BoostController");
        }
    }

    private void ApplyBoostForce(float dt)
    {
        if (playerStats != null && kayak != null)
        {
            Vector3 boostForce = boostDirection * playerStats.boostForce;
            var finalDirection = (boostDirection + (cheatDirection * cheatNudgeInfluence)).normalized;
            kayak.AddForce(finalDirection, boostForce.magnitude, dt, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogError("PlayerStats or Rigidbody is null in BoostController");
        }
    }
    
    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + cheatDirection.normalized * 3);
        
        Gizmos.DrawWireSphere(transform.position, cheatRadius);
    }
    #endif
}
