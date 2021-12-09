using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BombSpawner))]
[RequireComponent(typeof(Bot_Movement))]
public class Bot_Brain : MonoBehaviour
{
    BotDifficulty difficulty;

    int AIVision; // Represent tiles that the bot can "see"

    BombSpawner bombSpawner;
    Bot_Movement botMovement;

    List<Vector2> backupPath; // To retain a safe path when we got a position where we drop a bomb

    bool dropBomb = false;
    bool botSet = false;

    float waitUntilExplosion = 0f;
    float waitUntilSearchNewPath = 0f;

    // Start is called before the first frame update
    void Start()
    {
        SetDifficulty(BotDifficulty.Easy); // For now we just set the bot here. TODO Create BotManager in stages

        bombSpawner = GetComponent<BombSpawner>();
        botMovement = GetComponent<Bot_Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (botMovement.ReachedEndOfPath())
        {
            if (dropBomb)
            {
                dropBomb = false;
                bombSpawner.DropBomb();
                botMovement.SetPath(backupPath);
                waitUntilExplosion = Time.time + 7f;
            }

            if (waitUntilExplosion < Time.time)
            {
                if (waitUntilSearchNewPath < Time.time)
                {
                    waitUntilSearchNewPath = Time.time + 2f;

                    DeterminePlaceToGo();
                }
            }
        }
    }

