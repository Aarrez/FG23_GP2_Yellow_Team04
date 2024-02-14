using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public enum EHookState {
    NONE = 0,
    LEFTHOOK = 1 << 1,
    RIGHTHOOK = 1 << 2,    
}

public class HookController : MonoBehaviour, IKayakEntity
{
    [SerializeField] private LayerMask whatIsGrappleable;
    [Range(0, 1)][SerializeField] private float direction;
    [SerializeField] private Vector3 hookOffset;
    [SerializeField] private float hookDistance = 1;
    [SerializeField] public float hookStrength = 2;
    [SerializeField] private float hookDisengageRadius = 0.5f;
    [Range(0, 1)] [SerializeField] private float hookVeclocityOffsetMultipler = 0;
    [SerializeField] private float hookAnimationTime = 0.2f;  
    [SerializeField] private AnimationCurve hookTrajectory = AnimationCurve.Constant(0, 0, 0);
    [SerializeField] private EHookState currentState;

    // [SerializeField] float jointSpring = 10f;
    // [SerializeField] float jointDamper = 5f;
    // [SerializeField] float jointMassScale = 1f;    

    private LineRenderer leftRenderer;
    private LineRenderer rightRenderer; 

    private Vector3 leftHookPoint;
    private Vector3 rightHookPoint;
    private Vector3 mainHookPoint;

    private Kayak kayak;   

    private bool isRightDown;
    private bool isLeftDown;

    private float currentHookAnimationTime;

    public void Initialize(Kayak entity)
    {
        this.kayak = entity;
        leftRenderer = transform.Find("LeftLineRenderer").GetComponent<LineRenderer>(); 
        rightRenderer = transform.Find("RightLineRenderer").GetComponent<LineRenderer>();

        currentState = EHookState.NONE; 

        var v = new Vector3[10];
        for(int i=0; i<v.Length; i++) {
            v[i] = transform.position;
        }
        leftRenderer.positionCount = 10;
        rightRenderer.positionCount = 10;
        leftRenderer.SetPositions(v);
        rightRenderer.SetPositions(v);              
    }

    public void OnFixedUpdate(float dt)
    {        
        #if USE_DOUBLE_HOOK
        var leftDirection = Vector3.Lerp(transform.forward, -transform.right, direction).normalized;
        var rightDirection = Vector3.Lerp(transform.forward, transform.right, direction).normalized;

        if (currentState.HasFlag(EHookState.LEFTHOOK) && currentState.HasFlag(EHookState.RIGHTHOOK)) {
            kayak.AddForce((rightDirection + leftDirection).normalized, hookStrength, dt, ForceMode.Force);            
        } else if (currentState.HasFlag(EHookState.RIGHTHOOK)) {
            // kayak.rb.velocity = CalculateAngularVelocity(rightDirection, rightHookPoint, new Vector3(kayak.rb.velocity.x, 0, kayak.rb.velocity.z));
            // kayak.rb.velocity = CalculateJumpVelocity(transform.position, rightHookPoint, 0);                
            kayak.AddForce(rightDirection, hookStrength, dt, ForceMode.Force);            
        } else if (currentState.HasFlag(EHookState.LEFTHOOK)) {            
            // kayak.rb.velocity = CalculateAngularVelocity(leftDirection, leftHookPoint, new Vector3(kayak.rb.velocity.x, 0, kayak.rb.velocity.z));
            // kayak.rb.velocity = CalculateJumpVelocity(transform.position, leftHookPoint, 0);
            kayak.AddForce(leftDirection, hookStrength, dt, ForceMode.Force);          
        }
        #else
        // var leftDirection = Vector3.Lerp(transform.up, transform.forward, direction).normalized;
        // var rightDirection = Vector3.Lerp(transform.up, transform.forward, direction).normalized;

        var hookForceDirection = (mainHookPoint - transform.position).normalized;
        if (currentState.HasFlag(EHookState.LEFTHOOK) && currentState.HasFlag(EHookState.RIGHTHOOK)) {
            kayak.AddForce(hookForceDirection, hookStrength, dt, ForceMode.Force);            
        }
        #endif
    }

