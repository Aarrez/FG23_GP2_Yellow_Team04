using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public interface IOnStartTouch {
    public void InvokeLeftSideTouch(Vector2 screenSpacePosition);
    public void InvokeRightSideTouch(Vector2 screenSpacePosition);
}   

public interface IOnStickInput
{
    public void OnInvokeRightStick(Vector2 direction);
    public void OnInvokeLeftStick(Vector2 direction);

    public void OnInvokeLeftStickEnd();
    public void OnInvokeRightStickEnd();    
}

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool useTapControls;
    [SerializeField] private bool useWheelControls;
    [SerializeField] private bool useKeyboardControls = true;
    [SerializeField] private bool isInvertKeyboardControls = false;

    private TouchControls controls;
    private void Awake() {
        controls = new TouchControls();        
    }

    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }

    private void Start () {        
        controls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        controls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);

        controls.Touch.TouchPress002.started += ctx => StartTouch(ctx);
        controls.Touch.TouchPress002.canceled += ctx => EndTouch(ctx);        

        // controls.Keyboard.TouchPress.started += ctx => StartTouch(ctx);
        // controls.Keyboard.TouchPress.canceled += ctx => EndTouch(ctx);

        controls.Touch.LeftStick.performed += ctx => PerformLeftStick(ctx);
        controls.Touch.LeftStick.canceled += ctx => EndLeftStick(ctx);

        controls.Touch.RightStick.performed += ctx => PerformRightStick(ctx);
        controls.Touch.RightStick.canceled += ctx => EndRightStick(ctx);

        controls.Keyboard.LeftPress.performed += ctx => PerformLeftButtonPress(ctx);
        controls.Keyboard.RightPress.performed += ctx => PerformRightButtonPress(ctx);        
    }

    private void StartTouch(InputAction.CallbackContext context) {
        if (useTapControls) {
            var output = controls.Touch.TouchPosition.ReadValue<Vector2>();            

            var onTouch = FindObjectsOfType<MonoBehaviour>().OfType<IOnStartTouch>();
            if (output.x < Screen.width / 2)
            {
                foreach(var ot in onTouch) {
                    ot.InvokeLeftSideTouch(output);
                }
            }
            else
            {
                foreach(var ot in onTouch) {
                    ot.InvokeRightSideTouch(output);
                }
            }
        }
    }    
    
    private void EndTouch(InputAction.CallbackContext context) {
        // Debug.Log("Touch ended");
        if (useTapControls) {

        }
    }

    private void PerformLeftStick(InputAction.CallbackContext context)
    {
        if (useWheelControls) {
            var leftDirection = controls.Touch.LeftStick.ReadValue<Vector2>();
            
            var onStickInputs = FindObjectsOfType<MonoBehaviour>().OfType<IOnStickInput>();

            foreach (var osi in onStickInputs)
            {
                osi.OnInvokeLeftStick(leftDirection);
            }
        }
    }
    
    private void EndLeftStick(InputAction.CallbackContext context) {
        var onStickInputs = FindObjectsOfType<MonoBehaviour>().OfType<IOnStickInput>();
        if (useWheelControls) {
            foreach(var osi in onStickInputs) {
                osi.OnInvokeLeftStickEnd();
            }
        }
    }

    private void PerformRightStick(InputAction.CallbackContext context)
    {
        if (useWheelControls) {
            var rightDirection = controls.Touch.RightStick.ReadValue<Vector2>();
            
            var onStickInputs = FindObjectsOfType<MonoBehaviour>().OfType<IOnStickInput>();

            foreach (var osi in onStickInputs)
            {
                osi.OnInvokeRightStick(rightDirection);
            }
        }
    }

    private void EndRightStick(InputAction.CallbackContext context) {
        // Debug.Log("Touch ended");
        var onStickInputs = FindObjectsOfType<MonoBehaviour>().OfType<IOnStickInput>();
        if (useWheelControls) {
            foreach(var osi in onStickInputs) {
                osi.OnInvokeRightStickEnd();
            }
        }
    }

    private void PerformLeftButtonPress(InputAction.CallbackContext context) {
        if (useKeyboardControls) {
            var onTouch = FindObjectsOfType<MonoBehaviour>().OfType<IOnStartTouch>();
            foreach(var ot in onTouch) {
                if (!isInvertKeyboardControls) ot.InvokeLeftSideTouch(Vector3.zero);
                else ot.InvokeRightSideTouch(Vector3.zero);
            }
        }
    }

    private void PerformRightButtonPress(InputAction.CallbackContext context) {
        if (useKeyboardControls) {
            var onTouch = FindObjectsOfType<MonoBehaviour>().OfType<IOnStartTouch>();
            foreach(var ot in onTouch) {
                if (!isInvertKeyboardControls) ot.InvokeRightSideTouch(Vector3.zero);
                else ot.InvokeLeftSideTouch(Vector3.zero);
            }
        }
    }

    void OnGUI() {
        useTapControls = GUILayout.Toggle(useTapControls, "Use tap controls");
        useWheelControls = GUILayout.Toggle(useWheelControls, "Use wheel controls");
        using (new GUILayout.HorizontalScope()) {
            useKeyboardControls = GUILayout.Toggle(useKeyboardControls, "Use keyboard controls");
            isInvertKeyboardControls = GUILayout.Toggle(isInvertKeyboardControls, "Invert Keyboard tap controls");
        }
    }   
}
