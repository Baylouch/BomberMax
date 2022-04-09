using UnityEngine;

public class CharacterFirepoint : MonoBehaviour
{
    [SerializeField] LayerMask firePointMask; // Only used to filter bomb for now.

    MovementDirection firePointDirection = MovementDirection.Up;

    Vector2 raycastDir;
    BombKicker _bombKicker;
    BombSpawner _bombSpawner;

    bool bombNextNode = false;
    bool canShootOtherBomb = false; // To know if we can shoot only our bombs (false) or another bombs (true)

    private void Start()
    {
        if (GetComponent<BombSpawner>())
            _bombSpawner = GetComponent<BombSpawner>();

        if (GetComponent<BombKicker>())
            _bombKicker = GetComponent<BombKicker>();
    }

    private void FixedUpdate()
    {
        // No use of next lines if no BombKick.cs attached to the character.
        // We still can use CharacterFirepoint only for "Shooting" action
        if (!_bombKicker)
            return;

        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, raycastDir, 1f, firePointMask);

        if (raycastHit2D && !bombNextNode)
        {
            Bomb _bomb = raycastHit2D.collider.GetComponent<Bomb>();

            if (!canShootOtherBomb)
            {
                if (_bombSpawner && (_bomb.GetBomberID() == _bombSpawner.GetBomberID()))
                {
                    _bombKicker.UpdateBombToShoot(_bomb);
                }
                else
                {
                    _bombKicker.UpdateBombToShoot(null);
                }
            }
            else
            {
                _bombKicker.UpdateBombToShoot(_bomb);
            }

            bombNextNode = true;
        }
        else if (!raycastHit2D && bombNextNode)
        {
            bombNextNode = false;
            _bombKicker.UpdateBombToShoot(null);
        }
    }

    public void UpdateFirePointDirection(MovementDirection _direction)
    {
        firePointDirection = _direction;

        switch (_direction)
        {
            case MovementDirection.Up:
                raycastDir = new Vector2(0f, 1f);
                break;
            case MovementDirection.Down:
                raycastDir = new Vector2(0f, -1f);

                break;
            case MovementDirection.Left:
                raycastDir = new Vector2(-1f, 0f);

                break;
            case MovementDirection.Right:
                raycastDir = new Vector2(1f, 0f);

                break;
        }
    }

    public MovementDirection GetFirePointDirection()
    {
        return firePointDirection;
    }
}
