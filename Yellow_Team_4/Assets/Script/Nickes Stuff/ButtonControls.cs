using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControls : MonoBehaviour
{
    public Button leftPlayerPaddle;
    public Button leftPlayerHook;
    public Button leftPlayerDuck;

    public Button rightPlayerPaddle;
    public Button rightPlayerHook;
    public Button rightPlayerDuck;

    public Button LeftPlayerPaddle {
        get { return leftPlayerPaddle; }
    }
    public Button LeftPlayerHook {
        get { return leftPlayerHook; }
    }
    public Button LeftPlayerDuck {
        get { return leftPlayerDuck; }
    }

    public Button RightPlayerPaddle {
        get { return rightPlayerPaddle; }
    }
    public Button RightPlayerHook {
        get { return rightPlayerHook; }
    }
    public Button RightPlayerDuck {
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
