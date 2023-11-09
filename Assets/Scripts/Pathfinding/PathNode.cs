using System.Collections.Generic;
using GameManagement;
using UnityEngine;

namespace Pathfinding
{
    class PathNode
    {
        public int X;
        public int Y;
        public int GCost;
        public int HCost;
        public int FCost;
        public Vector2Int Coordinates
        {
            get { return new Vector2Int(X, Y); }
        }

        public List<PathNode> Neighbours = new List<PathNode>();
        public PathNode CameFromNode = null;
        public bool IsWalkable;

        public PathNodeOccupier NodeOccupier
        {
            get { return _nodeOccupier; }
            set
            {
                _nodeOccupier = value;
                GameManager.Instance.DrawGizmoAt(Coordinates, value == null ? Color.red : Color.green);
            }
        }
        private PathNodeOccupier _nodeOccupier;
        
        
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

        public void SetPathOwned(PathNodeOccupier occupier = null)
        {
            IsWalkable = occupier == null;
            NodeOccupier = occupier;
        }

        public override string ToString()
        {
            return $"{X}x, {Y}y";
        }
    }
}