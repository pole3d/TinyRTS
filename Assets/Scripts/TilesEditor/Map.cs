using System.Collections.Generic;
using System.IO;
using TilesEditor.Tiles;
using TilesEditor.Units;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TileData = TilesEditor.Tiles.TileData;

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

        [Header("Defaults references")] [SerializeField]
        private Tilemap _defaultTilemap;

        [SerializeField] private Tile _defaultTile;

        [Header("Brush Size")] [SerializeField, Range(1, 30)]
        private float _brushSizeMax;

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
                var containsTile = false;
                foreach (var tileData in tilemap.TilesDataAssociated)
                {
                    if (tileData.Data == tile) containsTile = true;
                }

                if (!containsTile)
                {
                    continue;
                }

                for (int x = 0; x < _brushSize; x++)
                {
                    for (int y = 0; y < _brushSize; y++)
                    {
                        Vector3Int pos = new Vector3Int(position.x + x, position.y + y);

                        if (IsInZone(pos) == false)
                        {
                            continue;
                        }

                        AddTilesToMap(tile, tilemap, pos);
                    }
                }
            }
        }

        /// <summary>
        /// Remove a tile from the map if it exists at the given position.
        /// </summary>
        /// <param name="cellPosition">Position to remove the tile at.</param>
        public void RemoveTileFromMap(Vector3Int cellPosition)
        {
            if (IsInZone(cellPosition) == false) return;
            var currentTileData = _tilesPos[cellPosition.x, cellPosition.y];
            if (currentTileData == null) return;
            
            SaveTileToMap(null, currentTileData.AssociatedTilemap, cellPosition);
        }

        private void SaveTileToMap(TileData tile, TilemapData tilemap, Vector3Int pos)
        {
            tilemap.CurrentTilemap.SetTile(pos, GetTileToDraw(tile, pos));
            if (tile != null) tile.AssociatedTilemap = tilemap;
            _tilesPos[pos.x, pos.y] = tile;
        }

        /// <summary>
        /// Add the tiles to draw, one by one
        /// </summary>
        /// <param name="firstTilePosition"></param>
        private void AddTilesToMap(TileData firstTileData, TilemapData tilemap, Vector3Int firstTilePosition)
        {
            var openCells = new Queue<TileDataPosition>();
            var closedCells = new Queue<TileDataPosition>();

            openCells.Enqueue(new TileDataPosition(firstTileData, firstTilePosition));

            if (firstTileData.UseTileset)
            {
                Vector3Int[] additionalPositions = new Vector3Int[]
                {
                    firstTilePosition + Vector3Int.up,
                    firstTilePosition + Vector3Int.right,
                    firstTilePosition + Vector3Int.down,
                    firstTilePosition + Vector3Int.left,
                };

                foreach (var additionalPosition in additionalPositions)
                {
                    if (_tilesPos[additionalPosition.x, additionalPosition.y]?.Tile != null)
                        openCells.Enqueue(new TileDataPosition(_tilesPos[additionalPosition.x, additionalPosition.y], additionalPosition));
                }
            }

            foreach (var additionalTile in firstTileData.AdditionalTilesPositions)
            {
                openCells.Enqueue(new TileDataPosition(firstTileData, firstTilePosition + (Vector3Int)additionalTile));
            }
            
            int maxIterations = 100; // To ensure the while loop ends.

            while (openCells.Count > 0 && maxIterations > 0)
            {
                var currentCell = openCells.Peek();

                SaveTileToMap(currentCell.Tile, tilemap, currentCell.Position);

                closedCells.Enqueue(currentCell);
                openCells.Dequeue();

                maxIterations--;
            }
        }
        
        /// <summary>
        /// Returns the tile to use based on adjacdents tiles
        /// </summary>
        /// <param name="tileData"></param>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        private Tile GetTileToDraw(TileData tileData, Vector3Int gridPos)
        {
            if (tileData == null) return null;
            if (tileData.UseTileset == false) return tileData.Tile;
                
            var value = 0;
            if (_tilesPos[gridPos.x, gridPos.y + 1] != null && _tilesPos[gridPos.x, gridPos.y + 1].ID == tileData.ID) value += 1;
            if (_tilesPos[gridPos.x + 1, gridPos.y] != null && _tilesPos[gridPos.x + 1, gridPos.y].ID == tileData.ID) value += 2;
            if (_tilesPos[gridPos.x, gridPos.y - 1] != null && _tilesPos[gridPos.x, gridPos.y - 1].ID == tileData.ID) value += 4;
            if (_tilesPos[gridPos.x - 1, gridPos.y] != null && _tilesPos[gridPos.x - 1, gridPos.y].ID == tileData.ID) value += 8;

            return tileData.TilesetTiles[value];
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
                AssociatedData = data.AssociatedData,
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
        /// Remove an unit from the list in the editor and deletes it.
        /// </summary>
        /// <param name="cellPosition"> The position to remove the unit at, if it exists. </param>
        public void RemoveUnitFromMap(Vector3Int cellPosition)
        {
            if (IsInZone(cellPosition) == false) return;

            foreach (var unitData in _unitForEditorDatas)
            {
                var currentCellPosition = Grid.WorldToCell(unitData.Position);
                if (currentCellPosition == cellPosition)
                {
                    var unit = _unitForEditor.Find(e => e.UnitForEditorData == unitData);

                    _unitForEditorDatas.Remove(unitData);
                    _unitForEditor.Remove(unit);
                    Destroy(unit.gameObject);
                    return;
                }
            } }

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
                    SaveTileToMap(mapData.TileDatas[i],mapData.TileDatas[i].AssociatedTilemap,mapData.TilePos[i]);
                }
                else
                {
                    _defaultTilemap.SetTile(mapData.TilePos[i], _defaultTile);
                }
            }

            // Refresh every tile placed to ensure their sprite is correct
            for (int x = 0; x < _tilesPos.GetLength(0); x++)
            {
                for (int y = 0; y < _tilesPos.GetLength(1); y++)
                {
                    var tile = _tilesPos[x, y];
                    if (tile != null) SaveTileToMap(tile,tile.AssociatedTilemap,new Vector3Int(x,y,0));
                }
            }

            foreach (UnitForEditorData unitEditorData in mapData.UnitEditorDatas)
            {
                UnitForEditor newUnit = Instantiate(TilesEditor.Instance.UnitPrefab, unitEditorData.Position,
                    Quaternion.identity, null);

                foreach (UnitData unit in TilesEditor.Instance.Data.Units)
                {
                    if (unitEditorData.AssociatedData.UnitType == unit.UnitType)
                    {
                        newUnit.SetDisplay(unit.Sprite);
                        newUnit.UnitForEditorData = new UnitForEditorData
                        {
                            AssociatedData = unit,
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

        /// <summary>
        /// Is the given coordinate valid in the map
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsInZone(Vector3Int position)
        {
            return position.x >= 0 && position.x < MapSize.x && position.y < MapSize.y && position.y >= 0;
        }
    }
}