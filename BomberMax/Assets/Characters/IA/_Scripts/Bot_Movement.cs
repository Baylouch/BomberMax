using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterMovement))]
public class Bot_Movement : MonoBehaviour
{
    CharacterMovement movement;

    MovementDirection movementDirection;

    [SerializeField]List<Vector2> path;

    int pathIndex = 0;
    bool hasPath = false;
    bool reachedEndOfPath = true;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<CharacterMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasPath && !reachedEndOfPath)
        {
            SetMovement();
            UpdatePathIndex();
        }
        else
        {
            if (movementDirection != MovementDirection.None)
            {
                movementDirection = MovementDirection.None;
                movement.StopMovement();
            }
        }
    }

    void SetMovement()
    {
        if (transform.position.y < path[pathIndex].y)
        {
            // Up direction
            if (movementDirection != MovementDirection.Up)
            {
                movementDirection = MovementDirection.Up;
                movement.PerformMovement(movementDirection);
            }
        }
        else if (transform.position.y > path[pathIndex].y)
        {
            // Down direction
            if (movementDirection != MovementDirection.Down)
            {
                movementDirection = MovementDirection.Down;
                movement.PerformMovement(movementDirection);
            }
        }
        else if (transform.position.x < path[pathIndex].x)
        {
            // Right direction
            if (movementDirection != MovementDirection.Right)
            {
                movementDirection = MovementDirection.Right;
                movement.PerformMovement(movementDirection);
            }
        }
        else if (transform.position.x > path[pathIndex].x)
        {
            // Left direction
            if (movementDirection != MovementDirection.Left)
            {
                movementDirection = MovementDirection.Left;
                movement.PerformMovement(movementDirection);
            }
        }
    }

    void UpdatePathIndex()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);

        if ((currentPos - path[pathIndex]).magnitude <= 0.1f)
        {
            if (pathIndex < path.Count - 1)
            {
                pathIndex++;
            }
            else
            {
                reachedEndOfPath = true;
                hasPath = false;
            }
        }
    }

    public void SetPath(List<Vector2> pathToSet)
    {
        path = pathToSet;
        pathIndex = 0;
        reachedEndOfPath = false;
        hasPath = true;
    }

    public bool ReachedEndOfPath()
    {
        return reachedEndOfPath;
    }
}
