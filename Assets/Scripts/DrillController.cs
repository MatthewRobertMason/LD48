using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using Assets.Scripts.Enums;

public class DrillController : MonoBehaviour
{
    public Vector2Int position;
    public float visionRadius = 3.25f;
    private int length = 0;

    public Sprite tile_right_drill;
    public Sprite tile_left_drill;
    public Sprite tile_up_drill;
    public Sprite tile_down_drill;

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
        sprite = GetComponent<SpriteRenderer>();
        facingDirection = Vector2Int.down;
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
            timePassed -= timePerAction;
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
        }

        GameManager.instance.AccumulateResourceScore(type);
        return true;
    }

    private void MoveCharacter()
    {
        if (facingDirection != Vector2.zero)
        {
            position += facingDirection;
            if (levelManager.LevelMap.OutOfBounds(position.x, -position.y))
            {
                GameOver();
                return;
            }

            if (!AccumulateResource(levelManager.CollectResource(position.x, position.y)))
            {
                return;
            }

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
}
