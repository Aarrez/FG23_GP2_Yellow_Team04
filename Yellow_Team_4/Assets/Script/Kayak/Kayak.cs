using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem foamEffect;

    [Header("Other")]
    [SerializeField] private float wallNudgeMultiplier = 1;

    [SerializeField] private GameObject controlsUIPrefab;
    
    
    
    private PaddleController paddleController;
    private BoostController boostController;
    private HookController hookController;
    private CollectController collectController;
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

    public GameObject kayakLow;
    private MeshRenderer kayakMeshRenderer;
    private SkinnedMeshRenderer player1MeshRenderer;
    private SkinnedMeshRenderer player2MeshRenderer;
    private InventoryMenu inventory;

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
        collectController = GetComponent<CollectController>();
        floaters = GetComponentsInChildren<Floater>();
        inventory = FindObjectOfType<InventoryMenu>();

        kayakMeshRenderer = kayakLow.transform.GetChild(0).GetComponent<MeshRenderer>();
        player1MeshRenderer = kayakLow.transform.Find("CharacterLeft/Mesh").GetComponent<SkinnedMeshRenderer>();
        player2MeshRenderer = kayakLow.transform.Find("CharacterRight/Mesh").GetComponent<SkinnedMeshRenderer>();
        if (inventory != null && inventory.kayakMat != null)
        {
            kayakMeshRenderer.material = inventory.kayakMat;
            player1MeshRenderer.material = inventory.playerMat;
            player2MeshRenderer.material = inventory.playerMat;
        }
        
        waterController = FindObjectOfType<WaterController>();
        
        if (transform.Find("Mesh") != null) {
            meshObject = transform.Find("Mesh").gameObject;
        }

        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        paddleController.Initialize(this);    
        boostController.Initialize(this);
        hookController.Initialize(this);
        collectController.Initialize(this);
        foreach(var f in floaters) {
            f.Initialize(this);
        }

        controls = FindObjectOfType<ButtonControls>(true);
        if (controls != null) {
            RegisterControls(controls);
            if (SceneManager.GetActiveScene().buildIndex == 1) {
                controls.LeftPlayerHook.gameObject.SetActive(false);
                controls.RightPlayerHook.gameObject.SetActive(false);
            } else {
                controls.LeftPlayerHook.gameObject.SetActive(true);
                controls.RightPlayerHook.gameObject.SetActive(true);
            }
        }
        else
        {
            Canvas c = FindObjectOfType<Canvas>();
            var ui = Instantiate(controlsUIPrefab, c.transform);

            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();            

            ui.transform.Find("LeftPlayer").GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            ui.transform.Find("RightPlayer").GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            
            controls = FindObjectOfType<ButtonControls>(true);
            RegisterControls(controls);            
        }
    }

    void Update() {
        if (GameManager.instance == null || GameManager.instance.state == GameManager.gameState.racingState)
        {
            var dt = Time.deltaTime;
            if (paddleController.enabled) paddleController.OnUpdate(dt);
            if (boostController.enabled) boostController.OnUpdate(dt);
            if (hookController.enabled) hookController.OnUpdate(dt);
            if (collectController.enabled) collectController.OnUpdate(dt);
            foreach (var f in floaters)
            {
                f.OnUpdate(dt);
            }
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
        
        if (rb.velocity.magnitude >= 1 && IsGrounded)
        {
            if (!foamEffect.isPlaying)
            {
                foamEffect.Play();
            }
        }
        else if(!foamEffect.isStopped)
        {
            foamEffect.Stop();
        }
    }

    void FixedUpdate() {
        if (GameManager.instance == null || GameManager.instance.state == GameManager.gameState.racingState)
        {
            var dt = Time.fixedDeltaTime;
            if (paddleController.enabled) paddleController.OnFixedUpdate(dt);
            if (boostController.enabled) boostController.OnFixedUpdate(dt);
            if (hookController.enabled) hookController.OnFixedUpdate(dt);
            if (collectController.enabled) collectController.OnFixedUpdate(dt);
            foreach(var f in floaters) {
                f.OnFixedUpdate(dt);
            }               
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

    public void AddForce(Vector3 direction, float strength, float dt, ForceMode forceMode = ForceMode.Force, float maxHVel = -1, float maxVVel = -1) {
        var horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        var verticalVel = new Vector3(0, rb.velocity.y, 0);
        
        // Debug.Log("horizontal: " + horizontalVel.magnitude + ", " + maxHorizontalVelocity);
        // Debug.Log("vertical: " + verticalVel.magnitude + ", " + maxVerticalVelocity);
        if (maxHVel < 0) maxHVel = maxHorizontalVelocity;
        if (maxVVel < 0) maxVVel = maxVerticalVelocity;
        if (horizontalVel.magnitude < maxHVel) {
            var horizontalDir = new Vector3(direction.x, 0, direction.z);
            if (hitDirection.magnitude > 0) {
                horizontalDir += hitDirection.normalized * wallNudgeMultiplier;                
            }
            rb.AddForce(horizontalDir.normalized * strength * dt, forceMode);
        }

        if (verticalVel.magnitude < maxVVel) {
            var verticalDir = new Vector3(0, direction.y, 0);
            rb.AddForce(verticalDir.normalized * strength * dt, forceMode);            
        }
    }

    public void AddTorque(Vector3 torque, float dt, ForceMode forceMode = ForceMode.Force) {
        if (torque.magnitude > 0 && rb.angularVelocity.magnitude < maxLateralTorque) {
            rb.AddTorque(torque * dt, forceMode);
        }
    }

    public void AddForceAtPosition(Vector3 direction, float strength, Vector3 position, ForceMode forceMode = ForceMode.Force) {
        var horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.y);
        if (horizontalVel.magnitude < maxHorizontalVelocity) {
            rb.AddForceAtPosition(direction * strength, position, forceMode);
        }

        var verticalVel = new Vector3(0, rb.velocity.y, 0);
        if (verticalVel.magnitude < maxVerticalVelocity) {
            rb.AddForceAtPosition(direction * strength, position, forceMode);            
        }
    }   

    private void RegisterControls(ButtonControls controls) {
        controls.LeftPlayerHook.PointerDown.RemoveAllListeners();
        controls.LeftPlayerHook.PointerUp.RemoveAllListeners();
        controls.RightPlayerHook.PointerDown.RemoveAllListeners();
        controls.RightPlayerHook.PointerUp.RemoveAllListeners();

        controls.LeftPlayerLeftPaddle.PointerDown.RemoveAllListeners();
        controls.LeftPlayerRightPaddle.PointerDown.RemoveAllListeners();
        controls.RightPlayerLeftPaddle.PointerDown.RemoveAllListeners();
        controls.RightPlayerRightPaddle.PointerDown.RemoveAllListeners();

        controls.LeftPlayerHook.PointerDown.AddListener(() => { controls.RightPlayerHook.animator.SetBool("isFlashing", true); hookController.OnLeftHookDown(); });
        controls.LeftPlayerHook.PointerUp.AddListener(() => { controls.RightPlayerHook.animator.SetBool("isFlashing", false); hookController.OnLeftHookUp(); });
        controls.RightPlayerHook.PointerDown.AddListener(() => { controls.LeftPlayerHook.animator.SetBool("isFlashing", true); hookController.OnRightHookDown(); });
        controls.RightPlayerHook.PointerUp.AddListener(() => { controls.LeftPlayerHook.animator.SetBool("isFlashing", false); hookController.OnRightHookUp(); });

        controls.LeftPlayerLeftPaddle.PointerDown.AddListener(() =>{ paddleController.leftCharacterPaddleAnimator?.SetTrigger("PaddleLeft"); paddleController.paddleState = PaddleController.EPaddleState.LEFT_PADDLE; paddleController.leftPaddleTimerInSeconds = 0; });
        controls.LeftPlayerRightPaddle.PointerDown.AddListener(() =>{ paddleController.leftCharacterPaddleAnimator?.SetTrigger("PaddleRight"); paddleController.paddleState = PaddleController.EPaddleState.RIGHT_PADDLE; paddleController.rightPaddleTimerInSeconds = 0; });
        controls.RightPlayerLeftPaddle.PointerDown.AddListener(() =>{ paddleController.rightCharacterPaddleAnimator?.SetTrigger("PaddleLeft"); paddleController.paddleState = PaddleController.EPaddleState.LEFT_PADDLE; paddleController.leftPaddleTimerInSeconds = 0; });
        controls.RightPlayerRightPaddle.PointerDown.AddListener(() =>{ paddleController.rightCharacterPaddleAnimator?.SetTrigger("PaddleRight"); paddleController.paddleState = PaddleController.EPaddleState.RIGHT_PADDLE; paddleController.rightPaddleTimerInSeconds = 0; });
        
        #if USE_TAP_AND_HOLD
        controls.LeftPlayerLeftPaddle.PointerUp.AddListener(() =>{ paddleController.leftPaddleActive = false; });
        controls.LeftPlayerRightPaddle.PointerUp.AddListener(() =>{ paddleController.rightPaddleActive = false; });
        controls.RightPlayerLeftPaddle.PointerUp.AddListener(() =>{ paddleController.leftPaddleActive = false; });
        controls.RightPlayerRightPaddle.PointerUp.AddListener(() =>{ paddleController.rightPaddleActive = false; });
        #endif
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

    #if DISPLAY_DEBUG
    bool displayOptions = false;
    void OnGUI()
    {
        if (displayOptions) {
            if (GUILayout.Button("Close", GUILayout.Width(400), GUILayout.Height(50))) {
                displayOptions = false;
            }

            if (GUILayout.Button("Restart", GUILayout.Width(400), GUILayout.Height(50))) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
            using (new GUILayout.HorizontalScope()) {
                GUILayout.TextArea("Paddle Strength:", GUILayout.Width(400), GUILayout.Height(50));
                GUILayout.TextArea(paddleController.paddleStrength.ToString(), GUILayout.Height(50));
            }
            paddleController.paddleStrength = GUILayout.HorizontalSlider(paddleController.paddleStrength, 0, 2000, GUILayout.Width(600), GUILayout.Height(50));
            
            using (new GUILayout.HorizontalScope()) {
                GUILayout.TextArea("Paddle Rotation:", GUILayout.Width(400), GUILayout.Height(50));
                GUILayout.TextArea(paddleController.rotationStrength.ToString(), GUILayout.Height(50));
            }
            var rotation = GUILayout.HorizontalSlider(paddleController.rotationStrength, -0, -2000, GUILayout.Width(600), GUILayout.Height(50));
            if (Mathf.Abs(rotation - paddleController.rotationStrength) > 0.01f) {
                paddleController.rotationStrength = rotation;
                paddleController.currentRotationStrength = rotation;
            }
            using (new GUILayout.HorizontalScope()) {
                GUILayout.TextArea("Hook Strength:", GUILayout.Width(400), GUILayout.Height(50));
                GUILayout.TextArea(hookController.hookStrength.ToString(), GUILayout.Height(50));
            }
            hookController.hookStrength = GUILayout.HorizontalSlider(hookController.hookStrength, 0, 2000, GUILayout.Width(600), GUILayout.Height(50));
            
            using (new GUILayout.HorizontalScope()) {
                GUILayout.TextArea("Max Horizontal Velocity:", GUILayout.Width(400), GUILayout.Height(50));
                GUILayout.TextArea(maxHorizontalVelocity.ToString(), GUILayout.Height(50));
            }
            maxHorizontalVelocity = GUILayout.HorizontalSlider(maxHorizontalVelocity, 0, 20, GUILayout.Width(600), GUILayout.Height(50));
            
            using (new GUILayout.HorizontalScope()) {
                GUILayout.TextArea("Max Vertical Velocity:", GUILayout.Width(400), GUILayout.Height(50));
                GUILayout.TextArea(maxVerticalVelocity.ToString(), GUILayout.Height(50));
            }
            maxVerticalVelocity = GUILayout.HorizontalSlider(maxVerticalVelocity, 0, 20, GUILayout.Width(600), GUILayout.Height(50));
            
            using (new GUILayout.HorizontalScope()) {
                GUILayout.TextArea("Max Lateral Torque:", GUILayout.Width(400), GUILayout.Height(50));
                GUILayout.TextArea(maxLateralTorque.ToString(), GUILayout.Height(50));
            }
            maxLateralTorque = GUILayout.HorizontalSlider(maxLateralTorque, 0, 20, GUILayout.Width(600), GUILayout.Height(50));

            using (new GUILayout.HorizontalScope()) {
                GUILayout.TextArea("Wall nudge multiplier:", GUILayout.Width(400), GUILayout.Height(50));
                GUILayout.TextArea(wallNudgeMultiplier.ToString(), GUILayout.Height(50));
            }
            wallNudgeMultiplier = GUILayout.HorizontalSlider(wallNudgeMultiplier, 0, 50, GUILayout.Width(600), GUILayout.Height(50));

        } else {
            if (GUILayout.Button("Display Debug Menu", GUILayout.Width(400), GUILayout.Height(50))) {
                displayOptions = true;
            }
        }
    }
    #endif
}
