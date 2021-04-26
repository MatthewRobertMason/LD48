using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Assets.Scripts.Enums;

public class GameManager : MonoBehaviour
{
    public GameObject characterPrefab;
    public GameObject drillPrefab;
    public GameObject cameraPrefab;

    private GameObject player;
    private GameObject cameraObject;

    private LevelManager levelManager;
    private AudioManager audioManager;
    private CameraFollow cameraFollow;
    public SFXManager soundEffectManager;

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

    bool rerunStartFunction = false;

    public void ResetGame(){
        Destroy(levelManager.gameObject);
        SceneManager.LoadScene("SampleScene");
        rerunStartFunction = true;
    }

    public void ContinueGame(){
        
        levelManager.RecyclePipes();
        SceneManager.LoadScene("SampleScene");
        levelManager.gameObject.SetActive(true);
        rerunStartFunction = true;
    }

    public void FixedUpdate()
    {
        if (rerunStartFunction)
        {
            Start();
            rerunStartFunction = false;
        }
    }

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(this);
        }
    }

    public void Start()
    {
        soundEffectManager = GetComponent<SFXManager>();

        if (levelManager == null)
        {
            levelManager = FindObjectOfType<LevelManager>();
        }

        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
        
        if (SceneManager.GetActiveScene().name == "SummaryScene")
        {
            EnterSummaryScene();
        }
        else
        {
            EnterGameScene();
        }
    }

    public void GameOver()
    {
        levelManager.gameObject.SetActive(false);
        rerunStartFunction = true;
        SceneManager.LoadScene("SummaryScene");
    }

    private void EnterSummaryScene(){
        Debug.Log("Summary scene starting");
        audioManager.PlaySummaryTrack();
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
        audioManager.SourceAudio.Pause();

        goldCount = copperCount = ironCount = diamondCount = maxDepth = 0;

        if (levelManager.LevelMap == null)
        {
            levelManager.Initialize();
        }

        levelManager.FindUI();
        
        CreateCharacterAndDrill();
        soundEffectManager.PlayStart();
    }

    private void CreateCharacterAndDrill()
    {
        Vector3Int pos = new Vector3Int(levelManager.levelWidth/2, 0, 0);
        if(player == null)
            player = Instantiate<GameObject>(characterPrefab, pos, Quaternion.Euler(Vector3.zero));
        DrillController drillComponent = player.GetComponent<DrillController>();
        drillComponent.position = new Vector2Int(pos.x, pos.y);

        levelManager.DigTile(pos.x, pos.y);
        levelManager.RemoveGrass(pos.x, pos.y);

        cameraObject = Instantiate<GameObject>(cameraPrefab, new Vector3(0.0f, 0.0f, -10.0f), Quaternion.Euler(Vector3.zero));
        cameraFollow = cameraObject.GetComponent<CameraFollow>();
        cameraFollow.Player = player;

        Vector3 machinePos = new Vector3(pos.x + 1, drillPrefab.transform.position.y, 0);
        GameObject drillMachineObject = Instantiate<GameObject>(drillPrefab, machinePos, Quaternion.Euler(Vector3.zero), levelManager.transform);

        drillComponent.LockMovement = true;
        drillComponent.ForcedMovement = 10;
        drillComponent.PauseTime = soundEffectManager.StartSource.clip.length * 0.9f;
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
