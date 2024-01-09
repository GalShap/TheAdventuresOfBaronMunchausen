using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class AudioManager : SingletonPersistent<AudioManager>
{
    
    [Header("Clips")]
    [SerializeField] private AudioClip iceBreakClip;

    [SerializeField] private AudioClip buttonPress;
    
    [Header("Horn Sounds")]
    [SerializeField] private AudioClip hornBlowClip;

    [SerializeField] private AudioClip hornCollectSound;

    [Header("Duck & Lake Sounds")]
    [SerializeField] private AudioClip duckSquishClip;

    [SerializeField] private AudioClip fallIntoLakeClip;
    
    [Header("Deer Sounds")]
    [SerializeField] private AudioClip YawnOne;
    
    [SerializeField] private AudioClip YawnTwo;
    
    [SerializeField] private AudioClip YawnThree;
    
    [SerializeField] private AudioClip DeerWakeUp;

    [SerializeField] private AudioClip DeerFootsteps;
    
    [Header("Horse Sounds")]
    [SerializeField] private AudioClip CallingHorse;
    
    [SerializeField] private AudioClip HorseSound2;
    
    [SerializeField] private AudioClip HorseSound1;
    
    [SerializeField] private AudioClip HorseBitCarrot;

    public AudioClip HorseFootsteps;
    
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource effectsAudioSource;
    
    [SerializeField] private AudioSource hornAudioSource;

    [SerializeField] private AudioSource musicAudioSource; 
    

    private const int First = 1;

    private const int Second = 2;

    private const int Third = 3;

    private const int Fourth = 4;

    private const float DefaultEffectsVolume = 0.3f;

    private void PlayEffectsAudioClip(AudioClip newClip)
    {
        effectsAudioSource.clip = newClip;
        effectsAudioSource.Play();
    }

    public void PlayIceBreaksSound()
    {
        PlayEffectsAudioClip(iceBreakClip);
    }

    public void PlayHornSound()
    {
        hornAudioSource.clip = hornBlowClip;
        hornAudioSource.Play();
    }

    public void PlayHornPickUpSound()
    {
        PlayEffectsAudioClip(hornCollectSound);
    }

    public void PlayDuckSquishSound()
    {
        effectsAudioSource.volume = DefaultEffectsVolume;
        effectsAudioSource.clip = duckSquishClip;
        effectsAudioSource.Play();
    }

    public void PlayDropIntoLakeSound()
    {   
        effectsAudioSource.volume = DefaultEffectsVolume / 2;
        effectsAudioSource.clip = fallIntoLakeClip;
        effectsAudioSource.Play();
    }

    public void PlayDeerYawnClip(int idx)
    {
        effectsAudioSource.volume = 1f;
        switch (idx)
        {
            case First:
              
                effectsAudioSource.clip = YawnOne;
                break;
            case Second:
                
                effectsAudioSource.clip = YawnTwo;
                break;
            case Third:
               
                effectsAudioSource.clip = YawnThree;
                break;
            case Fourth:
                effectsAudioSource.clip = DeerWakeUp;
                break;
        }
        
        effectsAudioSource.Play();
    }

    public void PlayDeerFootSteps()
    {
        effectsAudioSource.volume = 1f;
        effectsAudioSource.clip = DeerFootsteps;
        effectsAudioSource.loop = true;
        effectsAudioSource.Play();
    }

    
    public void StopDeerFootSteps()
    {
        effectsAudioSource.Stop();
        effectsAudioSource.loop = false;
    }

    public void PlayButtonPressSound()
    {
        effectsAudioSource.clip = buttonPress;
        effectsAudioSource.Play();
    }
    
    public void PlayHorseWalk()
    {
        effectsAudioSource.clip = HorseFootsteps;
        effectsAudioSource.Play();
    }

    public void PlayHorseSound1()
    {
        effectsAudioSource.clip = HorseSound1;
        effectsAudioSource.Play();
    }

    public void PlayHorseSound2()
    {
        effectsAudioSource.clip = HorseSound2;
        effectsAudioSource.Play();
    }

    public void PlayHorseBitCarrot()
    {
        effectsAudioSource.clip = HorseBitCarrot;
        effectsAudioSource.Play();
    }

    public void PlayPlayerCallHorse()
    {
        effectsAudioSource.clip = CallingHorse;
        effectsAudioSource.Play();
    }

    public void SetMusicVol(float vol)
    {
        musicAudioSource.volume = vol;
    }
    
    
    
    

}
