using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpinPlatform : MonoBehaviour
{
    
    
    
    // Rotation
    
    [SerializeField] private float currentRotation;
    [SerializeField] private float rotationSpeed = 50f;
    private float targetRotation = 0f;
    private bool bIsSpinning = false;
    const float spinningFactor = 90f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        RotatePlatform();
        
    }

    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.D) && !bIsSpinning)
        {
            targetRotation += spinningFactor;
            bIsSpinning = true;

        }
        else if (Input.GetKeyDown(KeyCode.A) && !bIsSpinning)
        {
            targetRotation -= spinningFactor;
            bIsSpinning = true;
        }
    }

    private void RotatePlatform()
    {
        
     
            
        if (bIsSpinning)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetRotation, 0), step);

        }

        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, targetRotation, 0)) < 0.1f)
        {
            transform.rotation = Quaternion.Euler(0, targetRotation, 0);
            bIsSpinning = false;
        }
    }
}
