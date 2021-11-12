using UnityEngine;

[RequireComponent(typeof(Collider2D))] // set as trigger
[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(BombSpawner))]
public class BonusReceiver : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameBonus _bonus = collision.GetComponent<GameBonus>();

        if (_bonus && !_bonus.bonusUsed)
        {
            switch (_bonus.bonusData.type)
            {
                case GameBonusType.Bomb:
                case GameBonusType.Explosion:
                    _bonus.UseBonus(GetComponent<BombSpawner>());

                    break;
                case GameBonusType.Speed:
                    _bonus.UseBonus(null, GetComponent<CharacterMovement>());

                    break;
                case GameBonusType.Invincibility:
                    _bonus.UseBonus(null, null, GetComponent<CharacterHealth>());

                    break;
            }
        }
    }
}
