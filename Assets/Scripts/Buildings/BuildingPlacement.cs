using System.Collections.Generic;
using GameManagement;
using UnityEngine;

namespace Buildings
{
    public class BuildingPlacement : MonoBehaviour
    {
        public Camera Camera;
        public Grid Grid;
        public List<Building> BuildingPrefabs;

        private Building _buildingToPlace;
        private Vector3 _previousMousePosition;

        void Update()
        {
            if (!IsPlacingBuilding())
            {
                return;
            }
            
            UpdateBuildingShadow();

            if (Input.GetMouseButtonDown(0))
            {
                _buildingToPlace.SpriteRenderer.color = Color.white;
                _buildingToPlace.IsGhost = false;

                foreach (Vector2Int position in _buildingToPlace.GetBuildingFootprint())
                {
                    GameManager.Instance.PathfindingController.SetTileNotWalkablePathfinding(position);
                }

                _buildingToPlace = null;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(_buildingToPlace.gameObject);

                _buildingToPlace = null;
            }
        }

        private bool IsPlacingBuilding()
        {
            return _buildingToPlace is not null;
        }

        private void UpdateBuildingShadow()
        {
            Vector3Int coordinates = GetMouseCoordinatesOnGrid();
            if (coordinates == _previousMousePosition)
            {
                return;
            }

            _previousMousePosition = coordinates;

            _buildingToPlace.transform.position = new Vector3(coordinates.x, coordinates.y);

            if (IsIllegalPosition()) 
            {
                _buildingToPlace.SpriteRenderer.color = Color.red;
            }
            else
            {
                _buildingToPlace.SpriteRenderer.color = Color.green;
            }
        }

        private bool IsIllegalPosition() {
            foreach (Vector2Int position in _buildingToPlace.GetBuildingFootprint())
            {
                if (GameManager.Instance.PathfindingController.Pathfinding.Grid.GetGridObject(position.x, position.y).IsWalkable == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private Vector3Int GetMouseCoordinatesOnGrid()
        {
            Vector3 pos = Camera.ScreenToWorldPoint(Input.mousePosition);

            return Grid.WorldToCell(pos);
        }

        public void PlaceBuilding(string name)
        {
            if (IsPlacingBuilding())
            {
                return;
            }
            
            _buildingToPlace = Instantiate(BuildingPrefabs.Find(building => building.Name == name));
            _buildingToPlace.SpriteRenderer.color = Color.green;
            _buildingToPlace.IsGhost = true;
            _buildingToPlace.PlayBuiltAnimation();
        }
    }
}