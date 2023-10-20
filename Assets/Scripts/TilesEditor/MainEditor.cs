using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace TilesEditor
{
    public class MainEditor : MonoBehaviour
    {
        public static MainEditor Instance;
        
        [field: SerializeField] public Tile CurrentTile { get; private set; }
        
        [SerializeField] private Map _currentMap;
        [SerializeField] private List<Tile> _tiles = new List<Tile>();
        [SerializeField] private Transform _scrollViewContentTransform;
        [SerializeField] private TileButton _tileButtonPrefab;

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
            
            SetTileButtons();
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
            if (Input.GetMouseButton(0))
            {
                Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = _currentMap.Grid.WorldToCell(screenToWorldPoint);

                // _currentMap.SetTile(_testTile, cellPos);
            }
        }

        /// <summary>
        /// Instantiate the tiles buttons in the scroll view.
        /// </summary>
        private void SetTileButtons()
        {
            foreach (Tile tile in _tiles)
            {
                TileButton newTileButton = Instantiate(_tileButtonPrefab, _scrollViewContentTransform);
                newTileButton.SetTile(tile, tile.sprite);
            }
        }
        
        /// <summary>
        /// Detect if mouse is over UI or not.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        
        private bool IsOverUI(Vector2 position)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = position;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            return raycastResults.Count > 0 || EventSystem.current.currentSelectedGameObject != null;
        }

        public void SetCurrentTile(Tile tile)
        {
            CurrentTile = tile;
        }
    }
}