using System.Collections.Generic;
using System.IO;
using Gameplay.Units;
using TilesEditor.Tiles;
using TilesEditor.Units;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TileData = TilesEditor.Tiles.TileData;
using Vector2 = System.Numerics.Vector2;

namespace TilesEditor
{
    /// <summary>
    /// Handle all the changes in the map like tile placement, unit and mainly Save and Load functions. 
    /// </summary>
    public class Map : MonoBehaviour
    {
        [field: SerializeField] public TilemapData[] TilemapDatas { get; private set; }
        [field: SerializeField] public Vector2Int MapSize { get; private set; }
        [field: SerializeField] public Grid Grid { get; private set; }

        [SerializeField] private CameraController _cameraController;
        
        [Header("Defaults references")]
        [SerializeField] private Tilemap _defaultTilemap;
        [SerializeField] private Tile _defaultTile;

        [Header("Brush Size")]
        [SerializeField, Range(1, 30)] private float _brushSizeMax;
        [SerializeField] private Slider _brushSizeSlider;

        private TileData[,] _tilesPos;
        private List<UnitForEditorData> _unitForEditorDatas = new List<UnitForEditorData>();
        private List<UnitForEditor> _unitForEditor = new List<UnitForEditor>();
        private float _brushSize;

        private void Start()
        {
            _tilesPos = new TileData[MapSize.x, MapSize.y];
            FillMap();

            SetBrushSizeSliderValues();
        }

        private void SetBrushSizeSliderValues()
        {
            SetBrushSize(_brushSizeSlider.value);
            _brushSizeSlider.minValue = 1;
            _brushSizeSlider.maxValue = _brushSizeMax;
            _brushSizeSlider.onValueChanged.AddListener(SetBrushSize);
        }

        /// <summary>
        /// Set the brush size according to the slider.
        /// </summary>
        /// <param name="value"> Slider value. </param>
        private void SetBrushSize(float value)
        {
            _brushSize = value;
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
            foreach (TilemapData tilemap in TilemapDatas)
            {
                if (!tilemap.TilesDataAssociated.Contains(tile))
                {
                    continue;
                }

                for (int x = 0; x < _brushSize; x++)
                {
                    for (int y = 0; y < _brushSize; y++)
                    {
                        Vector3Int pos = new Vector3Int(position.x + x, position.y + y);

                        bool isInZone = pos.x >= 0 && pos.x < MapSize.x && pos.y < MapSize.y && pos.y >= 0;

                        if (isInZone == false)
                        {
                            continue;
                        }

                        tilemap.CurrentTilemap.SetTile(pos, tile.Tile);

                        tile.AssociatedTilemap = tilemap;
                        _tilesPos[pos.x, pos.y] = tile;
                    }
                }
            }
        }
        
        /// <summary>
        /// Add unit to the map.
        /// </summary>
        /// <param name="prefab"> The default prefab to instantiate. </param>
        /// <param name="data"> The data we want to give it. </param>
        /// <param name="position"> The position it will spawns. </param>
        /// <param name="sprite"> The sprite we give it. </param>
        public void AddUnitToMap(UnitForEditor prefab, UnitForEditorData data, Vector3 position, Sprite sprite)
        {
            UnitForEditor unit = Instantiate(prefab, position, Quaternion.identity);
            unit.SetDisplay(sprite);

            unit.UnitForEditorData = new UnitForEditorData
            {
                UnitType = data.UnitType,
                Position = position
            };

            AddUnitToLists(unit);
        }

        
        /// <summary>
        /// Add the unit to the list in the editor.
        /// </summary>
        /// <param name="unit"> The unit to add to the list. </param>
        private void AddUnitToLists(UnitForEditor unit)
        {
            _unitForEditorDatas.Add(unit.UnitForEditorData);
            _unitForEditor.Add(unit);
        }

        /// <summary>
        /// Save the map in a json file.
        /// </summary>
        /// <param name="mapName"> The input field where the player is going to write the name of the map. </param>
        public void SaveMap(TMP_InputField mapName)
        {
            if (mapName == null || mapName.text == "")
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

            mapData.UnitEditorDatas = _unitForEditorDatas;
            mapData.CameraPosition = Camera.main.transform.position;

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

            foreach (TilemapData tilemap in TilemapDatas)
            {
                tilemap.CurrentTilemap.ClearAllTiles();
            }

            for (int i = 0; i < mapData.TileDatas.Count; i++)
            {
                if (mapData.TileDatas[i].Tile != null)
                {
                    TilemapDatas[mapData.TileDatas[i].AssociatedTilemap.TileMapIndex].CurrentTilemap.SetTile(mapData.TilePos[i], mapData.TileDatas[i].Tile);
                }
                else
                {
                    _defaultTilemap.SetTile(mapData.TilePos[i], _defaultTile);
                }
            }

            foreach (UnitForEditorData unitEditorData in mapData.UnitEditorDatas)
            {
                UnitForEditor newUnit = Instantiate(TilesEditor.Instance.UnitPrefab, unitEditorData.Position, Quaternion.identity, null);

                foreach (UnitData unit in TilesEditor.Instance.Data.Units)
                {
                    if (unitEditorData.UnitType == unit.UnitType)
                    {
                        newUnit.SetDisplay(unit.Sprite);
                        newUnit.UnitForEditorData = new UnitForEditorData
                        {
                            UnitType = unit.UnitType,
                            Position = unitEditorData.Position
                        };
                        break;
                    }
                }

                AddUnitToLists(newUnit);
            }

            Camera.main.transform.position = mapData.CameraPosition;

            Debug.Log("Map loaded");
        }

        /// <summary>
        /// Clear all the tiles of the map but set the background.
        /// </summary>
        public void ResetMap()
        {
            foreach (TilemapData tilemap in TilemapDatas)
            {
                tilemap.CurrentTilemap.ClearAllTiles();
            }

            foreach (UnitForEditor unit in _unitForEditor)
            {
                Destroy(unit.gameObject);
            }

            _unitForEditor.Clear();
            _unitForEditorDatas.Clear();
            
            _cameraController.Reset();

            FillMap();
        }
    }
}