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

    List<TeamMember> ennemies; // To have a reference to ennemies in game

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

        ennemies = new List<TeamMember>();

        int botTeam = GetComponent<TeamMember>().TeamNumber;

        foreach (TeamMember teamMember in FindObjectsOfType<TeamMember>())
        {
            if (botTeam != teamMember.TeamNumber)
            {
                ennemies.Add(teamMember);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // When a new bomb is drop, check if the path go on it to change it.

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

                    CheckAround();
                }
            }
        }
    }

    // This method check nodes around the bot (relative to its vision), and mark if it spot an enemy, a bonus, and get track of the best
    // position to go to destroy multiple blocks. If there is no enemy, bonus or blocks, it moves to a random position.
    void CheckAround()
    {
        // Handle multiple bonus, multiple ennemies to do check in the end when decide where to go
        // Check every tiles around
        // Is there an enemy ? a bonus ?
        // Search a path to access it
        // For the enemy, we should check where we can kill him relative to bomb explosion

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);

        GameBonus[] bonus = FindObjectsOfType<GameBonus>();

        List<Vector2> bonusInRange = new List<Vector2>();
        List<Vector2> ennemiesAttackPos = new List<Vector2>();
        Vector2 destroyBlocksPos = -Vector2.one;
        int lastBlocksCount = 0;

        for (int i = AIVision; i >= -AIVision; i--)
        {
            for (int j = AIVision; j >= -AIVision; j--)
            {
                Vector2 posToCheck = new Vector2(currentPos.x + j, currentPos.y + i);

                int currentBlocksCount = 0;

                if (posToCheck == currentPos)
                    continue;

                int gridIndex = PathFinding.instance.PathGrid.FindIndex(x => x.position == posToCheck);

                if (gridIndex != -1)
                {
                    // Check if the place is reachable
                    List<Vector2> temp = null;

                    bool reachable = PathFinding.instance.CreatePath(currentPos, posToCheck, ref temp, false);

                    if (reachable)
                    {
                        // We first check if there is a bonus or a player
                        int enemyIndex = ennemies.FindIndex(x => x.gameObject.transform.position.x == posToCheck.x && x.gameObject.transform.position.y == posToCheck.y );
                        
                        if (enemyIndex != -1)
                        {
                            // We go to a place where we can drop a bomb to get the enemy
                            // But here we just mark the place for the next step of this method
                            ennemiesAttackPos.Add(new Vector2(ennemies[enemyIndex].transform.position.x, ennemies[enemyIndex].transform.position.y));
                            continue;
                        }

                        // Then we check if there is a bonus
                        for (int b = 0; b < bonus.Length; b++)
                        {
                            if (bonus[b].transform.position.x == posToCheck.x && bonus[b].transform.position.y == posToCheck.y)
                            {
                                // We mark the place
                                bonusInRange.Add(new Vector2(bonus[b].transform.position.x, bonus[b].transform.position.y));
                                continue;
                            }
                        }

                        // TODO : Get inspiration with GetDestructibleBlocksAround() method to reduce size of this one.

                        // Then we check if we drop a bomb here what can we get
                        // Check relative to explosion length if we can access an enemy
                        // Count number of block we could destroy
                        for (int p = 0; p < 4; p++)
                        {
                            for (int k = 1; k <= bombSpawner.GetExplosionForce(); k++)
                            {
                                Vector2 explosionPos = -Vector2.one;

                                switch (p)
                                {
                                    case 0:
                                        explosionPos = new Vector2(posToCheck.x + k, posToCheck.y);
                                        break;
                                    case 1:
                                        explosionPos = new Vector2(posToCheck.x - k, posToCheck.y);

                                        break;
                                    case 2:
                                        explosionPos = new Vector2(posToCheck.x, posToCheck.y + k);

                                        break;
                                    case 3:
                                        explosionPos = new Vector2(posToCheck.x, posToCheck.y - k);

                                        break;
                                }

                                int index = StageManager.instance.GameGrid.FindIndex(x => x.position == explosionPos);

                                if (index != -1)
                                {
                                    if (StageManager.instance.GameGrid[index].hasUndestructibleBlock)
                                    {
                                        break;
                                    }

                                    if (StageManager.instance.GameGrid[index].hasDestructibleBlock)
                                    {
                                        currentBlocksCount++;

                                        if (currentBlocksCount > lastBlocksCount)
                                        {
                                            lastBlocksCount = currentBlocksCount;
                                            destroyBlocksPos = posToCheck;
                                        }

                                        break;
                                    }

                                    enemyIndex = ennemies.FindIndex(x => x.gameObject.transform.position.x == explosionPos.x && x.gameObject.transform.position.y == explosionPos.y);

                                    if (enemyIndex != -1)
                                    {
                                        // We go to a place where we can drop a bomb to get the enemy
                                        // But here we just mark the place for the next step of this method
                                        ennemiesAttackPos.Add(posToCheck);
                                        break; // Instead of break we could continue and check where is the maximum ennemies we could get with this explosion
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        // For ennemies in range
        for (int i = 0; i < ennemiesAttackPos.Count; i++)
        {
            // Get a safe place

            bool isSafe = GetClosestSafePlace(ennemiesAttackPos[i], bombSpawner.GetExplosionForce());

            if (isSafe)
            {
                List<Vector2> temp = null;

                bool security = PathFinding.instance.CreatePath(currentPos, ennemiesAttackPos[i], ref temp);

                // Set bomb when reached end path

                // Go to the safe place and wait
                if (security)
                {
                    botMovement.SetPath(temp);
                    dropBomb = true;
                    return; // We end the method because everything is set for this enemy
                }
            }
        }

        // For bonus in range
        for (int i = 0; i < bonusInRange.Count; i++)
        {
            List<Vector2> temp = null;

            bool security = PathFinding.instance.CreatePath(currentPos, bonusInRange[i], ref temp);

            if (security)
            {
                botMovement.SetPath(temp);
                return; // End method here
            }
        }

        // For destructible blocks
        if (destroyBlocksPos != -Vector2.one)
        {
            bool isSafe = GetClosestSafePlace(destroyBlocksPos, bombSpawner.GetExplosionForce());

            if (isSafe)
            {
                List<Vector2> temp = null;

                bool security = PathFinding.instance.CreatePath(currentPos, destroyBlocksPos, ref temp);

                // Set bomb when reached end path

                // Go to the safe place and wait
                if (security)
                {
                    botMovement.SetPath(temp);
                    dropBomb = true;
                    return; // We end the method because everything is set for this enemy
                }
            }
        }

        // If we still there, we use a random path
        CreateRandomPath();
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

                tileIndex = StageManager.instance.GameGrid.FindIndex(x => x.position == posToCheck);

                if (tileIndex != -1)
                {
                    if (StageManager.instance.GameGrid[tileIndex].hasDestructibleBlock)
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

    void CreateRandomPath()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);


        bool validPosition = false;
        int count = 0;

        while (!validPosition && count < 20)
        {
            count++;

            int onX = Random.Range(-3, 4);
            int onY = Random.Range(-3, 4);

            Vector2 newPos = new Vector2(transform.position.x + onX, transform.position.y + onY);

            if (newPos == currentPos)
                continue;

            int index = PathFinding.instance.PathGrid.FindIndex(x => x.position == newPos);

            if (index == -1)
                continue;

            if (PathFinding.instance.PathGrid[index].walkable)
            {
                List<Vector2> temp = null;
                bool canReach = PathFinding.instance.CreatePath(currentPos, newPos, ref temp);

                if (canReach)
                {
                    botMovement.SetPath(temp);
                    validPosition = true;
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

                int index = StageManager.instance.GameGrid.FindIndex(x => x.position == currentPos);

                if (index != -1)
                {
                    if (StageManager.instance.GameGrid[index].hasUndestructibleBlock || StageManager.instance.GameGrid[index].hasDestructibleBlock)
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

                    int index = PathFinding.instance.PathGrid.FindIndex(x => x.position == currentPos);

                    if (index == -1)
                        continue;

                    if (k == 0 && (!PathFinding.instance.PathGrid[index].walkable))
                    {
                        stopAxis[j] = true;
                        break;
                    }

                    // If index exists, not contained in explosion list, not a danger position and is walkable
                    if (!explosionPositions.Contains(currentPos) &&
                             PathFinding.instance.PathGrid[index].walkable &&
                             !stopAxis[j])
                    {
                        // Can we move to this position
                        List<Vector2> tempList = null;

                        bool reachable = PathFinding.instance.CreatePath(bombPos, currentPos, ref tempList);

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
