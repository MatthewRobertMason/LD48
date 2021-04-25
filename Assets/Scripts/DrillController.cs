using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using Assets.Scripts.Enums;

enum ResearchType{
    View,
    Efficiency,
    Speed,
    COUNT
};

public class DrillController : MonoBehaviour
{
    public Vector2Int position;
    public float visionRadius = 3.25f;
    private int length = 0;
    public int RemainingPipe = 70;
    public int PipePerIron = 5;
    public int ResearchCost = 3;

    public Sprite tile_right_drill;
    public Sprite tile_left_drill;
    public Sprite tile_up_drill;
    public Sprite tile_down_drill;

    private ResearchType active_research;
    private ResourceType research_resource;
    private int research_cost_remaining;
    private UnityEngine.UI.Text ResearchDescriptionText;
    private UnityEngine.UI.Text ResearchCostText;
    private UnityEngine.UI.Text ResearchResourceText;

    private bool lockMovement = false;
    public bool LockMovement
    {
        get => lockMovement;
        set => lockMovement = value;
    }

    private int forcedMovement = 0;
    public int ForcedMovement
    {
        get => forcedMovement;
        set => forcedMovement = value;
    }

    private SpriteRenderer sprite;

    private Vector2Int Position
    {
        get => position;
    }

    private Vector2Int facingDirection;
    private Vector2Int previousMove = new Vector2Int(0, 0);

    public Vector2 FacingDirection
    {
        get => facingDirection;
    }

    private GameManager gameManager;
    private LevelManager levelManager;
    private CameraFollow cameraFollow;

    public float timePerAction = 1.0f;
    public float timePerActionForced = 0.25f;
    private float timePassed = 0.0f;

    public void Awake()
    {
        facingDirection = Vector2Int.down;
    }

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
        cameraFollow = FindObjectOfType<CameraFollow>();

        sprite = GetComponent<SpriteRenderer>();
        levelManager.pipeDisplay.text = RemainingPipe.ToString();
        facingDirection = Vector2Int.down;
        levelManager.SetPipe(position.x, position.y, 0);
        length++;
        ResearchDescriptionText = GameObject.Find("ResearchDescription").GetComponent<UnityEngine.UI.Text>();
        ResearchCostText = GameObject.Find("ResearchCost").GetComponent<UnityEngine.UI.Text>();
        ResearchResourceText = GameObject.Find("ResearchResource").GetComponent<UnityEngine.UI.Text>();
        StartResearch();
    }

    public void FixedUpdate()
    {
        float maxTime = timePerAction;
        if (forcedMovement > 0)
        {
            maxTime = timePerActionForced;
        }

        timePassed += Time.fixedDeltaTime;

        if (timePassed > maxTime)
        {
            timePassed -= maxTime;
            MoveCharacter();

            if (forcedMovement > 0)
            {
                forcedMovement--;
                if (forcedMovement == 0)
                {
                    lockMovement = false;
                }
            }
        }
    }

    public void GameOver(){
        SceneManager.LoadScene("SummaryScene");
    }

    private bool AccumulateResource(ResourceType type){
        Debug.Log($"Resource get {type}");
        if(type == ResourceType.Pipe){
            GameOver();
            return false;
        } else if(type == ResourceType.Iron){
            RemainingPipe += PipePerIron;
        }
        AdvanceResearch(type);

        gameManager.AccumulateResourceScore(type);
        return true;
    }

    private void MoveCharacter()
    {
        if (facingDirection != Vector2.zero)
        {
            position += facingDirection;
            if ((levelManager.LevelMap.OutOfBounds(position.x, -position.y)) || (cameraFollow.OutOfBounds(position.x, position.y)))
            {
                GameOver();
                return;
            }

            if (!AccumulateResource(levelManager.CollectResource(position.x, position.y)))
            {
                return;
            }

            RemainingPipe--;
            levelManager.pipeDisplay.text = RemainingPipe.ToString();
            if(RemainingPipe < 0){
                GameOver();
                return;
            }

            gameManager.UpdateDepth(-position.y);
            levelManager.SetPipe(position.x, position.y, length);
            previousMove = facingDirection;
            length++;
            if (previousMove.x == 1)
            {
                this.sprite.sprite = tile_right_drill;
            }
            else if (previousMove.x == -1)
            {
                this.sprite.sprite = tile_left_drill;
            }
            else if (previousMove.y == 1)
            {
                this.sprite.sprite = tile_up_drill;
            }
            else if (previousMove.y == -1)
            {
                this.sprite.sprite = tile_down_drill;
            }
        }

        this.transform.position = new Vector3(position.x, position.y, 0.0f);
        levelManager.ClearFogOfWar(position.x, position.y, visionRadius);
    }

    private void OnMove(InputValue input)
    {
        if (!lockMovement)
        {
            Vector2 vec = input.Get<Vector2>();
            Vector2Int moveVec = Vector2Int.zero;

            if (vec.x != 0.0f)
            {
                moveVec.x = (vec.x < 0) ? -1 : 1;
            }

            if (vec.y != 0.0f)
            {
                moveVec.y = (vec.y < 0) ? -1 : 1;
            }

            if (moveVec.x != 0 && moveVec.y != 0)
            {
                moveVec.x = 0;
            }

            if (moveVec != Vector2.zero)
            {
                if (moveVec != -facingDirection && facingDirection != -previousMove)
                {
                    facingDirection = moveVec;
                    timePassed = 0;
                    MoveCharacter();
                }
            }
        }
    }

    private void StartResearch(){
        research_cost_remaining = ResearchCost;
        active_research = (ResearchType)UnityEngine.Random.Range(0, (int)ResearchType.COUNT);
        switch(UnityEngine.Random.Range(0, 2)){
            case 0: 
                research_resource = ResourceType.Copper; 
                ResearchResourceText.text = "Copper";
                break;
            case 1: 
                research_resource = ResourceType.Gold; 
                ResearchResourceText.text = "Gold";
                break;
        }
        switch(active_research){
            case ResearchType.Efficiency:
                ResearchDescriptionText.text = "Improve pipe efficiency.";
                break;
            case ResearchType.Speed:
                ResearchDescriptionText.text = "Allow slower drilling.";
                break;
            case ResearchType.View:
                ResearchDescriptionText.text = "Improve sensor radius.";
                break;
        }
        ResearchCostText.text = $"{research_cost_remaining}/{ResearchCost}";
    }

    private void AdvanceResearch(ResourceType type){
        if(type == research_resource){
            research_cost_remaining--;
            ResearchCostText.text = $"{research_cost_remaining}/{ResearchCost}";
            if(research_cost_remaining <= 0){
                FinishResearch();
            }
        }
    }

    private void FinishResearch(){
        switch(active_research){
            case ResearchType.Efficiency:
                PipePerIron += 2;
                break;
            case ResearchType.Speed:
                timePerAction++;
                break;
            case ResearchType.View:
                visionRadius++;
                break;
        }
        ResearchCost++;
        StartResearch();
    }
}
