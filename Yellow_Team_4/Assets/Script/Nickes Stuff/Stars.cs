using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Stars : MonoBehaviour
{
    public Image[] stars;
    [SerializeField] Sprite fullStar;
    [SerializeField] private FinishGoal goalScript;
    private void Update()
    {
        for (int i = 0; i < goalScript.starsEarned; i++)
        {
            stars[i].sprite = fullStar;
        }
    }
}
