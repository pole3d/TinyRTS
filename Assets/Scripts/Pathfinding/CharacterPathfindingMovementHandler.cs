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
                _currentPathIndex++;
                if (_currentPathIndex >= _pathVectorList.Count)
                {
                    StopMoving();
                    //stop animator
                }
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
        _currentPathIndex = 0;
        _pathVectorList = PathFinding.Instance.FindPath(GetPosition(), targetPos);
        if (_pathVectorList != null && _pathVectorList.Count > 1)
        {
            _pathVectorList.RemoveAt(0);
        }
    }

}
