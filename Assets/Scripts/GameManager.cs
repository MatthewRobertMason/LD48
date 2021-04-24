using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private LevelManager levelManager;

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(this);
        }
    }

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        levelManager.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
