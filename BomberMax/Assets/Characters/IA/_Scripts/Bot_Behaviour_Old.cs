/* Bot_Behaviour.cs
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */

using UnityEngine;
using System.Collections.Generic;

// public enum Bot_Mode { Easy, Medium, Hard };


[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(BombSpawner))]
[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterInfo))]
public class Bot_Behaviour_Old : MonoBehaviour
{
    CharacterHealth health;
    CharacterMovement movement;
    BombSpawner bombSpawner;
    CharacterInfo infos;

    List<Bomb> knownBombs = new List<Bomb>();
    List<Vector2> dangerPosition = new List<Vector2>();
    Dictionary<int, int> bombLengthByID = new Dictionary<int, int>(); // Dictionnary to match key (character ID) with value (known bomb length) :
    // When we register a bomb in knownBomb and this one explode, we set the current explosion length relative to character ID.
    // We don't include this character ID in the dictionnary because we can simply get it with bombSpawner comparing bomb ID with this char infos

    float AIVision = 3f;

    Vector2 lastPosition;

    // 0 = Up , 1 = Down , 2 = Right , 3 = Left
    int currentDirection = -1;
    bool movementSet = false;
    int retainLastDirection = -1; // To retain when we change perpendiculary the direction to try for instance left then right and not left left left...

    float raycastOffset = .5f;

    bool canMoveLeft, canMoveRight, canMoveUp, canMoveDown;


    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<CharacterHealth>();
        movement = GetComponent<CharacterMovement>();
        bombSpawner = GetComponent<BombSpawner>();
        infos = GetComponent<CharacterInfo>();

        currentDirection = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (health.IsDead())
            return;


        

        Vector2 currentPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

        for (int i = 0; i < dangerPosition.Count; i++)
        {
            // We want to check if current position is on a danger position then we go out of there


            // We check for each direction if the next move will get the bot on a danger position
            switch(currentDirection)
            {
                case 0:
                    if (currentPosition.x == dangerPosition[i].x && currentPosition.y + 1 == dangerPosition[i].y)
                    {
                        // Next move will be on a danger position, we must change direction

                    }
                    break;
                case 1:

                    break;
                case 2:

                    break;
                case 3:

                    break;
            }
        }

        switch (currentDirection)
        {
            case 0:
                PerformRaycastFront(Vector2.up);

                break;
            case 1:
                PerformRaycastFront(Vector2.down);

                break;
            case 2:
                PerformRaycastFront(Vector2.right);

                break;
            case 3:
                PerformRaycastFront(Vector2.left);

                break;
            default: 
                Debug.Log("Error with direction");
                break;
        }

