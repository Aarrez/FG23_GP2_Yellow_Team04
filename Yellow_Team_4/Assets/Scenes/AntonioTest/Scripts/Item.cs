using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private bool isUnlocked = false;
    
    public bool IsUnlocked()
    {
        return isUnlocked;
    }

    // Optional: Public method to set the isUnlocked value
    public void Unlock()
    {
        isUnlocked = true;
    }
}
