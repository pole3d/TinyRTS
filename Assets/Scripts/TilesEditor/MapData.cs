using System.Collections.Generic;
using TilesEditor.Tiles;
using Unity.VisualScripting;
using UnityEngine;

namespace TilesEditor
{
    public class MapData
    {
        public List<TileData> TileDatas = new List<TileData>();
        public TileData[,] TilesPos;
    }
}