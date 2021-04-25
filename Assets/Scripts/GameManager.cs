using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject characterPrefab;

    private GameObject character;

    private LevelManager levelManager;

    public static void ResetGame(){
        SceneManager.LoadScene("SampleScene");
    }

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(this);
            if(SceneManager.GetActiveScene().name == "SummaryScene"){
                EnterSummaryScene();
            } else {
                EnterGameScene();
            }
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void EnterSummaryScene(){
        Debug.Log("Summary scene starting");
    }

    private void EnterGameScene(){
        Debug.Log("Game scene starting");
        levelManager = FindObjectOfType<LevelManager>();
        levelManager.Initialize();

        Vector3Int pos = new Vector3Int(levelManager.levelWidth/2, -10, 0);
        character = Instantiate<GameObject>(characterPrefab, pos, Quaternion.Euler(Vector3.zero));
        character.GetComponent<DrillController>().position = new Vector2Int(pos.x, pos.y);

        levelManager.ClearFogOfWar(pos.x, pos.y, character.GetComponent<DrillController>().visionRadius);
    }

    public void Start()
    {
        Debug.Log("GameManager Start");
        levelManager = FindObjectOfType<LevelManager>();
        levelManager.Initialize();

        Vector3Int pos = new Vector3Int(levelManager.levelWidth/2, -10, 0);
        character = Instantiate<GameObject>(characterPrefab, pos, Quaternion.Euler(Vector3.zero));
        character.GetComponent<DrillController>().position = new Vector2Int(pos.x, pos.y);

        levelManager.ClearFogOfWar(pos.x, pos.y, character.GetComponent<DrillController>().visionRadius);
    }
}