    // Method who returns number of destructible blocks we can explode from a position
    int GetDestructibleBlocksAround(Vector2 _position, int _explosionLength)
    {
        Vector2 posToCheck = _position;

        int tileIndex = -1;

        int accessibleBlocks = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j <= _explosionLength; j++)
            {
                switch (i)
                {
                    case 0: // Up case
                        posToCheck = new Vector2(_position.x, _position.y + j);
                        break;
                    case 1: // Down case
                        posToCheck = new Vector2(_position.x, _position.y - j);
                        break;
                    case 2: // Left case
                        posToCheck = new Vector2(_position.x - j, _position.y);
                        break;
                    case 3: // Right case
                        posToCheck = new Vector2(_position.x + j, _position.y);
                        break;
                }

                tileIndex = StageManager.instance.Grid.FindIndex(x => x.position == posToCheck);

                if (tileIndex != -1)
                {
                    if (StageManager.instance.Grid[tileIndex].hasDestructibleBlock)
                    {
                        accessibleBlocks++;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return accessibleBlocks;
    }

    // Method to check nodes around and determine the best one to go
    void DeterminePlaceToGo()
    {
        int maximumBlocks = 0;

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);

        List<Vector2> retainPath = null;

        // Loop trough every case around the bot relative to its vision.
        // We start from upper right and end to down left.
        // Special condition if its stage borders, we dont continue on the row / line.
        for (int i = AIVision; i >= -AIVision; i--)
        {
            for (int j = AIVision; j >= -AIVision; j--)
            {
                Vector2 posToCheck = new Vector2(transform.position.x + j, transform.position.y + i);

                int index = StageManager.instance.Grid.FindIndex(x => x.position == posToCheck);

                if (index == -1)
                    continue;

                if (StageManager.instance.Grid[index].walkable)
                {
                    int tempBlocks = GetDestructibleBlocksAround(posToCheck, bombSpawner.GetExplosionForce());

                    // TODO Get the minimum movement we can get for the same amount of blocks
                    if (maximumBlocks < tempBlocks)
                    {
                        // Can we move to the posToCheck ?
                        List<Vector2> tempList = null;

                        bool movablePos = StageManager.instance.CreatePath(currentPos, posToCheck, ref tempList);

                        // Can we go to a safe pos from the position we check?
                        bool safeBackup = GetClosestSafePlace(posToCheck, bombSpawner.GetExplosionForce());

                        if (movablePos && safeBackup)
                        {
                            maximumBlocks = tempBlocks;
                            retainPath = new List<Vector2>();

                            for (int k = 0; k < tempList.Count; k++)
                            {
                                retainPath.Add(tempList[k]);
                            }
                        }
                    }
                }
            }
        }

        if (maximumBlocks > 0 && retainPath.Count > 0)
        {
            botMovement.SetPath(retainPath);
            dropBomb = true;
        }
        else
        {
            // Move randomly to a position
            bool validPosition = false;
            int count = 0;

            while (!validPosition && count < 20)
            {
                count++;

                int onX = Random.Range(-3, 4);
                int onY = Random.Range(-3, 4);

                Vector2 newPos = new Vector2(transform.position.x + onX, transform.position.y + onY);

                int index = StageManager.instance.Grid.FindIndex(x => x.position == newPos);

                if (index == -1)
                    continue;

                if (StageManager.instance.Grid[index].walkable)
                {
                    List<Vector2> temp = null;
                    bool canReach = StageManager.instance.CreatePath(currentPos, newPos, ref temp);

                    if (canReach)
                    {
                        botMovement.SetPath(temp);
                        validPosition = true;
                    }
                }
            }
        }
    }

    List<Vector2> CreateFictiveDangerZone(Vector2 bombPos, int explosionLength)
    {
        List<Vector2> returnList = new List<Vector2>();

        returnList.Add(bombPos);

        // We set the explosion lists
        for (int j = 0; j < 4; j++)
        {
            Vector2 currentPos = bombPos;

            for (int i = 1; i <= explosionLength; i++)
            {
                switch (j)
                {
                    case 0:
                        currentPos = new Vector2(bombPos.x, bombPos.y + i);
                        break;
                    case 1:
                        currentPos = new Vector2(bombPos.x, bombPos.y - i);

                        break;
                    case 2:
                        currentPos = new Vector2(bombPos.x + i, bombPos.y);

                        break;
                    case 3:
                        currentPos = new Vector2(bombPos.x - i, bombPos.y);

                        break;
                }

                int index = StageManager.instance.Grid.FindIndex(x => x.position == currentPos);

                if (index != -1)
                {
                    if (StageManager.instance.Grid[index].hasUndestructibleBlock || StageManager.instance.Grid[index].hasDestructibleBlock)
                        break;

                    returnList.Add(currentPos);
                }
            }
        }

        return returnList;
    }

    // We search for every tiles around bombPos until we get a safePlace
    bool GetClosestSafePlace(Vector2 bombPos, int explosionLength)
    {
        // We need to know the bomb explosion's position
        List<Vector2> explosionPositions = CreateFictiveDangerZone(bombPos, explosionLength);

        // For now we loop trough every position in a range of 10 around the bot. Maybe update this later
        bool[] stopAxis = new bool[4] { false, false, false, false };

        for (int i = 1; i <= 10; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector2 currentPos = bombPos;

                // K loop is to test perpendicular position too.
                for (int k = -1; k <= 1; k++)
                {
                    switch (j)
                    {
                        case 0:
                            if (bombPos.y + i > StageManager.instance.maxVertical)
                                continue;

                            currentPos = new Vector2(bombPos.x + k, bombPos.y + i);
                            break;
                        case 1:
                            if (bombPos.y - i < 0)
                                continue;

                            currentPos = new Vector2(bombPos.x + k, bombPos.y - i);

                            break;
                        case 2:
                            if (bombPos.x + i > StageManager.instance.maxHorizontal)
                                continue;

                            currentPos = new Vector2(bombPos.x + i, bombPos.y + k);

                            break;
                        case 3:
                            if (bombPos.x - i < 0)
                                continue;

                            currentPos = new Vector2(bombPos.x - i, bombPos.y + k);

                            break;
                    }

                    int index = StageManager.instance.Grid.FindIndex(x => x.position == currentPos);

                    if (index == -1)
                        continue;

                    if (k == 0 && (StageManager.instance.Grid[index].hasDestructibleBlock || StageManager.instance.Grid[index].hasUndestructibleBlock))
                    {
                        stopAxis[j] = true;
                        break;
                    }

                    // If index exists, not contained in explosion list, not a danger position and is walkable
                    if (!explosionPositions.Contains(currentPos) &&
                             !StageManager.instance.Grid[index].isDanger &&
                             StageManager.instance.Grid[index].walkable &&
                             !stopAxis[j])
                    {
                        // Can we move to this position
                        List<Vector2> tempList = null;

                        bool reachable = StageManager.instance.CreatePath(bombPos, currentPos, ref tempList);

                        if (reachable)
                        {
                            backupPath = tempList;
                            return true;
                        }
                    }
                }
            }        
        }

        return false;
    }

    public void SetDifficulty(BotDifficulty difficulty)
    {
        this.difficulty = difficulty;

        switch (difficulty)
        {
            case BotDifficulty.Easy:
                AIVision = 2;

                break;
            case BotDifficulty.Medium:
                AIVision = 4;

                break;
            case BotDifficulty.Hard:
                AIVision = 6;

                break;
            case BotDifficulty.Pro:
                AIVision = 8;

                break;
        }

        botSet = true;
    }

}
