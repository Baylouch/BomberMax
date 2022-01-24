using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletForce = 5f;

    bool hasHit = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * bulletForce * 100f);
        Destroy(gameObject, 20f);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && !hasHit)
        {
            hasHit = true;

            // We want to know the collision. There are all cases :
            // Destructible blocks, Undestructible blocks, Character(Player/IA), Stage limit, Bomb, Another bullet ?, ...
            if (collision.GetComponent<DestructibleBlock>())
            {
                collision.GetComponent<DestructibleBlock>().DestroyBlock();
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("StopMovement")) // (Undestructible blocks + Stage limit)
            {
                Debug.Log("Undestructible or stage limit !");

            }
            else if (collision.GetComponent<Bomb>())
            {
                collision.GetComponent<Bomb>().Explode();

            }
            else if (collision.GetComponent<CharacterHealth>()) // Set as the same layer as the shotGun gameobject (Player or IA)
            {
                collision.GetComponent<CharacterHealth>().DecrementHealth();

            }

            // Spawn explosion gfx?

            Destroy(gameObject);
        }
    }

}
