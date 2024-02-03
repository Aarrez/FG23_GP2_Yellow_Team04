using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public Transform target; // The target (kayak) that the camera will follow
    public float distance = 10.0f; // The distance behind the target
    public float height = 5.0f; // The height above the target
    public float followDamping = 0.1f; // Damping for the smooth follow

    private Vector3 desiredPosition; // Calculated desired position of the camera

    private void LateUpdate()
    {
        // Calculate the desired position: behind the target and at a set height
        desiredPosition = target.position - target.forward * distance;
        desiredPosition.y = target.position.y + height;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followDamping);

        // Make sure the camera is always looking at the target
        transform.LookAt(target);
    }
}
