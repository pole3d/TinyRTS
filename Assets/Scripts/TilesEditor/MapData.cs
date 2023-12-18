using System.Collections.Generic;
using TilesEditor.Tiles;
using TilesEditor.Units;
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
        public List<UnitForEditorData> UnitEditorDatas = new List<UnitForEditorData>();
        public Vector3 CameraPosition;
    }
}