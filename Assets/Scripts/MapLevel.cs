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
            return Cells[y / chunkHeight][x, y % chunkHeight]; 
        }
    }

    public void CreateLevel()
    {
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
