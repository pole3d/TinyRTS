using Selection;
using System.Threading.Tasks;
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
        Vector3 unitInCenter;
        private void CheckMovementInput()
        {
            if (Input.GetMouseButtonDown(1) == false || _unitSelectionController.SelectedUnitList.Count <= 0)
            {
                return;
            }

            unitInCenter = _unitSelectionController.SelectedUnitList[0].transform.position;
            foreach (UnitSelectable unit in _unitSelectionController.SelectedUnitList)
            {
                if (unit.TryGetComponent(out CharacterPathfindingMovementHandler unitPathfinding) == false)
                {
                    continue;
                }

                Vector3 unitOffset = unit.transform.position;
                Vector3 offset = unitInCenter - unitOffset;
                offset.z = unitOffset.z;

                Vector3 position = Utils.GetMouseWorldPosition();

                if (Vector3.Distance(unitInCenter, unitOffset) > 1.5f)
                {
                    CalculatePathFindingToPosition(unitPathfinding, position - offset.normalized);
                }
                else
                {
                    CalculatePathFindingToPosition(unitPathfinding, position - offset);
                }





                //unitPathfinding.SetTargetPosition(position - offset);
            }
            unitInCenter = Vector3.zero;
        }
        private async void CalculatePathFindingToPosition(CharacterPathfindingMovementHandler unitPathfinding, Vector3 target)
        {
            unitPathfinding.SetTargetPosition(target);
            await Task.Delay((int)(Time.deltaTime * 1000));
        }
    }
}
