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

        public int Width
        {
            get 
            { 
                if (Rows != null && Rows.Count() > 0)
                {
                    return Rows[0].Columns.Count();
                }

                return 0;
            }
        }
        public int Height
        {
            get
            {
                return Rows.Count();
            }
        }
    }

    [Serializable]
    public class FogTiles
    {
        public Tile[] Columns;
    }
}
