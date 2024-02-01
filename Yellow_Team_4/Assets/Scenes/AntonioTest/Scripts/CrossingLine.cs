using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossingLine : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenScene("MainMenu");
            print("Collided with Crossline");
        }
        else
        {
            print("Did not collide");
        }
        
    }

    private void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
