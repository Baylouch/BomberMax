using UnityEngine;

// TODO Maybe create the bonus invisibility ?

[RequireComponent(typeof(Collider2D))] // Must be set as a trigger
public class GameBonus : MonoBehaviour
{
    [SerializeField] SpriteRenderer gfx;

    [SerializeField] float timerBeforeDestroy = 20f;

    GameBonusData data;

    bool bonusUsed = false;

    private void OnDestroy()
    {
        if (StageManager.instance)
            StageManager.instance.SetBonusNode(new Vector2(transform.position.x, transform.position.y), false);
    }

    private void Start()
    {
        Destroy(gameObject, timerBeforeDestroy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bot")
        {
            if (bonusUsed)
                return;

            bonusUsed = true;

            switch (data.type)
            {
                case GameBonusType.Bomb:
                    if (collision.gameObject.GetComponent<BombSpawner>())
                    {
                        collision.gameObject.GetComponent<BombSpawner>().IncrementMaxBombNumb();
                    }
                    else if (collision.gameObject.GetComponentInChildren<BombSpawner>())
                    {
                        collision.gameObject.GetComponentInChildren<BombSpawner>().IncrementMaxBombNumb();
                    }

                    break;
                case GameBonusType.Explosion:
                    if (collision.gameObject.GetComponent<BombSpawner>())
                    {
                        collision.gameObject.GetComponent<BombSpawner>().IncrementExplosionForce();
                    }
                    else if (collision.gameObject.GetComponentInChildren<BombSpawner>())
                    {
                        collision.gameObject.GetComponentInChildren<BombSpawner>().IncrementExplosionForce();
                    }

                    break;
                case GameBonusType.Speed:
                    if (collision.gameObject.GetComponent<CharacterMovement>())
                    {
                        collision.gameObject.GetComponent<CharacterMovement>().ActivateSpeedBonus(data.duration);
                    }
                    else if (collision.gameObject.GetComponentInChildren<CharacterMovement>())
                    {
                        collision.gameObject.GetComponentInChildren<CharacterMovement>().ActivateSpeedBonus(data.duration);
                    }
                    break;
                case GameBonusType.Invincibility:
                    if (collision.gameObject.GetComponent<CharacterHealth>())
                    {
                        collision.gameObject.GetComponent<CharacterHealth>().SetupInvincibleBonus(data.duration);
                    }
                    else if (collision.gameObject.GetComponentInChildren<CharacterHealth>())
                    {
                        collision.gameObject.GetComponentInChildren<CharacterHealth>().SetupInvincibleBonus(data.duration);
                    }
                    break;
            }

            Destroy(gameObject);
        }
    }

    public void SetupBonus(GameBonusData _data)
    {
        data = _data;
        gfx.sprite = _data.sprite;

    }
}
