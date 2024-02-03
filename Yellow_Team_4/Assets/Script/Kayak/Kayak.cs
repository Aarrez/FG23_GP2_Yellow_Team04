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
    private PaddleController paddleController;
    private BoostController boostController;
    private HookController hookController;
    private ButtonControls controls;
    
    public Rigidbody rb;
    
    void Awake() {
        paddleController = GetComponent<PaddleController>();
        boostController = GetComponent<BoostController>();
        hookController = GetComponent<HookController>();
        
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        paddleController.Initialize(this);    
        boostController.Initialize(this);
        hookController.Initialize(this);

        controls = FindObjectOfType<ButtonControls>(true);
        controls.LeftPlayerHook.PointerDown.AddListener(hookController.OnLeftHookDown);
        controls.LeftPlayerHook.PointerUp.AddListener(hookController.OnLeftHookUp);
        controls.RightPlayerHook.PointerDown.AddListener(hookController.OnRightHookDown);
        controls.RightPlayerHook.PointerUp.AddListener(hookController.OnRightHookUp);

        controls.LeftPlayerLeftPaddle.PointerDown.AddListener(() =>{ paddleController.leftPaddleActive = true; });
        controls.LeftPlayerLeftPaddle.PointerUp.AddListener(() =>{ paddleController.leftPaddleActive = false; });

        controls.LeftPlayerRightPaddle.PointerDown.AddListener(() =>{ paddleController.rightPaddleActive = true; });
        controls.LeftPlayerRightPaddle.PointerUp.AddListener(() =>{ paddleController.rightPaddleActive = false; });

        controls.RightPlayerLeftPaddle.PointerDown.AddListener(() =>{ paddleController.leftPaddleActive = true; });
        controls.RightPlayerLeftPaddle.PointerUp.AddListener(() =>{ paddleController.leftPaddleActive = false; });

        controls.RightPlayerRightPaddle.PointerDown.AddListener(() =>{ paddleController.rightPaddleActive = true; });
        controls.RightPlayerRightPaddle.PointerUp.AddListener(() =>{ paddleController.rightPaddleActive = false; });
    }

    void Update() {
        var dt = Time.deltaTime;
        paddleController.OnUpdate(dt);
        boostController.OnUpdate(dt);
        hookController.OnUpdate(dt);
    }

    void FixedUpdate() {
        var dt = Time.fixedDeltaTime;
        paddleController.OnFixedUpdate(dt);
        boostController.OnFixedUpdate(dt);
        hookController.OnFixedUpdate(dt);    
    }
}
