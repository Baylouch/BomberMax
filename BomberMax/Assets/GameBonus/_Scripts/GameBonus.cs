using UnityEngine;

// TODO Maybe create the bonus invisibility ?
// All characters will have a specific bonus unique to them : TODO defines them

[RequireComponent(typeof(Collider2D))] // Must be set as a trigger
public class GameBonus : MonoBehaviour
{
    public GameBonusData bonusData; // Is set by DestructibleBlock.cs

    public bool bonusUsed = false;

    [SerializeField] SpriteRenderer gfx;

    public void SetupBonus(GameBonusData _data)
    {
        bonusData = _data;
        gfx.sprite = _data.sprite;
    }

    public void UseBonus(BombSpawner _bombSpawner = null, CharacterMovement _movement = null, CharacterHealth _health = null)
    {
        bonusUsed = true;

        switch (bonusData.type)
        {
            case GameBonusType.Bomb:
                // Affects BombSpawner.cs
                _bombSpawner.IncrementMaxBombNumb();
                break;
            case GameBonusType.Explosion:
                // Affects BombSpawner.cs
                _bombSpawner.IncrementExplosionForce();

                break;
            case GameBonusType.Speed:
                // Affects movement script
                _movement.ActivateSpeedBonus(bonusData.duration);
                break;
            case GameBonusType.Invincibility:
                // Affects health script
                _health.SetupInvincibleBonus(bonusData.duration);
                break;
        }

        Destroy(gameObject);
    }
}
