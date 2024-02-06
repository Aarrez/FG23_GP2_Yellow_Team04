using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKayakEntity {
    void OnUpdate(float dt);
    void OnFixedUpdate(float dt);
    void Initialize(Kayak entity);
}

public enum EKayakState {
    NONE,
    HOOK
}

[RequireComponent(typeof(PaddleController))]
[RequireComponent(typeof(BoostController))]
[RequireComponent(typeof(HookController))]
public class Kayak : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float maxHorizontalVelocity = 5;
    [SerializeField] private float maxVerticalVelocity = 5;
    [SerializeField] private float maxLateralTorque = 5;

    private PaddleController paddleController;
    private BoostController boostController;
    private HookController hookController;
    private ButtonControls controls;
    private Floater[] floaters;
    
    private Rigidbody rb;
    private WaterController waterController;
    private Tuple<Vector3, Vector3> closest;

    Vector3 newUp;
    Vector3 newRight;
    Vector3 newForward;
    private GameObject meshObject;

    private Vector3 hitPoint;
    private Vector3 hitDirection;

    public float MaxHorizontalVelocity {
        get { return maxHorizontalVelocity; }
    }

    public float MaxVerticalVelocity {
        get { return maxVerticalVelocity; }
    }

    public Vector3 Velocity {
        get { return rb.velocity; }
    }

    public bool IsGrounded = false;
    
    void Awake() {
        paddleController = GetComponent<PaddleController>();
        boostController = GetComponent<BoostController>();
        hookController = GetComponent<HookController>();
        floaters = GetComponentsInChildren<Floater>();
        
        waterController = FindObjectOfType<WaterController>();
        
        if (transform.Find("Mesh") != null) {
            meshObject = transform.Find("Mesh").gameObject;
        }

        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        paddleController.Initialize(this);    
        boostController.Initialize(this);
        hookController.Initialize(this);
        foreach(var f in floaters) {
            f.Initialize(this);
        }

        controls = FindObjectOfType<ButtonControls>(true);
        if (controls != null) {
            controls.LeftPlayerHook.PointerDown.AddListener(hookController.OnLeftHookDown);
            controls.LeftPlayerHook.PointerUp.AddListener(hookController.OnLeftHookUp);
            controls.RightPlayerHook.PointerDown.AddListener(hookController.OnRightHookDown);
            controls.RightPlayerHook.PointerUp.AddListener(hookController.OnRightHookUp);

            controls.LeftPlayerLeftPaddle.PointerDown.AddListener(() =>{ paddleController.leftPaddleActive = true; });
            controls.LeftPlayerRightPaddle.PointerDown.AddListener(() =>{ paddleController.rightPaddleActive = true; });
            controls.RightPlayerLeftPaddle.PointerDown.AddListener(() =>{ paddleController.leftPaddleActive = true; });
            controls.RightPlayerRightPaddle.PointerDown.AddListener(() =>{ paddleController.rightPaddleActive = true; });
            
            #if USE_TAP_AND_HOLD
            controls.LeftPlayerLeftPaddle.PointerUp.AddListener(() =>{ paddleController.leftPaddleActive = false; });
            controls.LeftPlayerRightPaddle.PointerUp.AddListener(() =>{ paddleController.rightPaddleActive = false; });
            controls.RightPlayerLeftPaddle.PointerUp.AddListener(() =>{ paddleController.leftPaddleActive = false; });
            controls.RightPlayerRightPaddle.PointerUp.AddListener(() =>{ paddleController.rightPaddleActive = false; });
            #endif
        }
    }

    void Update() {
        var dt = Time.deltaTime;
        if (paddleController.enabled) paddleController.OnUpdate(dt);
        if (boostController.enabled) boostController.OnUpdate(dt);
        if (hookController.enabled) hookController.OnUpdate(dt); 
        foreach(var f in floaters) {
            f.OnUpdate(dt);
        }

        if (waterController != null && meshObject != null) {
            closest = waterController.GetClosestNormal(transform.position);
            newUp = closest.Item2;        
            newRight = Vector3.Cross(closest.Item2.normalized, transform.forward);
            newForward = Vector3.Cross(newRight, newUp);

            var targetRotation = Quaternion.LookRotation(newForward, newUp);            
            meshObject.transform.rotation = targetRotation;

            if (IsGrounded) {
                var worldSpaceHeight = waterController.WaveController.GetWaveHeight(transform.position.x);
                var localSpacePosition = transform.InverseTransformPoint(new Vector3(transform.position.x, worldSpaceHeight, transform.position.z));
                meshObject.transform.localPosition = localSpacePosition;       
            }
        }
    }

    void FixedUpdate() {
        var dt = Time.fixedDeltaTime;
        paddleController.OnFixedUpdate(dt);
        boostController.OnFixedUpdate(dt);
        hookController.OnFixedUpdate(dt);
        foreach(var f in floaters) {
            f.OnFixedUpdate(dt);
        }               
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<WaterController>()) {
            IsGrounded = true;
        }
        if (collision.gameObject.tag == "Wall") {
            var c = collision.contacts[0];
            hitPoint = c.point;
            hitDirection = c.normal;        
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.GetComponent<WaterController>()) {
            IsGrounded = false;            
        }
        if (collision.gameObject.tag == "Wall") {
            hitPoint = Vector3.zero;
            hitDirection = Vector3.zero;
        }
    }

    public void AddForce(Vector3 direction, float strength, ForceMode forceMode = ForceMode.Force) {
        var horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        var verticalVel = new Vector3(0, rb.velocity.y, 0);
        
        // Debug.Log("horizontal: " + horizontalVel.magnitude + ", " + maxHorizontalVelocity);
        // Debug.Log("vertical: " + verticalVel.magnitude + ", " + maxVerticalVelocity);

        if (horizontalVel.magnitude < maxHorizontalVelocity) {
            var horizontalDir = new Vector3(direction.x, 0, direction.z);
            if (hitDirection.magnitude > 0) {
                horizontalDir += hitDirection.normalized;                
            }
            rb.AddForce(horizontalDir.normalized * strength, forceMode);
        }

        if (verticalVel.magnitude < maxVerticalVelocity) {
            var verticalDir = new Vector3(0, direction.y, 0);
            rb.AddForce(verticalDir.normalized * strength, forceMode);            
        }
    }

    public void AddTorque(Vector3 torque, ForceMode forceMode = ForceMode.Force) {
        if (rb.angularVelocity.magnitude < maxLateralTorque) {
            rb.AddTorque(torque, forceMode);
        }
    }

    public void AddForceAtPosition(Vector3 direction, float strength, Vector3 position, ForceMode forceMode = ForceMode.Force) {
        var horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.y);
        Debug.Log(horizontalVel.magnitude);
        if (horizontalVel.magnitude < maxHorizontalVelocity) {
            rb.AddForceAtPosition(direction * strength, position, forceMode);
        }

        var verticalVel = new Vector3(0, rb.velocity.y, 0);
        if (verticalVel.magnitude < maxVerticalVelocity) {
            rb.AddForceAtPosition(direction * strength, position, forceMode);            
        }
    }    

    #if UNITY_EDITOR
    void OnDrawGizmos() {                
        Gizmos.color = Color.red;

        if (closest != null && meshObject != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            Gizmos.DrawLine(meshObject.transform.position, meshObject.transform.position + newUp.normalized * 3);        

            Gizmos.color = Color.red;
            Gizmos.DrawLine(meshObject.transform.position, meshObject.transform.position + newRight.normalized * 3);
        
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(meshObject.transform.position, meshObject.transform.position + newForward.normalized * 3);
        }

        Gizmos.DrawWireSphere(hitPoint, 0.1f);
        Gizmos.DrawLine(hitPoint, hitPoint + hitDirection.normalized * 2);
    }
    #endif
}
