using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrillController : MonoBehaviour
{
    public Vector2Int position;
    public float visionRadius = 3.25f;
    private int length = 0;

    private Vector2Int Position
    {
        get => position;
    }

    private Vector2Int facingDirection;

    public Vector2 FacingDirection
    {
        get => facingDirection;
    }

    private GameManager gameManager;
    private LevelManager levelManager;

    public float timePerAction = 1.0f;
    private float timePassed = 0.0f;

    public void Awake()
    {
        facingDirection = Vector2Int.down;
    }

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void FixedUpdate()
    {
        timePassed += Time.fixedDeltaTime;

        if (timePassed > timePerAction)
        {
            timePassed -= timePerAction;
            MoveCharacter();
        }
    }

    private void MoveCharacter()
    {
        if (facingDirection != Vector2.zero)
        {
            position += facingDirection;
            levelManager.SetPipe(position.x, position.y, length);
            length++;
        }

        this.transform.position = new Vector3(position.x, position.y, 0.0f);
        levelManager.ClearFogOfWar(position.x, position.y, visionRadius);
    }

    private void OnMove(InputValue input)
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
            if (moveVec != (facingDirection * -1))
            {
                facingDirection = moveVec;
            }
        }
    }
}
