using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DuckMechanic : MonoBehaviour
{
    private Animator playerAnim;
    
    void Awake()
    {
        playerAnim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        print(playerAnim.GetBool("bIsDodging"));
        
        if (playerAnim.GetBool("bIsDodging") != true)
        {
            Duck();
        }
        
    }

    private void Duck()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            playerAnim.SetBool("bIsDodging", true);
            StartCoroutine(waiter());
        }
        
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(1);
        playerAnim.SetBool("bIsDodging", false);
    }
}
