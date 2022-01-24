/* ExplosionSetup.cs
 * 
 * This script is attached on a bomb to handle its explosion.
 * 
 * 
 * 
 * */

using System.Collections.Generic;
using UnityEngine;

public class ExplosionSetup : MonoBehaviour
{
    public const float DownRotation = -90f;
    public const float UpRotation = 90f;
    public const float LeftRotation = 180f;
    public const float RightRotation = 0f;

    public const float ExplosionDuration = 1f; // To know how many times the explosion stay
 
    // TODO : Add more than one settings and handle array mecanism
    [SerializeField] BombExplosionSettings settings;

    public List<ExplosionNode> explosionNodes;

    Bomb bombComponent;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<Bomb>())
        {
            Debug.LogError("ExplosionSetup.cs must be attached on a gameObject with Bomb.cs !");
            return;
        }

        bombComponent = GetComponent<Bomb>();

        GetExplosionPos();
    }

    public void GetExplosionPos()
    {
        Vector2 posToCheck = new Vector2(transform.position.x, transform.position.y);
        int gridIndex = -1;
        int explosionForce = bombComponent.GetBombSpawner().GetExplosionForce();

        float currentRotation = 0f;
        Sprite currentGfx = null;
        int currentIndex = 0; // To be able to set endGfx when reached undestructible block

        explosionNodes = new List<ExplosionNode>();

        // We add the current rotation where start each axis.
        explosionNodes.Add(new ExplosionNode(settings.centerGfx, posToCheck, currentRotation));

        // We loop trough axis
        for (int i = 0; i < 4; i++)
        {
            // For each axis we loop until obstacle or explosion force are reached.
            for (int j = 1; j <= explosionForce; j++)
            {
                switch (i)
                {
                    case 0: // Up case
                        posToCheck = new Vector2(transform.position.x, transform.position.y + j);
                        currentRotation = UpRotation;

                        break;
                    case 1: // Down case
                        posToCheck = new Vector2(transform.position.x, transform.position.y - j);
                        currentRotation = DownRotation;

                        break;
                    case 2: // Right case
                        posToCheck = new Vector2(transform.position.x + j, transform.position.y);
                        currentRotation = RightRotation;

                        break;
                    case 3: // Left case
                        posToCheck = new Vector2(transform.position.x - j, transform.position.y);
                        currentRotation = LeftRotation;

                        break;
                }

                // Get the gridIndex from StageManager GameGrid.
                gridIndex = StageManager.instance.GameGrid.FindIndex(x => x.position == posToCheck);

                if (gridIndex == -1) // We reached end of stage
                    break;

                // If we reached end of explosion
                currentGfx = (j == explosionForce) ? settings.endGfx : settings.middleGfx;

                // If we reached here and the bomb is overPowered, only end of stage (test above) or explosion force
                // who's the loop limit can stop it. So we can just add a node to the list
                if (bombComponent.overPowered)
                {
                    explosionNodes.Add(new ExplosionNode(currentGfx, posToCheck, currentRotation));
                    currentIndex++;
                    continue; // Go to the next loop iteration
                }

                // If we encounter a destructible block we want to spawn an explosion on it then stop.
                if (StageManager.instance.GameGrid[gridIndex].hasDestructibleBlock)
                {
                    currentGfx = settings.endGfx;
                    explosionNodes.Add(new ExplosionNode(currentGfx, posToCheck, currentRotation));
                    currentIndex++;
                    break; // Go on the next axis
                }

                // If there is an undestructible block we stop the explosion now
                if (StageManager.instance.GameGrid[gridIndex].hasUndestructibleBlock)
                {
                    if (currentIndex > 0)
                    {
                        explosionNodes[currentIndex].gfx = settings.endGfx;
                    }

                    break; // Go on the next axis
                }

                // If we're here nothing stopped us, so we just add an explosion node
                explosionNodes.Add(new ExplosionNode(currentGfx, posToCheck, currentRotation));
                currentIndex++;
            }
        }
    }

    public void SpawnExplosion()
    {
        // We create the explosion parent
        GameObject explosion = new GameObject("Explosion");
        explosion.transform.position = transform.position;

        // We must add a rigidbody on to detect collision with other things in game
        Rigidbody2D _rb = explosion.AddComponent<Rigidbody2D>();
        _rb.isKinematic = true;
        _rb.freezeRotation = true;

        // We also add a DangerTilesReseter on it to reset danger tile when the explosion is done
        explosion.AddComponent<DangerTilesReseter>();

        for (int i = 0; i < explosionNodes.Count; i++)
        {
            GameObject explosionPart = Instantiate(settings.explosionPartPrefab, explosion.transform);

            explosionPart.transform.position = explosionNodes[i].position;
            explosionPart.transform.localEulerAngles = new Vector3(0, 0, explosionNodes[i].zRotation);
            explosionPart.GetComponent<SpriteRenderer>().sprite = explosionNodes[i].gfx;
        }

        Destroy(explosion, ExplosionDuration);
    }
}
