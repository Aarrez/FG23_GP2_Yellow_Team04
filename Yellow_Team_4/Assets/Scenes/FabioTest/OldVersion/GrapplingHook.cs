using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private SpringJoint leftJoint;
    private SpringJoint rightJoint;

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
        //if players use both hooks they somehow break
        //can't understand why this is happening
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

        //restart for testing
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        leftJoint = player.gameObject.AddComponent<SpringJoint>();
        leftJoint.autoConfigureConnectedAnchor = false;
        leftJoint.connectedAnchor = leftGrapplePoint;

        float distanceFromPoint = Vector3.Distance(a: player.position, b: leftGrapplePoint);
        leftJoint.maxDistance = distanceFromPoint * 0.8f;
        leftJoint.minDistance = distanceFromPoint * 0.25f;

        leftJoint.spring = jointSpring;
        leftJoint.damper = jointDamper;
        leftJoint.massScale = jointMassScale;

        leftLr.positionCount = 2;

    }void StartRightGrapple()
    {
        RaycastHit rightHit;
        if (Physics.Raycast(origin: rightCannonTip.transform.position, direction: rightCannonTip.transform.right, out rightHit, maxDistance));
        rightGrapplePoint = rightHit.point;
        rightJoint = player.gameObject.AddComponent<SpringJoint>();
        rightJoint.autoConfigureConnectedAnchor = false;
        rightJoint.connectedAnchor = rightGrapplePoint;

        float distanceFromPoint = Vector3.Distance(a: player.position, b: rightGrapplePoint);
        rightJoint.maxDistance = distanceFromPoint * 0.8f;
        rightJoint.minDistance = distanceFromPoint * 0.25f;

        rightJoint.spring = jointSpring;
        rightJoint.damper = jointDamper;
        rightJoint.massScale = jointMassScale;

        rightLr.positionCount = 2;
    }

    void DrawLeftRope()
    {
        if (!leftJoint) 
            return;
        leftLr.SetPosition(index: 0, leftCannonTip.transform.position);
        leftLr.SetPosition(index: 1, leftGrapplePoint);
    }
    void DrawRightRope()
    {
        if (!rightJoint) 
            return;
        rightLr.SetPosition(index: 0, rightCannonTip.transform.position);
        rightLr.SetPosition(index: 1, rightGrapplePoint);
    }

    void StopLeftGrapple()
    {
        leftLr.positionCount = 0;
        Destroy(leftJoint);
    }
    void StopRightGrapple()
    {
        rightLr.positionCount = 0;
        Destroy(rightJoint);
    }

}
