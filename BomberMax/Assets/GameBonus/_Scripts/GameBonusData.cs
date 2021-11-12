using UnityEngine;

public enum GameBonusType { Bomb, Explosion, Speed, Invincibility };

[CreateAssetMenu(fileName = "Bonus", menuName = "ScriptableObjects/Bonus", order = 1)]
public class GameBonusData : ScriptableObject
{
    public GameBonusType type;
    public Sprite sprite;
    public float duration; // 0 = Infinity
    public float chanceToSpawn;
}
