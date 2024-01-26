using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ForceData {
    [SerializeField] public Vector3 ForceDirection;
    [SerializeField] public float ForceStrength;
    [SerializeField] public AnimationCurve Decay;
    [SerializeField] public float Duration;
    [SerializeField] public bool IsForceDisablePlayerMovement;
    [HideInInspector] public float CurrentTime;
}


[RequireComponent(typeof(CharacterController))]
public class Boat : MonoBehaviour, IOnStartTouch, IOnStickInput, 
                    IOnCollisionTrigger<PushBackObstacle>
{
    [Header("Movement")]
    [SerializeField] private Vector3 gravity = new Vector3(0, -1, 0);
    [SerializeField] private Vector3 initialConstantVelocity = new Vector3(0, 0, 0);
    [Range(0, 2f)][SerializeField] private float rateOfDirectionChange = 0.1f;            
    [Range(0, 1f)][SerializeField] private float rateOfRotationChange = 0.5f;
    [SerializeField] private float paddleStrength = 3;
    [SerializeField] private float maxSpeedHorizontal;
    [SerializeField] private float maxSpeedVertical;
    [Range(-1, 1)] [SerializeField] private float travelBounds = 0;        

    [Header("Drag")]
    [SerializeField] private float minDragCoefficient= 0;
    [SerializeField] private float maxDragCoefficient = 1;
    [SerializeField] private float dragCoefficientRate = 0.5f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask obstacleMask;
    
    [Header("Runtime")]
    [SerializeField] private Vector3 leftInputDirection;
    [SerializeField] private Vector3 rightInputDirection;
    [SerializeField] private Vector3 horizontalForce;
    [SerializeField] private Vector3 externalForce;    
    [SerializeField] private Vector3 finalVelocity;

    private LinkedList<ForceData> forceList= new LinkedList<ForceData>();

    private CharacterController controller;
    
    private float rightInputNormalized;
    private float leftInputNormalized;

    private float timeBeforeResetLeft = -1;    
    private float timeBeforeResetRight = -1;

    private Vector3 cacheRightStickDirection;
    private Vector3 cacheLeftStickDirection;

    private Quaternion cacheRotation;
    private Vector3 cacheHorizontalForce;


    private float currentRotationTime;
    private float maxRotationTime = 1f;
    
    public Vector3 HorizontalVelocity {
        get { return horizontalForce; }
    }

    public Vector3 InitialConstantVelocity {
        get { return initialConstantVelocity; }
        set { initialConstantVelocity = value; }
    }
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        horizontalForce = Vector3.forward;
        forceList = new LinkedList<ForceData>();        
    }

    private void Update() {
        if (dragCoefficientRate <= 0) {
            cacheRotation = transform.rotation;
            cacheHorizontalForce = horizontalForce;
            currentRotationTime = 0;
        } else {
            currentRotationTime = Mathf.Clamp(currentRotationTime + Time.deltaTime, 0, maxRotationTime);
        }        

        if (controller.isGrounded) {
            transform.rotation = Quaternion.Slerp(cacheRotation, Quaternion.FromToRotation(transform.forward, cacheHorizontalForce) * cacheRotation, Mathf.InverseLerp(0, maxRotationTime, currentRotationTime));        
        }

        if (timeBeforeResetRight >= 0) {
            if (timeBeforeResetRight > 0.5f) {
                rightInputNormalized = 0;
                rightInputDirection = Vector3.zero;
                
                timeBeforeResetRight = -1;                
            } else {
                timeBeforeResetRight += Time.deltaTime;
            }        
        }

        if (timeBeforeResetLeft >= 0) {
            if (timeBeforeResetLeft > 0.5f) {
                leftInputNormalized = 0;
                leftInputDirection = Vector3.zero;
                
                timeBeforeResetLeft = -1;                
            } else {
                timeBeforeResetLeft += Time.deltaTime;
            }        
        }
    }
    
    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        // calculate forces
        var finalAcceleration = Vector3.zero;
        if (!controller.isGrounded) {
            finalAcceleration += gravity;            
            // if (allowAirJump && airJumpCount <= numOfAirJumps && airJumpCooldownFrameCount >= airJumpCooldownFrames) {
            //     finalAcceleration += jumpAcceleration;
            // }            
        }

        for(LinkedListNode<ForceData> node = forceList.First; node != null; node=node.Next){
            var f = node.Value;
            finalAcceleration += f.ForceDirection * f.ForceStrength * f.Decay.Evaluate(Mathf.InverseLerp(0, f.Duration, f.CurrentTime));
            f.CurrentTime += Time.fixedDeltaTime;            
            if (f.CurrentTime > f.Duration) {
                forceList.Remove(node);
            }
        }        

        // add horizontal forces
        if ((leftInputDirection.magnitude > 0 || rightInputDirection.magnitude > 0) && 
            controller.isGrounded) {            
            horizontalForce = (leftInputDirection + rightInputDirection).normalized * paddleStrength;            
        }                                    
        
        
        var drag = EvaluateDrag(horizontalForce, dt);
        horizontalForce -= drag;        
        
        horizontalForce += new Vector3(finalAcceleration.x, 0, finalAcceleration.z);
        horizontalForce = Mathf.Min(maxSpeedHorizontal, horizontalForce.magnitude) * horizontalForce.normalized;                    

        finalVelocity.x = horizontalForce.x * dt;
        finalVelocity.z = horizontalForce.z * dt;                        
        
        // add vertical forces                
        finalVelocity.y += finalAcceleration.y * dt;
        finalVelocity.y = Mathf.Sign(finalVelocity.y) * Mathf.Min(Mathf.Abs(finalVelocity.y), maxSpeedVertical);        

        
        finalVelocity += initialConstantVelocity;
        
        var flags = controller.Move(finalVelocity);
    }    

    private Vector3 EvaluateDrag(Vector3 velocity, float dt) {
        var vMag = velocity.magnitude;
        var dragCo = Mathf.Lerp(minDragCoefficient, maxDragCoefficient, dragCoefficientRate / 2.0f);

        var drag = 0.5f * vMag * vMag * dragCo * velocity.normalized;
        drag = (drag.y < 0.01f) ? new Vector3(drag.x, 0, drag.z) : drag;
        drag = (drag.magnitude < 0.001f) ? Vector3.zero : drag;
        drag = (drag.magnitude > vMag) ? velocity : drag;

        dragCoefficientRate = Mathf.Clamp(dragCoefficientRate + dt, 0, 2.0f);
        
        return drag;
    }

    void IOnStartTouch.InvokeLeftSideTouch(Vector2 screenSpacePosition)
    {
        // make boat paddle right
        rightInputNormalized = Mathf.Clamp(rightInputNormalized + rateOfDirectionChange, 0, 1);                                    

        var maxRight = Vector3.Slerp(transform.forward, transform.right, rateOfRotationChange);
        if (travelBounds >= 0) {
            maxRight = Vector3.Slerp(Vector3.forward, Vector3.right, travelBounds);        
        }
        rightInputDirection = Vector3.Slerp(transform.forward, maxRight, rightInputNormalized);
        rightInputDirection = rightInputDirection.normalized;        

        dragCoefficientRate = 0;
        timeBeforeResetRight = 0;
    }

    void IOnStartTouch.InvokeRightSideTouch(UnityEngine.Vector2 screenSpacePosition)
    {
        // make boat paddle left
        leftInputNormalized = Mathf.Clamp(leftInputNormalized + rateOfDirectionChange, 0, 1);

        var maxLeft = Vector3.Slerp(transform.forward, -transform.right, rateOfRotationChange);
        if (travelBounds >= 0) {
            maxLeft = Vector3.Slerp(Vector3.forward, -Vector3.right, travelBounds);
        }
        
        leftInputDirection = Vector3.Slerp(transform.forward, maxLeft, leftInputNormalized);
        leftInputDirection = leftInputDirection.normalized;

        dragCoefficientRate = 0;
        timeBeforeResetLeft = 0;
    }        

    void IOnStickInput.OnInvokeRightStick(Vector2 direction)
    {
        if (Vector3.Dot(direction, cacheRightStickDirection) < 0.9f) {
            var changeAmount = rateOfDirectionChange * direction.magnitude;
            leftInputNormalized = Mathf.Clamp(leftInputNormalized + changeAmount, 0, 1);
            
            var maxLeft = Vector3.Slerp(Vector3.forward, -Vector3.right, travelBounds);

            leftInputDirection = Vector3.Slerp(transform.forward, maxLeft, leftInputNormalized);
            leftInputDirection = leftInputDirection.normalized;

            dragCoefficientRate = 0;
            cacheRightStickDirection = direction;            
        }
    }

    void IOnStickInput.OnInvokeLeftStick(Vector2 direction)
    {
        if (Vector3.Dot(direction, cacheLeftStickDirection) < 0.9f) {
            var changeAmount = rateOfDirectionChange * direction.magnitude;
            rightInputNormalized = Mathf.Clamp(rightInputNormalized + changeAmount, 0, 1);                                    
            
            var maxRight = Vector3.Slerp(Vector3.forward, Vector3.right, travelBounds);

            rightInputDirection = Vector3.Slerp(transform.forward, maxRight, rightInputNormalized);
            rightInputDirection = rightInputDirection.normalized;

            dragCoefficientRate = 0;
            cacheLeftStickDirection = direction;
        }
    }

    void IOnStickInput.OnInvokeRightStickEnd() {
        leftInputNormalized = 0;
        leftInputDirection = Vector3.zero; 
    }

    void IOnStickInput.OnInvokeLeftStickEnd() {
        rightInputNormalized = 0;
        rightInputDirection = Vector3.zero; 
    }

    void IOnCollisionTrigger<PushBackObstacle>.Invoke(PushBackObstacle trigger)
    {
        Quaternion qTransform = Quaternion.FromToRotation(Vector3.forward, transform.forward);        
        RaycastHit hitInfo;
        var direction = (trigger.transform.position - transform.position).normalized;
        bool isHit = Physics.Raycast(transform.position, direction, out hitInfo, 5, obstacleMask);         
        if (isHit) {            
            trigger.ForceData.ForceDirection = hitInfo.normal.normalized;
            forceList.AddLast(trigger.ForceData);            
        }
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + horizontalForce.normalized * 4);

        if (travelBounds >= 0) {
            var maxLeft = Vector3.Lerp(Vector3.forward, -Vector3.right, travelBounds);
            var maxRight = Vector3.Lerp(Vector3.forward, Vector3.right, travelBounds);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + maxLeft.normalized * 3);
            Gizmos.DrawLine(transform.position, transform.position + maxRight.normalized * 3);            

        }

        // Gizmos.color = Color.magenta;
        // Quaternion qTransform = Quaternion.FromToRotation(Vector3.forward, transform.forward);
        // Matrix4x4 originalMatrix = Gizmos.matrix;
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        // Gizmos.DrawWireCube(new Vector3(0, 0, 1.5f), boxcastSize);
        // Gizmos.matrix = originalMatrix;
    }
    #endif
}
