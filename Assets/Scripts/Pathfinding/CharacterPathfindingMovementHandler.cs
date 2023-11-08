using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPathfindingMovementHandler : MonoBehaviour
{
    [SerializeField] float _speed = 40f;
    private int _currentPathIndex;
    private List<Vector3> _pathVectorList;

    void Update()
    {
        HandleMovement();
    }

    private int _lastIndex = 0;
    private void HandleMovement()
    {
        if (_pathVectorList != null)
        {
            Vector3 targetPos = _pathVectorList[_currentPathIndex];
            Vector3 position = transform.position;
            if (Vector3.Distance(position, targetPos) > 1f)
            {
                Vector3 moveDir = (targetPos - position).normalized;

                //float distanceBefore = Vector3.Distance(position, targetPos);
                transform.position = position + moveDir * _speed * Time.deltaTime;
            }
            else
            {
                PathFinding pathFinding = PathFinding.Instance;


                if (_currentPathIndex < _pathVectorList.Count)
                {
                    pathFinding.GetGrid().GetXY(_pathVectorList[_currentPathIndex], out int x, out int y);
                    if (pathFinding.GetNode(x, y).pathReserved != null && pathFinding.GetNode(x, y).pathReserved != this.gameObject)
                    {
                        SetTargetPosition(_pathVectorList[_pathVectorList.Count - 1]);

                    }
                }


                //old
                pathFinding.SetPathReserved(_pathVectorList, _currentPathIndex);

                _currentPathIndex++;

                //current
                if (_currentPathIndex >= _pathVectorList.Count)
                {
                    StopMoving();
                    //stop animator
                    return;
                }
                else
                {
                    pathFinding.SetPathReserved(_pathVectorList, _currentPathIndex, this.gameObject);
                }

                if (_currentPathIndex < _pathVectorList.Count - 1)
                {
                    //next
                    pathFinding.SetPathReserved(_pathVectorList, _currentPathIndex + 1, this.gameObject);
                }

                _lastIndex = _currentPathIndex;
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
        if (_pathVectorList != null) PathFinding.Instance.ResetNodeWalkable(_pathVectorList, _lastIndex);

        _currentPathIndex = 0;

        _pathVectorList = PathFinding.Instance.FindPath(GetPosition(), targetPos);
        if (_pathVectorList != null && _pathVectorList.Count > 1)
        {
            _pathVectorList.RemoveAt(0);
        }
    }

}
