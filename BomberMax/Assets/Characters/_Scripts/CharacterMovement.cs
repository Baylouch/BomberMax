using UnityEngine;

[RequireComponent(typeof(CharacterInfo))]
public class CharacterMovement : MonoBehaviour
{
    public bool canMove = true; // Used when player die

    [SerializeField] float speed = 3f;
    [SerializeField] Transform movePoint;
    [SerializeField] LayerMask stopMovement;

    Animator _anim;

    bool bonusMove = false; // For now we just multiply by 2 the original speed
    float endBonusTime = 0f; // To know when player gets the bonus to stop it

    int moveVertical, moveHorizontal; // -1 is for down/left +1 is for up/right 0's are for no movement
    bool isMoving; // To know when to stop the animation, set to true when any movement is done, set to false when we want to stop move
    bool movementHasBeenSwitch; // To fix the icy look of the animation when player update its movement to another direction

    Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        movePoint.parent = null;
        previousPosition = transform.position;

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
            if (endBonusTime <= Time.time)
            {
                bonusMove = false;
                _anim.speed = 1;
            }
        }
    }

    private void FixedUpdate()
    {
        // If direction has changed, we don't want to wait until player get to the next waypoint,
        // we want a fast transition to get a responsive movement from the character
        // We want the character to go back where he was before movement has been changed IF he already moves from the next waypoint that we dont want anymore
        // only if its distance from the previous position is less than the next waypoint position
        if (movementHasBeenSwitch)
        {
            if ((transform.position - movePoint.position).magnitude >= 0.5f && movePoint.position != previousPosition)
            {
                movePoint.position = previousPosition;
            }
        }


        if ((transform.position - movePoint.position).magnitude <= 0.05f)
        {
            // To avoid "icy" issue
            if (movementHasBeenSwitch)
            {
                movementHasBeenSwitch = false;

                if (moveVertical == 0)
                {
                    _anim.SetInteger("MovementVertical", 0);
                    _anim.SetInteger("MovementHorizontal", moveHorizontal);

                }
                else
                {
                    _anim.SetInteger("MovementHorizontal", 0);
                    _anim.SetInteger("MovementVertical", moveVertical);
                }
            }

            if (moveVertical == 1)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .2f, stopMovement))
                {
                    movePoint.position += new Vector3(0f, 1f, 0f);
                }
            }
            else if (moveVertical == -1)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .2f, stopMovement))
                {
                    movePoint.position += new Vector3(0f, -1f, 0f);
                }
            }
            else if (moveHorizontal == 1)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .2f, stopMovement))
                {
                    movePoint.position += new Vector3(1f, 0f, 0f);
                }
            }
            else if (moveHorizontal == -1)
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .2f, stopMovement))
                {
                    movePoint.position += new Vector3(-1f, 0f, 0f);
                }
            }
        }
        else
        {
            if (previousPosition != movePoint.position)
            {
                previousPosition = movePoint.position;
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

    // Method to set the movement parameters : anim param is the same as int param used in this script
    // this method always set isMoving to true because it is only call when movement is ask
    void SetMovementParemeters(int _moveVertical, int _moveHorizontal)
    {
        isMoving = true;

        moveVertical = _moveVertical;
        moveHorizontal = _moveHorizontal;

        movementHasBeenSwitch = true;
    }

    public void StopMove()
    {
        moveVertical = 0;
        moveHorizontal = 0;

        isMoving = false;
    }

    public void MoveUp()
    {
        if (!canMove)
            return;

        SetMovementParemeters(1, 0);
    }

    public void MoveDown()
    {
        if (!canMove)
            return;

        SetMovementParemeters(-1, 0);
    }

    public void MoveLeft()
    {
        if (!canMove)
            return;

        SetMovementParemeters(0, -1);
    }

    public void MoveRight()
    {
        if (!canMove)
            return;

        SetMovementParemeters(0, 1);
    }

    public void ActivateSpeedBonus(float _duration)
    {
        bonusMove = true;
        endBonusTime = Time.time + _duration; // In Update we can check if bonusMove and if Time.time > to endBonusTime.
        _anim.speed = 1.5f;
    }
}
