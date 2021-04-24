using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Enums;

[System.Serializable]
public struct GameTile
{
    public BlockType blockType;
    public int variant;
    public float cost;
}
