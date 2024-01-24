using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    [SerializeField] private int health = 100;

    [SerializeField] private int baseDmg = 30;

    [SerializeField] private float maxSpeedForCollision = 3;

    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private TextMeshProUGUI healthText;
    // Update is called once per frame
    void Update()
    {
        healthText.text = health + "/" + "100";
        if (health <= 0)
        {
            gameOverScreen.SetActive(true);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            health -= baseDmg;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Water Current")
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 10, ForceMode.Acceleration);
        }
    }
}
