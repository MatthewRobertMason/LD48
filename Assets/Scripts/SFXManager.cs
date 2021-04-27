using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource MoveSource;
    public AudioSource ExplodeSource;
    public AudioSource StartSource;

    public float volMultiplier = 0.2f;

    public float Volume{
        get => MoveSource.volume;
        set {
            MoveSource.volume = value * volMultiplier;
            ExplodeSource.volume = value * volMultiplier;
            StartSource.volume = value * volMultiplier;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayMove(){
        MoveSource.Play();
    }

    public void PlayExplode(){
        ExplodeSource.Play();
    }

    public void PlayStart(){
        StartSource.Play();
    }
}
