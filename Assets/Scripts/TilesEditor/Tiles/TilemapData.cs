using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace TilesEditor.Tiles
{
    [Serializable]
    public class TilemapData
    {
        [field: SerializeField] public Tilemap CurrentTilemap { get; private set; }
        [field: SerializeField] public List<Tile> TilesAssociated { get; private set; } = new List<Tile>();
        public List<TileData> TilesDataAssociated { get; } = new List<TileData>();
    }
}