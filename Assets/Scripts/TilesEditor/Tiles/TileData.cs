using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilesEditor.Tiles
{
    /// <summary>
    /// Used to know on which tilemap the tile must be paint.
    /// </summary>
    [Serializable]
    public class TileData
    {
        public Tile Tile;
        public TilemapData AssociatedTilemap;
    }
}