using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip TitleTrack;
    public AudioClip SummarySceneTrack;

    public AudioClip[] tracks;

    public int track = 0;

    private AudioSource audioSource;
    private Camera existingCamera;

    public AudioSource SourceAudio
    {
        get => audioSource;
    }

    public void Awake()
    {
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        existingCamera = FindObjectOfType<Camera>();
    }

    public void NextTrack()
    {
        SourceAudio.clip = tracks[(track + 1) % tracks.Length];
        SourceAudio.Play();
    }

    public void PrevTrack()
    {
        SourceAudio.clip = tracks[(track - 1) % tracks.Length];
        SourceAudio.Play();
    }

    public void PlayTitleTrack()
    {
        SourceAudio.clip = TitleTrack;
        SourceAudio.Play();
    }

    public void PlaySummaryTrack()
    {
        SourceAudio.clip = SummarySceneTrack;
        SourceAudio.Play();
    }

    public void ReturnToTrack()
    {
        SourceAudio.clip = tracks[track];
        SourceAudio.Play();
    }

    // Update is called once per frame
    public void Update()
    {
        if (existingCamera == null)
        {
            existingCamera = FindObjectOfType<Camera>();
        }

        if (existingCamera != null)
        {
            this.transform.position = existingCamera.transform.position;
        }
    }
}
