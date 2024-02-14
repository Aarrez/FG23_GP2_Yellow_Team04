using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PaddleController : MonoBehaviour, IKayakEntity
{
    [System.Flags]
    public enum EPaddleState
    {
        NONE = 0,
        LEFT_PADDLE = 1 << 1,
        RIGHT_PADDLE = 1 << 2
    }
    
    [Header("Paddle")]
    [SerializeField] public float paddleStrength = 10f;
    [SerializeField] public float rotationStrength = 5f;
    [SerializeField] private float lateralStrength = 2f; 
    [SerializeField] private float paddleForceApplicationTimer = 0.1f;

    [SerializeField] private AnimationCurve paddleStrengthDecay = AnimationCurve.Constant(0, 1, 1);
    
    public EPaddleState paddleState;

    public Animator leftCharacterPaddleAnimator;
    public Animator rightCharacterPaddleAnimator;

    private Kayak kayak;
    public float currentRotationStrength;    

    public float leftPaddleTimerInSeconds = -1;
    public float rightPaddleTimerInSeconds = -1;
    
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

        paddleState = EPaddleState.NONE;
        leftPaddleTimerInSeconds = -1;
        rightPaddleTimerInSeconds = -1;

        var left = transform.Find("Mesh/Kayak_low/CharacterLeft");
        var right = transform.Find("Mesh/Kayak_low/CharacterRight");

        if (left != null) {
            leftCharacterPaddleAnimator = left.GetComponent<Animator>();
        }
        if (right != null) {
            rightCharacterPaddleAnimator = right.GetComponent<Animator>();
        }
    }

    public void OnUpdate(float dt)
    {
        // Check for key presses
        if (Input.GetKeyDown(KeyCode.A))
        {
            paddleState |= EPaddleState.LEFT_PADDLE;
            leftPaddleTimerInSeconds = 0;
            leftCharacterPaddleAnimator?.SetTrigger("PaddleLeft");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            paddleState |= EPaddleState.LEFT_PADDLE;
            leftPaddleTimerInSeconds = 0;
            rightCharacterPaddleAnimator?.SetTrigger("PaddleLeft");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            paddleState |= EPaddleState.RIGHT_PADDLE;
            rightPaddleTimerInSeconds = 0;
            leftCharacterPaddleAnimator?.SetTrigger("PaddleRight");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            paddleState |= EPaddleState.RIGHT_PADDLE;
            rightPaddleTimerInSeconds = 0;
            rightCharacterPaddleAnimator?.SetTrigger("PaddleRight");
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
        if (leftPaddleTimerInSeconds >= 0)
        {
            if (leftPaddleTimerInSeconds >= 0 && leftPaddleTimerInSeconds < paddleForceApplicationTimer) {
                leftPaddleTimerInSeconds += dt;
            } else
            {
                paddleState &= ~EPaddleState.LEFT_PADDLE;
                leftPaddleTimerInSeconds = -1;            
            }
        }

        if (rightPaddleTimerInSeconds >= 0)
        {
            if (rightPaddleTimerInSeconds >= 0 && rightPaddleTimerInSeconds < paddleForceApplicationTimer) {
                rightPaddleTimerInSeconds += dt;
            } else
            {
                paddleState &= ~EPaddleState.RIGHT_PADDLE;
                rightPaddleTimerInSeconds = -1;            
            } 
        }
        #endif   
    }

    public void OnFixedUpdate(float dt)
    {
        // Apply forces based on paddle input
        if (paddleState.HasFlag(EPaddleState.LEFT_PADDLE))
        {
            ApplyPaddleForce(transform.forward * paddleStrength + transform.right * lateralStrength, dt, -currentRotationStrength, leftPaddleTimerInSeconds);
        }
        if (paddleState.HasFlag(EPaddleState.RIGHT_PADDLE))
        {
            ApplyPaddleForce(transform.forward * paddleStrength - transform.right * lateralStrength, dt, currentRotationStrength, rightPaddleTimerInSeconds);
        }
    }

    private void ApplyPaddleForce(Vector3 force, float dt, float rotation, float currentPaddleTime)
    {
        if (kayak != null)
        {
            kayak.AddForce(force.normalized, force.magnitude * paddleStrengthDecay.Evaluate(Mathf.InverseLerp(paddleForceApplicationTimer, 0, currentPaddleTime)), dt, ForceMode.Force);                            
            kayak.AddTorque(new Vector3(0f, rotation * paddleStrengthDecay.Evaluate(Mathf.InverseLerp(paddleForceApplicationTimer, 0, currentPaddleTime)), 0f), dt, ForceMode.Force);
        } else {
            Debug.LogError("Kayak script is missing. Please add that script in");            
        }
    }     

    #if UNITY_EDITOR
    void OnGUI() {

    }
    #endif
}
