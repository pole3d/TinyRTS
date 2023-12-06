using System.Collections.Generic;
using TilesEditor.Tiles;
using Unity.VisualScripting;
using UnityEngine;

namespace TilesEditor
{
    /// <summary>
    /// The map data to load and save tilemaps.
    /// </summary>
    public class MapData
    {
        public List<TileData> TileDatas = new List<TileData>();
        public List<Vector3Int> TilePos = new List<Vector3Int>();
    }
}