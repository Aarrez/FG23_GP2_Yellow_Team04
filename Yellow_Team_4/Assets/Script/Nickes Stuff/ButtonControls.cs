using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControls : MonoBehaviour
{
    [SerializeField] private ControlButton leftPlayerLeftPaddle;
    [SerializeField] private ControlButton leftPlayerRightPaddle;
    [SerializeField] private ControlButton leftPlayerHook;
    [SerializeField] private ControlButton leftPlayerDuck;

    [SerializeField] private ControlButton rightPlayerLeftPaddle;
    [SerializeField] private ControlButton rightPlayerRightPaddle;
    [SerializeField] private ControlButton rightPlayerHook;
    [SerializeField] private ControlButton rightPlayerDuck;

    public ControlButton LeftPlayerLeftPaddle {
        get { return leftPlayerLeftPaddle; }
    }
    public ControlButton LeftPlayerRightPaddle {
        get { return leftPlayerRightPaddle; }
    }
    public ControlButton LeftPlayerHook {
        get { return leftPlayerHook; }
    }
    public ControlButton LeftPlayerDuck {
        get { return leftPlayerDuck; }
    }

    public ControlButton RightPlayerLeftPaddle {
        get { return rightPlayerLeftPaddle; }
    }
    public ControlButton RightPlayerRightPaddle {
        get { return rightPlayerRightPaddle; }
    }
    public ControlButton RightPlayerHook {
        get { return rightPlayerHook; }
    }
    public ControlButton RightPlayerDuck {
        get { return rightPlayerDuck; }
    }            
}
