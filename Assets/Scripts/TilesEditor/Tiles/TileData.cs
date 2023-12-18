using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilesEditor.Tiles
{
    /// <summary>
    /// Associate the tile and its tilemap.
    /// </summary>
    [Serializable]
    public class TileData
    {
        public Tile Tile;
        public TilemapData AssociatedTilemap;
    }
}