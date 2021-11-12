using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bomb : MonoBehaviour
{
    public const float DownRotation = -90f;
    public const float UpRotation = 90f;
    public const float LeftRotation = 180f;
    public const float RightRotation = 0f;

    public static int bombID = 0;

    [SerializeField] float explosionTiming;
    [SerializeField] SpriteRenderer bombGfx;
    [SerializeField] BombExplosionSettings explosionSettings;

    BombSpawner bombSpawner;

    Collider2D bombCollider;

    int tileIndex; // To know the tile index in the StageManager list trough all of this script
    // Each direction explosion's length (set in SetExplosionLength() and use in SpawnExplosion())
    int explosionLengthLeft = 0, explosionLengthRight = 0, explosionLengthDown = 0, explosionLengthUp = 0;

    bool explosed = false;


    // Variables used for the bot : bombSpawnerID is to know the length of explosion (with conditions)
    int bombSpawnerID;
    public void SetBombSpawnerID(int _id) { bombSpawnerID = _id; }
    public int GetBombSpawnerID() { return bombSpawnerID; }

    // thisBombID is for not have multiple times the bomb in bot bomb's list
    int thisBombID;
    public int GetThisBombID() { return thisBombID; }

    private void Start()
    {
        thisBombID = bombID;

        if (bombID < int.MaxValue)
            bombID++;
        else
            bombID = 0;

        bombCollider = GetComponent<Collider2D>();

        tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position == (Vector2)transform.position);
        if (tileIndex != -1)
        {
            StageManager.instance.TilesInfo[tileIndex].hasBomb = true;
        }
        else
        {
            Debug.LogError("Bomb tileIndex wrong !!!!");
        }

        StartCoroutine(Explosion());
        StartCoroutine(ColorModifier());
    }

    // When a bomb is created, its collider is Trigger because the player needs to runaway.
    // When he left, we set the trigger as false.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
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
        bool stopLeftAxis = false, stopRightAxis = false, stopDownAxis = false, stopUpAxis = false;

        // We have to reset length because we set it multiple times
        explosionLengthUp = 0; explosionLengthRight = 0; explosionLengthLeft = 0; explosionLengthDown = 0;

        int tileToCheckIndex = -1;

        int currentTileRange = 1; // To know how many times we loop trough tiles relative to the explosion force

        int explosionForce = bombSpawner.GetExplosionForce();

        // Now we set the explosion's length for each axis
        while (currentTileRange <= explosionForce)
        {
            // Left axis
            if (!stopLeftAxis)
            {
                // We get the tile index in StageManager tiles list
                tileToCheckIndex = StageManager.instance.TilesInfo.FindIndex(x => (x.position.x == transform.position.x - currentTileRange) && (x.position.y == transform.position.y));
                // If we found an occurence
                if (tileToCheckIndex != -1)
                {
                    // We check if the tile contain an undestructible block, so we dont want explosion on it and stop check on this axis
                    if (StageManager.instance.TilesInfo[tileToCheckIndex].hasUndestructibleBlock)
                    {
                        stopLeftAxis = true;

                    }
                    // We check if there is a destructible block, we want an explosion on it then stop check on this axis
                    else if (StageManager.instance.TilesInfo[tileToCheckIndex].hasDestructibleBlock)
                    {
                        explosionLengthLeft++;
                        stopLeftAxis = true;
                    }
                    // Else axis is clear of block, we can continue until force is reached
                    else
                    {
                        explosionLengthLeft++;

                    }
                }
                else // We reached end of stage
                {
                    stopLeftAxis = true;
                }
            }

            // Right axis
            if (!stopRightAxis)
            {
                tileToCheckIndex = StageManager.instance.TilesInfo.FindIndex(x => (x.position.x == transform.position.x + currentTileRange) && (x.position.y == transform.position.y));
                if (tileToCheckIndex != -1)
                {
                    if (StageManager.instance.TilesInfo[tileToCheckIndex].hasUndestructibleBlock)
                    {
                        stopRightAxis = true;

                    }
                    else if (StageManager.instance.TilesInfo[tileToCheckIndex].hasDestructibleBlock)
                    {
                        explosionLengthRight++;
                        stopRightAxis = true;
                    }
                    else
                    {
                        explosionLengthRight++;

                    }
                }
                else
                {
                    stopRightAxis = true;
                }
            }

            // Down axis
            if (!stopDownAxis)
            {
                tileToCheckIndex = StageManager.instance.TilesInfo.FindIndex(x => (x.position.x == transform.position.x) && (x.position.y == transform.position.y - currentTileRange));
                if (tileToCheckIndex != -1)
                {
                    if (StageManager.instance.TilesInfo[tileToCheckIndex].hasUndestructibleBlock)
                    {
                        stopDownAxis = true;

                    }
                    else if (StageManager.instance.TilesInfo[tileToCheckIndex].hasDestructibleBlock)
                    {
                        explosionLengthDown++;
                        stopDownAxis = true;
                    }
                    else
                    {
                        explosionLengthDown++;

                    }
                }
                else
                {
                    stopDownAxis = true;
                }
            }

            // Up axis
            if (!stopUpAxis)
            {
                tileToCheckIndex = StageManager.instance.TilesInfo.FindIndex(x => (x.position.x == transform.position.x) && (x.position.y == transform.position.y + currentTileRange));
                if (tileToCheckIndex != -1)
                {
                    if (StageManager.instance.TilesInfo[tileToCheckIndex].hasUndestructibleBlock)
                    {
                        stopUpAxis = true;
                    }
                    else if (StageManager.instance.TilesInfo[tileToCheckIndex].hasDestructibleBlock)
                    {
                        explosionLengthUp++;
                        stopUpAxis = true;
                    }
                    else
                    {
                        explosionLengthUp++;
                    }
                }
                else
                {
                    stopUpAxis = true;
                }
            }

            currentTileRange++;
        }
    }

    // Method to avoid repetitive code to create explosion's parts on an axis
    // When _length == 0, it's create the center part of the explosion, the only time _centerSprite is required in param
    // bool xSpread to determine if we want to spread the explosion on X or Y (true = X, false = Y) (x for horizontal, y for vertical)
    // bool positiveSpread to determine if we want a left / right or up / down (true = right/up, false = left/down)
    void CreateExplosionParts(int _length, float _zRot, bool xSpread, bool positiveSpread, Sprite _centerSprite = null)
    {
        // To create the center part (with length of 0)
        if (_centerSprite != null)
        {
            GameObject explosionPart = Instantiate(explosionSettings.explosionPartPrefab, transform);
            explosionPart.transform.position = transform.position;
            explosionPart.transform.localEulerAngles = new Vector3(0, 0, _zRot);
            explosionPart.GetComponent<SpriteRenderer>().sprite = _centerSprite;

            return;
        }

        // To create middle and end part
        for (int i = 1; i <= _length; i++)
        {
            GameObject explosionPart = Instantiate(explosionSettings.explosionPartPrefab, transform);

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

    // Methods to spawn explosion's parts
    void SpawnExplosion()
    {
        // If length > 0 for 1 axis
        // Condition for down axis
        if (explosionLengthDown > 0 
            && explosionLengthLeft <= 0 && explosionLengthRight <= 0 && explosionLengthUp <= 0)
        {
            CreateExplosionParts(0, UpRotation, false, false, explosionSettings.endGfx);

            CreateExplosionParts(explosionLengthDown, DownRotation, false, false);
        }
        // Condition for up axis
        else if (explosionLengthUp > 0 
                 && explosionLengthLeft <= 0 && explosionLengthRight <= 0 && explosionLengthDown <= 0)
        {
            CreateExplosionParts(0, DownRotation, false, false, explosionSettings.endGfx);

            CreateExplosionParts(explosionLengthUp, UpRotation, false, true);
        }
        // Condition for left axis
        else if (explosionLengthLeft > 0 
                 && explosionLengthUp <= 0 && explosionLengthRight <= 0 && explosionLengthDown <= 0)
        {
            CreateExplosionParts(0, RightRotation, false, false, explosionSettings.endGfx);

            CreateExplosionParts(explosionLengthLeft, LeftRotation, true, false);
        }
        // Condition for right axis
        else if (explosionLengthRight > 0 
                 && explosionLengthLeft <= 0 && explosionLengthUp <= 0 && explosionLengthDown <= 0)
        {
            CreateExplosionParts(0, LeftRotation, false, false, explosionSettings.endGfx);

            CreateExplosionParts(explosionLengthRight, RightRotation, true, true);
        }
        // Condition for Y axis (up and down)
        else if (explosionLengthDown > 0 && explosionLengthUp > 0 
                 && explosionLengthLeft <= 0 && explosionLengthRight <= 0)
        {
            CreateExplosionParts(0, UpRotation, false, false, explosionSettings.middleGfx);

            CreateExplosionParts(explosionLengthUp, UpRotation, false, true);
            CreateExplosionParts(explosionLengthDown, DownRotation, false, false);
        }
        // Condition for X axis (left and right)
        else if (explosionLengthLeft > 0 && explosionLengthRight > 0 
                 && explosionLengthDown <= 0 && explosionLengthUp <= 0)
        {
            CreateExplosionParts(0, RightRotation, false, false, explosionSettings.middleGfx);

            CreateExplosionParts(explosionLengthLeft, LeftRotation, true, false);
            CreateExplosionParts(explosionLengthRight, RightRotation, true, true);
        }
        // If lengths > 0 for each axis
        // Spawn explosions in every axis relative to each axis length
        else
        {
            CreateExplosionParts(0, 0f, false, false, explosionSettings.centerGfx);

            CreateExplosionParts(explosionLengthDown, DownRotation, false, false);
            CreateExplosionParts(explosionLengthUp, UpRotation, false, true);
            CreateExplosionParts(explosionLengthLeft, LeftRotation, true, false);
            CreateExplosionParts(explosionLengthRight, RightRotation, true, true);

        }
    }

    public void Explode()
    {
        if (explosed)
            return;

        explosed = true;

        // We disable the bomb render and collider before destruction to not spawn explosion on it
        StopAllCoroutines();
        Destroy(bombGfx.gameObject);
        bombCollider.enabled = false;

        SetExplosionLength();

        SpawnExplosion();

        // Decrement current spawner bomb
        bombSpawner.DecrementBombsNumb();

        // Remove the bomb from the TileInfo list
        StageManager.instance.TilesInfo[tileIndex].hasBomb = false;

        Destroy(gameObject, 1f);
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
