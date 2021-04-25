using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Assets.Scripts.Enums;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject characterPrefab;
    public GameObject drillPrefab;
    public GameObject cameraPrefab;

    private GameObject player;
    private GameObject cameraObject;

    private LevelManager levelManager;
    private CameraFollow cameraFollow;

    private int goldCount = 0;
    private int ironCount = 0;
    private int copperCount = 0;
    private int diamondCount = 0;
    private int maxDepth = 0;
    public int DepthMultiplier = 50; 
    public int CopperPoints = 2;
    public int IronPoints = 1;
    public int GoldPoints = 3;
    public int DiamondPoints = 5;

    public static void ResetGame(){
        Destroy(instance.levelManager.gameObject);
        instance.levelManager = null;
        SceneManager.LoadScene("SampleScene");
    }

    public static void ContinueGame(){
        instance.levelManager.RecyclePipes();
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
        float depthModifier = (float)maxDepth/DepthMultiplier;
        float score = (CopperPoints * copperCount + IronPoints * ironCount + GoldPoints * goldCount + DiamondPoints * diamondCount) * depthModifier;

        GameObject.Find("CopperValue").GetComponent<UnityEngine.UI.Text>().text = copperCount.ToString();
        GameObject.Find("CopperScore").GetComponent<UnityEngine.UI.Text>().text = $"{CopperPoints * copperCount} points";
        GameObject.Find("IronValue").GetComponent<UnityEngine.UI.Text>().text = ironCount.ToString();
        GameObject.Find("IronScore").GetComponent<UnityEngine.UI.Text>().text = $"{IronPoints * ironCount} points";
        GameObject.Find("GoldValue").GetComponent<UnityEngine.UI.Text>().text = goldCount.ToString();
        GameObject.Find("GoldScore").GetComponent<UnityEngine.UI.Text>().text = $"{GoldPoints * goldCount} points";
        GameObject.Find("DiamondValue").GetComponent<UnityEngine.UI.Text>().text = diamondCount.ToString();
        GameObject.Find("DiamondScore").GetComponent<UnityEngine.UI.Text>().text = $"{DiamondPoints * diamondCount} points";
        GameObject.Find("DepthValue").GetComponent<UnityEngine.UI.Text>().text = maxDepth.ToString();
        GameObject.Find("DepthScore").GetComponent<UnityEngine.UI.Text>().text = string.Format("x {0:0.}%", 100*depthModifier);
        GameObject.Find("ScoreValue").GetComponent<UnityEngine.UI.Text>().text = string.Format("{0:0.}", score);
    }

    private void EnterGameScene(){
        Debug.Log("Game scene starting");
        goldCount = copperCount = ironCount = diamondCount = maxDepth = 0;

        cameraFollow = FindObjectOfType<CameraFollow>();
        if(!levelManager){
            levelManager = FindObjectOfType<LevelManager>();
            levelManager.Initialize();
        } else {
            levelManager.FindUI();
        }

        CreateCharacterAndDrill();
    }

    private void CreateCharacterAndDrill()
    {
        Vector3Int pos = new Vector3Int(levelManager.levelWidth/2, 0, 0);
        if(player == null)
            player = Instantiate<GameObject>(characterPrefab, pos, Quaternion.Euler(Vector3.zero));
        DrillController drillComponent = player.GetComponent<DrillController>();
        drillComponent.position = new Vector2Int(pos.x, pos.y);

        cameraObject = Instantiate<GameObject>(cameraPrefab, new Vector3(0.0f, 0.0f, -10.0f), Quaternion.Euler(Vector3.zero));
        cameraFollow = cameraObject.GetComponent<CameraFollow>();
        cameraFollow.Player = player;

        Vector3 machinePos = new Vector3(pos.x + 1, drillPrefab.transform.position.y, 0);
        GameObject drillMachineObject = Instantiate<GameObject>(drillPrefab, machinePos, Quaternion.Euler(Vector3.zero));

        drillComponent.LockMovement = true;
        drillComponent.ForcedMovement = 10;
    }

    public void Start()
    {
        Debug.Log("GameManager Start");
        levelManager = FindObjectOfType<LevelManager>();
        cameraFollow = FindObjectOfType<CameraFollow>();
        levelManager.Initialize();

        CreateCharacterAndDrill();
    }

    
    public void AccumulateResourceScore(ResourceType type){
        switch(type){
            case ResourceType.Iron: ironCount++; break;
            case ResourceType.Copper: copperCount++; break;
            case ResourceType.Gold: goldCount++; break;
            case ResourceType.Diamond: diamondCount++; break;
        }
    }

    public void UpdateDepth(int depth){
        maxDepth = Mathf.Max(depth, maxDepth);
    }

    public MapLevel GetMap(){
        return levelManager.LevelMap;
    }
}
