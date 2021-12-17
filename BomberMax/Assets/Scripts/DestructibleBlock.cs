using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DestructibleBlock : MonoBehaviour
{  
    [SerializeField] GameObject bonusPrefab; // Move it into GameBonusData ?

    [SerializeField] bool spawnBonus = false;
    bool explosed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Explosion" && !explosed)
        {
            explosed = true;
            DestroyBlock();
        }
    }

    void SpawnBonus()
    {
        GameObject _bonusGO = Instantiate(bonusPrefab, transform.position, Quaternion.identity);
        _bonusGO.GetComponent<GameBonus>().SetupBonus();
        StageManager.instance.SetBonusNode(new Vector2(transform.position.x, transform.position.y), true);
    }

    public void SetupBlockBonus()
    {
        spawnBonus = true;
    }

    public void DestroyBlock()
    {
        if (spawnBonus)
        {
            SpawnBonus();
        }

        StageManager.instance.UpdateDestructibleBlock(new Vector2(transform.position.x, transform.position.y));

        Destroy(gameObject);
    }
}
