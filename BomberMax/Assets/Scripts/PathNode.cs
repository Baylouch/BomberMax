using UnityEngine;
using System.Collections.Generic;

public class PathNode
{
    public Vector2 position;

    public bool walkable;

    public PathNode parent;

    public int gCost;
    public int hCost;

    public int fCost()
    {
        return gCost + hCost;
    }

    public List<PathNode> neighboors;

    public PathNode(Vector2 _position)
    {
        position = _position;
        walkable = true;
        neighboors = new List<PathNode>();
    }
}