        if (!movementSet)
        {
            movementSet = true;
            SetMovement();
        }
    }



    // A method to randomly switch directions when AI enter a new tiles
    // it must check if the AI can move in directions with canMove variables and detect if the move represent a danger
    void RandomlySwitchDirection()
    {

    }

    // This method will cast raycasts all around the AI relative to its vision (who's set with bot difficulty)
    // determine what bot can see : bombs, bonus, enemy...
    void PerformAIVision()
    {
        // 0 = up hit ; 1 = down hit ; 2 = left hit ; 3 = right hit
        RaycastHit2D[] hits2D = new RaycastHit2D[4];

        Vector2 startRaycastPos;
        Vector2 direction;

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    startRaycastPos = new Vector2(transform.position.x, transform.position.y + raycastOffset);
                    direction = Vector2.up;
                    break;
                case 1:
                    startRaycastPos = new Vector2(transform.position.x, transform.position.y - raycastOffset);
                    direction = Vector2.down;
                    break;
                case 2:
                    startRaycastPos = new Vector2(transform.position.x - raycastOffset, transform.position.y);
                    direction = Vector2.left;
                    break;
                case 3:
                    startRaycastPos = new Vector2(transform.position.x + raycastOffset, transform.position.y);
                    direction = Vector2.right;
                    break;
                default:
                    Debug.LogError("This should never happen");
                    startRaycastPos = Vector2.zero;
                    direction = Vector2.zero;
                    break;
            }

            hits2D[i] = Physics2D.Raycast(startRaycastPos, direction, AIVision);

            if (hits2D[i].collider != null)
            {
                if (hits2D[i].collider.GetComponent<Bomb>())
                {
                    if (AddBombInList(hits2D[i].collider.GetComponent<Bomb>()))
                    {
                        SetDangerPositions();

                    }
                }
                else if (hits2D[i].collider.GetComponent<GameBonus>())
                {

                }
                else if (hits2D[i].collider.tag == "Player")
                {

                }
            }
        }
    }

    // true = we added a new bomb , false = bomb was already in the list
    bool AddBombInList(Bomb _bomb)
    {
        int _index = knownBombs.FindIndex(x => x.GetThisBombID() == _bomb.GetThisBombID());

        if (_index == -1) // We can add the bomb
        {
            knownBombs.Add(_bomb);
            return true;
        }

        return false;
    }

    // Method to set the danger position list using bombs known position
    void SetDangerPositions()
    {
        // Reset danger position
        dangerPosition.Clear();

        // We loop trough every known bombs
        for (int i = 0; i < knownBombs.Count; i++)
        {
            // We get the current bomb position
            Vector2 _bombPos = knownBombs[i].transform.position;
            // We set a temporary bomb length (used if we havnt the ID in the dictionnary)
            int _bombLength = 1;

            // We try to get the bomb ID in the dictionnary to know the last length retains
            bool _containsID = bombLengthByID.ContainsKey(knownBombs[i].GetBombSpawnerID());

            if (_containsID)
            {
                _bombLength = bombLengthByID[knownBombs[i].GetBombSpawnerID()];
            }

            // If it's ouf bomb, we set the exact length
            if (knownBombs[i].GetBombSpawnerID() == infos.CharID)
            {
                _bombLength = bombSpawner.GetExplosionForce();
            }

            // Now we can set dangerPosition : we need to use a mecanics as SetExplosionLength() in Bomb.cs

            bool stopLeftAxis = false, stopRightAxis = false, stopDownAxis = false, stopUpAxis = false;

            int _tileIndex = -1;

            for (int j = 1; j <= _bombLength; j++)
            {
                // TODO Make array of stopAxis and set position comparing Vector2 directly to not have repetitive code for each axis
                // Get upper bomb's tiles
                if (!stopUpAxis)
                {
                    // We get the index relative to the position we want to check
                    _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _bombPos.x && x.position.y == _bombPos.y + j);

                    // If we found the index
                    if (_tileIndex != -1)
                    {
                        // If there is a destructible block or undestructible one, we stop checking this axis
                        if (StageManager.instance.TilesInfo[_tileIndex].hasDestructibleBlock || StageManager.instance.TilesInfo[_tileIndex].hasUndestructibleBlock)
                        {
                            stopUpAxis = true;
                        }
                        // Else we add it to the danger position list
                        else
                        {
                            dangerPosition.Add(StageManager.instance.TilesInfo[_tileIndex].position);
                        }
                    }
                    else
                    {
                        stopUpAxis = true;
                    }
                }

                // Get down bomb's tiles
                if (!stopDownAxis)
                {
                    // We get the index relative to the position we want to check
                    _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _bombPos.x && x.position.y == _bombPos.y - j);

                    // If we found the index
                    if (_tileIndex != -1)
                    {
                        // If there is a destructible block or undestructible one, we stop checking this axis
                        if (StageManager.instance.TilesInfo[_tileIndex].hasDestructibleBlock || StageManager.instance.TilesInfo[_tileIndex].hasUndestructibleBlock)
                        {
                            stopDownAxis = true;
                        }
                        // Else we add it to the danger position list
                        else
                        {
                            dangerPosition.Add(StageManager.instance.TilesInfo[_tileIndex].position);
                        }
                    }
                    else
                    {
                        stopDownAxis = true;
                    }
                }

                // Get left bomb's tiles
                if (!stopLeftAxis)
                {
                    // We get the index relative to the position we want to check
                    _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _bombPos.x - j && x.position.y == _bombPos.y);

                    // If we found the index
                    if (_tileIndex != -1)
                    {
                        // If there is a destructible block or undestructible one, we stop checking this axis
                        if (StageManager.instance.TilesInfo[_tileIndex].hasDestructibleBlock || StageManager.instance.TilesInfo[_tileIndex].hasUndestructibleBlock)
                        {
                            stopLeftAxis = true;
                        }
                        // Else we add it to the danger position list
                        else
                        {
                            dangerPosition.Add(StageManager.instance.TilesInfo[_tileIndex].position);
                        }
                    }
                    else
                    {
                        stopLeftAxis = true;
                    }
                }

                // Get right bomb's tiles
                if (!stopRightAxis)
                {
                    // We get the index relative to the position we want to check
                    _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _bombPos.x + j && x.position.y == _bombPos.y);

                    // If we found the index
                    if (_tileIndex != -1)
                    {
                        // If there is a destructible block or undestructible one, we stop checking this axis
                        if (StageManager.instance.TilesInfo[_tileIndex].hasDestructibleBlock || StageManager.instance.TilesInfo[_tileIndex].hasUndestructibleBlock)
                        {
                            stopRightAxis = true;
                        }
                        // Else we add it to the danger position list
                        else
                        {
                            dangerPosition.Add(StageManager.instance.TilesInfo[_tileIndex].position);
                        }
                    }
                    else
                    {
                        stopRightAxis = true;
                    }
                }
            }
        }
    }

    // Method to know if there is something in front of the AI (relative to its direction)
    void PerformRaycastFront(Vector2 _direction)
    {
        RaycastHit2D hit2D;
        Vector2 startRaycastPos; // We must set the start position to not collide with the bot

        if (_direction.x != 0) 
        { 
            if (_direction.x > 0) 
                startRaycastPos = new Vector2(transform.position.x + raycastOffset, transform.position.y);
            else 
                startRaycastPos = new Vector2(transform.position.x - raycastOffset, transform.position.y);
        }
        else
        {
            if (_direction.y > 0)
                startRaycastPos = new Vector2(transform.position.x, transform.position.y + raycastOffset);
            else
                startRaycastPos = new Vector2(transform.position.x, transform.position.y - raycastOffset);
        }

        hit2D = Physics2D.Raycast(startRaycastPos, _direction, 0.5f);

        if (hit2D.collider != null)
        {
            if (hit2D.collider.gameObject.layer == LayerMask.NameToLayer("StopMovement"))
            {
                if (hit2D.collider.GetComponent<DestructibleBlock>())
                {
                    Debug.Log("It's a destructible block, we drop a bomb");

                    bombSpawner.SpawnBomb();
                    InverseDirectionMovement();

                }
                else if (hit2D.collider.GetComponent<Bomb>())
                {
                    Debug.Log("It's a bomb, we must hide");

                    InverseDirectionMovement();
                }
                else
                {
                    Debug.Log("It's an undestructible block or a stage limit, we change direction");
                    ChangeMovementPerpendicular();
                }
            }
        }
    }

    void SetMovement()
    {
        switch (currentDirection)
        {
            case 0:
                movement.MoveUp();
                break;
            case 1:
                movement.MoveDown();
                break;
            case 2:
                movement.MoveRight();
                break;
            case 3:
                movement.MoveLeft();
                break;
        }
    }

    // Method to inverse the current direction
    void InverseDirectionMovement()
    { 
        switch (currentDirection)
        {
            case 0:
                currentDirection = 1;
                break;
            case 1:
                currentDirection = 0;
                break;
            case 2:
                currentDirection = 3;
                break;
            case 3:
                currentDirection = 2;
                break;

        }

        movementSet = false;
    }

    // Method to switch movement perpendiculary
    void ChangeMovementPerpendicular()
    {
        int _randomValue = Random.Range(0, 1);

        if (retainLastDirection != -1)
        {
            if (retainLastDirection == 0) { currentDirection = 1; }
            else if (retainLastDirection == 1) { currentDirection = 0; }
            else if (retainLastDirection == 2) { currentDirection = 3; }
            else if (retainLastDirection == 3) { currentDirection = 2; }

            movementSet = false;
            retainLastDirection = -1;

            return;
        }

        switch (currentDirection)
        {
            case 0:
            case 1:
                if (_randomValue == 0)
                    currentDirection = 2;
                else
                    currentDirection = 3;
                
                break;
            case 2:
            case 3:
                if (_randomValue == 0)
                    currentDirection = 0;
                else
                    currentDirection = 1;

                break;
        }

        movementSet = false;
        retainLastDirection = currentDirection;
    }
}
