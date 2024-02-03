using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float hookStrength = 2;
    [SerializeField] private float maxHorizontalVelocity = 5;
    [SerializeField] private EHookState currentState;

    // [SerializeField] float jointSpring = 10f;
    // [SerializeField] float jointDamper = 5f;
    // [SerializeField] float jointMassScale = 1f;    

    private LineRenderer leftRenderer;
    private LineRenderer rightRenderer; 

    private Vector3 leftHookPoint;
    private Vector3 rightHookPoint;

    private Kayak kayak;   

    public void Initialize(Kayak entity)
    {
        this.kayak = entity;
        leftRenderer = transform.Find("LeftLineRenderer").GetComponent<LineRenderer>(); 
        rightRenderer = transform.Find("RightLineRenderer").GetComponent<LineRenderer>();

        currentState = EHookState.NONE;               
    }

    public void OnFixedUpdate(float dt)
    {
        var leftDirection = Vector3.Lerp(transform.forward, -transform.right, direction).normalized;
        var rightDirection = Vector3.Lerp(transform.forward, transform.right, direction).normalized;

        if (currentState.HasFlag(EHookState.LEFTHOOK) && currentState.HasFlag(EHookState.RIGHTHOOK)) {
            if (kayak.rb.velocity.magnitude < maxHorizontalVelocity) {
                kayak.rb.AddForce((rightDirection + leftDirection).normalized * hookStrength, ForceMode.Force);
            }
        } else if (currentState.HasFlag(EHookState.RIGHTHOOK)) {
            // kayak.rb.velocity = CalculateAngularVelocity(rightDirection, rightHookPoint, new Vector3(kayak.rb.velocity.x, 0, kayak.rb.velocity.z));
            if (kayak.rb.velocity.magnitude < maxHorizontalVelocity) {
                // kayak.rb.velocity = CalculateJumpVelocity(transform.position, rightHookPoint, 0);                
                kayak.rb.AddForce(rightDirection * hookStrength, ForceMode.Force);
            }

        } else if (currentState.HasFlag(EHookState.LEFTHOOK)) {            
            // kayak.rb.velocity = CalculateAngularVelocity(leftDirection, leftHookPoint, new Vector3(kayak.rb.velocity.x, 0, kayak.rb.velocity.z));
            if (kayak.rb.velocity.magnitude < maxHorizontalVelocity) {
                // kayak.rb.velocity = CalculateJumpVelocity(transform.position, leftHookPoint, 0);
                kayak.rb.AddForce(leftDirection * hookStrength, ForceMode.Force);
            }
        }
    }

    public void OnUpdate(float dt)
    {            
        rightRenderer.SetPosition(index: 0, transform.position);
        leftRenderer.SetPosition(index: 0, transform.position); 
        
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
        
    }

    public void OnRightHookDown() {
        var rightDirection = Vector3.Lerp(transform.forward, transform.right, direction).normalized;
        RaycastHit rightHit;
        var isRightHit = Physics.Raycast(transform.position + hookOffset, rightDirection, out rightHit, hookDistance, whatIsGrappleable);
        if (isRightHit) {
            rightRenderer.enabled = true;
            rightHookPoint = rightHit.point;
            rightRenderer.SetPosition(index: 1, rightHit.point);
            currentState |= EHookState.RIGHTHOOK;            
        }
    }

    public void OnLeftHookDown() {
        var leftDirection = Vector3.Lerp(transform.forward, -transform.right, direction).normalized;
        RaycastHit leftHit;
        var isLeftHit = Physics.Raycast(transform.position + hookOffset, leftDirection, out leftHit, hookDistance, whatIsGrappleable);
        if (isLeftHit) {
            leftRenderer.enabled = true;
            leftHookPoint = leftHit.point;
            leftRenderer.SetPosition(index: 1, leftHit.point);
            currentState |= EHookState.LEFTHOOK;
        }
    }

    public void OnLeftHookUp() {
        currentState &= ~EHookState.LEFTHOOK;
        leftRenderer.enabled = false;
    }

    public void OnRightHookUp() {
        currentState &= ~EHookState.RIGHTHOOK;
        rightRenderer.enabled = false;
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

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        var leftDirection = Vector3.Lerp(transform.forward, -transform.right, direction);
        var rightDirection = Vector3.Lerp(transform.forward, transform.right, direction); 

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position + hookOffset, transform.position + hookOffset + rightDirection * 3);
        Gizmos.DrawLine(transform.position  + hookOffset, transform.position + hookOffset + leftDirection * 3);

        if (kayak != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, kayak.rb.velocity.normalized * 5);
        }

        Gizmos.DrawWireSphere(rightHookPoint, 0.5f);
        Gizmos.DrawWireSphere(leftHookPoint, 0.5f);
    }
    #endif
}
