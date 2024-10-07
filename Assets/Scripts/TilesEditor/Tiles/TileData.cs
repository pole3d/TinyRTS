using System;
using System.Collections.Generic;
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
        public int ID;
        public Tile Tile;
        public TilemapData AssociatedTilemap;
        public List<Vector2Int> AdditionalTilesPositions;
        public bool UseTileset;
        public List<Tile> TilesetTiles;
    }
}