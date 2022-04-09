using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DestructibleBlock : MonoBehaviour
{  
    bool explosed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Explosion" && !explosed)
        {
            explosed = true;
            DestroyBlock();
        }
    }

    public void DestroyBlock()
    {
        if (GetComponent<BonusSpawner>())
        {
            GetComponent<BonusSpawner>().SpawnBonus();
        }

        StageManager.instance.UpdateDestructibleBlock(new Vector2(transform.position.x, transform.position.y));

        Destroy(gameObject);
    }
}
