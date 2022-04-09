/* BombMover.cs
 * 
 * Script attached on a bomb when it has been kicked.
 * 
 * Handle the movement of the bomb and the collision to launch explosion if it hurts a blocks / an enemy / another bomb...
 * 
 * 
 * */

using UnityEngine;

public class BombMover : MonoBehaviour
{
    public Vector3 endPosition;

    public bool canMove = false;

    public LayerMask explosionMask;

    // Because this script is attached only on a bomb on the same hierarchy object that have Bomb.cs on it, we can assume no check is required
    Bomb bombComponent;

    private void Start()
    {
        bombComponent = GetComponent<Bomb>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (transform.position != endPosition)
                transform.position = Vector3.MoveTowards(transform.position, endPosition, Time.deltaTime * 5f);
            else
            {
                int tileIndex = StageManager.instance.GameGrid.FindIndex(x => x.position.x == transform.position.x && x.position.y == transform.position.y);
                if (tileIndex != -1)
                    StageManager.instance.GameGrid[tileIndex].hasBomb = true;

                StageManager.instance.SetDangerTiles();
                bombComponent.UpdateIsTrigger(false);

                Destroy(this);
            }
        }
    }

    // Because attached on a bomb, we are sure there is already a collider linked to this script
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Maybe TODO : Explosion happens on the node where a character is or on the node just before the block and the bomb.
        // TODO Read a course on layermask (solution below as been found on unity forum (Patyrn's question)...)
        if ((explosionMask | (1 << collision.gameObject.layer)) == explosionMask)
        {
            bombComponent.Explode();
        }
    }
}
