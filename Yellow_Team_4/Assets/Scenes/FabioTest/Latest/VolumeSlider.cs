using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType
    {
        MASTER,
        MUSIC,
        AMBIENCE,
        SFX
    }

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;

    private void Awake()
    {
        volumeSlider = this.GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = FAudioMan.instance.masterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = FAudioMan.instance.musicVolume;
                break;
            case VolumeType.AMBIENCE:
                volumeSlider.value = FAudioMan.instance.ambienceVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = FAudioMan.instance.SFXVolume;
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType);
                break;
        }
    }

    public void OnSliderValueChanged()
    {
        if (volumeSlider == null) volumeSlider = this.GetComponentInChildren<Slider>();
        int vs = (int) Mathf.Lerp(0, 100, volumeSlider.value);
        switch (volumeType)
        {
            case VolumeType.MASTER:
                FAudioMan.instance.masterVolume = volumeSlider.value;
                MainMenu.SetMasterVolume(volumeSlider.value);
                if (volumeText != null) volumeText.text = vs.ToString() + "%";
                break;
            case VolumeType.MUSIC:
                FAudioMan.instance.musicVolume = volumeSlider.value;
                MainMenu.SetMusicVolume(volumeSlider.value);
                if (volumeText != null) volumeText.text = vs.ToString() + "%";
                break;
            case VolumeType.AMBIENCE:
                FAudioMan.instance.ambienceVolume = volumeSlider.value;
                if (volumeText != null) volumeText.text = vs.ToString() + "%";
                break;
            case VolumeType.SFX:
                FAudioMan.instance.SFXVolume = volumeSlider.value;
                MainMenu.SetSFXVolume(volumeSlider.value);
                volumeText.text = vs.ToString() + "%";
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType);
                break;
        }
    }
}
