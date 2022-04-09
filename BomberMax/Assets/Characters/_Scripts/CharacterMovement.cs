using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterFirepoint))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField] Transform movePoint;
    [SerializeField] LayerMask stopMovement;

    Animator _anim;
    CharacterHealth _health;
    CharacterFirepoint _firepoint;

    bool bonusMove = false; // For now we just multiply by 2 the original speed
    float endBonusTime = 0f; // To know when player gets the bonus to stop it

    MovementDirection currentDirection = MovementDirection.None;

    bool isMoving; // To know when to stop the animation, set to true when any movement is done, set to false when we want to stop move

    bool oppositeDirection; // When we go to the opposite direction from where we went, we can fast transition on.
    // But if we change direction perpendicularly we must wait until we reach the last position we going to.
    // It must be on tile to tile movement

    float idleTimer = 0f; // To be able to just rotate the character and not moving instantly (rly short timing)
    float idleThreshold = 0.14f;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _health = GetComponent<CharacterHealth>();
        _firepoint = GetComponent<CharacterFirepoint>();

        movePoint.parent = null;

        if (gameObject.tag == "Player")
        {
            foreach (MovementButton button in FindObjectsOfType<MovementButton>())
            {
                button.SetupMovement(this);
            }

            //if (Local Player)
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

        if (!isMoving && idleTimer < Time.time)
        {
            if (!_anim.GetBool("Idle"))
                _anim.SetBool("Idle", true);
        }
        else
        {
            if (idleTimer < Time.time)
            {
                if (_anim.GetBool("Idle"))
                    _anim.SetBool("Idle", false);
            }
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
                    if (_anim.GetInteger("MovementHorizontal") != 0)
                        _anim.SetInteger("MovementHorizontal", 0);

                    if (_anim.GetInteger("MovementVertical") != 1)
                        _anim.SetInteger("MovementVertical", 1);

                    if (_anim.GetBool("Idle"))
                        break;

                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .2f, stopMovement))
                    {
                        movePoint.position += new Vector3(0f, 1f, 0f);
                    }

                    break;
                case MovementDirection.Down:
                    if (_anim.GetInteger("MovementHorizontal") != 0)
                        _anim.SetInteger("MovementHorizontal", 0);

                    if (_anim.GetInteger("MovementVertical") != -1)
                        _anim.SetInteger("MovementVertical", -1);

                    if (_anim.GetBool("Idle"))
                        break;

                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .2f, stopMovement))
                    {
                        movePoint.position += new Vector3(0f, -1f, 0f);
                    }

                    break;
                case MovementDirection.Left:
                    if (_anim.GetInteger("MovementVertical") != 0)
                        _anim.SetInteger("MovementVertical", 0);

                    if (_anim.GetInteger("MovementHorizontal") != -1)
                        _anim.SetInteger("MovementHorizontal", -1);

                    if (_anim.GetBool("Idle"))
                        break;

                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .2f, stopMovement))
                    {
                        movePoint.position += new Vector3(-1f, 0f, 0f);
                    }

                    break;
                case MovementDirection.Right:
                    if (_anim.GetInteger("MovementVertical") != 0)
                        _anim.SetInteger("MovementVertical", 0);

                    if (_anim.GetInteger("MovementHorizontal") != 1)
                        _anim.SetInteger("MovementHorizontal", 1);

                    if (_anim.GetBool("Idle"))
                        break;

                    if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .2f, stopMovement))
                    {
                        movePoint.position += new Vector3(1f, 0f, 0f);
                    }

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
                if (_anim.GetBool("Idle"))
                {
                    _anim.SetBool("Idle", true);
                }
            }
        }
    }

    public void StopMovement()
    {
        currentDirection = MovementDirection.None;

        idleTimer = Time.time + idleThreshold;
    }

    public void PerformMovement(MovementDirection _direction)
    {
        MovementDirection lastDirection = currentDirection;

        _firepoint.UpdateFirePointDirection(_direction);

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

        idleTimer = Time.time + idleThreshold;
    }

    public void ActivateSpeedBonus(float _duration)
    {
        bonusMove = true;
        endBonusTime = Time.time + _duration; // In Update we can check if bonusMove and if Time.time > to endBonusTime.
        _anim.speed = 1.5f;
    }
}
