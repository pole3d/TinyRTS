using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class BuildingPlacement : MonoBehaviour
    {
        public Camera Camera;
        public Grid Grid;
        public List<Building> BuildingPrefabs;

        private Building _buildingToPlace;

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

            _buildingToPlace.transform.position = new Vector3(
                    coordinates.x + 1,
                    coordinates.y + 1
            );
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
        }
    }
}