using System;
using System.IO;
using System.Net;
using TilesEditor.Tiles;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = TilesEditor.Tiles.TileData;

namespace TilesEditor
{
    /// <summary>
    /// Used to get and set map datas.
    /// </summary>
    public class Map : MonoBehaviour
    {
        [field: SerializeField] public Vector2Int MapSize { get; private set; }
        [field: SerializeField] public Grid Grid { get; private set; }

        [SerializeField] private Tile _defaultTile;
        [SerializeField] private Tilemap _defaultTilemap;
        private TileData[,] _tilesPos;

        private void Start()
        {
            _tilesPos = new TileData[MapSize.x, MapSize.y];
            FillMap();
        }

        /// <summary>
        /// Create a map with the given size and the default tile, on the given tilemap.
        /// </summary>
        public void FillMap()
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    _defaultTilemap.SetTile(new Vector3Int(x, y, 0), _defaultTile);
                }
            }
        }

        /// <summary>
        /// Add a tile at a position on the tilemap.
        /// </summary>
        /// <param name="tile"> Tile to add. </param>
        /// <param name="position"> Position to add the tile. </param>
        public void AddTileToMap(TileData tile, Vector3Int position)
        {
            foreach (TilemapData tilemap in TilesEditor.Instance.TilemapDatas)
            {
                if (tilemap.TilesDataAssociated.Contains(tile))
                {
                    if (_tilesPos[position.x, position.y] != tile)
                    {
                        tilemap.CurrentTilemap.SetTile(position, tile.Tile);

                        tile.AssociatedTilemap = tilemap.CurrentTilemap;

                        _tilesPos[position.x, position.y] = tile;
                    }
                }
            }
        }

        /// <summary>
        /// Save the map in a json file.
        /// </summary>
        /// <param name="mapName"> The input field where the player is going to write the name of the map. </param>
        public void SaveMap(TMP_InputField mapName)
        {
            if(mapName == null || mapName.text == "")
            {
                return;
            }
            
            MapData mapData = new MapData();

            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    TileData tileData = _tilesPos[x, y];

                    mapData.TileDatas.Add(tileData);
                    mapData.TilePos.Add(new Vector3Int(x, y, 0));
                }
            }

            string json = JsonUtility.ToJson(mapData, true);
            File.WriteAllText(Application.dataPath + $"/Saves/{mapName.text}.json", json);

            Debug.Log("Map saved");
        }

        /// <summary>
        /// Load a map from its name.
        /// </summary>
        /// <param name="mapName"> Name of the map. </param>
        public void LoadMap(string mapName)
        {
            string json = File.ReadAllText(Application.dataPath + $"/Saves/{mapName}.json");
            MapData mapData = JsonUtility.FromJson<MapData>(json);

            foreach (TilemapData tilemap in TilesEditor.Instance.TilemapDatas)
            {
                tilemap.CurrentTilemap.ClearAllTiles();
            }

            for (int i = 0; i < mapData.TileDatas.Count; i++)
            {
                if (mapData.TileDatas[i].Tile != null)
                {
                    mapData.TileDatas[i].AssociatedTilemap.SetTile(mapData.TilePos[i], mapData.TileDatas[i].Tile);
                }
                else
                {
                    _defaultTilemap.SetTile(mapData.TilePos[i], _defaultTile);
                }
            }

            Debug.Log("Map loaded");
        }
    }
}