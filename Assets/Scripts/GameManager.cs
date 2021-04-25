using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Assets.Scripts.Enums;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject characterPrefab;

    private GameObject character;

    private LevelManager levelManager;

    private int goldCount = 0;
    private int ironCount = 0;
    private int copperCount = 0;

    public static void ResetGame(){
        SceneManager.LoadScene("SampleScene");
    }

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(this);
            if(SceneManager.GetActiveScene().name == "SummaryScene"){
                instance.EnterSummaryScene();
            } else {
                instance.EnterGameScene();
            }
        } else {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void EnterSummaryScene(){
        Debug.Log("Summary scene starting");
        GameObject.Find("CopperValue").GetComponent<UnityEngine.UI.Text>().text = copperCount.ToString();
        GameObject.Find("IronValue").GetComponent<UnityEngine.UI.Text>().text = ironCount.ToString();
        GameObject.Find("GoldValue").GetComponent<UnityEngine.UI.Text>().text = goldCount.ToString();
    }

    private void EnterGameScene(){
        Debug.Log("Game scene starting");
        goldCount = copperCount = ironCount = 0;

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

    
    public void AccumulateResourceScore(ResourceType type){
        switch(type){
            case ResourceType.Iron: ironCount++; break;
            case ResourceType.Copper: copperCount++; break;
            case ResourceType.Gold: goldCount++; break;
        }
    }

}
