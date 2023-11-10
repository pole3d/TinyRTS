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

    [SerializeField] List<CharacterPathfindingMovementHandler> characters;

    private PathFinding _pathFinding;
    private void Start()
    {
        _pathFinding = new PathFinding(_width, _height, _textParent, _cellSize);
    }
    public int NBPath = 1000;
    int nb_calculate=0;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //foreach (var character in characters)
            //{
            //    character.SetTargetPosition(GetMouseWorldPos());
            //}
            var time = Time.realtimeSinceStartupAsDouble;
            for (int i = 0; i < NBPath; i++)
            {
                PathFinding.Instance.FindPath(Vector3.zero, GetMouseWorldPos());
                //characters[0].SetTargetPosition(GetMouseWorldPos());
            }
            Debug.Log(time - Time.realtimeSinceStartupAsDouble + "ms" + nb_calculate + " nombre de fois");
            nb_calculate++;
        }


        if (Input.GetMouseButtonDown(1))
        {
            //take the case unwalkable (no visuale + temporary)
            Vector3 mouseWorldPos = GetMouseWorldPos();
            _pathFinding.GetGrid().GetXY(mouseWorldPos, out int x, out int y);
            _pathFinding.GetNode(x, y).SetIsWalkable(0);
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
}
