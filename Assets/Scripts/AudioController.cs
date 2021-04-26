using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioManager audioManager;
    public SFXManager sfxManager;
    public UnityEngine.UI.Slider volumeSlider;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float volume;

    public float Volume
    {
        get => volume;
        set => volume = value;
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        sfxManager = FindObjectOfType<SFXManager>();
        volumeSlider.value = volume = sfxManager.Volume;
    }

    void Update()
    {
        if (audioManager != null)
        {
            audioManager.SourceAudio.volume = volume;
            sfxManager.Volume = volume;
        }
    }

    public void Pause()
    {
        if (audioManager != null)
        {
            audioManager.SourceAudio.Pause();
        }
    }

    public void Play()
    {
        if (audioManager != null)
        {
            audioManager.SourceAudio.Play();
        }
    }

    public void NextTrack()
    {
        if (audioManager != null)
        {
            audioManager.NextTrack();
        }
    }
    
    public void PrevTrack()
    {
        if (audioManager != null)
        {
            audioManager.NextTrack();
        }
    }
}
