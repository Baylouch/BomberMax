using UnityEngine;

public class GameNode
{
    public Vector2 position;

    public bool hasUndestructibleBlock;
    public bool hasDestructibleBlock;
    public bool hasBomb;
    public bool hasBonus;
    public bool isDanger; // To know if there will be an explosion on it

    public GameNode(Vector2 _position, bool _hasUndestructibleBlock = false, bool _hasDestructibleBlock = false, bool _hasBomb = false, bool _hasBonus = false, bool _isDanger = false)
    {
        position = _position;
        hasUndestructibleBlock = _hasUndestructibleBlock;
        hasDestructibleBlock = _hasDestructibleBlock;
        hasBomb = _hasBomb;
        hasBonus = _hasBonus;
        isDanger = _isDanger;
    }
}

