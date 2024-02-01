using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushBackObstacle : MonoBehaviour, IForceTrigger
{    
    [Range(-1, 1)][SerializeField] private float forceCone = -1;
    [SerializeField] private ForceData forceData;    

    public ForceData ForceData {
        get { return forceData; }
    }

    public float ForceCone {
        get { return forceCone; }
    }

    public GameObject GObject { get => gameObject; }
    public Vector3 ForcePoint { get; set; }
    public Vector3 ForceDirection { get; set; }
    
    private BoxCollider boxCollider;
    private CapsuleCollider capCollider;
    private SphereCollider sphereCollider; 
    private MeshCollider meshCollider;

    private void Awake() {
        boxCollider = GetComponentInChildren<BoxCollider>();
        capCollider = GetComponentInChildren<CapsuleCollider>();
        sphereCollider = GetComponentInChildren<SphereCollider>();
        meshCollider = GetComponentInChildren<MeshCollider>();

        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }   

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = Color.blue;       

        var maxLeft = Vector3.Lerp(-transform.right, -transform.forward, forceCone);
        var maxRight = Vector3.Lerp(transform.right, -transform.forward, forceCone);
        
        Gizmos.color = Color.blue;
        
        if (sphereCollider != null) {
            if (forceCone > 0) {
                Gizmos.DrawLine(transform.position, transform.position + maxLeft.normalized * sphereCollider.radius);
                Gizmos.DrawLine(transform.position, transform.position + maxRight.normalized * sphereCollider.radius); 
            } else {
                Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
            }
        }
        if (boxCollider != null) {
            Gizmos.DrawWireCube(transform.position, boxCollider.size);
        }
        if (capCollider != null) {
            if (forceCone > 0) {
                Gizmos.DrawLine(transform.position, transform.position + maxLeft.normalized * capCollider.radius);
                Gizmos.DrawLine(transform.position, transform.position + maxRight.normalized * capCollider.radius); 
            } else {
                Gizmos.DrawWireSphere(transform.position, capCollider.radius);
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ForcePoint, 0.2f);
        Gizmos.DrawLine(ForcePoint, ForcePoint + (ForceDirection * 2));
    }
    #endif
}
