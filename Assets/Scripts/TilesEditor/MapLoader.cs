using System;
using GameManagement;
using Gameplay.Units;
using TilesEditor.Tiles;
using TilesEditor.Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilesEditor
{
    public class MapLoader : MonoBehaviour
    {
        [Header("Map to load values")] [SerializeField]
        private TextAsset _jsonFile;

        [SerializeField, Tooltip("All the tiles will be placed in this.")]
        private Tilemap _mainTilemap;
        [SerializeField, Tooltip("All the tiles with colliders will be placed in this.")]
        private Tilemap _obstaclesTilemap;

        [Header("Defaults values")] [SerializeField, Tooltip("The tile to put when there is no tile assigned at this position.")]
        private Tile _defaultTile;
        [SerializeField] private Unit _defaultUnit;
        [SerializeField, Tooltip("The default tile will be placed in this.")]
        private Tilemap _defaultTilemap;

        [SerializeField] private GameplayData _gameplayData;

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
                    if (mapData.TileDatas[i].AssociatedTilemap.Type == TilemapType.Walkable)
                    {
                        _mainTilemap.SetTile(mapData.TilePos[i], mapData.TileDatas[i].Tile);
                    }
                }
                
                _defaultTilemap.SetTile(mapData.TilePos[i], _defaultTile);
            }

            Camera.main.transform.position = mapData.CameraPosition;

            LoadUnits(mapData);
        }

        private void LoadUnits(MapData mapData)
        {
            foreach (UnitForEditorData data in mapData.UnitEditorDatas)
            {
                Unit newUnit = Instantiate(_defaultUnit, data.Position, Quaternion.identity, null);

                foreach (UnitData unit in _gameplayData.Units)
                {
                    if (data.UnitType == unit.UnitType)
                    {
                        newUnit.Initialize(unit);
                        break;
                    }
                }
            }
        }

        public void LoadObstacles()
        {
            MapData mapData = JsonUtility.FromJson<MapData>(_jsonFile.text);

            for (int i = 0; i < mapData.TileDatas.Count; i++)
            {
                if (mapData.TileDatas[i].Tile == null ||
                    mapData.TileDatas[i].AssociatedTilemap.Type != TilemapType.NonWalkable)
                {
                    continue;
                }
                
                _obstaclesTilemap.SetTile(mapData.TilePos[i], mapData.TileDatas[i].Tile);
                GameManager.Instance.PathfindingController.SetTileNotWalkablePathfinding((Vector2)(Vector2Int)mapData.TilePos[i]);
            }
        }
    }
}