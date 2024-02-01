using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using FMOD;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("SOUNDS ARRAY MUST BE THE TOTAL SIZE OF sfx + music")]
    public Sound[] sounds;
    [Header("Dont forget to name all sounds")]
    public Sound[] sfx;
    public Sound[] music;
    public AudioMixer masterMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup musicMixer;

    public static AudioManager instance;

    void Awake() {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        foreach(Sound s in sfx)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = sfxMixer;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        foreach(Sound s in music)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = musicMixer;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        sfx.CopyTo(sounds, 0);
        music.CopyTo(sounds, sfx.Length);
    }

    
    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }

        s.source.Play();
    }
    
}
