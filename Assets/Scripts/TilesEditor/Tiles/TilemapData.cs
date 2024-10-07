using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilesEditor.Tiles
{
    /// <summary>
    /// The values required to correctly load the tilemaps and initialize the buttons.
    /// </summary>
    [Serializable]
    public class TilemapData
    {
        public int TileMapIndex;
        [field: SerializeField] public TilemapType Type { get; private set; }
        [field: SerializeField] public Tilemap CurrentTilemap { get; private set; }
        //[field: SerializeField] public List<Tile> TilesAssociated { get; private set; } = new List<Tile>();
        [field: SerializeField] public List<TileDataScriptable> TilesDataAssociated { get; private set; } = new List<TileDataScriptable>();
        public List<TileButton> TilesButtonsAssociated { get; set; } = new List<TileButton>();
    }
    
    public enum TilemapType
    {
        Walkable, 
        NonWalkable
    }
}