    public void OnUpdate(float dt)
    {
        CalculateHookAnimation(currentHookAnimationTime, hookAnimationTime, transform.position, mainHookPoint);            
        // rightRenderer.SetPosition(index: 0, transform.position);
        // leftRenderer.SetPosition(index: 0, transform.position); 
        
        if (Input.GetKeyDown(KeyCode.I)) {
            OnRightHookDown();
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            OnLeftHookDown();    
        }

        if (Input.GetKeyUp(KeyCode.I)) {
            OnRightHookUp();
        }
        
        if (Input.GetKeyUp(KeyCode.W)) {
            OnLeftHookUp();
        }

        #if USE_DOUBLE_HOOK
        if (currentState.HasFlag(EHookState.LEFTHOOK)) {
            if ((transform.position - leftHookPoint).magnitude < hookDisengageRadius) {
                OnLeftHookUp();
            }
        }
        if (currentState.HasFlag(EHookState.RIGHTHOOK)) {
            if ((transform.position - rightHookPoint).magnitude < hookDisengageRadius) {
                OnRightHookUp();
            }
        }
        #else
        if (currentState.HasFlag(EHookState.LEFTHOOK) && currentState.HasFlag(EHookState.RIGHTHOOK)) {
            if ((transform.position - mainHookPoint).magnitude < hookDisengageRadius) {
                OnRightHookUp();
                OnLeftHookUp();
            }            
        }                
        #endif

        currentHookAnimationTime = Mathf.Clamp(currentHookAnimationTime + dt, 0, 1);        
    }

    public void OnRightHookDown() {
        #if USE_DOUBLE_HOOK
        var rightDirection = Vector3.Lerp(transform.forward, transform.right, direction).normalized;
        RaycastHit rightHit;
        var isRightHit = Physics.Raycast(transform.position + transform.TransformVector(hookOffset), rightDirection, out rightHit, hookDistance, whatIsGrappleable);
        if (isRightHit) {
            rightRenderer.enabled = true;
            rightHookPoint = rightHit.point;
            CalculateHookAnimation(currentHookAnimationTime, hookAnimationTime, transform.position, rightHit.point);
            // rightRenderer.SetPosition(index: 1, rightHit.point);
            currentState |= EHookState.RIGHTHOOK;            
        }
        #else
        if (currentState.HasFlag(EHookState.LEFTHOOK) && currentState.HasFlag(EHookState.RIGHTHOOK)) {
            return;
        }

        var rightDirection = Vector3.Lerp(transform.up, transform.forward, direction).normalized;
        RaycastHit rightHit;        
        
        var horizontalVel = transform.forward * new Vector3(kayak.Velocity.x, 0, kayak.Velocity.z).magnitude;       
        
        var isRightHit = Physics.Raycast(transform.position + (horizontalVel * hookVeclocityOffsetMultipler) + transform.TransformVector(hookOffset), rightDirection, out rightHit, hookDistance, whatIsGrappleable);
        if (isRightHit) {
            rightHookPoint = rightHit.point;
            currentState |= EHookState.RIGHTHOOK;

            if (currentState.HasFlag(EHookState.LEFTHOOK) && currentState.HasFlag(EHookState.RIGHTHOOK)) {
                currentHookAnimationTime = 0;
                if (leftRenderer.enabled) leftRenderer.enabled = false;
                rightRenderer.enabled = true;
                // rightRenderer.SetPosition(index: 1, rightHit.point);
                CalculateHookAnimation(currentHookAnimationTime, hookAnimationTime, transform.position, rightHit.point);
                mainHookPoint = rightHookPoint;
            } 
        }
        #endif
    }

    public void OnLeftHookDown() {
        #if USE_DOUBLE_HOOK
        var leftDirection = Vector3.Lerp(transform.forward, -transform.right, direction).normalized;
        RaycastHit leftHit;
        var isLeftHit = Physics.Raycast(transform.position + transform.TransformVector(hookOffset), leftDirection, out leftHit, hookDistance, whatIsGrappleable);
        if (isLeftHit) {
            leftRenderer.enabled = true;
            leftHookPoint = leftHit.point;
            // leftRenderer.SetPosition(index: 1, leftHit.point);
            CalculateHookAnimation(currentHookAnimationTime, hookAnimationTime, transform.position, leftHit.point);
            currentState |= EHookState.LEFTHOOK;
        }
        #else
        if (currentState.HasFlag(EHookState.LEFTHOOK) && currentState.HasFlag(EHookState.RIGHTHOOK)) {
            return;
        }

        var horizontalVel = transform.forward * new Vector3(kayak.Velocity.x, 0, kayak.Velocity.z).magnitude;
        
        var leftDirection = Vector3.Lerp(transform.up, transform.forward, direction).normalized;
        RaycastHit leftHit;
        var isLeftHit = Physics.Raycast(transform.position + (horizontalVel * hookVeclocityOffsetMultipler) + transform.TransformVector(hookOffset), leftDirection, out leftHit, hookDistance, whatIsGrappleable);
        if (isLeftHit) {
            leftHookPoint = leftHit.point;            
            currentState |= EHookState.LEFTHOOK;

            if (currentState.HasFlag(EHookState.LEFTHOOK) && currentState.HasFlag(EHookState.RIGHTHOOK)) {
                currentHookAnimationTime = 0;
                if (rightRenderer.enabled) rightRenderer.enabled = false;
                leftRenderer.enabled = true;
                // leftRenderer.SetPosition(index: 1, leftHit.point);
                CalculateHookAnimation(currentHookAnimationTime, hookAnimationTime, transform.position, leftHit.point);
                mainHookPoint = leftHookPoint;
            }
        }
        #endif
    }

