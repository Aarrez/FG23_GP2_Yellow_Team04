using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControls : MonoBehaviour
{
    public ControlButton leftPlayerPaddle;
    public ControlButton leftPlayerHook;
    public ControlButton leftPlayerDuck;

    public ControlButton rightPlayerPaddle;
    public ControlButton rightPlayerHook;
    public ControlButton rightPlayerDuck;

    public ControlButton LeftPlayerPaddle {
        get { return leftPlayerPaddle; }
    }
    public ControlButton LeftPlayerHook {
        get { return leftPlayerHook; }
    }
    public ControlButton LeftPlayerDuck {
        get { return leftPlayerDuck; }
    }

    public ControlButton RightPlayerPaddle {
        get { return rightPlayerPaddle; }
    }
    public ControlButton RightPlayerHook {
        get { return rightPlayerHook; }
    }
    public ControlButton RightPlayerDuck {
        get { return rightPlayerDuck; }
    }    

    /*void Awake() {
        leftPlayerPaddle = transform.Find("LeftPlayer/Paddle").GetComponent<Button>();
        leftPlayerHook = transform.Find("LeftPlayer/Hook").GetComponent<Button>();
        leftPlayerDuck = transform.Find("LeftPlayer/Duck").GetComponent<Button>();

        rightPlayerPaddle = transform.Find("RightPlayer/Paddle").GetComponent<Button>();
        rightPlayerHook = transform.Find("RightPlayer/Hook").GetComponent<Button>();
        rightPlayerDuck = transform.Find("RightPlayer/Duck").GetComponent<Button>();        
    }
    */    
}
