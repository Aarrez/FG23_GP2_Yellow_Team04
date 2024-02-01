using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnEnableSettings : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider masterSlider;
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("sfxVolume") || PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("musicVolume"))
        {
            try
            {
                sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
                masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
                musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }
}
