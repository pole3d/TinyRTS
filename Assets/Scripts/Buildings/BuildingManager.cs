using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class BuildingManager : MonoBehaviour {
        
        public List<Building> BuildingPrefabs;
        public Camera Camera;
        public float CellSize;

        private Building _buildingToPlace;

        void Start()
        {
            
        }

        void Update()
        {
            if (_buildingToPlace != null) {
                UpdateBuildingShadow();
                
                if (Input.GetMouseButtonDown(0)) {
                    _buildingToPlace.SpriteRenderer.color = Color.white;
                    _buildingToPlace = null;
                }
            }

            if (Input.GetKeyDown(KeyCode.U)) {
                PlaceBuilding("Gold Mine");
            }
        }

        private void UpdateBuildingShadow() {
            Vector3 pos = Camera.ScreenToWorldPoint(Input.mousePosition);
            
            _buildingToPlace.transform.position = new Vector3(
                    Mathf.FloorToInt(pos.x / CellSize) * CellSize + 1,
                    Mathf.FloorToInt(pos.y / CellSize) * CellSize + 1,
                    0
            );
        }

        public void PlaceBuilding(string name) {
            _buildingToPlace = Instantiate(BuildingPrefabs.Find(building => building.Name == name));
            _buildingToPlace.SpriteRenderer.color = Color.green;
        }
    }
}

