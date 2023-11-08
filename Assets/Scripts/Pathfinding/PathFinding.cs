using System;
using System.Collections.Generic;
using UnityEngine;

internal class PathFinding
{
    const int MOVE_STRAIGHT_COST = 10;
    const int MOVE_DIAG_COST = 14;

    public static PathFinding Instance { get; private set; }

    private ShowGrid<PathNode> _grid;
    private List<PathNode> _openList;
    private List<PathNode> _closedList;

    private PathNode _closestToTarget = null;

    public PathFinding(int width, int height, float cellSize = 10)
    {
        if (Instance == null) Instance = this;
        _grid = new ShowGrid<PathNode>(width, height, cellSize, Vector3.zero, (ShowGrid<PathNode> grid, int x, int y) => new PathNode(x, y));
    }
    public PathFinding(int width, int height, Transform parent, float cellSize = 10)
    {
        if (Instance == null) Instance = this;
        _grid = new ShowGrid<PathNode>(width, height, cellSize, Vector3.zero, (ShowGrid<PathNode> grid, int x, int y) => new PathNode(x, y), parent);
    }

    public void ResetNodeWalkable(List<Vector3> listPos, int index)
    {
        if (listPos.Count > 0)
            SetPathReserved(listPos, index);

        if (listPos.Count > 1 && index < listPos.Count - 1)
        {
            //next
            SetPathReserved(listPos, index + 1);
        }
    }
    public void SetPathReserved(List<Vector3> listPos, int index, GameObject owner = null)
    {
        GetGrid().GetXY(listPos[index], out int x, out int y);
        GetNode(x, y).ReservedPath(owner);
    }
    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        _grid.GetXY(startWorldPosition, out int startX, out int startY);
        _grid.GetXY(endWorldPosition, out int endX, out int endY);
        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null) return null;
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.X, pathNode.Y) * _grid.GetCellSize() + Vector3.one * _grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = _grid.GetGridObject(startX, startY);
        PathNode endNode = _grid.GetGridObject(endX, endY);

        _openList = new List<PathNode> { startNode };
        _closedList = new List<PathNode>();

        InitGrid();

        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startNode, endNode);
        startNode.FCost = startNode.CalculateFCost();

        while (_openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(_openList);

            if (currentNode == endNode)
            {
                return CalculatePath(endNode);

            }

            _openList.Remove(currentNode);
            _closedList.Add(currentNode);

            foreach (PathNode neighbourNode in currentNode.Neighbours)
            {
                if (_closedList.Contains(neighbourNode) == true) continue;
                if (neighbourNode.IsWalkable == false)
                {
                    _closedList.Add(neighbourNode);
                    continue;
                }
                int tentativeGCost = currentNode.GCost + CalculateDistance(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.GCost)
                {
                    neighbourNode.GCost = tentativeGCost;
                    neighbourNode.HCost = CalculateDistance(neighbourNode, endNode);
                    neighbourNode.FCost = neighbourNode.CalculateFCost();
                    neighbourNode.cameFromNode = currentNode;
                    if (_openList.Contains(neighbourNode) == false) _openList.Add(neighbourNode);
                }

            }

        }
        if (_closestToTarget != null)
            return CalculatePath(_closestToTarget);


        return null;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> paths = new List<PathNode>();
        paths.Add(endNode);
        PathNode current = endNode;

        while (current.cameFromNode != null)
        {
            paths.Add(current.cameFromNode);
            current = current.cameFromNode;
        }
        paths.Reverse();
        return paths;
    }

    public PathNode GetNode(PathNode current, int x, int y)
    {
        return _grid.GetGridObject(current.X + x, current.Y + y);
    }

    public PathNode GetNode(int x, int y)
    {
        return _grid.GetGridObject(x, y);
    }
    public ShowGrid<PathNode> GetGrid()
    {
        return _grid;
    }

    private List<PathNode> GetNeighbour(PathNode current)
    {
        List<PathNode> neighbour = new List<PathNode>();

        if (current.X - 1 >= 0)
        {
            //Left
            neighbour.Add(GetNode(current, -1, 0));
            //Left Down
            if (current.Y - 1 >= 0) neighbour.Add(GetNode(current, -1, -1));
            //Left up
            if (current.Y + 1 < _grid.GetHeight()) neighbour.Add(GetNode(current, -1, 1));
        }

        if (current.X + 1 < _grid.GetWidth())
        {
            //Right
            neighbour.Add(GetNode(current, 1, 0));
            //Right Down
            if (current.Y - 1 >= 0) neighbour.Add(GetNode(current, 1, -1));
            //Right Up
            if (current.Y + 1 < _grid.GetHeight()) neighbour.Add(GetNode(current, 1, 1));
        }
        //Down
        if (current.Y - 1 >= 0) neighbour.Add(GetNode(current, 0, -1));
        //Up
        if (current.Y + 1 < _grid.GetHeight()) neighbour.Add(GetNode(current, 0, 1));

        return neighbour;
    }


    private PathNode GetLowestFCostNode(List<PathNode> openList)
    {
        PathNode lowestFCost = openList[0];

        foreach (var item in openList)
        {
            if (lowestFCost.FCost > item.FCost)
            {
                lowestFCost = item;
            }

            if (_closestToTarget != null && _closestToTarget.HCost > item.HCost) _closestToTarget = item;
            else if (_closestToTarget == null) _closestToTarget = item;
        }

        return lowestFCost;
    }
    private void InitGrid()
    {
        for (int y = 0; y < _grid.GetWidth(); y++)
        {
            for (int x = 0; x < _grid.GetHeight(); x++)
            {
                PathNode pathNode = _grid.GetGridObject(x, y);

                pathNode.Neighbours = GetNeighbour(pathNode);

                pathNode.GCost = int.MaxValue;
                pathNode.FCost = pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
    }
    private int CalculateDistance(PathNode a, PathNode b)
    {
        int xDist = Math.Abs(a.X - b.X);
        int yDist = Math.Abs(a.Y - b.Y);
        int remaining = Math.Abs(xDist - yDist);

        return MOVE_DIAG_COST * Math.Min(xDist, yDist) + MOVE_STRAIGHT_COST * remaining;
    }
}





