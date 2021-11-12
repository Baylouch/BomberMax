using UnityEngine;

[CreateAssetMenu(fileName = "Explosion", menuName = "ScriptableObjects/Explosion", order = 1)]
public class BombExplosionSettings : ScriptableObject
{
    public GameObject explosionPartPrefab; // Must have spriteRender and collider2D set as trigger on it

    public Sprite centerGfx; // Represent the center of an explosion

    public Sprite middleGfx; // Represent the middle of explosion's axis

    public Sprite endGfx; // Represent the end of explosion's axis
}
