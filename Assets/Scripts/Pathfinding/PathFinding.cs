using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    internal class PathFinding
    {
        const int MOVE_STRAIGHT_COST = 10;
        const int MOVE_DIAG_COST = 14;

        public static PathFinding Instance;
        public Grid<PathNode> Grid;

        private List<PathNode> _openList;
        private List<PathNode> _closedList;

        private PathNode _closestToTarget = null;

        private PathNode[,] _openArray2D;
        private PathNode[,] _closedArray2D;

        #region Initialisation
        public PathFinding(int width, int height, float cellSize = 10)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("2 pathfinding");
            }

            Grid = new Grid<PathNode>(width, height, cellSize, Vector3.zero,
                (Grid<PathNode> grid, int x, int y) => new PathNode(x, y));

            Initialize(height, width);

        }

        public PathFinding(int width, int height, Transform parent, float cellSize = 10)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("2 pathfinding");
            }
            Grid = new Grid<PathNode>(width, height, cellSize, Vector3.zero,
                (Grid<PathNode> grid, int x, int y) => new PathNode(x, y), parent);

            Initialize(height, width);
        }

        private void Initialize(int height, int width)
        {
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    PathNode pathNode = Grid.GetGridObject(x, y);

                    pathNode.Neighbours = GetNeighbour(pathNode);

                    pathNode.GCost = int.MaxValue;
                    pathNode.FCost = pathNode.CalculateFCost();
                    pathNode.CameFromNode = null;
                }
            }

            _openArray2D = new PathNode[height, width];
            _closedArray2D = new PathNode[height, width];
        }

        private List<PathNode> GetNeighbour(PathNode current)
        {
            List<PathNode> neighbour = new List<PathNode>();

            Vector2Int[] directions = new Vector2Int[8]
           {
                new Vector2Int(0, 1),//down
                new Vector2Int(0, -1),//up
                new Vector2Int(-1, 0),//left
                new Vector2Int(-1, -1),//left down
                new Vector2Int(-1, 1),//left up
                new Vector2Int(1, 0),//right
                new Vector2Int(1, 1),//right up
                new Vector2Int(1, -1)//right down
           };


            foreach (Vector2Int direction in directions)
            {
                Vector2Int coordinateNodeToCheck = current.Coordinates + direction;
                if (coordinateNodeToCheck.x < 0
                    || coordinateNodeToCheck.y < 0
                    || coordinateNodeToCheck.x >= Grid.GetWidth()
                    || coordinateNodeToCheck.y > Grid.GetHeight())
                {
                    continue;
                }

                neighbour.Add(Grid.GetGridObject(current.X + direction.x, current.Y + direction.y));
            }

            return neighbour;
        }
        #endregion


        #region Path
        public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            Grid.GetXY(startWorldPosition, out int startX, out int startY);
            Grid.GetXY(endWorldPosition, out int endX, out int endY);
            List<PathNode> path = FindPath(startX, startY, endX, endY);
            if (path == null)
            {
                return null;
            }

            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.X, pathNode.Y) * Grid.GetCellSize() + Vector3.one * (Grid.GetCellSize() * .5f));
            }

            return vectorPath;
        }

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            if (_openList != null && _openList.Count > 0)
            {
                foreach (var item in _openList)
                {
                    item.GCost = int.MaxValue;
                    item.FCost = item.CalculateFCost();
                    item.CameFromNode = null;
                }
                _openList.Clear();
            }
            if (_closedList != null && _closedList.Count > 0)
            {
                foreach (var item in _closedList)
                {
                    item.GCost = int.MaxValue;
                    item.FCost = item.CalculateFCost();
                    item.CameFromNode = null;
                }
                _closedList.Clear();
            }

            Array.Clear(_openArray2D, 0, _openArray2D.Length);
            Array.Clear(_closedArray2D, 0, _closedArray2D.Length);

            PathNode startNode = Grid.GetGridObject(startX, startY);
            PathNode endNode = Grid.GetGridObject(endX, endY);
            if (endNode.NodeOccupier != null || endNode.IsWalkable == 0)
            {
                endNode = FindClosestFreePathNodeTo(endNode);
            }

            _openList = new List<PathNode> { startNode };
            _closedList = new List<PathNode>();


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

                _openArray2D[currentNode.Coordinates.x, currentNode.Coordinates.y] = null;
                _closedArray2D[currentNode.Coordinates.x, currentNode.Coordinates.y] = currentNode;


                foreach (PathNode neighbourNode in currentNode.Neighbours)
                {
                    if (_closedArray2D[neighbourNode.Coordinates.x, neighbourNode.Coordinates.y] != null) continue;

                    //if (_closedList.Contains(neighbourNode) == true) continue;

                    if (neighbourNode.IsWalkable == 0)
                    {
                        _closedList.Add(neighbourNode);
                        _closedArray2D[neighbourNode.Coordinates.x, neighbourNode.Coordinates.y] = neighbourNode;

                        continue;
                    }

                    int tentativeGCost = currentNode.GCost + CalculateDistance(currentNode, neighbourNode);

                    if (tentativeGCost < neighbourNode.GCost)
                    {
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateDistance(neighbourNode, endNode);
                        neighbourNode.FCost = neighbourNode.CalculateFCost();
                        neighbourNode.CameFromNode = currentNode;

                        if (_openArray2D[neighbourNode.Coordinates.x, neighbourNode.Coordinates.y] == null)
                        {
                            _openList.Add(neighbourNode);
                            _openArray2D[neighbourNode.Coordinates.x, neighbourNode.Coordinates.y] = neighbourNode;
                        }
                    }
                }
            }

            if (_closestToTarget != null)
            {
                Debug.Log("nopath");
                return CalculatePath(_closestToTarget);
            }

            Debug.Log("nopath");
            return null;
        }

        private PathNode FindClosestFreePathNodeTo(PathNode node)
        {
            Vector2Int[] directions = new Vector2Int[4]
            {
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0)
            };

            foreach (Vector2Int direction in directions)
            {
                Vector2Int coordinateNodeToCheck = node.Coordinates + direction;
                if (coordinateNodeToCheck.x < 0
                    || coordinateNodeToCheck.y < 0
                    || coordinateNodeToCheck.x >= Grid.GetWidth()
                    || coordinateNodeToCheck.y > Grid.GetHeight())
                {
                    continue;
                }

                PathNode nodeToCheck = Grid.GridArray[coordinateNodeToCheck.x, coordinateNodeToCheck.y];
                if (nodeToCheck.NodeOccupier == null && nodeToCheck.IsWalkable == 1)
                {
                    return nodeToCheck;
                }
            }

            foreach (Vector2Int direction in directions)
            {
                Vector2Int coordinateNodeToCheck = node.Coordinates + direction;
                if (coordinateNodeToCheck.x < 0
                    || coordinateNodeToCheck.y < 0
                    || coordinateNodeToCheck.x >= Grid.GetWidth()
                    || coordinateNodeToCheck.y > Grid.GetHeight())
                {
                    continue;
                }

                PathNode nodeToCheck = Grid.GridArray[coordinateNodeToCheck.x, coordinateNodeToCheck.y];
                return FindClosestFreePathNodeTo(nodeToCheck);
            }

            return null;
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> paths = new List<PathNode>();
            paths.Add(endNode);
            PathNode current = endNode;

            while (current.CameFromNode != null)
            {
                paths.Add(current.CameFromNode);
                current = current.CameFromNode;
            }

            paths.Reverse();
            return paths;
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

        private int CalculateDistance(PathNode a, PathNode b)
        {
            int xDist = Math.Abs(a.X - b.X);
            int yDist = Math.Abs(a.Y - b.Y);
            int remaining = Math.Abs(xDist - yDist);

            return MOVE_DIAG_COST * Math.Min(xDist, yDist) + MOVE_STRAIGHT_COST * remaining;
        }
        #endregion


        #region Other
        public void ResetNodeWalkable(List<Vector3> listPos, int index)
        {
            if (listPos.Count > 0)
            {
                SetPathReserved(listPos, index, null);
            }

            if (listPos.Count > 1 && index < listPos.Count - 1)
            {
                //next
                SetPathReserved(listPos, index + 1, null);
            }
        }

        public void SetPathReserved(List<Vector3> listPos, int index, PathNodeOccupier occupier)
        {
            Grid.GetXY(listPos[index], out int x, out int y);
            Grid.GetGridObject(x, y).SetPathOwned(occupier);
        }
        #endregion
    }
}