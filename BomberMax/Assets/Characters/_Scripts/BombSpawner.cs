using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public const int MaxBombNumber = 5;
    public const int MaxExplosionForce = 10;

    private int maxBombNumb = 1;
    private int currentBombNumb = 0; // To know the current bombs number spawned
    [SerializeField]private int explosionForce = 1;

    [SerializeField] GameObject bombPrefab;

    private void Start()
    {
        // Handle multiplayer later
        if (gameObject.tag == "Player")
        {
            FindObjectOfType<PlayerAction_Bomb>().SetupBombSpawner(this);
            GameCanvas.instance.SetBombBonusText(maxBombNumb);
            GameCanvas.instance.SetExplosionBonusText(explosionForce);
        }
    }

    private void Update()
    {
        // TODO Create a button to spawn the bomb
        if (gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DropBomb();
            }
        }
    }

    public void DropBomb()
    {
        if (currentBombNumb >= maxBombNumb)
            return;

        // Get the bomb spawn position
        // we want a spawn at the center of the tile where player is
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        // We want a reference to the current TileInfo position to make check of blocks, bombs, and set bomb on it
        int _currentTileIndex = StageManager.instance.GameGrid.FindIndex(x => x.position == spawnPos);

        // Logic to know if there is already a bomb on the spot
        if (StageManager.instance.GameGrid[_currentTileIndex].hasBomb == true)
        {
            // Just make it impossible to spawn bomb
            return;
        }

        // Spawn the bomb
        GameObject _curBomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        Bomb _bomb = _curBomb.GetComponent<Bomb>();

        _bomb.SetupBomb(this);

        StageManager.instance.GameGrid[_currentTileIndex].hasBomb = true;

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
            if (gameObject.tag == "Player")
                GameCanvas.instance.SetBombBonusText(maxBombNumb);
        }
    }

    public void IncrementExplosionForce()
    {
        if (explosionForce < MaxExplosionForce)
        {
            explosionForce++;
            if (gameObject.tag == "Player")
                GameCanvas.instance.SetExplosionBonusText(explosionForce);
        }
    }

    public int GetExplosionForce()
    {
        return explosionForce;
    }
}
