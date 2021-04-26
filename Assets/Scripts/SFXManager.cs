using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource MoveSource;
    public AudioSource ExplodeSource;

    public float Volume{
        get => MoveSource.volume;
        set {
            MoveSource.volume = value;
            ExplodeSource.volume = value;
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
}
