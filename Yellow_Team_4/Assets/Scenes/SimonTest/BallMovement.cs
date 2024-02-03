using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataandStats;

public class BallMovement : MonoBehaviour
{
    private BoostController boostController;

    void Start()
    {
        boostController = GetComponent<BoostController>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        // Check if boosting and adjust speed accordingly
        float currentSpeed;

        if (boostController.IsActive())
        {
           
            currentSpeed = boostController.GetPlayerStats().maxSpeed + boostController.GetPlayerStats().boostForce;
        }
        else
        {
            currentSpeed = boostController.GetPlayerStats().maxSpeed;
        }

        GetComponent<Rigidbody>().velocity= (movement * currentSpeed);
    } 
}
