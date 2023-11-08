



using System.Collections.Generic;
using UnityEngine;

class PathNode
{
    public int X;
    public int Y;
    public int GCost;
    public int HCost;
    public int FCost;

    public List<PathNode> Neighbours = new List<PathNode>();
    public PathNode cameFromNode = null;
    public bool IsWalkable;

    public GameObject pathReserved = null;

    public PathNode(int x, int y)
    {
        this.X = x;
        this.Y = y;
        IsWalkable = true;
    }

    public int CalculateFCost()
    {
        return GCost + HCost;
    }
    public void SetIsWalkable(bool isWalkable)
    {
        IsWalkable = isWalkable;
    }

    public void ReservedPath(GameObject owner = null)
    {
        IsWalkable = owner == null ? true : false;
        pathReserved = owner;
    }

    public override string ToString()
    {
        return $"{X}x, {Y}y";
    }
}