using System;
using System.Collections.Generic;
using System.IO;
using TilesEditor.Tiles;
using TilesEditor.Units;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TileData = TilesEditor.Tiles.TileData;

namespace TilesEditor
{
    /// <summary>
    /// Manage all the player actions and has all the current datas.
    /// </summary>
    public class TilesEditor : MonoBehaviour
    {
        public static TilesEditor Instance;

        private TileData CurrentTile { get; set; }
        private UnitForEditorData CurrentUnit { get; set; }

        public UnitForEditor UnitPrefab => _unitPrefab;
        public GameplayData Data => _gameplayData;

        [SerializeField] private Map _currentMap;

        [Header("Object References")]
        [SerializeField] private GameplayData _gameplayData;
        [SerializeField] private TilePreview _previewObj;
        [SerializeField] private UnitForEditor _unitPrefab;
        
        [Header("Button References")]
        [SerializeField] private TileButton _tileButtonPrefab;
        [SerializeField] private TilemapButton _tilemapButtonPrefab;
        [SerializeField] private UnitButton _unitButtonPrefab;
        [SerializeField] private Button _loadMapButton;

        [Header("Layout References")]
        [SerializeField] private LayoutGroup _scrollViewContentLayout;
        [SerializeField] private LayoutGroup _unitsButtonLayout;
        [SerializeField] private LayoutGroup _tilemapsButtonLayout;

        [Header("Map References")]
        [SerializeField] private Transform _savePanel;
        [SerializeField] private Transform _loadPanel;

