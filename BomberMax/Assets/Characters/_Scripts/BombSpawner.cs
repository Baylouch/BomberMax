using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
public class BombSpawner : MonoBehaviour
{
    public const int MaxBombNumber = 5;
    public const int MaxExplosionForce = 10;

    private int maxBombNumb = 1;
    private int currentBombNumb = 0; // To know the current bombs number spawned
    private int explosionForce = 1;

    [SerializeField] GameObject bombPrefab;

    CharacterInfo charInfo;

    private void Start()
    {
        charInfo = GetComponent<CharacterInfo>();

        if (charInfo.IsLocal)
        {
            FindObjectOfType<PlayerAction_Bomb>().SetupBombSpawner(this);
            GameCanvas.instance.SetBombBonusText(maxBombNumb);
            GameCanvas.instance.SetExplosionBonusText(explosionForce);
        }
    }

    private void Update()
    {
        // TODO Create a button to spawn the bomb
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBomb();
        }
    }

    public void SpawnBomb()
    {
        if (currentBombNumb >= maxBombNumb)
            return;

        // Get the bomb spawn position
        // we want a spawn at the center of the tile where player is
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        // We want a reference to the current TileInfo position to make check of blocks, bombs, and set bomb on it
        int _currentTileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position == spawnPos);

        // Logic to know if there is already a bomb on the spot
        if (StageManager.instance.TilesInfo[_currentTileIndex].hasBomb == true)
        {
            // Just make it impossible to spawn bomb
            return;
        }

        // Spawn the bomb
        GameObject _curBomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        Bomb _bomb = _curBomb.GetComponent<Bomb>();

        _bomb.SetupBomb(this);
        _bomb.SetBombSpawnerID(charInfo.CharID);

        currentBombNumb++;
    }

    // Used from Bomb script when a bomb explode
    public void DecrementBombsNumb()
    {
        currentBombNumb--;
    }

    public void IncrementMaxBombNumb()
    {
        if (maxBombNumb < MaxBombNumber)
        {
            maxBombNumb++;
            GameCanvas.instance.SetBombBonusText(maxBombNumb);
        }
    }

    public void IncrementExplosionForce()
    {
        if (explosionForce < MaxExplosionForce)
        {
            explosionForce++;
            GameCanvas.instance.SetExplosionBonusText(explosionForce);
        }
    }

    public int GetExplosionForce()
    {
        return explosionForce;
    }
}
