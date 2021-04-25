using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject characterPrefab;

    private GameObject character;

    private LevelManager levelManager;

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        levelManager.Initialize();

        Vector3Int pos = new Vector3Int(levelManager.levelWidth/2, -10, 0);
        character = Instantiate<GameObject>(characterPrefab, pos, Quaternion.Euler(Vector3.zero));
        character.GetComponent<DrillController>().position = new Vector2Int(pos.x, pos.y);

        levelManager.ClearFogOfWar(pos.x, pos.y, character.GetComponent<DrillController>().visionRadius);
    }
}
