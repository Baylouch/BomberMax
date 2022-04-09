using UnityEngine;

[RequireComponent(typeof(CharacterFirepoint))]
public class BombKicker : MonoBehaviour
{
    public int kickForce = 2; // Represent the case number the bomb will be kicked off

    [SerializeField] LayerMask explosionLayerMask;

    private CharacterFirepoint _firepoint;
    private Bomb bombToShoot;

    bool canKick = false;

    // Start is called before the first frame update
    void Start()
    {
        _firepoint = GetComponent<CharacterFirepoint>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (gameObject.tag == "Player")
                KickBomb();
        }
    }

    public void KickBomb()
    {
        // Just a security
        if (!canKick)
            return;

        // We set the bomb as trigger (to avoid pushing other collider)
        bombToShoot.UpdateIsTrigger(true);

        // We update the node's variable that contains the bomb as false
        StageManager.instance.GameGrid[bombToShoot.GetBombTileIndex()].hasBomb = false;

        // Reset danger tiles
        StageManager.instance.SetDangerTiles();

        // We get the position where the bomb will move
        Vector2 endPos = Vector2.zero;

        switch (_firepoint.GetFirePointDirection())
        {
            case MovementDirection.Up:
                endPos = new Vector2(bombToShoot.transform.position.x, bombToShoot.transform.position.y + kickForce);
                break;
            case MovementDirection.Down:
                endPos = new Vector2(bombToShoot.transform.position.x, bombToShoot.transform.position.y - kickForce);
                break;
            case MovementDirection.Right:
                endPos = new Vector2(bombToShoot.transform.position.x + kickForce, bombToShoot.transform.position.y);
                break;
            case MovementDirection.Left:
                endPos = new Vector2(bombToShoot.transform.position.x - kickForce, bombToShoot.transform.position.y);
                break;
        }

        // We move the bomb
        BombMover _bombMover = bombToShoot.gameObject.AddComponent<BombMover>();
        _bombMover.endPosition = new Vector3(endPos.x, endPos.y, 0f);
        _bombMover.explosionMask = explosionLayerMask;
        _bombMover.canMove = true;

        // When the bomb has arrived, we update it as it's just dropped, or explode if it collide with character, bomb or block.
        // (Handle by BombMover.cs)
    }

    public void UpdateBombToShoot(Bomb _bomb)
    {
        bombToShoot = _bomb;

        if (bombToShoot != null)
            canKick = true;
        else
            canKick = false;
    }
    
}
