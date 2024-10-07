using UnityEngine;

namespace TilesEditor.Tiles
{
    public class TileDataPosition
    {
        public TileData Tile;
        public Vector3Int Position;
        
        public TileDataPosition(TileData tileData, Vector3Int position)
        {
            Tile = tileData;
            Position = position;
        }

    }
}