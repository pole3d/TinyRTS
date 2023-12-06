using System;
using System.Collections.Generic;
using System.IO;
using TilesEditor.Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TileData = TilesEditor.Tiles.TileData;

namespace TilesEditor
{
    /// <summary>
    /// Handle the editor system.
    /// </summary>
    public class TilesEditor : MonoBehaviour
    {
        public static TilesEditor Instance;

        private TileData CurrentTile { get; set; }

        [SerializeField] private Map _currentMap;

        [Header("Object References")]
        [SerializeField] private TileButton _tileButtonPrefab;
        [SerializeField] private TilemapButton _tilemapButtonPrefab;
        [SerializeField] private SpriteRenderer _tilePreviewObj;

        [Header("Layout References")]
        [SerializeField] private LayoutGroup _scrollViewContentLayout;
        [SerializeField] private LayoutGroup _tilemapsButtonLayout;

        [Header("Map References")]
        [SerializeField] private Transform _savePanel;
        [SerializeField] private Transform _loadPanel;
        [SerializeField] private Button _loadMapButton;

        private Action _updateCurrentTile;
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
                Debug.Log("There is already an Instance in the scene");
            }
        }

        private void Start()
        {
            _tilemapButtons = new TilemapButton[_currentMap.TilemapDatas.Length];
            _mainCamera = Camera.main;
            CurrentTile = null;

            SetCameraPosition();

            _updateCurrentTile += UpdateTilePreview;

            foreach (TilemapData tilemap in _currentMap.TilemapDatas)
            {
                foreach (Tile tile in tilemap.TilesAssociated)
                {
                    tilemap.TilesDataAssociated.Add(
                        new TileData
                        {
                            Tile = tile,
                            AssociatedTilemap = null
                        });
                }
            }

            CreateTilemapButtons();
            CreateTileButtons();
        }

        /// <summary>
        /// Clear all the tiles of the map but set the background.
        /// </summary>
        public void ResetMap()
        {
            foreach (TilemapData tilemap in _currentMap.TilemapDatas)
            {
                tilemap.CurrentTilemap.ClearAllTiles();
            }

            _currentMap.FillMap();
        }

        public void SetCurrentTile(TileData tile)
        {
            CurrentTile = tile;
            _updateCurrentTile();
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
            PaintMap();
        }

        /// <summary>
        /// If the player is pressing the left button on the mouse, it will paint the selected tile.
        /// </summary>
        private void PaintMap()
        {
            if (CurrentTile == null || MenuIsOpen())
            {
                return;
            }

            Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = _currentMap.Grid.WorldToCell(screenToWorldPoint);

            if (IsOverUI(Input.mousePosition))
            {
                return;
            }

            _tilePreviewObj.transform.position = new Vector3(cellPos.x, cellPos.y, 0);

            if (!Input.GetMouseButton(0))
            {
                return;
            }

            if (IsInZone(cellPos) == false)
            {
                return;
            }

            _currentMap.AddTileToMap(CurrentTile, cellPos);
        }

        public bool IsInZone(Vector3 position)
        {
            return position.x > 0 && position.x < _currentMap.MapSize.x && position.y < _currentMap.MapSize.y && position.y > 0;
        }

        public void DisplaySavePanel()
        {
            _savePanel.gameObject.SetActive(!_savePanel.gameObject.activeSelf);
        }

        /// <summary>
        /// Display the load panel.
        /// </summary>
        public void DisplayLoadPanel()
        {
            _loadPanel.gameObject.SetActive(!_loadPanel.gameObject.activeSelf);

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
            _tilePreviewObj.sprite = CurrentTile.Tile.sprite;
        }

        /// <summary>
        /// Instantiate the tiles buttons in the scroll view.
        /// </summary>
        private void CreateTileButtons()
        {
            foreach (TilemapData tilemap in _currentMap.TilemapDatas)
            {
                foreach (TileData tile in tilemap.TilesDataAssociated)
                {
                    TileButton newTileButton = Instantiate(_tileButtonPrefab, _scrollViewContentLayout.transform);
                    newTileButton.SetTileDisplay(tile, tile.Tile.sprite);
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
        /// Set active false all the tiles buttons that are not 
        /// </summary>
        /// <param name="currentTilemapButton"></param>
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

        /// <summary>
        /// Detect if mouse is over UI or not.
        /// </summary>
        /// <param name="position"> Mouse position. </param>
        /// <returns></returns>
        public bool IsOverUI(Vector2 position)
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