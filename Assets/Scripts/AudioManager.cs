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
    private TextMarquee songName;
    public TextMarquee SongName
    {
        get => songName;
        set => songName = value;
    }

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
        track = (track + 1) % tracks.Length;
        SourceAudio.clip = tracks[track];
        SetSongName(SourceAudio.clip);
        SourceAudio.Play();
    }

    public void PrevTrack()
    {
        track = (track + tracks.Length - 1) % tracks.Length;
        SourceAudio.clip = tracks[track];
        SetSongName(SourceAudio.clip);
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
        SetSongName(SourceAudio.clip);
        SourceAudio.Play();
    }

    public void ReturnToTrack()
    {
        SourceAudio.clip = tracks[track];
        SetSongName(SourceAudio.clip);
        SourceAudio.Play();
    }

    public void SetSongName(AudioClip clip)
    {
        if (songName != null)
        {
            if (clip != null)
            {
                songName.SetUpText(clip.name);
            }
        }
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
