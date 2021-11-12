/* Bot_behaviour.cs
 * 
 * Bot get infos such as bombs in game, game bonus, enemy positions and will react to this relatively to its difficulty and the distance of them.
 * 
 * It retain the bomber ID and its explosion force when a bomb explode.
 * 
 * A path is created to set its movement when it goes for chase player, get bonus...
 * 
 * It has a list of danger positions and a method is used to create a path to a safe position.
 * 
 * When it's stuck because of undestructible block, border limit and danger position, it's stop and wait until the bomb has explode.
 * 
 * */

using System.Collections.Generic;
using UnityEngine;

// Enumeration to have clearly description of path direction
public enum PathDirection { Up = 0, Down = 1, Left = 2, Right = 3 };

// To know bot priority, more the value is more the priority is. None is when there is nothing around the bot he just walk to another place
public enum BotPriority { None = -1, DestructibleBlock = 0, Bonus = 1, Enemy = 2, InDanger = 3 };

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(BombSpawner))]
[RequireComponent(typeof(CharacterHealth))]
[RequireComponent(typeof(CharacterInfo))]
public class Bot_Behaviour : MonoBehaviour
{
    CharacterHealth health;
    CharacterMovement movement;
    BombSpawner bombSpawner;
    CharacterInfo infos;

    float AIVision = 3f;

    Dictionary<int, int> bombLengthByID = new Dictionary<int, int>(); // Dictionnary to match key (character ID) with value (known bomb length)
    List<Bomb> bombsList = new List<Bomb>();
    List<Vector2> dangerPosition = new List<Vector2>();

    bool hasPath;
    Vector2[] path;

