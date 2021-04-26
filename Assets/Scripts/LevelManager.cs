using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Assets.Scripts.Enums;
using Assets.Scripts.Classes;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;
    private MapLevel levelMap;

    [Header("UI")]
    public UnityEngine.UI.Text pipeDisplay;

    [Header("Generation Parameters")]
    public int levelWidth = 64;
    public int initialHeight = 200;
    public int chunkHeight = 32;
    public int bedrockBorder = 8;
    public int fogStartDepth = 3;

    [Header("TileMaps")]
    public Tilemap Background;
    public Tilemap Foreground;
    public Tilemap Resource;
    public Tilemap FogOfWar;
    public Tilemap FogOfWarDecoration;

    [Header("Tiles")]
    public Tile BedrockTile;
    public Tile DirtTile;
    public Tile StoneTile;

    public Tile IronTile;
    public Tile CopperTile;
    public Tile GoldTile;
    public Tile DiamondTile;
    public Tile RadioactiveTile;
    
    public Tile GrassTile;
    public TileBase FogOfWarTile;

    public TileBase PipeTile;
    public TileBase RustyPipeTile;

    public MapLevel LevelMap
    {
        get => levelMap;
        set => levelMap = value;
    }

    public void Awake()
    {
        if (FindObjectsOfType<LevelManager>().Length > 1)
        {
            DestroyImmediate(this.gameObject);
        } else {
            DontDestroyOnLoad(this);
        }
    }

    public void FindUI(){
        pipeDisplay = GameObject.Find("PipeText").GetComponent<UnityEngine.UI.Text>();
    }

    public void Initialize()
    {
        gameManager = FindObjectOfType<GameManager>();
        levelMap = new MapLevel(this);

        for (int i = 0; i < LevelMap.CurrentChunks; i++)
        {
            DrawLayer(i);
        }
    }

    public void EnsureDepth(int yy){
        int chunk = 1 + yy / chunkHeight;
        int generated = LevelMap.EnsureDepth(yy + chunkHeight);
        for(int ii = 0; ii < generated; ii++){
            DrawLayer(chunk + ii);
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

    public void DrawTile(int x, int y, bool fog=true)
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

            case ResourceType.RustyPipe:
                resource = RustyPipeTile;
                break;

            case ResourceType.Radioactive:
                resource = RadioactiveTile;
                break;
        }

        Background.SetTile(pos, foreGround);
        Foreground.SetTile(pos, foreGround);
        if (resource != null)
        {
            Resource.SetTile(pos, resource);
        }

        if (y > fogStartDepth && fog)
        {
            FogOfWar.SetTile(pos, foreGround);
        }
    }

    public void DigTile(int x, int y)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Foreground.SetTile(pos, null);
    }

    public void RemoveGrass(int x, int y)
    {
        GameTile tile = levelMap[x, y];
        if (tile.resourceType == ResourceType.Grass)
        {
            tile.resourceType = ResourceType.None;
        }
    }

    public void ClearFogOfWar(int x, int y, float radius)
    {
        int xClampMax = levelWidth-1;
        // int yClampMax = (levelMap.CurrentChunks * chunkHeight - 1) * -1;
        int intRadius = (int)radius;

        int xMin = Mathf.Clamp(x - intRadius, 0, xClampMax);
        int xMax = Mathf.Clamp(x + intRadius, 0, xClampMax);
        int yMin = y - intRadius;
        int yMax = y + intRadius;
        EnsureDepth(-yMin);

        Vector2 pos = new Vector2Int(x, y);
        Vector2 test = new Vector2Int();

        for (int xx = xMin; xx <= xMax; xx++)
        {
            for (int yy = yMin; yy <= yMax; yy++)
            {
                test.x = xx;
                test.y = yy;
                if (Vector2.Distance(pos, test) <= radius)
                {
                    FogOfWar.SetTile(new Vector3Int(xx, yy, 0), null);
                    FogOfWarDecoration.SetTile(new Vector3Int(xx, yy, 0), null);
                }
            }
        }
    }

    public GameObject[] fogObjects3;

    public FogObject[] fogObjects;

    public void AddFogSprites(int layer)
    {
        bool[,] test = new bool[levelWidth, chunkHeight];

        if (fogObjects != null && fogObjects.Length > 0)
        {
            int genNum = Random.Range(4, 16);

            for (int i = 0; i < genNum; i++)
            {
                int sprite = Random.Range(0, fogObjects.Length);

                FogObject fObject = fogObjects[sprite];

                int sHeight = fObject.Height;
                int sWidth = fObject.Width;

                int yVar = (Random.Range(0, chunkHeight - sHeight));
                int xVar = Random.Range(bedrockBorder, levelWidth - bedrockBorder - sWidth);

                bool canPlace = true;
                for (int x = xVar; x < xVar + sWidth; x++)
                {
                    for (int y = yVar; y < yVar + sHeight; y++)
                    {
                        if (test[x, y] == true)
                        {
                            canPlace = false;
                            break;
                        }
                    }
                    if (!canPlace)
                    {
                        break;
                    }
                }

                if (canPlace)
                {
                    Vector3Int pos = new Vector3Int(0, 0, 0);

                    int x = xVar;
                    int y = yVar;

                    foreach(FogTiles row in fObject.Rows)
                    {
                        foreach (Tile tile in row.Columns)
                        {
                            test[x, y] = true;

                            pos.x = x;
                            pos.y = -1 * (y + chunkHeight * layer);

                            if (pos.y < -fogStartDepth)
                            {
                                FogOfWarDecoration.SetTile(pos, tile);
                            }
                            x++;
                        }

                        x = xVar;
                        y++;
                    }
                }
            }
        }
    }

    public void RecyclePipes(){
        var changes = levelMap.Replace(ResourceType.Pipe, ResourceType.RustyPipe);
        foreach(var index in changes){
            DrawTile(index.x, index.y, false);
        }
    }
}
