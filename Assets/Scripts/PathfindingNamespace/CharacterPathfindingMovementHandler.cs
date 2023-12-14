using System.Collections.Generic;
using UnityEngine;

namespace PathfindingNamespace
{
    public class CharacterPathfindingMovementHandler : PathNodeOccupier
    {
        [SerializeField] private float _speed = 40f;

        private int _currentPathIndex;
        private List<Vector3> _pathVectorList = new List<Vector3>();

        private void Update()
        {
            HandleMovement();
        }

        private Vector3 targetEnd;

        private void HandleMovement()
        {
            if (_pathVectorList != null && _pathVectorList.Count > 0)
            {
                Vector3 targetPosition = _pathVectorList[_currentPathIndex];

                Pathfinding.Instance.Grid.GetXY(targetPosition, out int targetX, out int targetY);
                PathNode targetNode = Pathfinding.Instance.Grid.GetGridObject(targetX, targetY);

                if (targetNode.NodeOccupier != null && targetNode.NodeOccupier != this)
                {
                    targetEnd = _pathVectorList[^1];
                    StopMoving();
                    SetTargetPosition(targetEnd);
                }

                Vector3 currentPosition = transform.position;
                float distanceToTargetPosition = Vector3.Distance(currentPosition, targetPosition);

                if (distanceToTargetPosition > .05f) //if not close
                {
                    Vector3 moveDirection = (targetPosition - currentPosition).normalized;
                    transform.position = currentPosition + moveDirection * (_speed * Time.deltaTime);
                }
                else //if close
                {
                    if (_currentPathIndex + 1 >= _pathVectorList.Count)
                    {
                        StopMoving();
                        //stop animator
                        return;
                    }


                    Pathfinding.Instance.SetPathReserved(_pathVectorList, _currentPathIndex, null);
                    Debug.Log("c'est lui l'enfoiré");

                    _currentPathIndex++;


                    Pathfinding.Instance.SetPathReserved(_pathVectorList, _currentPathIndex, this);
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
            //reset node if moving when set target
            if (_pathVectorList != null && _pathVectorList.Count > 0)
            {
                Pathfinding.Instance.ResetNodeWalkable(_pathVectorList, _currentPathIndex);

                //Pathfinding.Instance.SetPathReserved(_pathVectorList, _currentPathIndex, null);
                //Pathfinding.Instance.SetPathReserved(_pathVectorList, _pathVectorList.Count - 1, null);
            }


            _currentPathIndex = 0;
            _pathVectorList = Pathfinding.Instance.FindPath(this, GetPosition(), targetPos);

            Pathfinding.Instance.SetPathReserved(_pathVectorList, _pathVectorList.Count - 1, this);

            //if (_pathVectorList != null && _pathVectorList.Count > 1)
            //{
            //    //Pathfinding.Instance.SetPathReserved(_pathVectorList, _currentPathIndex, null);
            //    Debug.Log("here?");
            //}
        }
    }
}
