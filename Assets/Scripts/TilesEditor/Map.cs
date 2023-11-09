using System;
using System.IO;
using System.Net;
using TilesEditor.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = TilesEditor.Tiles.TileData;

namespace TilesEditor
{
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
        private void FillMap()
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
            foreach (TilemapData tilemap in MainEditor.Instance.TilemapDatas)
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

        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                SaveMap();
            }
        }

        private void SaveMap()
        {
            MapData mapData = new MapData();
            mapData.TilesPos = _tilesPos;

            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    TileData tileData = _tilesPos[x, y];

                    mapData.TileDatas.Add(tileData);

                    // if (tileData != null)
                    // {
                    //     Debug.Log(
                    //         $"Tile {tileData.Tile} added, on the tilemap {tileData.AssociatedTilemap} at coordinates {x} and {y}");
                    // }
                }
            }

            string json = JsonUtility.ToJson(mapData, true);
            File.WriteAllText(Application.dataPath + "/testLevel.json", json);
        }
    }
}