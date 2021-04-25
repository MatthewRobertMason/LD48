using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Assets.Scripts.Enums;

[CreateAssetMenu]
public class PipeTileScript : Tile {
    public Sprite horizontal;
    public Sprite vertical;
    public Sprite top_right;
    public Sprite top_left;
    public Sprite bottom_left;
    public Sprite bottom_right;

    // This refreshes itself and other RoadTiles that are orthogonally and diagonally adjacent
    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        int index = pipeIndex(location);
        for (int yd = -1; yd <= 1; yd++)
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (isPipe(tilemap, position, index)) tilemap.RefreshTile(position);
            }
    }

    // This determines if the Tile at the position is the same RoadTile.
    private bool isPipe(ITilemap tilemap, Vector3Int position, int index)
    {
        return tilemap.GetTile(position) == this && Mathf.Abs(index - pipeIndex(position)) <= 1;
    }

    private int pipeIndex(Vector3Int location){
        MapLevel map = LevelManager.level.LevelMap;
        if(map[location.x, -location.y].resourceType == ResourceType.Pipe)
            return map[location.x, -location.y].variant;

        return -10;
    }

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData){
        int index = pipeIndex(location);
        bool top = isPipe(tilemap, location + new Vector3Int(0, 1, 0), index);
        bool right = isPipe(tilemap, location + new Vector3Int(1, 0, 0), index);
        bool bottom = isPipe(tilemap, location + new Vector3Int(0, -1, 0), index);
        bool left = isPipe(tilemap, location + new Vector3Int(-1, 0, 0), index);

        var m = tileData.transform;
        m.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
        tileData.transform = m;

        if(top && bottom){
            tileData.sprite = vertical;
        } else if(right && left){
            tileData.sprite = horizontal;
        } else if(right && top){
            tileData.sprite = top_right;
        } else if(right && bottom){
            tileData.sprite = bottom_right;
        } else if(left && top){
            tileData.sprite = top_left;
        } else if(left && bottom){
            tileData.sprite = bottom_left;
        } else if(bottom){
            tileData.sprite = vertical;
        }
    }

}