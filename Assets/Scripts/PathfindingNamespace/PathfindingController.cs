using Selection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PathfindingNamespace
{
    /// <summary>
    /// Check and control the pathfinding requests for selected characters
    /// </summary>
    public class PathfindingController : MonoBehaviour
    {
        public Pathfinding Pathfinding { get; private set; }
        
        [SerializeField] private UnitSelectionController _unitSelectionController;
        [SerializeField] private Tilemap _baseTileMap;

        public void Initialize()
        {
            Vector3Int size = _baseTileMap.size;
            Pathfinding = new Pathfinding(size.x, size.y, 1f);
        }

        private void Update()
        {
            CheckMovementInput();
        }

        /// <summary>
        /// Check if the player has made an input to request units to move toward a position
        /// </summary>
        private void CheckMovementInput()
        {
            if (Input.GetMouseButtonDown(1) == false)
            {
                return;
            }

            Debug.Log("movement pathfinding input");
            
            foreach (UnitSelectable unit in _unitSelectionController.SelectedUnitList)
            {
                if (unit.TryGetComponent(out CharacterPathfindingMovementHandler unitPathfinding) == false)
                {
                    continue;
                }
                
                Vector3 position = Utils.GetMouseWorldPosition();
                unitPathfinding.SetTargetPosition(position);
            }
        }
    }
}
