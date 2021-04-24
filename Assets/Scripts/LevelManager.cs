using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Assets.Scripts.Enums;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;
    private MapLevel levelMap;

    [Header("Generation Parameters")]
    public int levelWidth = 64;
    public int initialHeight = 200;
    public int chunkHeight = 32;
    public int bedrockBorder = 8;

    [Header("TileMaps")]
    public Tilemap Background;
    public Tilemap Foreground;
    public Tilemap Resource;
    public Tilemap FogOfWar;

    [Header("Tiles")]
    public Tile BedrockTile;
    public Tile DirtTile;
    public Tile StoneTile;

    public Tile IronTile;
    public Tile CopperTile;
    public Tile GoldTile;
    public Tile DiamondTile;

    public Tile FogOfWarTile;

    public MapLevel LevelMap
    {
        get => levelMap;
        set => levelMap = value;
    }

    public void Awake()
    {
        if (FindObjectsOfType<LevelManager>().Length > 1)
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Initialize()
    {
        levelMap = new MapLevel(this);

        for (int i = 0; i < LevelMap.CurrentChunks; i++)
        {
            DrawLayer(i);
        }
    }

    public void DrawLayer(int layer)
    {
        int yAdd = layer * chunkHeight;

        for (int y = yAdd; y < chunkHeight + yAdd; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                DrawTile(x, y);
            }
        }
    }

    public void DrawTile(int x, int y)
    {
        GameTile tile = levelMap[x, y];
        Vector3Int pos = new Vector3Int(x, y * -1, 0);
        
        Tile foreGround = BedrockTile;
        Tile resource = null;
        
        switch (tile.blockType)
        {
            case BlockType.Dirt:
                foreGround = DirtTile;
                break;

            case BlockType.Stone:
                foreGround = StoneTile;
                break;
        }

        switch (tile.resourceType)
        {
            case ResourceType.Iron:
                resource = IronTile;
                break;

            case ResourceType.Copper:
                resource = CopperTile;
                break;

            case ResourceType.Gold:
                resource = GoldTile;
                break;

            case ResourceType.Diamond:
                resource = DiamondTile;
                break;
        }

        Background.SetTile(pos, foreGround);
        Foreground.SetTile(pos, foreGround);
        if (resource != null)
        {
            Resource.SetTile(pos, resource);
        }
        FogOfWar.SetTile(pos, FogOfWarTile);
    }
}
