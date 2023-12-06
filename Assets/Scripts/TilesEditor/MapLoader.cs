using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = TilesEditor.Tiles.TileData;

namespace TilesEditor
{
    public class MapLoader : MonoBehaviour
    {
        [Header("Map to load values")] [SerializeField]
        private TextAsset _jsonFile;

        [SerializeField, Tooltip("All the tiles will be placed in this.")]
        private Tilemap _mainTilemap;

        [Header("Defaults values")]
        [SerializeField, Tooltip("The tile to put when there is no tile assigned at this position.")]
        private Tile _defaultTile;

        [SerializeField, Tooltip("The default tile will be placed in this.")]
        private Tilemap _defaultTilemap;

        public void Initialize()
        {
            LoadMapFromFile();
        }

        /// <summary>
        /// Load the map from a json file.
        /// </summary>
        private void LoadMapFromFile()
        {
            MapData mapData = JsonUtility.FromJson<MapData>(_jsonFile.text);

            for (int i = 0; i < mapData.TileDatas.Count; i++)
            {
                if (mapData.TileDatas[i].Tile != null)
                {
                    _mainTilemap.SetTile(mapData.TilePos[i], mapData.TileDatas[i].Tile);
                }
                else
                {
                    _defaultTilemap.SetTile(mapData.TilePos[i], _defaultTile);
                }
            }
        }
    }
}