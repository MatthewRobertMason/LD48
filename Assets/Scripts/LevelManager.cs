using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Assets.Scripts.Enums;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;
    private MapLevel levelMap;
    public static LevelManager level;

    [Header("UI")]
    public UnityEngine.UI.Text pipeDisplay;

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
    
    public Tile GrassTile;
    public Tile FogOfWarTile;

    public TileBase PipeTile;

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
        } else {
            level = this;
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

    public void SetPipe(int x, int y, int length){
        levelMap.SetPipe(x, -y, length);
        DrawTile(x, -y);
    }

    public ResourceType CollectResource(int x, int y){
        return levelMap[x, -y].resourceType;
    }

    public void DrawTile(int x, int y)
    {
        GameTile tile = levelMap[x, y];
        Vector3Int pos = new Vector3Int(x, -y, 0);
        
        Tile foreGround = BedrockTile;
        TileBase resource = null;

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

            case ResourceType.Grass:
                resource = GrassTile;
                break;

            case ResourceType.Pipe:
                resource = PipeTile;
                break;
        }

        Background.SetTile(pos, foreGround);
        Foreground.SetTile(pos, foreGround);
        if (resource != null)
        {
            Resource.SetTile(pos, resource);
        }

        if (y > 3)
        {
            FogOfWar.SetTile(pos, FogOfWarTile);
        }
    }

    public void ClearFogOfWar(int xx, int yy, float radius)
    {
        int xClampMax = levelWidth-1;
        int yClampMax = (levelMap.CurrentChunks * chunkHeight - 1) * -1;
        int intRadius = (int)radius;

        int xMin = Mathf.Clamp(xx - intRadius, 0, xClampMax);
        int xMax = Mathf.Clamp(xx + intRadius, 0, xClampMax);
        int yMin = Mathf.Clamp(yy - intRadius, yClampMax, 0);
        int yMax = Mathf.Clamp(yy + intRadius, yClampMax, 0);

        Vector2 pos = new Vector2Int(xx, yy);
        Vector2 test = new Vector2Int();

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                test.x = x;
                test.y = y;

                if (Vector2.Distance(pos, test) <= radius)
                {
                    FogOfWar.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }
}
