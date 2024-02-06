using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour, IKayakEntity
{
    [Header("Paddle")]
    [SerializeField] private float paddleStrength = 10f;
    [SerializeField] private float rotationStrength = 5f;
    [SerializeField] private float lateralStrength = 2f; 
    [SerializeField] private float paddleForceApplicationTimer = 0.1f;   
        
    public bool leftPaddleActive = false;
    public bool rightPaddleActive = false;

    private Kayak kayak;
    private float currentRotationStrength;    

    private float paddleTimerInSeconds = -1;

    void OnCollisionExit(Collision collisionInfo) {
        if (collisionInfo.gameObject.tag == "Wall") {
            currentRotationStrength = rotationStrength;
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Wall") {
            currentRotationStrength = rotationStrength * 2;
        }        
    }

    public void Initialize(Kayak kayak)
    {
        if (kayak != null) {
            this.kayak = kayak as Kayak;
            currentRotationStrength = rotationStrength;
        }    
    }

    public void OnUpdate(float dt)
    {
        // Check for key presses
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.J))
        {
            leftPaddleActive = true;
            paddleTimerInSeconds = 0;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.L))
        {
            rightPaddleActive = true;
            paddleTimerInSeconds = 0;
        }

        #if USE_TAP_AND_HOLD
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.J)) {
            leftPaddleActive = false;
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.L))
        {
            rightPaddleActive = false;
        }
        #else
        if (paddleTimerInSeconds >= 0 && paddleTimerInSeconds < paddleForceApplicationTimer) {
            paddleTimerInSeconds += dt;
        } else {
            rightPaddleActive = false;
            leftPaddleActive = false;
            paddleTimerInSeconds = -1;            
        }             
        #endif   
    }

    public void OnFixedUpdate(float dt)
    {
        // Apply forces based on paddle input
        if (leftPaddleActive)
        {
            // Debug.Log("left paddle");
            ApplyPaddleForce(transform.forward * paddleStrength + transform.right * lateralStrength, -currentRotationStrength);
            // leftPaddleActive = false;
        }
        if (rightPaddleActive)
        {
            // Debug.Log("right paddle");
            ApplyPaddleForce(transform.forward * paddleStrength - transform.right * lateralStrength, currentRotationStrength);
            // rightPaddleActive = false;
        }
    }

    private void ApplyPaddleForce(Vector3 force, float rotation)
    {
        if (kayak != null) {
            kayak.AddForce(force.normalized, force.magnitude, ForceMode.Force);                            
            kayak.AddTorque(new Vector3(0f, rotation, 0f), ForceMode.Force);
        } else {
            Debug.LogError("Kayak script is missing. Please add that script in");            
        }
    }     

    #if UNITY_EDITOR
    void OnGUI() {

    }
    #endif
}
