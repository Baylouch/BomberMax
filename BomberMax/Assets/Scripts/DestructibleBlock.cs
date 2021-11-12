using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DestructibleBlock : MonoBehaviour
{
    [SerializeField] GameBonusData[] bonusDatas;
    [SerializeField] GameObject bonusPrefab; // Move it into GameBonusData ?

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
        for (int i = 0; i < bonusDatas.Length; i++)
        {        
            int _randomValue = Random.Range(0,100);
            if (bonusDatas[i].chanceToSpawn > _randomValue)
            {
                GameObject _bonusGO = Instantiate(bonusPrefab, transform.position, Quaternion.identity);
                _bonusGO.GetComponent<GameBonus>().SetupBonus(bonusDatas[i]);

                break;
            }
        }
    }

    public void DestroyBlock()
    {
        SpawnBonus();

        int tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == transform.position.x && x.position.y == transform.position.y);
        StageManager.instance.TilesInfo[tileIndex].hasDestructibleBlock = false;

        Destroy(gameObject);
    }
}
