using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FAudioMan : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]

    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;
    [Range(0, 1)]

    public Bus masterBus;
    public Bus musicBus;
    public Bus ambienceBus;
    public Bus sfxBus;

    public static FAudioMan instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Found more than one audio managers, destroying duplicate...");
            Destroy(gameObject);
            return;
        }
        instance = this;
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX"); 
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

}
