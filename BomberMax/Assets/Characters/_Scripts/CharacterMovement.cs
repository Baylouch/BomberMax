using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterInfo))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] Transform movePoint;
    [SerializeField] LayerMask stopMovement;

    Animator _anim;
    CharacterHealth _health;

    bool bonusMove = false; // For now we just multiply by 2 the original speed
    float endBonusTime = 0f; // To know when player gets the bonus to stop it

    MovementDirection currentDirection = MovementDirection.None;
    MovementDirection firePointDirection = MovementDirection.Up;

    bool isMoving; // To know when to stop the animation, set to true when any movement is done, set to false when we want to stop move

    bool oppositeDirection; // When we go to the opposite direction from where we went, we can fast transition on.
    // But if we change direction perpendicularly we must wait until we reach the last position we going to.
    // It must be on tile to tile movement

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _health = GetComponent<CharacterHealth>();

        movePoint.parent = null;

        if (gameObject.tag == "Player")
        {
            FindObjectOfType<MovementCursor>().SetCharacterMovement(this);

            //if (GetComponent<CharacterInfo>().IsLocal)
            //{
            //}
        }


    }

    private void Update()
    {
        if (bonusMove)
        {
            if (endBonusTime <= Time.time || _health.IsDead())
            {
                bonusMove = false;
                _anim.speed = 1;
            }
        }

        if (_health.IsDead())
        {
            if (isMoving)
                StopMovement();

            return;
        }

        if ((transform.position - movePoint.position).magnitude <= 0.05f || oppositeDirection)
        {
            if (oppositeDirection)
                oppositeDirection = false;

            switch (currentDirection)
            {
                case MovementDirection.None:
                    isMoving = false;

                    break;
                case MovementDirection.Up:
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .2f, stopMovement))
                    {
                        movePoint.position += new Vector3(0f, 1f, 0f);
                    }

                    if (_anim.GetInteger("MovementHorizontal") != 0)
                        _anim.SetInteger("MovementHorizontal", 0);

                    if (_anim.GetInteger("MovementVertical") != 1)
                        _anim.SetInteger("MovementVertical", 1);

                    break;
                case MovementDirection.Down:
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .2f, stopMovement))
                    {
                        movePoint.position += new Vector3(0f, -1f, 0f);
                    }

                    if (_anim.GetInteger("MovementHorizontal") != 0)
                        _anim.SetInteger("MovementHorizontal", 0);

                    if (_anim.GetInteger("MovementVertical") != -1)
                        _anim.SetInteger("MovementVertical", -1);

                    break;
                case MovementDirection.Left:
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .2f, stopMovement))
                    {
                        movePoint.position += new Vector3(-1f, 0f, 0f);
                    }

                    if (_anim.GetInteger("MovementVertical") != 0)
                        _anim.SetInteger("MovementVertical", 0);

                    if (_anim.GetInteger("MovementHorizontal") != -1)
                        _anim.SetInteger("MovementHorizontal", -1);

                    break;
                case MovementDirection.Right:
                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .2f, stopMovement))
                    {
                        movePoint.position += new Vector3(1f, 0f, 0f);
                    }

                    if (_anim.GetInteger("MovementVertical") != 0)
                        _anim.SetInteger("MovementVertical", 0);

                    if (_anim.GetInteger("MovementHorizontal") != 1)
                        _anim.SetInteger("MovementHorizontal", 1);

                    break;
            }
        }

        if (transform.position != movePoint.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, ((bonusMove) ? speed + 2f : speed) * Time.deltaTime);
        }
        else
        {
            if (!isMoving)
            {
                if (_anim.GetInteger("MovementHorizontal") != 0)
                {
                    _anim.SetInteger("MovementHorizontal", 0);
                }
                if (_anim.GetInteger("MovementVertical") != 0)
                {
                    _anim.SetInteger("MovementVertical", 0);
                }
            }
        }
    }

    public void StopMovement()
    {
        currentDirection = MovementDirection.None;
    }

    public void PerformMovement(MovementDirection _direction)
    {
        MovementDirection lastDirection = currentDirection;

        firePointDirection = _direction;

        switch (_direction)
        {
            case MovementDirection.Up:
                currentDirection = MovementDirection.Up;

                if (lastDirection == MovementDirection.Down)
                {
                    oppositeDirection = true;
                }
                break;
            case MovementDirection.Down:
                currentDirection = MovementDirection.Down;

                if (lastDirection == MovementDirection.Up)
                {
                    oppositeDirection = true;
                }
                break;
            case MovementDirection.Left:
                currentDirection = MovementDirection.Left;

                if (lastDirection == MovementDirection.Right)
                {
                    oppositeDirection = true;

                }
                break;
            case MovementDirection.Right:
                currentDirection = MovementDirection.Right;

                if (lastDirection == MovementDirection.Left)
                {
                    oppositeDirection = true;

                }
                break;
        }

        if (!isMoving)
            isMoving = true;
    }

    public void ActivateSpeedBonus(float _duration)
    {
        bonusMove = true;
        endBonusTime = Time.time + _duration; // In Update we can check if bonusMove and if Time.time > to endBonusTime.
        _anim.speed = 1.5f;
    }

    // Method(getter) used in shotgun to know the shoot orientation
    public MovementDirection GetFirePointDirection()
    {
        return firePointDirection;
    }
}
