using UnityEngine;
using System.Collections.Generic;

public class Node
{
    public Vector2 position;

    public bool walkable;

    public Node parent;

    public int gCost;
    public int hCost;

    public int fCost()
    {
        return gCost + hCost;
    }

    public List<Node> neighboors;

    public bool hasUndestructibleBlock;
    public bool hasDestructibleBlock;
    public bool hasBomb;
    public bool isDanger; // To know if there will be an explosion on it

    public Node(Vector2 _position, bool _walkable = true, bool _hasUndestructibleBlock = false, bool _hasDestructibleBlock = false, bool _hasBomb = false, bool _isDanger = false)
    {
        position = _position;
        walkable = _walkable;
        hasUndestructibleBlock = _hasUndestructibleBlock;
        hasDestructibleBlock = _hasDestructibleBlock;
        hasBomb = _hasBomb;
        isDanger = _isDanger;

        neighboors = new List<Node>();
    }
}

