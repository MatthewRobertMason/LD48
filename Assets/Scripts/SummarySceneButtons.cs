using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummarySceneButtons : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void ResetGame()
    {
        gameManager.ResetGame();
    }

    public void ContinueGame()
    {
        gameManager.ContinueGame();
    }

}