    bool isInDanger = false; // to know if the bot is on a danger position

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<CharacterHealth>();
        movement = GetComponent<CharacterMovement>();
        bombSpawner = GetComponent<BombSpawner>();
        infos = GetComponent<CharacterInfo>();


    }

    // Update is called once per frame
    void Update()
    {
        if (health.IsDead())
            return;

        if (!hasPath)
        {
            CreatePath();
        }
    }

    void CreatePath()
    {
        Debug.Log("Creating path...");

        Vector2 _currentPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

        List<Vector2> pathUp = new List<Vector2>();
        List<Vector2> pathDown = new List<Vector2>();
        List<Vector2> pathLeft = new List<Vector2>();
        List<Vector2> pathRight = new List<Vector2>();

        // Establish priority : - Need a safe place because in danger place 
        //                      - Is there a bonus in AI Vision ?
        //                      - Is there a player in AI Vision we want to attack ? (relative to bomb length)
        //                      - Is there a destructible block we can destroy and be safe after ? (relative to bomb length)

        BotPriority _priority = BotPriority.None;

        // Most priority is if the bot is in danger he must to find a safe place
        if (isInDanger)
        {
            _priority = BotPriority.InDanger;
        }
        else
        {

        }



        // We loop while we didn't do the 4 axis
        for (int i = 0; i < 4; i++)
        {
            PathDirection _pathDirection = (PathDirection)i;

            int _tileIndex = -1;

            // We keep track of the position we check
            int _currentRangeOnX = 0;
            int _currentRangeOnY = 0;

            // Usefull to keep memory of last range when we must back to previous tile when we're block to try another way
            int _lastRangeOnX = 0;
            int _lastRangeOnY = 0;

            int _testedDirection = 0; // To know when we change direction, if there is no way left to go.

            // For instance we look for up path, if we're stuck by any block or border, we try to go left or right, then if we're stuck again we continue up or down...
            bool _perpendicularDirection = false;
            // When we switch perpendicular movement, we need a variable to set if its left/right , up/down...
            bool _oppositeDirection = false;

            // To know when we randomize the new perpendicular way if we already did once, so we don't loop on left left left for instance
            bool _alreadyRandomizeValue = false;

            bool _pathDone = false;

            bool _firstIteration = true; // To know when it's the first iteration, to change path if it's actually stuck

            while (!_pathDone || (_currentRangeOnX + _currentRangeOnY) < AIVision)
            {
                switch (_pathDirection)
                {
                    // Up path
                    case PathDirection.Up:
                        if (!_perpendicularDirection) // It means we increment the axis we're testing
                        {
                            if (!_oppositeDirection) _currentRangeOnY++;
                            else _currentRangeOnY--;
                        }
                        else
                        {
                            if (!_oppositeDirection) _currentRangeOnX++;
                            else _currentRangeOnX--;
                        }
                        break;
                    // Down path
                    case PathDirection.Down:
                        if (!_perpendicularDirection) // It means we increment the axis we're testing
                        {
                            if (!_oppositeDirection) _currentRangeOnY--;
                            else _currentRangeOnY++;

                        }
                        else
                        {
                            if (!_oppositeDirection) _currentRangeOnX++;
                            else _currentRangeOnX--;
                        }
                        break;
                    // Left path
                    case PathDirection.Left:
                        if (!_perpendicularDirection) // It means we increment the axis we're testing
                        {
                            if (!_oppositeDirection) _currentRangeOnX--;
                            else _currentRangeOnX++;
                        }
                        else
                        {
                            if (!_oppositeDirection) _currentRangeOnY++;
                            else _currentRangeOnY--;
                        }
                        break;
                    // Right path
                    case PathDirection.Right:
                        if (!_perpendicularDirection) // It means we increment the axis we're testing
                        {
                            if (!_oppositeDirection) _currentRangeOnX++;
                            else _currentRangeOnX--;

                        }
                        else
                        {
                            if (!_oppositeDirection) _currentRangeOnY++;
                            else _currentRangeOnY--;
                        }
                        break;
                }

                // We get the tile to check
                _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _currentPosition.x + _currentRangeOnX &&
                                                                            x.position.y == _currentPosition.y + _currentRangeOnY);


                // Test if we're stuck in this way
                if (_tileIndex == -1 ||
                    StageManager.instance.TilesInfo[_tileIndex].hasDestructibleBlock ||
                    StageManager.instance.TilesInfo[_tileIndex].hasUndestructibleBlock)
                {
                    // We can't continue this way
                    // We must decrement the last range we incremented
                    // and try to go perpendicular
                    if (_firstIteration)
                        _pathDone = true;

                    _testedDirection++;

                    _currentRangeOnX = _lastRangeOnX;
                    _currentRangeOnY = _lastRangeOnY;

                    _perpendicularDirection = !_perpendicularDirection;

                    if (!_alreadyRandomizeValue)
                    {
                        int _randomValue = Random.Range(0, 2);

                        if (_randomValue == 0) { _oppositeDirection = false; }
                        else { _oppositeDirection = true; }

                        _alreadyRandomizeValue = true;
                    }
                    else
                    {
                        _oppositeDirection = !_oppositeDirection;
                    }
                }
                else
                {
                    _testedDirection = 0;
                    _alreadyRandomizeValue = false;


                    switch (_pathDirection)
                    {
                        case PathDirection.Up:

                            pathUp.Add(StageManager.instance.TilesInfo[_tileIndex].position);

                            if (pathUp.Count >= AIVision)
                                _pathDone = true;
                            break;
                        case PathDirection.Down:

                            pathDown.Add(StageManager.instance.TilesInfo[_tileIndex].position);

                            if (pathDown.Count >= AIVision)
                                _pathDone = true;
                            break;
                        case PathDirection.Left:

                            pathLeft.Add(StageManager.instance.TilesInfo[_tileIndex].position);

                            if (pathLeft.Count >= AIVision)
                                _pathDone = true;
                            break;
                        case PathDirection.Right:

                            pathRight.Add(StageManager.instance.TilesInfo[_tileIndex].position);

                            if (pathRight.Count >= AIVision)
                                _pathDone = true;
                            break;
                    }

                    _lastRangeOnX = _currentRangeOnX;
                    _lastRangeOnY = _currentRangeOnY;
                }

                _firstIteration = false;

                if (_testedDirection >= 3)
                {
                    _pathDone = true;
                }

                if (_pathDone)
                {
                    break;
                }

            }

        }

        Debug.Log("Path Up :");
        for (int i = 0; i < pathUp.Count; i++)
        {
            Debug.Log(i + " : " + pathUp[i].x + " ; " + pathUp[i].y);
        }

        Debug.Log("Path Down :");
        for (int i = 0; i < pathDown.Count; i++)
        {
            Debug.Log(i + " : " + pathDown[i].x + " ; " + pathDown[i].y);
        }

        Debug.Log("Path Left :");
        for (int i = 0; i < pathLeft.Count; i++)
        {
            Debug.Log(i + " : " + pathLeft[i].x + " ; " + pathLeft[i].y);
        }

        Debug.Log("Path Right :");
        for (int i = 0; i < pathRight.Count; i++)
        {
            Debug.Log(i + " : " + pathRight[i].x + " ; " + pathRight[i].y);
        }

        hasPath = true;
    }

    void SetInDanger()
    {
        Vector2 _currentPosition = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        int _tileIndex = dangerPosition.FindIndex(x => x == _currentPosition);

        if (_tileIndex != -1)
        {
            isInDanger = true;
            return;
        }

        isInDanger = false;
    }

    // Method to set the danger position list using bombs list position
    void RefreshDangerPositions()
    {
        // Reset danger position
        dangerPosition.Clear();

        // We loop trough every known bombs
        for (int i = 0; i < bombsList.Count; i++)
        {
            // We get the current bomb position
            Vector2 _bombPos = bombsList[i].transform.position;
            // We set a temporary bomb length (used if we havnt the ID in the dictionnary)
            int _bombLength = 1;

            // We try to get the bomb ID in the dictionnary to know the last length retains
            bool _containsID = bombLengthByID.ContainsKey(bombsList[i].GetBombSpawnerID());

            if (_containsID)
            {
                _bombLength = bombLengthByID[bombsList[i].GetBombSpawnerID()];
            }

            // If it's our bomb, we set the exact length
            if (bombsList[i].GetBombSpawnerID() == infos.CharID)
            {
                _bombLength = bombSpawner.GetExplosionForce();
            }

            // Now we can set dangerPosition : we need to use a mecanics as SetExplosionLength() in Bomb.cs
            bool[] stopAxis = new bool[4]; // 0 = up, 1 = down, 2 = left, 3 = right

            int _tileIndex = -1;

            for (int j = 1; j <= _bombLength; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    switch (k)
                    {
                        case 0:
                            _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _bombPos.x && x.position.y == _bombPos.y + j);
                            break;
                        case 1:
                            _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _bombPos.x && x.position.y == _bombPos.y - j);
                            break;
                        case 2:
                            _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _bombPos.x - j && x.position.y == _bombPos.y);
                            break;
                        case 3:
                            _tileIndex = StageManager.instance.TilesInfo.FindIndex(x => x.position.x == _bombPos.x + j && x.position.y == _bombPos.y);
                            break;
                    }

                    if (!stopAxis[k])
                    {
                        // If we found the index
                        if (_tileIndex != -1)
                        {
                            // If there is a destructible block or undestructible one, we stop checking this axis
                            if (StageManager.instance.TilesInfo[_tileIndex].hasDestructibleBlock || StageManager.instance.TilesInfo[_tileIndex].hasUndestructibleBlock)
                            {
                                stopAxis[k] = true;
                            }
                            // Else we add it to the danger position list
                            else
                            {
                                dangerPosition.Add(StageManager.instance.TilesInfo[_tileIndex].position);
                            }
                        }
                        else
                        {
                            stopAxis[k] = true;
                        }
                    }
                }
            }   
        }
    }

    // This method is used in BombSpawner.cs when SpawnBomb() method is called
    public void AddBombInList(Bomb _bomb)
    {
        bombsList.Add(_bomb);

        RefreshDangerPositions();
    }

    // This method is used in Bomb.cs when Explode() method is called
    public void RemoveBombFromList(int _id)
    {
        int _index = bombsList.FindIndex(x => x.GetThisBombID() == _id);
        if (_index != -1)
        {
            // We try to get the bomb ID in the dictionnary to set the new length we know
            bool _containsID = bombLengthByID.ContainsKey(bombsList[_index].GetBombSpawnerID());

            if (_containsID)
            {
                bombLengthByID[bombsList[_index].GetBombSpawnerID()] = bombsList[_index].GetBombSpawner().GetExplosionForce();
            }
            else
            {
                bombLengthByID.Add(bombsList[_index].GetBombSpawnerID(), bombsList[_index].GetBombSpawner().GetExplosionForce());
            }

            bombsList.RemoveAt(_index);

            RefreshDangerPositions();
        }
    }
}
