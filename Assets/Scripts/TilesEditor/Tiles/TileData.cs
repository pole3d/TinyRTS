using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilesEditor.Tiles
{
    [Serializable]
    public class TileData
    {
        public Tile Tile;
        public Vector3Int TilePosition;
        public Tilemap AssociatedTilemap;
    }
}