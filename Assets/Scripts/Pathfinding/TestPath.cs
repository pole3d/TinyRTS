using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPath : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] float _cellSize;
    [SerializeField] Transform _textParent;

    [SerializeField] List<CharacterPathfindingMovementHandler> characters;

    private PathFinding pathFinding;
    void Start()
    {
        pathFinding = new PathFinding(_width, _height, _textParent, _cellSize);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var character in characters)
            {
                character.SetTargetPosition(GetMouseWorldPos());
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            //take the case unwalkable (no visuale + temporary)
            Vector3 mouseWorldPos = GetMouseWorldPos();
            pathFinding.GetGrid().GetXY(mouseWorldPos, out int x, out int y);
            pathFinding.GetNode(x, y).SetIsWalkable(!pathFinding.GetNode(x, y).IsWalkable);
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
