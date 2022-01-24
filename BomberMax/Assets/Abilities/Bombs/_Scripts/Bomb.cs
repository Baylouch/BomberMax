using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(ExplosionSetup))]
public class Bomb : MonoBehaviour
{
    public bool overPowered = false;

    [SerializeField] float explosionTiming;
    [SerializeField] SpriteRenderer bombGfx;

    BombSpawner bombSpawner;
    Collider2D bombCollider;
    ExplosionSetup explosionSetup;

    bool explosed = false;

    private void Start()
    {
        bombCollider = GetComponent<Collider2D>();
        explosionSetup = GetComponent<ExplosionSetup>();

        StartCoroutine(Explosion());
        StartCoroutine(ColorModifier());

        StageManager.instance.SetDangerTiles(); // TODO Delete and move into ExplosionSetup.cs
    }

    // When a bomb is created, its collider is Trigger because the player needs to runaway.
    // When he left, we set the trigger as false.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bot")
        {
            bombCollider.isTrigger = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Explosion")
        {
            Explode();
        }
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(explosionTiming);

        Explode();
    }

    IEnumerator ColorModifier()
    {
        bool _switch = false;

        while (true)
        {
            float _r = bombGfx.color.r;
            float _g = bombGfx.color.g;
            float _b = bombGfx.color.b;

            if (!_switch)
            {
                while (bombGfx.color.r > 0.5f)
                {
                    _r -= 0.05f;
                    _g -= 0.2f;
                    _b -= 0.2f;

                    bombGfx.color = new Color(_r, _g, _b);

                    yield return new WaitForSeconds(0.05f);
                }

                _switch = true;
            }
            else
            {
                while (bombGfx.color.r < 1f)
                {
                    _r += 0.05f;
                    _g += 0.2f;
                    _b += 0.2f;

                    bombGfx.color = new Color(_r, _g, _b);

                    yield return new WaitForSeconds(0.05f);
                }

                _switch = false;
            }
        }
    }

    public void Explode()
    {
        if (explosed)
            return;

        explosed = true;

        // We disable the bomb render and collider before destruction to not spawn explosion on it
        StopAllCoroutines();
        bombGfx.enabled = false;
        bombCollider.enabled = false;

        explosionSetup.GetExplosionPos();
        explosionSetup.SpawnExplosion();

        // Decrement current spawner bomb
        bombSpawner.DecrementBombsNumb();

        int tileIndex = StageManager.instance.GameGrid.FindIndex(x => x.position == (Vector2)transform.position);
        if (tileIndex != -1)
        {
            // Remove the bomb from the TileInfo list
            StageManager.instance.GameGrid[tileIndex].hasBomb = false;
        }

        Destroy(gameObject);
    }

    public void SetupBomb(BombSpawner _spawner)
    {
        bombSpawner = _spawner;
    }

    // Method used for bot to know the bomb's length
    public BombSpawner GetBombSpawner()
    {
        if (bombSpawner)
            return bombSpawner;
        else
            return null;
    }
}
