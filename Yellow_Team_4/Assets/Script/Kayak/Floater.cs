using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Floater : MonoBehaviour, IKayakEntity
{
    public LayerMask waterLayerMask;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;     

    private WaterController waterController;

    private float oldHeight;
    private float heightVelocity;

    Kayak kayak;  

    void OnTriggerEnter(Collider collisionInfo) {        
        var wc = collisionInfo.gameObject.GetComponent<WaterController>();
        if (wc != null) {
            waterController = wc;
            // kayak.IsGrounded = true;
        }
    }    

    private void FixedUpdate()
    {
        if (GetComponent<Kayak>() != null) {
            OnFixedUpdate(Time.fixedDeltaTime);            
        }
    }

    public void Initialize(Kayak entity)
    {
        if (entity != null) {
            kayak = entity;
        } else {
            Debug.LogError("Missing Kayak script. Please add a Kayak script onto object");
        }

        waterController = FindObjectOfType<WaterController>();
    }

    public void OnFixedUpdate(float dt)
    {
        // rigidBody.AddForceAtPosition(Physics.gravity / 4, transform.position, ForceMode.Acceleration);
        if (waterController != null) {            
            float waveHeight = waterController.WaveController.GetWaveHeight(transform.position.x);
            var newHeightVelocity = (waveHeight - oldHeight) / dt;
            // displacementMultiplier = Mathf.Lerp(waterController.transform.position.y, waveHeight, transform.position.y) * displacementAmount;                                    
            var multiplier = ((newHeightVelocity - heightVelocity) > 0) ? 1.05f : 1; 
            var deltaDirection = new Vector3(0f, (newHeightVelocity - heightVelocity), 0f).normalized;
            kayak.AddForceAtPosition(deltaDirection, deltaDirection.magnitude * multiplier, transform.position, ForceMode.VelocityChange);
            // Debug.Log("velocity: " + (newHeightVelocity - heightVelocity) + ", " + rigidBody.velocity);
            // rigidBody.AddForce(displacementMultiplier * -rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            // rigidBody.AddTorque(displacementMultiplier * -rigidBody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);            
            oldHeight = waveHeight;
            heightVelocity = newHeightVelocity;
        }
    }

    public void OnUpdate(float dt)
    {         
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.1f);

        Gizmos.color = Color.red;

        if (waterController != null) {
            var s = waterController.WaveController.GetWaveHeight(transform.position.x);
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, oldHeight, transform.position.z), 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, s, transform.position.z), 0.25f);
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, heightVelocity, 0));
        }
    }
    #endif
}
    


