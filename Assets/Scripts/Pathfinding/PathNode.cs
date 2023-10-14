



using System.Collections.Generic;

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
    public override string ToString()
    {
        return $"{X}x, {Y}y";
    }
}