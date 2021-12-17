using UnityEngine;

// TODO Maybe create the bonus invisibility ?
// All characters will have a specific bonus unique to them : TODO defines them

[RequireComponent(typeof(Collider2D))] // Must be set as a trigger
public class GameBonus : MonoBehaviour
{
    [SerializeField] GameBonusData[] datas;

    [SerializeField] SpriteRenderer gfx;

    [SerializeField] float timerBeforeDestroy = 20f;

    GameBonusType bonusType;

    int dataIndex = -1; // To get access to the GameBonusData in the array with only 1 research.

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

            switch (bonusType)
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
                        collision.gameObject.GetComponent<CharacterMovement>().ActivateSpeedBonus(datas[dataIndex].duration);
                    }
                    else if (collision.gameObject.GetComponentInChildren<CharacterMovement>())
                    {
                        collision.gameObject.GetComponentInChildren<CharacterMovement>().ActivateSpeedBonus(datas[dataIndex].duration);
                    }
                    break;
                case GameBonusType.Invincibility:
                    if (collision.gameObject.GetComponent<CharacterHealth>())
                    {
                        collision.gameObject.GetComponent<CharacterHealth>().SetupInvincibleBonus(datas[dataIndex].duration);
                    }
                    else if (collision.gameObject.GetComponentInChildren<CharacterHealth>())
                    {
                        collision.gameObject.GetComponentInChildren<CharacterHealth>().SetupInvincibleBonus(datas[dataIndex].duration);
                    }
                    break;
            }

            Destroy(gameObject);
        }
    }

    public void SetupBonus()
    {
        bool isSet = false;

        // The bonus define what type it'll be
        // By design i put datas in this order : first index is the more common bonus, last index is the rarest.
        // So we start by the end of the array to determine the bonus type
        for (int i = datas.Length - 1; i > 0; i--)
        {
            float randomValue = Random.Range(0f, 100f);

            if (datas[i].chanceToSpawn >= randomValue)
            {
                dataIndex = i;
                gfx.sprite = datas[i].sprite;
                bonusType = datas[i].type;
                isSet = true;

                break;
            }

        }

        // If bonus isn't set here, set between bomb+ or explosion+
        if (!isSet)
        {
            float randomValue = Random.Range(0f, 100f);

            if (randomValue > 50f)
            {
                dataIndex = 0;
                gfx.sprite = datas[0].sprite;
                bonusType = datas[0].type;
            }
            else
            {
                dataIndex = 1;
                gfx.sprite = datas[1].sprite;
                bonusType = datas[1].type;
            }
        }
    }
}
