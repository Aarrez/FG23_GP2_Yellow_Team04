using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private LineRenderer leftLr;
    private LineRenderer rightLr;
    private Vector3 leftGrapplePoint;
    private Vector3 rightGrapplePoint;
    [Header("General Settings")]
    public LayerMask whatIsGrappleable;
    public GameObject leftCannonTip, rightCannonTip;
    public Transform /*leftCannonTip, rightCannonTip,*/ player;
    public float maxDistance = 100f;
    private SpringJoint joint;

    [Header("Joint Settings")]
    [SerializeField] float jointSpring = 10f;
    [SerializeField] float jointDamper = 5f;
    [SerializeField] float jointMassScale = 1f;

    private void Awake()
    {
        leftLr = leftCannonTip.GetComponent<LineRenderer>();
        rightLr = rightCannonTip.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartLeftGrapple();
        }
        if (Input.GetMouseButtonDown(1))
        {
            StartRightGrapple();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopLeftGrapple();
        }
        if (Input.GetMouseButtonUp(1))
        {
            StopRightGrapple();
        }


        
    }

    private void LateUpdate()
    {
        DrawLeftRope();
        DrawRightRope();
    }

    void StartLeftGrapple()
    {
        RaycastHit leftHit;
        if (Physics.Raycast(origin: leftCannonTip.transform.position, direction: leftCannonTip.transform.right, out leftHit, maxDistance));
        leftGrapplePoint = leftHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = leftGrapplePoint;

        float distanceFromPoint = Vector3.Distance(a: player.position, b: leftGrapplePoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = jointSpring;
        joint.damper = jointDamper;
        joint.massScale = jointMassScale;

        leftLr.positionCount = 2;

    }void StartRightGrapple()
    {
        RaycastHit rightHit;
        if (Physics.Raycast(origin: rightCannonTip.transform.position, direction: rightCannonTip.transform.right, out rightHit, maxDistance));
        rightGrapplePoint = rightHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = rightGrapplePoint;

        float distanceFromPoint = Vector3.Distance(a: player.position, b: rightGrapplePoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = jointSpring;
        joint.damper = jointDamper;
        joint.massScale = jointMassScale;

        rightLr.positionCount = 2;
    }

    void DrawLeftRope()
    {
        if (!joint) 
            return;
        leftLr.SetPosition(index: 0, leftCannonTip.transform.position);
        leftLr.SetPosition(index: 1, leftGrapplePoint);
    }
    void DrawRightRope()
    {
        if (!joint) 
            return;
        rightLr.SetPosition(index: 0, rightCannonTip.transform.position);
        rightLr.SetPosition(index: 1, rightGrapplePoint);
    }

    void StopLeftGrapple()
    {
        leftLr.positionCount = 0;
        Destroy(joint);
    }
    void StopRightGrapple()
    {
        rightLr.positionCount = 0;
        Destroy(joint);
    }

}
