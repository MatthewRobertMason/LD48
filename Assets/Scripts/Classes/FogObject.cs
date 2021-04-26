using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes
{
    [Serializable]
    public class FogObject
    {
        public string name;
        public FogTiles[] Rows;
    }

    [Serializable]
    public class FogTiles
    {
        public Tile[] Columns;
    }
}
