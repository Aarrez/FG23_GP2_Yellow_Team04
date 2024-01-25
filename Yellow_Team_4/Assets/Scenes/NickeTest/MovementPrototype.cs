using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MovementPrototype : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject player;
    [SerializeField] private float force = 10;
    [SerializeField] private float idleForce = 5;
    private LayerMask groundMask;
    void Start()
    {
        Application.targetFrameRate = 120;
        StartCoroutine(DelayedFPSOptim());
        groundMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PaddleLeft()
    {
        rb.AddForce(player.transform.forward * force, ForceMode.Impulse);
        player.transform.Rotate(0,20,0, Space.Self);
    }

    public void PaddleRight()
    {
        rb.AddForce(player.transform.forward * force, ForceMode.Impulse);
        player.transform.Rotate(0,-20,0, Space.Self);
    }

    public void Restart()
    {
        SceneManager.LoadScene("NickeTestScene");
    }

    IEnumerator DelayedFPSOptim()
    {
        yield return new WaitForSeconds(1);
        Application.targetFrameRate = 120;
    }
}
