using System;
using System.Collections;
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

        public TileData CurrentTile { get; set; }
        [field: SerializeField] public TilemapData[] TilemapDatas { get; private set; }
        public Action UpdateCurrentTile;

        [SerializeField] private Map _currentMap;

        [Header("Object References")]
        [SerializeField] private TileButton _tileButtonPrefab;
        [SerializeField] private Button _tilemapButtonPrefab;
        [SerializeField] private SpriteRenderer _tilePreviewObj;

        [Header("Layout References")]
        [SerializeField] private LayoutGroup _scrollViewContentLayout;
        [SerializeField] private LayoutGroup _tilemapsButtonLayout;

        [Header("Map References")]
        [SerializeField] private Transform _savePanel;
        [SerializeField] private Transform _loadPanel;
        [SerializeField] private Button _loadMapButton;

        private Camera _mainCamera;
        private List<string> _filesButtons = new List<string>();

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
            _mainCamera = Camera.main;

            SetCameraPosition();

            UpdateCurrentTile += UpdateTilePreview;

            CurrentTile = null;

            foreach (TilemapData tilemap in TilemapDatas)
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

            // CreateTilemapButtons();
            CreateTileButtons();
        }

        public void SetCurrentTile(TileData tile)
        {
            CurrentTile = tile;
            UpdateCurrentTile();
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
            if (CurrentTile == null)
            {
                return;
            }

            if (_savePanel.gameObject.activeSelf || _loadPanel.gameObject.activeSelf)
            {
                return;
            }

            Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = _currentMap.Grid.WorldToCell(screenToWorldPoint);

            _tilePreviewObj.transform.parent.position = new Vector3(cellPos.x, cellPos.y, 0);

            if (!Input.GetMouseButton(0) || IsOverUI(Input.mousePosition))
            {
                return;
            }

            if (cellPos.x < 0 || cellPos.x > _currentMap.MapSize.x || cellPos.y > _currentMap.MapSize.y ||
                cellPos.y < 0)
            {
                return;
            }

            _currentMap.AddTileToMap(CurrentTile, cellPos);
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
            foreach (TilemapData tilemap in TilemapDatas)
            {
                foreach (TileData tile in tilemap.TilesDataAssociated)
                {
                    TileButton newTileButton = Instantiate(_tileButtonPrefab, _scrollViewContentLayout.transform);
                    newTileButton.SetTileDisplay(tile, tile.Tile.sprite);
                }
            }
        }

        /// <summary>
        /// Instantiate the tilemap buttons in the viewport.
        /// </summary>
        private void CreateTilemapButtons()
        {
            foreach (TilemapData tilemap in TilemapDatas)
            {
                Button button = Instantiate(_tilemapButtonPrefab, _tilemapsButtonLayout.transform);
                button.GetComponentInChildren<TMP_Text>().text = tilemap.CurrentTilemap.name;
            }
        }

        /// <summary>
        /// Detect if mouse is over UI or not.
        /// </summary>
        /// <param name="position"> Mouse position. </param>
        /// <returns></returns>
        private bool IsOverUI(Vector2 position)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = position;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            return raycastResults.Count > 0 || EventSystem.current.currentSelectedGameObject != null;
        }
    }
}