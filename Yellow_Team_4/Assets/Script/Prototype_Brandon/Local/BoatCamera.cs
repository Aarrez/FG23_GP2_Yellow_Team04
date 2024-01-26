using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatCamera : MonoBehaviour, IOnStartTouch
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private float followDistance = 3;
    [SerializeField] private float lagTimeInSeconds = 3;

    private float currentMoveTime = 0;
    private Vector3 followDirection;

    private float targetForwardRotationAngle;
    private float currentForwardRotationAngle;    

    Vector3 targetPosition;
    Vector3 startPosition;

    Vector3 horizontalVelocity; 

    Boat boat;

    private void Awake() {
        followDirection = (transform.position - followTarget.position).normalized;        
        startPosition = transform.position;
        boat = FindObjectOfType<Boat>();
    }

    private void Update() {
        var newTargetPosition = followTarget.position + (followDirection * followDistance);
        if ((newTargetPosition - targetPosition).magnitude > 0.1f) {
            startPosition = transform.position;
            targetPosition = newTargetPosition;
            currentMoveTime = 0;
        }

        currentForwardRotationAngle = Mathf.Lerp(currentForwardRotationAngle, targetForwardRotationAngle, Time.deltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, currentForwardRotationAngle));

        targetForwardRotationAngle = (targetForwardRotationAngle > 0.01f) ? targetForwardRotationAngle - Time.deltaTime * 5 : (targetForwardRotationAngle < 0.01f) ? targetForwardRotationAngle + Time.deltaTime * 5 : 0;        
        currentMoveTime = Mathf.Clamp(currentMoveTime + Time.deltaTime, 0, lagTimeInSeconds);
    }

    public void FixedUpdate() {        
        transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.InverseLerp(0, lagTimeInSeconds, currentMoveTime));  
    }

    void IOnStartTouch.InvokeLeftSideTouch(Vector2 screenSpacePosition)
    {
        // for now, check left and right
        targetForwardRotationAngle = Mathf.Clamp(targetForwardRotationAngle - 1, -5, 5);           
    }

    void IOnStartTouch.InvokeRightSideTouch(UnityEngine.Vector2 screenSpacePosition) 
    {
        targetForwardRotationAngle = Mathf.Clamp(targetForwardRotationAngle + 1, -5, 5);
    }    
    
    #if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(targetPosition, 0.25f);
    }
    #endif
}