    public void OnLeftHookUp() {
        currentState &= ~EHookState.LEFTHOOK;
        leftRenderer.enabled = false;
        
        #if !USE_DOUBLE_HOOK
        rightRenderer.enabled = false;
        #endif
    }

    public void OnRightHookUp() {
        currentState &= ~EHookState.RIGHTHOOK;
        rightRenderer.enabled = false;

        #if !USE_DOUBLE_HOOK
        leftRenderer.enabled = false;
        #endif
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public Vector3 CalculateAngularVelocity(Vector3 hookDir, Vector3 hookPoint, Vector3 horizontalVelocity) {
        var tangentialDirection = (Vector3.Dot(Vector3.Cross(hookDir, horizontalVelocity), Vector3.up) > 0) ? Vector3.Cross(Vector3.up, hookDir) : Vector3.Cross(hookDir, Vector3.up); 
        var tangentialMagnitude = Vector3.Dot(horizontalVelocity, tangentialDirection);

        var tangentDirection = tangentialDirection;
        var finalTangentForce = tangentialDirection * tangentialMagnitude;
        var positionAfterTangent = transform.position + finalTangentForce;
        var hookDirectionNew = hookPoint - positionAfterTangent;
        var hookPositionTarget = hookPoint + (-hookDirectionNew.normalized * hookDistance);
        var hookOffset = hookPositionTarget - positionAfterTangent;

        return (finalTangentForce + hookOffset).normalized;
    }

    public void CalculateHookAnimation(float currentTime, float finalTime, Vector3 initialPosition, Vector3 finalPosition) {
        var normalizedTime = Mathf.InverseLerp(0, finalTime, currentTime);
        var directionDiff = finalPosition - initialPosition;
        float split = directionDiff.magnitude / 10;
        for (int i=0; i<10; i++) {
            var fPos = initialPosition + directionDiff.normalized * split * i;            
            var lerpPos = Vector3Utility.InverseLerp(initialPosition, finalPosition, fPos);

            var fCurvePos = hookTrajectory.Evaluate(lerpPos);
            // var cUp = Vector3.Cross(directionDiff.normalized, transform.right);
            var realFPos = Vector3.Lerp(fPos + (transform.right * fCurvePos), fPos, normalizedTime);

            leftRenderer.SetPosition(i, Vector3.Lerp(initialPosition, realFPos, normalizedTime));
            rightRenderer.SetPosition(i, Vector3.Lerp(initialPosition, realFPos, normalizedTime));            
        }        
    } 

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        #if USE_DOUBLE_HOOK
        var leftDirection = Vector3.Lerp(transform.forward, -transform.right, direction);
        var rightDirection = Vector3.Lerp(transform.forward, transform.right, direction); 
        #else
        var leftDirection = Vector3.Lerp(transform.up, transform.forward, direction);
        var rightDirection = Vector3.Lerp(transform.up, transform.forward, direction); 
        #endif

        Gizmos.color = Color.magenta;
        var horizontalVel = Vector3.zero;    
        if (kayak != null) {
            horizontalVel = transform.forward * new Vector3(kayak.Velocity.x, 0, kayak.Velocity.z).magnitude;
        }
        Gizmos.DrawWireSphere(transform.position + (horizontalVel * hookVeclocityOffsetMultipler) + transform.TransformVector(hookOffset), 0.1f);
        Gizmos.DrawLine(transform.position + (horizontalVel * hookVeclocityOffsetMultipler) + transform.TransformVector(hookOffset), transform.position + (horizontalVel * hookVeclocityOffsetMultipler) + transform.TransformVector(hookOffset) + (rightDirection.normalized * hookDistance));
        Gizmos.DrawLine(transform.position + (horizontalVel * hookVeclocityOffsetMultipler) + transform.TransformVector(hookOffset), transform.position + (horizontalVel * hookVeclocityOffsetMultipler) + transform.TransformVector(hookOffset) + (leftDirection.normalized * hookDistance));

        if (kayak != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, kayak.Velocity.normalized * 5);
        }

        #if USE_DOUBLE_HOOK
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rightHookPoint, 0.5f);
        Gizmos.DrawWireSphere(leftHookPoint, 0.5f);
        #else
        Gizmos.DrawWireSphere(mainHookPoint, hookDisengageRadius);
        #endif
    }
    #endif
}
