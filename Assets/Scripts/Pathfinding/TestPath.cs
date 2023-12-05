using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class TestPath : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] float _cellSize;
    [SerializeField] Transform _textParent;
    private List<Vector3> _pathVectorList = new List<Vector3>();

    [SerializeField] List<CharacterPathfindingMovementHandler> characters;

    private PathFinding _pathFinding;
    private void Start()
    {
        _pathFinding = new PathFinding(_width, _height, _textParent, _cellSize);
    }
    public int NBPath = 1;
    int nb_calculate = 0;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //foreach (var character in characters)
            //{
            //    character.SetTargetPosition(GetMouseWorldPos());
            //}
            //characters[0].SetTargetPosition(GetMouseWorldPos());
            var time = Time.realtimeSinceStartupAsDouble;
            for (int i = 0; i < NBPath; i++)
            {
                _pathVectorList = PathFinding.Instance.FindPath(Vector3.zero, GetMouseWorldPos());
            }
            Debug.Log(time - Time.realtimeSinceStartupAsDouble + "ms" + nb_calculate + " nombre de fois");
            nb_calculate++;
        }

        if (Input.GetMouseButtonDown(1))
        {
            //take the case unwalkable (no visuale + temporary)
            Vector3 mouseWorldPos = GetMouseWorldPos();
            _pathFinding.Grid.GetXY(mouseWorldPos, out int x, out int y);
            _pathFinding.Grid.GetGridObject(x, y).SetIsWalkable(0);
            Debug.DrawLine(new Vector3(mouseWorldPos.x, mouseWorldPos.y - 2, 0), new Vector3(mouseWorldPos.x,mouseWorldPos.y+2,0),Color.red, 100f);
        }
    }

    public static Vector3 GetMouseWorldPos()
    {
        Vector3 vec = GetMouseWorldPosWithZ(Input.mousePosition, Camera.main);
        vec.z = 0;
        return vec;
    }
    public static Vector3 GetMouseWorldPosWithZ(Vector3 screenPos, Camera worldCam)
    {
        Vector3 worldPos = worldCam.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    private void OnDrawGizmos()
    {
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

}
