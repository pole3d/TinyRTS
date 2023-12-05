using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Pathfinding
{
    public class DebugPathfindingController : MonoBehaviour
    {
        public int NumberOfPathToCalculate = 1;
    
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private float _cellSize;
        [SerializeField] private Transform _textParent;
        [SerializeField] List<CharacterPathfindingMovementHandler> _characters = new List<CharacterPathfindingMovementHandler>();
    
        private List<Vector3> _pathVectorList = new List<Vector3>();
        private PathFinding _pathFinding;

        private void Start()
        {
            _pathFinding = new PathFinding(_width, _height, _textParent, _cellSize);
        }
    
        private void Update()
        {
            CheckForPathfindingCall();
            SetTileNotWalkableFromClick();
        }

        private void CheckForPathfindingCall()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CalculatePathFindingToPosition(GetMouseWorldPosition());
            }
        }

        private void SetTileNotWalkableFromClick()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mouseWorldPos = GetMouseWorldPosition();
                _pathFinding.Grid.GetXY(mouseWorldPos, out int x, out int y);
                _pathFinding.Grid.GetGridObject(x, y).SetIsWalkable(0);
                Debug.DrawLine(new Vector3(mouseWorldPos.x, mouseWorldPos.y - 2, 0),
                    new Vector3(mouseWorldPos.x, mouseWorldPos.y + 2, 0), Color.red, 100f);
            }
        }

        private async void CalculatePathFindingToPosition(Vector3 position)
        {
            // double time = Time.realtimeSinceStartupAsDouble;
            //
            // for (int i = 0; i < NumberOfPathToCalculate; i++)
            // {
            //     _pathVectorList = PathFinding.Instance.FindPath(Vector3.zero, position);
            //     Debug.Log("calculated pathfinding " + i);
            //     await Task.Delay((int)(Time.deltaTime * 1000));
            // }
            //
            // Debug.Log(time - Time.realtimeSinceStartupAsDouble + "ms");
            
            foreach (var character in _characters)
            {
                await Task.Delay((int)(Time.deltaTime * 1000));
                character.SetTargetPosition(GetMouseWorldPosition());
            }
        }

        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            return mouseWorldPosition;
        }

#if UNITY_EDITOR
    
        private void OnDrawGizmos()
        {
            if (PathFinding.Instance == null || PathFinding.Instance.OpenList == null || PathFinding.Instance.OpenList.Count <= 0)
            {
                return;
            }
            foreach (var item in PathFinding.Instance.OpenList)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(new Vector3((item.Coordinates.x * 5) + 5 / 2, (item.Coordinates.y * 5) + 5 / 2, 0), 2);
            }
            foreach (var item in PathFinding.Instance.ClosedList)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector3((item.Coordinates.x * 5) + 5 / 2, (item.Coordinates.y * 5) + 5 / 2, 0), 2);
            }
            foreach (var item in _pathVectorList)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(new Vector3(item.x, item.y, 0), 2);
            }
        }
    
#endif
    }
}
