using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bomb : MonoBehaviour
{
    public const float DownRotation = -90f;
    public const float UpRotation = 90f;
    public const float LeftRotation = 180f;
    public const float RightRotation = 0f;

    public const float ExplosionDuration = 1f; // To know how many times the explosion stay

    [SerializeField] float explosionTiming;
    [SerializeField] SpriteRenderer bombGfx;
    [SerializeField] BombExplosionSettings explosionSettings;

    BombSpawner bombSpawner;
    Collider2D bombCollider;

    // Each direction explosion's length (set in SetExplosionLength() and use in SpawnExplosion())
    int explosionLengthLeft = 0, explosionLengthRight = 0, explosionLengthDown = 0, explosionLengthUp = 0;

    bool explosed = false;

    private void Start()
    {
        bombCollider = GetComponent<Collider2D>();

        StartCoroutine(Explosion());
        StartCoroutine(ColorModifier());

        StageManager.instance.SetDangerTiles();
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

    // Get every tile in all 4 axis relative to the explosion force to check if there is a block on the way to
    // set the right amount of explosion objects
    void SetExplosionLength()
    {
        // To know if we meet a block to not continue on the axis
        bool[] stopAxis = new bool[4] { false, false, false, false };

        explosionLengthUp = 0; explosionLengthRight = 0; explosionLengthLeft = 0; explosionLengthDown = 0;

        int tileToCheckIndex = -1;

        int currentTileRange = 1; // To know how many times we loop trough tiles relative to the explosion force

        int explosionForce = bombSpawner.GetExplosionForce();

        // Now we set the explosion's length for each axis
        while (currentTileRange <= explosionForce)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!stopAxis[i])
                {
                    switch (i)
                    {
                        case 0: // Up
                            tileToCheckIndex = StageManager.instance.GameGrid.FindIndex(x => (x.position.x == transform.position.x) && (x.position.y == transform.position.y + currentTileRange));

                            break;
                        case 1: // Down
                            tileToCheckIndex = StageManager.instance.GameGrid.FindIndex(x => (x.position.x == transform.position.x) && (x.position.y == transform.position.y - currentTileRange));

                            break;
                        case 2: // Left
                            tileToCheckIndex = StageManager.instance.GameGrid.FindIndex(x => (x.position.x == transform.position.x - currentTileRange) && (x.position.y == transform.position.y));

                            break;
                        case 3: // Right
                            tileToCheckIndex = StageManager.instance.GameGrid.FindIndex(x => (x.position.x == transform.position.x + currentTileRange) && (x.position.y == transform.position.y));

                            break;
                    }

                    if (tileToCheckIndex == -1 || StageManager.instance.GameGrid[tileToCheckIndex].hasUndestructibleBlock)
                    {
                        stopAxis[i] = true;
                    }
                    else 
                    {
                        switch (i)
                        {
                            case 0:
                                explosionLengthUp++;
                                break;
                            case 1:
                                explosionLengthDown++;
                                break;
                            case 2:
                                explosionLengthLeft++;
                                break;
                            case 3:
                                explosionLengthRight++;
                                break;
                        }

                        if (StageManager.instance.GameGrid[tileToCheckIndex].hasDestructibleBlock)
                        {
                            stopAxis[i] = true;
                        }
                    }
                }
            }

            currentTileRange++;
        }
    }

    // Method to avoid repetitive code to create explosion's parts on an axis
    // When _length == 0, it's create the center part of the explosion, the only time _centerSprite is required in param
    // bool xSpread to determine if we want to spread the explosion on X or Y (true = X, false = Y) (x for horizontal, y for vertical)
    // bool positiveSpread to determine if we want a left / right or up / down (true = right/up, false = left/down)
    void CreateExplosionParts(Transform explosionParent, int _length, float _zRot, bool xSpread, bool positiveSpread, Sprite _centerSprite = null)
    {
        // To create the center part (with length of 0)
        if (_centerSprite != null)
        {
            GameObject explosionPart = Instantiate(explosionSettings.explosionPartPrefab, explosionParent);

            explosionPart.transform.position = transform.position;
            explosionPart.transform.localEulerAngles = new Vector3(0, 0, _zRot);
            explosionPart.GetComponent<SpriteRenderer>().sprite = _centerSprite;

            return;
        }

        // To create middle and end part
        for (int i = 1; i <= _length; i++)
        {
            GameObject explosionPart = Instantiate(explosionSettings.explosionPartPrefab, explosionParent);

            if (positiveSpread)
            {
                if (xSpread) { explosionPart.transform.position = new Vector2(transform.position.x + i, transform.position.y); }
                else         { explosionPart.transform.position = new Vector2(transform.position.x, transform.position.y + i); }

            }
            else
            {
                if (xSpread) { explosionPart.transform.position = new Vector2(transform.position.x - i, transform.position.y); }
                else         { explosionPart.transform.position = new Vector2(transform.position.x, transform.position.y - i); }
            }

            if (i == _length)
            {
                explosionPart.transform.localEulerAngles = new Vector3(0, 0, _zRot);
                explosionPart.GetComponent<SpriteRenderer>().sprite = explosionSettings.endGfx;
            }
            else
            {
                explosionPart.transform.localEulerAngles = new Vector3(0, 0, _zRot);
                explosionPart.GetComponent<SpriteRenderer>().sprite = explosionSettings.middleGfx;
            }
        }
    }

    // Methods to spawn explosion's parts (and destroy it)
    void SpawnExplosion()
    {
        // We create the explosion parent here to pass it trough CreateExplosionParts() method
        GameObject explosion = new GameObject("Explosion");
        explosion.transform.position = transform.position;

        // We must add a rigidbody on to detect collision with other things in game
        Rigidbody2D _rb = explosion.AddComponent<Rigidbody2D>();
        _rb.isKinematic = true;
        _rb.freezeRotation = true;

        // We also add a DangerTilesReseter on it to reset danger tile when the explosion is done
        explosion.AddComponent<DangerTilesReseter>();

        // If length > 0 for 1 axis
        // Condition for down axis
        if (explosionLengthDown > 0 
            && explosionLengthLeft <= 0 && explosionLengthRight <= 0 && explosionLengthUp <= 0)
        {
            CreateExplosionParts(explosion.transform, 0, UpRotation, false, false, explosionSettings.endGfx);

            CreateExplosionParts(explosion.transform, explosionLengthDown, DownRotation, false, false);
        }
        // Condition for up axis
        else if (explosionLengthUp > 0 
                 && explosionLengthLeft <= 0 && explosionLengthRight <= 0 && explosionLengthDown <= 0)
        {
            CreateExplosionParts(explosion.transform, 0, DownRotation, false, false, explosionSettings.endGfx);

            CreateExplosionParts(explosion.transform, explosionLengthUp, UpRotation, false, true);
        }
        // Condition for left axis
        else if (explosionLengthLeft > 0 
                 && explosionLengthUp <= 0 && explosionLengthRight <= 0 && explosionLengthDown <= 0)
        {
            CreateExplosionParts(explosion.transform, 0, RightRotation, false, false, explosionSettings.endGfx);

            CreateExplosionParts(explosion.transform, explosionLengthLeft, LeftRotation, true, false);
        }
        // Condition for right axis
        else if (explosionLengthRight > 0 
                 && explosionLengthLeft <= 0 && explosionLengthUp <= 0 && explosionLengthDown <= 0)
        {
            CreateExplosionParts(explosion.transform, 0, LeftRotation, false, false, explosionSettings.endGfx);

            CreateExplosionParts(explosion.transform, explosionLengthRight, RightRotation, true, true);
        }
        // Condition for Y axis (up and down)
        else if (explosionLengthDown > 0 && explosionLengthUp > 0 
                 && explosionLengthLeft <= 0 && explosionLengthRight <= 0)
        {
            CreateExplosionParts(explosion.transform, 0, UpRotation, false, false, explosionSettings.middleGfx);

            CreateExplosionParts(explosion.transform, explosionLengthUp, UpRotation, false, true);
            CreateExplosionParts(explosion.transform, explosionLengthDown, DownRotation, false, false);
        }
        // Condition for X axis (left and right)
        else if (explosionLengthLeft > 0 && explosionLengthRight > 0 
                 && explosionLengthDown <= 0 && explosionLengthUp <= 0)
        {
            CreateExplosionParts(explosion.transform, 0, RightRotation, false, false, explosionSettings.middleGfx);

            CreateExplosionParts(explosion.transform, explosionLengthLeft, LeftRotation, true, false);
            CreateExplosionParts(explosion.transform, explosionLengthRight, RightRotation, true, true);
        }
        // If lengths > 0 for each axis
        // Spawn explosions in every axis relative to each axis length
        else
        {
            CreateExplosionParts(explosion.transform, 0, 0f, false, false, explosionSettings.centerGfx);

            CreateExplosionParts(explosion.transform, explosionLengthDown, DownRotation, false, false);
            CreateExplosionParts(explosion.transform, explosionLengthUp, UpRotation, false, true);
            CreateExplosionParts(explosion.transform, explosionLengthLeft, LeftRotation, true, false);
            CreateExplosionParts(explosion.transform, explosionLengthRight, RightRotation, true, true);

        }

        Destroy(explosion, ExplosionDuration);
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

        SetExplosionLength();

        SpawnExplosion();

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
