using System;
using System.Collections;
using System.Collections.Generic;
using PresistentData;
using UnityEngine;
using TMPro;

public class RaceScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private GameObject[] stars = new GameObject[3];

    private PickupManager pickupScript;
    private TimeMeasurement timeScript;

    private bool pickupScriptInScene;

    private bool timeScriptInScene;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindingPickupMan();
        if (pickupScriptInScene)
        {
            coinTxt.text = ":"+pickupScript.pickupPool.Count.ToString();   
        }
    }

    void FindingPickupMan()
    {
        pickupScript = FindObjectOfType<PickupManager>();
        if (pickupScript == null)
        {
            pickupScriptInScene = false;
        }
        else
        {
            pickupScriptInScene = true;
        }
    }
/*
    void FindingTimeScript()
    {
        timeScript = FindObjectOfType<TimeMeasurement>();
        if (timeScript == null)
        {
            timeScriptInScene = false;
        }
        else
        {
            timeScriptInScene = true;
        }
    }

    void StarDisplay()
    {
        if (timeScriptInScene)
        {
            
        }  
    }
    */
}
