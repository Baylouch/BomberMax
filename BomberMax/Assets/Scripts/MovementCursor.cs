using UnityEngine;
using UnityEngine.EventSystems;

public class MovementCursor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform cursorRectLimits;

    RectTransform cursorRect;

    Vector3 startPosition;

    bool cursorFollowing = false;

    // We takes the higher value from x or y to decide if we apply vertical or horizontal moves
    float moveValue; // 1 = moveUp ; 2 = moveDown ; 3 = moveRight ; 4 = moveLeft ; 0 = StopMove

    CharacterMovement movement;

    void Start()
    {
        startPosition = transform.position;
        cursorRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (cursorFollowing)
        {
            // We don't limit the Input.mousePosition value, just limit the button on the movement pad to receive movement
            // data while player has its finger on the screen.

            // Use mathf.clamp to restrict cursor pos on the rect limit
            cursorRect.position = Input.mousePosition;
            
            float _clampOnX = Mathf.Clamp(cursorRect.localPosition.x, -(cursorRectLimits.rect.width / 2) + (cursorRect.rect.width / 2), (cursorRectLimits.rect.width / 2) - (cursorRect.rect.width / 2));
            float _clampOnY = Mathf.Clamp(cursorRect.localPosition.y, -(cursorRectLimits.rect.height / 2) + (cursorRect.rect.height / 2), (cursorRectLimits.rect.height / 2) - (cursorRect.rect.height / 2));
            
            cursorRect.localPosition = new Vector3(_clampOnX, _clampOnY, 0f);

            // Set value to character movement relative to cursorRect.localPosition
            if (Mathf.Abs(cursorRect.localPosition.y) >= Mathf.Abs(cursorRect.localPosition.x))
            {
                if (cursorRect.localPosition.y >= (cursorRectLimits.rect.height / 8))
                {
                    if (moveValue != 1)
                    {
                        moveValue = 1;
                        movement.PerformMovement(MovementDirection.Up);
                    }
                }
                else if (cursorRect.localPosition.y <= -(cursorRectLimits.rect.height / 8))
                {
                    if (moveValue != 2)
                    {
                        moveValue = 2;
                        movement.PerformMovement(MovementDirection.Down);
                    }
                }
                else
                {
                    if (moveValue != 0)
                    {
                        moveValue = 0;
                        movement.StopMovement();
                    }
                }
            }
            else
            {
                if (cursorRect.localPosition.x >= (cursorRectLimits.rect.width / 8))
                {
                    if (moveValue != 3)
                    {
                        moveValue = 3;
                        movement.PerformMovement(MovementDirection.Right);
                    }
                }
                else if (cursorRect.localPosition.x <= -(cursorRectLimits.rect.width / 8))
                {
                    if (moveValue != 4)
                    {
                        moveValue = 4;
                        movement.PerformMovement(MovementDirection.Left);
                    }
                }
                else
                {
                    if (moveValue != 0)
                    {
                        moveValue = 0;
                        movement.StopMovement();
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Cursor follow the finger movement
        cursorFollowing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Cursor comes back to the start position
        cursorFollowing = false;
        transform.position = startPosition;
        movement.StopMovement();
        moveValue = 0;
    }

    public void SetCharacterMovement(CharacterMovement _movement)
    {
        movement = _movement;
    }
}
