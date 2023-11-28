using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GizmosPathFinding : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        foreach (var item in PathFinding.Instance.OpenList)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(new Vector3((item.Coordinates.x * 5) + 5 /2, (item.Coordinates.y * 5)+5 / 2, 0), 2);
        }
        foreach (var item in PathFinding.Instance.ClosedList)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3((item.Coordinates.x * 5) + 5 / 2, (item.Coordinates.y * 5) + 5 / 2, 0), 2);
        }
    }



}