        private Action _updateCurrentTile;
        private Action _updateCurrentUnit;
        private Camera _mainCamera;
        private List<string> _filesButtons = new List<string>();
        private TilemapButton[] _tilemapButtons;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("There is already an Instance in the scene");
            }
        }

        private void Start()
        {
            _tilemapButtons = new TilemapButton[_currentMap.TilemapDatas.Length];
            _mainCamera = Camera.main;
            CurrentTile = null;

            SetCameraPosition();

            _updateCurrentTile += UpdateTilePreview;
            _updateCurrentUnit += UpdateTilePreview;

            CreateTilemapButtons();
            CreateTileButtons();
            CreateUnitButtons();
        }

        public void SetCurrentTile(TileData tile)
        {
            CurrentTile = tile;
            CurrentUnit = null;
            _updateCurrentTile?.Invoke();
        }

        public void SetCurrentUnit(UnitForEditorData data)
        {
            CurrentUnit = data;
            CurrentTile = null;
            _updateCurrentUnit?.Invoke();
        }

        /// <summary>
        /// Set the camera to the left bottom corner of the map.
        /// </summary>
        private void SetCameraPosition()
        {
            float halfWidth = _mainCamera.orthographicSize * Screen.width / Screen.height;
            float halfHeight = _mainCamera.orthographicSize;

            _mainCamera.transform.position = new Vector3(halfWidth, halfHeight, -10);
        }

        private void Update()
        {
            AddTileAndUnitToScene();
        }

        /// <summary>
        /// Detects if the player is clicking and place a tile or a unit.
        /// </summary>
        private void AddTileAndUnitToScene()
        {
            if (MenuIsOpen())
            {
                return;
            }

            Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenToWorldPoint.z = 0;
            Vector3Int cellPos = _currentMap.Grid.WorldToCell(screenToWorldPoint);

            _previewObj.SetPosition(new Vector3(cellPos.x, cellPos.y, 0));

            if (IsOverUI(Input.mousePosition))
            {
                return;
            }

            if (IsInZone(screenToWorldPoint) == false)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(1) && CurrentTile != null)
            {
                SetCurrentTile(null);
            }

            if (Input.GetMouseButton(0) && CurrentTile != null)
            {
                _currentMap.AddTileToMap(CurrentTile, cellPos);
            }

            if (Input.GetMouseButtonDown(0) && CurrentUnit != null)
            {
                Sprite sprite = null;
                foreach (var unit in Data.Units)
                {
                    if (unit.UnitType == CurrentUnit.UnitType)
                    {
                        sprite = unit.Sprite;
                    }
                }
                _currentMap.AddUnitToMap(UnitPrefab, CurrentUnit, screenToWorldPoint, sprite);
            }
        }

        /// <summary>
        /// Is in a grid boundaries.
        /// </summary>
        /// <param name="position"> The position we want to check. </param>
        /// <returns></returns>
        private bool IsInZone(Vector3 position)
        {
            return position.x > 0 && position.x < _currentMap.MapSize.x && position.y < _currentMap.MapSize.y && position.y > 0;
        }

        public void DisplaySavePanel()
        {
            _savePanel.gameObject.SetActive(!_savePanel.gameObject.activeSelf && _loadPanel.gameObject.activeSelf == false);
        }

        public void DisplayLoadPanel()
        {
            _loadPanel.gameObject.SetActive(!_loadPanel.gameObject.activeSelf && _savePanel.gameObject.activeSelf == false);

            foreach (FileInfo file in GetMapSaves())
            {
                string fileName = Path.GetFileNameWithoutExtension(file.Name);

                if (!_filesButtons.Contains(fileName))
                {
                    Button newButton = Instantiate(_loadMapButton, _loadPanel);

                    newButton.GetComponentInChildren<TMP_Text>().text = fileName;
                    newButton.onClick.AddListener(
                        () =>
                        {
                            _currentMap.LoadMap(fileName);
                            DisplayLoadPanel();
                        });

                    _filesButtons.Add(fileName);
                }
            }
        }

        /// <summary>
        /// Get the saves from the saves folder.
        /// </summary>
        /// <returns> Return a list of the files. </returns>
        private List<FileInfo> GetMapSaves()
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Saves");
            FileInfo[] info = dir.GetFiles("*.json");

            foreach (FileInfo file in info)
            {
                files.Add(file);
            }

            return files;
        }

        /// <summary>
        /// Update the sprite of the tile preview when click on a new tile button.
        /// </summary>
        private void UpdateTilePreview()
        {
            if (CurrentTile != null && CurrentUnit == null)
            {
                _previewObj.SpriteRenderer.sprite = CurrentTile.Tile.sprite;
            }
            else
            {
                _previewObj.SpriteRenderer.sprite = null;
            }
        }

        /// <summary>
        /// Instantiate the tiles buttons in the scroll view.
        /// </summary>
        private void CreateTileButtons()
        {
            foreach (TilemapData tilemap in _currentMap.TilemapDatas)
            {
                foreach (var tile in tilemap.TilesDataAssociated)
                {
                    TileButton newTileButton = Instantiate(_tileButtonPrefab, _scrollViewContentLayout.transform);
                    newTileButton.SetTileDisplay(tile.Data, tile.Data.Tile.sprite);
                    tilemap.TilesButtonsAssociated.Add(newTileButton);
                    newTileButton.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Instantiate the tilemap buttons in the viewport.
        /// </summary>
        private void CreateTilemapButtons()
        {
            for (int index = 0; index < _currentMap.TilemapDatas.Length; index++)
            {
                TilemapData tilemap = _currentMap.TilemapDatas[index];
                TilemapButton button = Instantiate(_tilemapButtonPrefab, _tilemapsButtonLayout.transform);

                button.GetComponentInChildren<TMP_Text>().text = tilemap.CurrentTilemap.name;
                button.SetTilemapData(tilemap);

                _tilemapButtons[index] = button;
            }
        }

        /// <summary>
        /// Instantiate the unit buttons in the viewport.
        /// </summary>
        private void CreateUnitButtons()
        {
            foreach (UnitData unit in Data.Units)
            {
                UnitButton newButton = Instantiate(_unitButtonPrefab, _unitsButtonLayout.transform);
                newButton.SetUnitData(
                    new UnitForEditorData
                    {
                        UnitType = unit.UnitType,
                        Position = default
                    },
                    unit.Sprite);
            }
        }

        /// <summary>
        /// Set active false all the tiles buttons that are not associated to the tilemaps.
        /// </summary>
        /// <param name="currentTilemapButton"> The button to check. </param>
        public void DontDisplayTiles(TilemapButton currentTilemapButton)
        {
            foreach (TilemapButton button in _tilemapButtons)
            {
                if (currentTilemapButton == button)
                {
                    continue;
                }

                foreach (TileButton tileButton in button.TilemapData.TilesButtonsAssociated)
                {
                    tileButton.gameObject.SetActive(false);
                }
            }
        }

        private bool IsOverUI(Vector2 position)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = position;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            return raycastResults.Count > 0 || EventSystem.current.currentSelectedGameObject != null;
        }

        public bool MenuIsOpen()
        {
            return _loadPanel.gameObject.activeSelf || _savePanel.gameObject.activeSelf;
        }
    }
}