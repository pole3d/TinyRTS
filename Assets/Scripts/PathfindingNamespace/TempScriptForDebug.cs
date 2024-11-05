using GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempScriptForDebug : MonoBehaviour
{
    public Vector3[] Pos;
    public Vector3 Offset = new Vector3(0.5f, 0.5f, 0);

    void Start()
    {
        for (int i = 0; i < Pos.Length; i++)
        {
            GameManager.Instance.PathfindingController.SetTileNotWalkablePathfinding(Pos[i]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < Pos.Length; i++)
        {
            Gizmos.DrawWireCube(Pos[i] + Offset, Vector3.one);
        }
    }
}
