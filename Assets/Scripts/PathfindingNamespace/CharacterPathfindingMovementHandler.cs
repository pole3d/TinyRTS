using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PathfindingNamespace
{
    public class CharacterPathfindingMovementHandler : PathNodeOccupier
    {
        [SerializeField] private float _speed = 0;

        private int _currentPathIndex;
        private List<Vector3> _pathVectorList = new List<Vector3>();

        private void Update()
        {
            HandleMovement();
        }

        private Vector3 targetEnd;

        float _timeBeforeNewPath = 1f;
        float _timerBeforeNewPath = 0;


        private void HandleMovement()
        {
            if (_pathVectorList != null && _pathVectorList.Count > 0)
            {
                Vector3 targetPosition = _pathVectorList[_currentPathIndex];

                Pathfinding.Instance.Grid.GetXY(targetPosition, out int targetX, out int targetY);
                PathNode targetNode = Pathfinding.Instance.Grid.GetGridObject(targetX, targetY);
                if (targetNode.NodeOccupier == null || targetNode.NodeOccupier == this)
                {
                    _timerBeforeNewPath = _timeBeforeNewPath;

                    if (_currentPathIndex > 0)
                    {
                        Pathfinding.Instance.SetPathReserved(this, null, _pathVectorList, _currentPathIndex - 1);
                    }

                    Vector3 currentPosition = transform.position;
                    float distanceToTargetPosition = Vector3.Distance(currentPosition, targetPosition);

                    if (distanceToTargetPosition > .05f) //if not close
                    {
                        Vector3 moveDirection = (targetPosition - currentPosition).normalized;
                        transform.position = currentPosition + moveDirection * (_speed * Time.deltaTime);

                        Pathfinding.Instance.SetPathReserved(this, this, _pathVectorList, _currentPathIndex);

                    }
                    else //if close
                    {
                        if (_currentPathIndex + 1 >= _pathVectorList.Count)
                        {
                            StopMoving();
                            //stop animator
                            return;
                        }


                        _currentPathIndex++;
                        //Pathfinding.Instance.SetPathReserved(this, _pathVectorList, _currentPathIndex - 1, null);

                        //Pathfinding.Instance.SetPathReserved(_pathVectorList, _currentPathIndex - 1, null);
                    }
                }
                else
                {
                    _timerBeforeNewPath -= Time.deltaTime;

                    if (_timerBeforeNewPath <= 0)
                    {
                        targetEnd = _pathVectorList[^1] + _offset;

                        if (_currentPathIndex > 0)
                        {
                            Pathfinding.Instance.SetPathReserved(this, this, _pathVectorList, _currentPathIndex - 1);
                        }


                        StopMoving();
                        SetTargetPosition(targetEnd);

                    }
                }
            }
            else
            {
                //stop animator
                Pathfinding.Instance.Grid.GetXY(transform.position, out int x, out int y);
                PathNode node = Pathfinding.Instance.Grid.GetGridObject(x, y);
                if (node.NodeOccupier != this)
                {
                    Pathfinding.Instance.Grid.GetGridObject(x, y).SetPathOwned(this);
                }
            }

        }

        private void StopMoving()
        {

            _pathVectorList = null;
        }
        public Vector3 GetPosition()
        {
            return transform.position;
        }


        public void SetTargetPosition(Vector3 targetPos)
        {
            //reset node if moving when set target
            if (_pathVectorList != null && _pathVectorList.Count > 0)
            {
                Pathfinding.Instance.ResetNodeWalkable(this, _pathVectorList, _currentPathIndex);
                _pathVectorList = Pathfinding.Instance.FindPath(this, _pathVectorList[_currentPathIndex], targetPos);
                _currentPathIndex = 0;
                //Pathfinding.Instance.SetPathReserved(_pathVectorList, _currentPathIndex, null);
                //Pathfinding.Instance.SetPathReserved(_pathVectorList, _pathVectorList.Count - 1, null);
            }
            else
            {
                _currentPathIndex = 0;
                _pathVectorList = Pathfinding.Instance.FindPath(this, GetPosition(), targetPos);
            }
        }

        Vector3 _offset = Vector3.zero;


        public List<Vector3> GetPathList => _pathVectorList;

        public void SetPath(List<Vector3> path, Vector3 offset)
        {
            _pathVectorList = path;
            _offset = offset;
        }

    }
}
