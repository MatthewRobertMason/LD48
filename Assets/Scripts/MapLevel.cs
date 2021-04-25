using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Enums;

public class MapLevel
{
    public List<GameTile[,]> Cells
    { 
        get; 
        set; 
    }

    public int levelWidth;
    public int initialHeight;
    public int chunkHeight;
    public int bedrockBorder;

    private int currentChunks = 1;
    public int CurrentChunks
    {
        get
        {
            return currentChunks;
        }
    }

    private LevelManager levelManager;

    public MapLevel(LevelManager levelManager)
    {
        this.levelManager = levelManager;

        levelWidth = levelManager.levelWidth;
        initialHeight = levelManager.initialHeight;
        chunkHeight = levelManager.chunkHeight;
        bedrockBorder = levelManager.bedrockBorder;

        levelWidth = levelWidth + (bedrockBorder * 2);
        levelManager.levelWidth = levelWidth;

        currentChunks = initialHeight / chunkHeight;

        Cells = new List<GameTile[,]>();

        CreateLevel();
    }

    public GameTile this[int x, int y]
    {
        get 
        { 
            int chunk_index = y/chunkHeight;
            while(chunk_index > Cells.Count){
                Cells.Add(CreateChunk(Cells.Count));
            }
            currentChunks = Cells.Count;
            return Cells[chunk_index][x, y % chunkHeight]; 
        }
    }

    public bool OutOfBounds(int x, int y){
        if(y <= 0) return true;
        if(x < bedrockBorder || levelWidth - bedrockBorder <= x) return true;
        return false;
    }

    public void SetPipe(int x, int y, int length){
        var cell = this[x, y];
        Cells[y / chunkHeight][x, y % chunkHeight].resourceType = ResourceType.Pipe;
        Cells[y / chunkHeight][x, y % chunkHeight].variant = length;
    }

    public void CreateLevel()
    {
        Debug.Log("Create Level");
        for (int c = 0; c < currentChunks; c++)
        {
            Cells.Add(CreateChunk(c));
        }

        currentChunks = Cells.Count;
    }

    public GameTile[,] CreateChunk(int LayerLevel)
    {
        GameTile[,] temp = new GameTile[levelWidth, chunkHeight];

        for (int y = 0; y < chunkHeight; y ++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                if ((x < bedrockBorder) || (x + bedrockBorder >= levelWidth))
                {
                    temp[x, y].blockType = BlockType.Bedrock;
                }
                else if (LayerLevel <= 0)
                {
                    temp[x, y].blockType = BlockType.Dirt;
                    
                    if (y == 0)
                    {
                        temp[x, y].resourceType = ResourceType.Grass;
                    }

                    if(y > 10){
                        int resource = Random.Range(0, Mathf.Max(20, 100-y));
                        if(resource <= 2){
                            temp[x, y].resourceType = ResourceType.Copper;
                        } else if(resource <= 4){
                            temp[x, y].resourceType = ResourceType.Iron;
                        } else if(resource <= 6){
                            temp[x, y].resourceType = ResourceType.Gold;
                        }
                    }
                }
                else if (LayerLevel >= 1)
                {
                    temp[x, y].blockType = BlockType.Stone;
                }
            }
        }

        return temp;
    }
}
