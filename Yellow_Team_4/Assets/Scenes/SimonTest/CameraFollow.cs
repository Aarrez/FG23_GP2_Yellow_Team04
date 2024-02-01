using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0f, 5f, -10f); 

    void LateUpdate()
    {
        if (target != null)
        {
            
            transform.position = target.position + offset;
        }
    }
}
