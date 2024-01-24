using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCurrentTest : MonoBehaviour
{
    [SerializeField] private float speedBoost = 10;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Water Current")
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * speedBoost, ForceMode.Acceleration);
        }
    }
}
