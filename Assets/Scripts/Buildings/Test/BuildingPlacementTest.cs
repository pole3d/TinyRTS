using UnityEngine;

namespace Buildings.Test
{
    public class BuildingPlacementTest : MonoBehaviour
    {
        public BuildingPlacement BuildingPlacement;

        public KeyCode KeyCode;
        public string BuildingName;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode))
            {
                BuildingPlacement.PlaceBuilding(BuildingName);
            }
        }
    }
}