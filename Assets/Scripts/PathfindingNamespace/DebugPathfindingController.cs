using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PathfindingNamespace
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
        private Pathfinding _pathfinding;

        private void Start()
        {
            _pathfinding = new Pathfinding(_width, _height, _textParent, _cellSize);
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
                _pathfinding.Grid.GetXY(mouseWorldPos, out int x, out int y);
                _pathfinding.Grid.GetGridObject(x, y).SetIsWalkable(0);
                Debug.DrawLine(new Vector3(mouseWorldPos.x, mouseWorldPos.y - 2, 0),
                    new Vector3(mouseWorldPos.x, mouseWorldPos.y + 2, 0), Color.red, 1000000000f);
            }
        }

        private async void CalculatePathFindingToPosition(Vector3 position)
        {
            for (int i = 0; i < NumberOfPathToCalculate; i++)
            {
                //_pathVectorList = Pathfinding.Instance.FindPath(Vector3.zero, position);
                await Task.Delay((int)(Time.deltaTime * 1000));
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
           
            if (Pathfinding.Instance == null || Pathfinding.Instance.OpenList == null /*|| PathFinding.Instance.OpenList.Count <= 0*/)
            {
                return;
            }
            foreach (var item in Pathfinding.Instance.OpenList)
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
                Gizmos.DrawSphere(new Vector3((item.Coordinates.x * 5) + 5 / 2, (item.Coordinates.y * 5) + 5 / 2, 0), 0.5f);
            }
            foreach (var item in Pathfinding.Instance.ClosedList)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
                Gizmos.DrawSphere(new Vector3((item.Coordinates.x * 5) + 5 / 2, (item.Coordinates.y * 5) + 5 / 2, 0), 0.5f);
            }
            foreach (var item in _pathVectorList)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.4f);
                Gizmos.DrawSphere(new Vector3(item.x, item.y, 0), 0.5f);
            }
        }
    
#endif
    }
}
