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

        if(LayerLevel >= 1){
            // Number of veins decrease as you go down
            int veins = Mathf.Max(2, (int)Mathf.Ceil(5.0f - LayerLevel/4.0f));

            for(var ii = 0; ii < veins; ii++){
                // But get bigger as you go down
                int size = Random.Range(3 + LayerLevel, 10 + LayerLevel*3);
                GenerateVein(size, ResourceType.Iron, temp, new Vector2Int(Random.Range(bedrockBorder, levelWidth - bedrockBorder), Random.Range(0, chunkHeight)));

                size = Random.Range(3 + LayerLevel, 15 + LayerLevel*2);
                GenerateVein(size, ResourceType.Copper, temp, new Vector2Int(Random.Range(bedrockBorder, levelWidth - bedrockBorder), Random.Range(0, chunkHeight)));

                size = Random.Range(3 + LayerLevel, 8 + LayerLevel);
                GenerateVein(size, ResourceType.Gold, temp, new Vector2Int(Random.Range(bedrockBorder, levelWidth - bedrockBorder), Random.Range(0, chunkHeight)));

                size = Random.Range(3 + LayerLevel/2, 5 + LayerLevel/2);
                GenerateVein(size, ResourceType.Diamond, temp, new Vector2Int(Random.Range(bedrockBorder, levelWidth - bedrockBorder), Random.Range(0, chunkHeight)));
            }
        }

        return temp;
    }

    private void GenerateVein(int size, ResourceType kind, GameTile[,] chunk, Vector2Int start){
        var covered = new HashSet<Vector2Int>();
        var neighbors = new HashSet<Vector2Int>();
        var neighbor_list = new List<Vector2Int>();
        neighbor_list.Add(start);
        int created = 0;

        while(created < size && neighbor_list.Count > 0){
            // Get a random unconsidered index
            int index = Random.Range(0, neighbor_list.Count);
            Vector2Int current = neighbor_list[index];
            neighbor_list[index] = neighbor_list[neighbor_list.Count-1];
            neighbor_list.RemoveAt(neighbor_list.Count-1);
            neighbors.Remove(current);
            covered.Add(current);

            // Check if we can make this part of the vein
            if(current.y < 0 || current.y >= chunkHeight) continue;
            if(current.x < bedrockBorder || current.x >= levelWidth - bedrockBorder) continue;
            ref GameTile cell = ref chunk[current.x, current.y];
            if(cell.resourceType != ResourceType.None) continue;
            cell.resourceType = kind;
            created++;

            // Add neighbours for added tile
            for(int dx = -1; dx <= 1; dx++){
                for(int dy = -1; dy <= 1; dy++){
                    var point = new Vector2Int(dx, dy);
                    point += current;
                    if(!neighbors.Contains(point) && !covered.Contains(point)){
                        neighbor_list.Add(point);
                        neighbors.Add(point);
                    }
                }
            }
        }
    }

    public List<Vector2Int> Replace(ResourceType a, ResourceType b){
        var changed = new List<Vector2Int>();
        for(int index = 0; index < Cells.Count; index++){
            GameTile[,] chunk = Cells[index];
            for(int xx = 0; xx < levelWidth; xx++){
                for(int yy = 0; yy < chunkHeight; yy++){
                    if(chunk[xx, yy].resourceType == a){
                        chunk[xx, yy].resourceType = b;
                        changed.Add(new Vector2Int(xx, yy));
                    }
                }
            }
        }
        return changed;
    }
}
