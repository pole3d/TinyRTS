using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class CharacterPathfindingMovementHandler : PathNodeOccupier
    {
        [SerializeField] private float _speed = 40f;

        private int _currentPathIndex;
        private List<Vector3> _pathVectorList = new List<Vector3>();
        private int _lastIndex = 0;

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            if (_pathVectorList != null && _pathVectorList.Count > 0)
            {
                PathFinding pathFinding = PathFinding.Instance;

                Vector3 targetPosition = _pathVectorList[_currentPathIndex];
                Vector3 currentPosition = transform.position;
                float distanceToTargetPosition = Vector3.Distance(currentPosition, targetPosition);

                if (distanceToTargetPosition > 1f) //if not close
                {
                    //pathFinding.SetPathReserved(_pathVectorList, _currentPathIndex, this);

                    Vector3 moveDirection = (targetPosition - currentPosition).normalized;
                    transform.position = currentPosition + moveDirection * (_speed * Time.deltaTime);
                }
                else //if close
                {
                    _currentPathIndex++;


                    if (_currentPathIndex >= _pathVectorList.Count)
                    {
                        StopMoving();
                        //stop animator
                        return;
                    }
                    //else
                    //{
                    //    //pathFinding.SetPathReserved(_pathVectorList, _currentPathIndex, this);
                    //    SetTargetPosition(_pathVectorList[^1]);
                    //}

                    //if (_currentPathIndex < _pathVectorList.Count - 1)
                    //{
                    //    //next
                    //    pathFinding.SetPathReserved(_pathVectorList, _currentPathIndex + 1, this);
                    //}

                    //_lastIndex = _currentPathIndex;
                }
            }
            else
            {
                //stop animator
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
            //if (_pathVectorList != null && _pathVectorList.Count > 0)
            //{
            //    PathFinding.Instance.ResetNodeWalkable(_pathVectorList, _lastIndex);
            //}

            _currentPathIndex = 0;

            _pathVectorList = PathFinding.Instance.FindPath(GetPosition(), targetPos);

            //PathFinding.Instance.SetPathReserved(_pathVectorList, _currentPathIndex, null);

            if (_pathVectorList != null && _pathVectorList.Count > 1)
            {
                _pathVectorList.RemoveAt(0);
            }
        }

    }
}
