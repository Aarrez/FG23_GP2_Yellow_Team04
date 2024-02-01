using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using DataandStats;

[Serializable]
public class ForceData {
    [SerializeField] public ITrigger Obstacle;
    [SerializeField] public Vector3 ForceDirection;
    [SerializeField] public float ForceStrength;
    [SerializeField] public AnimationCurve Decay;
    [SerializeField] public float Duration;
    [SerializeField] public bool IsForceDisablePlayerMovement;
    [HideInInspector] public float CurrentTime;
}

public enum EBoatState {
    NONE,
    HOOKLEFT,
    HOOKRIGHT,
    UNHOOKED
}


[RequireComponent(typeof(CharacterController))]
public class Boat : MonoBehaviour, IOnStartTouch, IOnStickInput, 
                    IOnCollisionTrigger<PushBackObstacle>
{
    [Header("Boat stats")]
    [SerializeField] private PlayerStats stats;         
    
    [Header("Runtime")]
    [SerializeField] private Vector3 inputHorizontalForce;
    [SerializeField] private Vector3 externalHorizontalForce;        
    [SerializeField] private Vector3 finalVelocity;
    
    private CharacterController controller;
    private MeshFilter[] meshes;
    private LineRenderer line;

    private LinkedList<ForceData> forceList= new LinkedList<ForceData>();

    private float rightInputNormalized;
    private float leftInputNormalized;
    private Vector3 leftInputDirection;
    private Vector3 rightInputDirection;

    private float timeBeforeResetLeft = -1;    
    private float timeBeforeResetRight = -1;

    private Vector3 cacheRightStickDirection;
    private Vector3 cacheLeftStickDirection;

    private Quaternion cacheRotation;
    private Vector3 cacheHorizontalForce;
    private float cachePaddleStrength;

    private float currentRotationTime;
    private float maxRotationTime = 1f;    
    
    private float movementDisabledTime;
    private bool isMovementDisabled;

    // drag coefficient
    private float dragCoefficientRate = 0.5f;

    // hook
    private Vector3 hookPoint;
    private Vector3 tangentDirection;
    private float hookDistance;
    private Vector3 hookDirectionLine;
    private float hookRampupTime;

    // velocity
    [SerializeField] private Vector3 externalVelocity;     

    [SerializeField] private EBoatState currentBoatState;
    
    public Vector3 InputHorizontalForce {
        get { return inputHorizontalForce; }
    }

    public Vector3 ExternalVelocity {
        get { return externalVelocity; }
        set { externalVelocity = value; }
    }
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputHorizontalForce = Vector3.forward;
        forceList = new LinkedList<ForceData>();
        meshes = GetComponentsInChildren<MeshFilter>();

        cachePaddleStrength = stats.paddleStrength;
        currentBoatState = EBoatState.UNHOOKED; 

        line = GetComponent<LineRenderer>();
        externalVelocity = stats.initialConstantVelocity;       
    }

    private void Update() {
        var dt = Time.deltaTime;

        if (dragCoefficientRate <= 0) {
            cacheRotation = transform.rotation;
            cacheHorizontalForce = inputHorizontalForce;
            currentRotationTime = 0;
        } else {
            currentRotationTime = Mathf.Clamp(currentRotationTime + dt, 0, maxRotationTime);
        }        

        if (controller.isGrounded) {
            if (cacheHorizontalForce.magnitude > 0.01) {
                transform.rotation = Quaternion.Slerp(cacheRotation, Quaternion.FromToRotation(transform.forward, cacheHorizontalForce) * cacheRotation, Mathf.InverseLerp(0, maxRotationTime, currentRotationTime));        
            }
        }

        if (timeBeforeResetRight >= 0) {
            if (timeBeforeResetRight > 0.5f) {
                rightInputNormalized = 0;
                rightInputDirection = Vector3.zero;
                
                timeBeforeResetRight = -1;                
            } else {
                timeBeforeResetRight += dt;
            }        
        }

        if (timeBeforeResetLeft >= 0) {
            if (timeBeforeResetLeft > 0.5f) {
                leftInputNormalized = 0;
                leftInputDirection = Vector3.zero;
                
                timeBeforeResetLeft = -1;                
            } else {
                timeBeforeResetLeft += dt;
            }        
        }

        // force removal check here
        var newMovementDisabled = isMovementDisabled;
        for(LinkedListNode<ForceData> node = forceList.First; node != null; node=node.Next){
            var f = node.Value;            
            f.CurrentTime += dt;            
            newMovementDisabled = f.IsForceDisablePlayerMovement;
            if (f.CurrentTime > f.Duration) {
                newMovementDisabled = false;
                f.CurrentTime = 0;
                forceList.Remove(node);
                foreach(var mesh in meshes) {
                    mesh.gameObject.SetActive(true);
                }
            }            
        }
        isMovementDisabled = newMovementDisabled;        
        if (isMovementDisabled == true && newMovementDisabled == false) {
            dragCoefficientRate = 0;
        }

        // TODO: move disable indication to another script
        if (isMovementDisabled) {
            movementDisabledTime += dt;
            if (movementDisabledTime > 0.1f) {
                foreach(var mesh in meshes) {
                    mesh.gameObject.SetActive(!mesh.gameObject.activeSelf);
                }
                movementDisabledTime = 0;
            }            
            leftInputDirection = Vector3.zero;
            rightInputDirection = Vector3.zero;
            timeBeforeResetRight = 0;
            timeBeforeResetLeft = 0;                        
        }

        switch(currentBoatState) {
            case EBoatState.HOOKRIGHT:
            case EBoatState.HOOKLEFT:
            if (hookRampupTime < stats.hookRampUpMaxTime) {
                hookRampupTime += dt;            
            }
            if ((hookPoint - transform.position).magnitude < stats.unHookRadius) {
                currentBoatState = EBoatState.UNHOOKED;
            }
            line.SetPosition(index: 1, transform.position);
            break;
            case EBoatState.UNHOOKED:
            line.enabled = false;
            cachePaddleStrength = stats.paddleStrength;
            break;
        } 
    }
    
    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        // calculate all acceleration
        var finalForce = Vector3.zero;
        if (!controller.isGrounded) {
            finalForce += stats.gravity;                                  
        }

        for(LinkedListNode<ForceData> node = forceList.First; node != null; node=node.Next){
            var f = node.Value;
            var strength = f.ForceDirection * f.ForceStrength * f.Decay.Evaluate(Mathf.InverseLerp(0, f.Duration, f.CurrentTime));            
            finalForce += strength;            
        }

        // external force
        externalHorizontalForce = new Vector3(finalForce.x, 0, finalForce.z);        

        // calculate horizontal forces
        var horizontalForces = Vector3.zero;
        if ((leftInputDirection.magnitude > 0 || rightInputDirection.magnitude > 0) && 
            controller.isGrounded) {            
            inputHorizontalForce = (leftInputDirection + rightInputDirection).normalized * cachePaddleStrength;            
        }                                            
        
        var drag = EvaluateDrag(inputHorizontalForce, dt);
        inputHorizontalForce -= drag;                        
        horizontalForces = inputHorizontalForce + externalHorizontalForce;
        
        switch(currentBoatState) {
            case EBoatState.HOOKRIGHT:
            case EBoatState.HOOKLEFT:            
            // we find tangential force            
            var d = Vector3.Dot(horizontalForces, hookDirectionLine);
            // we ignore forces that are in the direction of hook direction first            
            // our tangential direction is equal to whichever gives us the worldspace up 
            var tangentialDirection = (Vector3.Dot(Vector3.Cross(hookDirectionLine, horizontalForces), Vector3.up) > 0) ? Vector3.Cross(Vector3.up, hookDirectionLine) : Vector3.Cross(hookDirectionLine, Vector3.up); 
            var tangentialMagnitude = Vector3.Dot(horizontalForces, tangentialDirection);

            tangentDirection = tangentialDirection;
            var finalTangentForce = tangentialDirection * tangentialMagnitude;
            var positionAfterTangent = transform.position + finalTangentForce;
            var hookDirectionNew = hookPoint - positionAfterTangent;
            var hookPositionTarget = hookPoint + (-hookDirectionNew.normalized * hookDistance);
            var hookOffset = hookPositionTarget - positionAfterTangent;

            hookDirectionLine = hookDirectionNew.normalized;
            horizontalForces = finalTangentForce + hookOffset;                 
            // if (d < 0) {
            // } else {
            //     horizontalForces = Vector3.zero;
            // }
            // apply retract force
            if (stats.hookStrength > 0) {
                var retractForce = hookDirectionLine.normalized * (stats.hookPowerRampUp.Evaluate(hookRampupTime) * stats.hookStrength + horizontalForces.magnitude);                        
                horizontalForces += retractForce;
            }
            hookDistance = ((transform.position + horizontalForces * dt) - hookPoint).magnitude;
            break;

            case EBoatState.UNHOOKED:    
            break;
        }        
        horizontalForces = Mathf.Min(stats.maxSpeedHorizontal, horizontalForces.magnitude) * horizontalForces.normalized;                    

        // add horizontal velocity    
        finalVelocity.x = horizontalForces.x * dt;
        finalVelocity.z = horizontalForces.z * dt;                        
        
        // add vertical velocity                
        finalVelocity.y += finalForce.y * dt;
        finalVelocity.y = Mathf.Sign(finalVelocity.y) * Mathf.Min(Mathf.Abs(finalVelocity.y), stats.maxSpeedVertical);        
        
        // add initial velocity
        if (currentBoatState == EBoatState.UNHOOKED) {
            finalVelocity += externalVelocity;
        }
        
        var flags = controller.Move(finalVelocity);
    }    

    private Vector3 EvaluateDrag(Vector3 velocity, float dt) {
        var vMag = velocity.magnitude;
        var dragCo = Mathf.Lerp(stats.minDragCoefficient, stats.maxDragCoefficient, dragCoefficientRate / 2.0f);

        var drag = 0.5f * vMag * vMag * dragCo * velocity.normalized;
        drag = (drag.y < 0.01f) ? new Vector3(drag.x, 0, drag.z) : drag;
        drag = (drag.magnitude < 0.001f) ? Vector3.zero : drag;
        drag = (drag.magnitude > vMag) ? velocity : drag;

        dragCoefficientRate = Mathf.Clamp(dragCoefficientRate + dt, 0, 2.0f);
        
        return drag;
    }

    void IOnStartTouch.InvokeLeftSideTouch(Vector2 screenSpacePosition)
    {
        if (isMovementDisabled) return;
        // make boat paddle right
        rightInputNormalized = Mathf.Clamp(rightInputNormalized + stats.rateOfDirectionChange, 0, 1);                                    

        var maxRight = Vector3.Slerp(transform.forward, transform.right, stats.rateOfRotationChange);
        if (stats.travelBounds >= 0) {
            maxRight = Vector3.Slerp(Vector3.forward, Vector3.right, stats.travelBounds);        
        }
        rightInputDirection = Vector3.Slerp(transform.forward, maxRight, rightInputNormalized);
        rightInputDirection = rightInputDirection.normalized;        

        dragCoefficientRate = 0;
        timeBeforeResetRight = 0;
    }

    void IOnStartTouch.InvokeRightSideTouch(UnityEngine.Vector2 screenSpacePosition)
    {
        if (isMovementDisabled) return;

        // make boat paddle left
        leftInputNormalized = Mathf.Clamp(leftInputNormalized + stats.rateOfDirectionChange, 0, 1);

        var maxLeft = Vector3.Slerp(transform.forward, -transform.right, stats.rateOfRotationChange);
        if (stats.travelBounds >= 0) {
            maxLeft = Vector3.Slerp(Vector3.forward, -Vector3.right, stats.travelBounds);
        }
        
        leftInputDirection = Vector3.Slerp(transform.forward, maxLeft, leftInputNormalized);
        leftInputDirection = leftInputDirection.normalized;

        dragCoefficientRate = 0;
        timeBeforeResetLeft = 0;
    }        

    void IOnStartTouch.InvokeLeftSwipeTouch(UnityEngine.Vector2 screenSpacePosition) {
        switch(currentBoatState) {
            case EBoatState.HOOKLEFT:
            // after unhooking, we want to change horizontal forces to be an external force            
            currentBoatState = EBoatState.HOOKRIGHT;
            var rHForce = new Vector3(finalVelocity.x, 0, finalVelocity.z);
            externalHorizontalForce = rHForce;
            cachePaddleStrength = stats.hookPaddleStrength;

            HookRight();
            break;
            case EBoatState.HOOKRIGHT:
            // after unhooking, we want to change horizontal forces to be an external force            
            currentBoatState = EBoatState.UNHOOKED;
            var lHForce = new Vector3(finalVelocity.x, 0, finalVelocity.z);
            externalHorizontalForce = lHForce;
            cachePaddleStrength = stats.paddleStrength;            
            break;

            case EBoatState.UNHOOKED:
            cachePaddleStrength = stats.hookPaddleStrength;    
            HookRight();
            break;
        }
        
    }

    void IOnStartTouch.InvokeRightSwipeTouch(UnityEngine.Vector2 screenSpacePosition) {
        switch(currentBoatState) {
            case EBoatState.HOOKRIGHT:
            currentBoatState = EBoatState.HOOKRIGHT;
            var lHForce = new Vector3(finalVelocity.x, 0, finalVelocity.z);
            externalHorizontalForce = lHForce;
            cachePaddleStrength = stats.hookPaddleStrength;

            HookLeft();
            break;
            case EBoatState.HOOKLEFT:
            // after unhooking, we want to change horizontal forces to be an external force            
            currentBoatState = EBoatState.UNHOOKED;
            var rHForce = new Vector3(finalVelocity.x, 0, finalVelocity.z);
            externalHorizontalForce = rHForce;
            cachePaddleStrength = stats.paddleStrength;
            break;

            case EBoatState.UNHOOKED: 
            cachePaddleStrength = stats.hookPaddleStrength;   
            HookLeft();
            break;
        }
    }

    void IOnStickInput.OnInvokeRightStick(Vector2 direction)
    {
        if (isMovementDisabled) return;

        if (Vector3.Dot(direction, cacheRightStickDirection) < 0.9f) {
            var changeAmount = stats.rateOfDirectionChange * direction.magnitude;
            leftInputNormalized = Mathf.Clamp(leftInputNormalized + changeAmount, 0, 1);
            
            var maxLeft = Vector3.Slerp(Vector3.forward, -Vector3.right, stats.travelBounds);

            leftInputDirection = Vector3.Slerp(transform.forward, maxLeft, leftInputNormalized);
            leftInputDirection = leftInputDirection.normalized;

            dragCoefficientRate = 0;
            cacheRightStickDirection = direction;            
        }
    }

    void IOnStickInput.OnInvokeLeftStick(Vector2 direction)
    {
        if (isMovementDisabled) return;

        if (Vector3.Dot(direction, cacheLeftStickDirection) < 0.9f) {
            var changeAmount = stats.rateOfDirectionChange * direction.magnitude;
            rightInputNormalized = Mathf.Clamp(rightInputNormalized + changeAmount, 0, 1);                                    
            
            var maxRight = Vector3.Slerp(Vector3.forward, Vector3.right, stats.travelBounds);

            rightInputDirection = Vector3.Slerp(transform.forward, maxRight, rightInputNormalized);
            rightInputDirection = rightInputDirection.normalized;

            dragCoefficientRate = 0;
            cacheLeftStickDirection = direction;
        }
    }

    void IOnStickInput.OnInvokeRightStickEnd() {
        if (isMovementDisabled) return;
        
        leftInputNormalized = 0;
        leftInputDirection = Vector3.zero; 
    }

    void IOnStickInput.OnInvokeLeftStickEnd() {
        if (isMovementDisabled) return;

        rightInputNormalized = 0;
        rightInputDirection = Vector3.zero; 
    }

    void IOnCollisionTrigger<PushBackObstacle>.Invoke(PushBackObstacle trigger, ControllerColliderHit collision)
    {         
        var checkContains = forceList.Where(x => x.Obstacle.GObject.GetInstanceID() == trigger.GObject.GetInstanceID());
        if (checkContains.Count() <= 0) {
            Quaternion qTransform = Quaternion.FromToRotation(Vector3.forward, transform.forward);                    
            var horizontalForces = inputHorizontalForce + externalHorizontalForce;                    
            if (Vector3.Dot(horizontalForces.normalized, trigger.transform.forward) > trigger.ForceCone) {
                var direction = (trigger.transform.position - transform.position).normalized;
                trigger.ForceData.ForceDirection = collision.normal.normalized;
                trigger.ForceData.Obstacle = trigger;
                
                trigger.ForcePoint = collision.point;
                trigger.ForceDirection = collision.normal.normalized;

                forceList.AddLast(trigger.ForceData);                                        
            }
        }
    }

    void HookLeft() {
        var rightHook = Vector3.Lerp(Vector3.forward, Vector3.right, stats.hookDirection);

        RaycastHit hitInfo;
        var isHit = Physics.Raycast(transform.position, rightHook.normalized, out hitInfo, 999, stats.hookMasks);

        if (isHit) {
            line.enabled = true;
            line.SetPosition(index: 0, hitInfo.point);
            line.SetPosition(index: 1, transform.position);

            hookPoint = hitInfo.point;
            hookDistance = (transform.position - hookPoint).magnitude;
            hookDirectionLine = -(transform.position - hookPoint).normalized;
            currentBoatState = EBoatState.HOOKLEFT;                
        }
    }

    void HookRight() {
        var leftHook = Vector3.Lerp(Vector3.forward, -Vector3.right, stats.hookDirection);

        RaycastHit hitInfo;
        var isHit = Physics.Raycast(transform.position, leftHook.normalized, out hitInfo, 999, stats.hookMasks);

        if (isHit) {
            line.enabled = true;
            line.SetPosition(index: 0, hitInfo.point);
            line.SetPosition(index: 1, transform.position);

            hookPoint = hitInfo.point;
            hookDistance = (transform.position - hookPoint).magnitude;
            hookDirectionLine = -(transform.position - hookPoint).normalized;
            currentBoatState = EBoatState.HOOKRIGHT;                
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.GetComponent<PushBackObstacle>()) {
            PushBackObstacle pbo = hit.gameObject.GetComponent<PushBackObstacle>();
            var pushBack = GetComponent<IOnCollisionTrigger<PushBackObstacle>>();
            if (pushBack != null) {
                if (pbo.ForceCone < 0) {
                    pushBack.Invoke(pbo, hit);
                } else {
                    var toObject = (hit.gameObject.transform.position - transform.position).normalized;
                    if (Vector3.Dot(toObject, -transform.forward) > pbo.ForceCone) {
                        pushBack.Invoke(pbo, hit);                    
                    }
                }
            }  
        }               
    }

    void OnGUI() {
        for(LinkedListNode<ForceData> node = forceList.First; node != null; node=node.Next){
            var f = node.Value;
            var strength = f.ForceDirection * f.ForceStrength * f.Decay.Evaluate(Mathf.InverseLerp(0, f.Duration, f.CurrentTime));
            using (new GUILayout.HorizontalScope()) {
                GUILayout.TextArea(f.ForceDirection.ToString());
                GUILayout.TextArea(f.ForceStrength.ToString());
                GUILayout.TextArea(f.Decay.Evaluate(Mathf.InverseLerp(0, f.Duration, f.CurrentTime)).ToString());
                GUILayout.TextArea(f.CurrentTime.ToString() + "/" + f.Duration.ToString());
            }
        }    
    }

    #if UNITY_EDITOR    
    void OnDrawGizmos() {
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, transform.position + inputHorizontalForce.normalized * 4);
        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(transform.position, transform.position + externalHorizontalForce.normalized * 4);        
        if (stats == null) {
            return;
        }

        if (stats.travelBounds >= 0) {
            var maxLeft = Vector3.Lerp(Vector3.forward, -Vector3.right, stats.travelBounds);
            var maxRight = Vector3.Lerp(Vector3.forward, Vector3.right, stats.travelBounds);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + maxLeft.normalized * 3);
            Gizmos.DrawLine(transform.position, transform.position + maxRight.normalized * 3);            
        }


        // hook point
        if (currentBoatState == EBoatState.HOOKLEFT || 
            currentBoatState == EBoatState.HOOKRIGHT) {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(hookPoint, stats.unHookRadius);
            Gizmos.DrawLine(transform.position, hookPoint);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + hookDirectionLine.normalized * hookDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + tangentDirection * 2);            
        }

        // final horizontal velocity
        Gizmos.color = Color.cyan;
        var finalHVel = new Vector3(finalVelocity.x, 0, finalVelocity.z);
        Gizmos.DrawLine(transform.position, transform.position + finalHVel.normalized * 2);

        // hook direction
        var leftHook = Vector3.Lerp(Vector3.forward, -Vector3.right, stats.hookDirection);
        var rightHook = Vector3.Lerp(Vector3.forward, Vector3.right, stats.hookDirection);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + leftHook.normalized * 3);
        Gizmos.DrawLine(transform.position, transform.position + rightHook.normalized * 3);

        // Gizmos.color = Color.magenta;
        // Quaternion qTransform = Quaternion.FromToRotation(Vector3.forward, transform.forward);
        // Matrix4x4 originalMatrix = Gizmos.matrix;
        // Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        // Gizmos.DrawWireCube(new Vector3(0, 0, 1.5f), boxcastSize);
        // Gizmos.matrix = originalMatrix;
    }
    #endif
}
