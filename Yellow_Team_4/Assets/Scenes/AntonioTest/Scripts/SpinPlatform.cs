using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpinPlatform : MonoBehaviour
{
    
    [SerializeField] private GameObject[] stands;
    private int currentStandIndex = 0;
    private int lastStandIndex = 0;
    
    //[SerializeField] private GameObject stand01;
    //[SerializeField] private GameObject stand02;
    //[SerializeField] private GameObject stand03;
    //[SerializeField] private GameObject stand04;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            print("Rotated Right");
            Rotate("right");
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            print("Rotated Left");
            Rotate("left");
        }
    }

    private void Rotate(string side)
    {
        if (side == "left")
        {
            
            transform.Rotate(0f, -90f, 0f, Space.Self);
            lastStandIndex = currentStandIndex;
            currentStandIndex++;
        }

        if (side == "right")
        {
            transform.Rotate(0f, 90f, 0f, Space.Self);
            lastStandIndex = currentStandIndex;
            currentStandIndex++;
        }
    }

    private void ModifyStand()
    {
        //private GameObject stand = stands[currentStandIndex];
    }
}
