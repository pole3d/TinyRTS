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
    public class MainEditor : MonoBehaviour
    {
        public static MainEditor Instance;

        public TileData CurrentTile { get; set; }
        [field: SerializeField] public TilemapData[] TilemapDatas { get; private set; }
        public Action UpdateCurrentTile;

        [SerializeField] private Map _currentMap;

        [Header("Object References")] [SerializeField]
        private TileButton _tileButtonPrefab;

        [SerializeField] private Button _tilemapButtonPrefab;
        [SerializeField] private SpriteRenderer _tilePreviewObj;

        [Header("Layout References")] [SerializeField]
        private LayoutGroup _scrollViewContentLayout;

        [SerializeField] private LayoutGroup _tilemapsButtonLayout;

        private Camera _mainCamera;

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
                    tilemap.TilesDataAssociated.Add(new TileData
                    {
                        Tile = tile,
                        TilePosition = default,
                        AssociatedTilemap = null
                    });
                }
            }

            // CreateTilemapButtons();
            CreateTileButtons();
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

            Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = _currentMap.Grid.WorldToCell(screenToWorldPoint);

            _tilePreviewObj.transform.parent.position = new Vector3(cellPos.x, cellPos.y, 0);

            if (!Input.GetMouseButton(0) || IsOverUI(Input.mousePosition))
            {
                return;
            }

            _currentMap.AddTileToMap(CurrentTile, cellPos);
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