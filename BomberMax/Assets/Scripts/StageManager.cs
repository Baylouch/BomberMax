/* StageManager.cs
 * 
 * This script is used to manage the stage.
 * 
 * Contain a list of tileStruct to know every tiles info such as there is a block on it, a bomb...
 * 
 * TODO see more use of it
 * 
 * We know each stage tile start at (0.0) and is incremented by 1 each time. So we can have a variable to count each tile
 * 
 * */

using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    public List<Node> Grid = new List<Node>();

    public int maxHorizontal; // To know how many tile there are horizontaly
    public int maxVertical; // Same for vertical tile

    [SerializeField] GameObject destructibleBlockPrefab;
    [SerializeField] int numberOfDestructibleBlocks = 15; // When set to -1 it creates blocks on every available tiles



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();

        //CreateDestructibleBlocks();

        // See if we already set destructible blocks
        //DestructibleBlock[] destructibles = FindObjectsOfType<DestructibleBlock>();
        //if (destructibles.Length > 0)
        //{
        //    for (int i = 0; i < destructibles.Length; i++)
        //    {
        //        int _indexDest = Grid.FindIndex(x => x.position.x == destructibles[i].transform.position.x && x.position.y == destructibles[i].transform.position.y);
        //        if (_indexDest != -1)
        //        {
        //            Grid[_indexDest].hasDestructibleBlock = true;
        //            Grid[_indexDest].walkable = false;
        //        }
        //    }
        //}
    }

    void CreateGrid()
    {
        GameObject[] UndestructibleBlock = GameObject.FindGameObjectsWithTag("UndestructibleBlock");

        int _index = 0;

        for (int i = 0; i < maxVertical; i++)
        {
            for (int j = 0; j < maxHorizontal; j++)
            {
                Grid.Add(new Node(new Vector2(j, i)));

                for (int k = 0; k < UndestructibleBlock.Length; k++)
                {
                    if (UndestructibleBlock[k].transform.position.x == Grid[_index].position.x &&
                        UndestructibleBlock[k].transform.position.y == Grid[_index].position.y)
                    {
                        Grid[_index].hasUndestructibleBlock = true;
                        Grid[_index].walkable = false;
                    }
                }

                _index++;
            }
        }

        foreach (Node node in Grid)
        {
            int gridIndex = -1;
            // we find for a left node to add
            gridIndex = Grid.FindIndex(x => x.position == new Vector2(node.position.x - 1, node.position.y));
            if (gridIndex != -1)
                node.neighboors.Add(Grid[gridIndex]);

            // for a right
            gridIndex = Grid.FindIndex(x => x.position == new Vector2(node.position.x + 1, node.position.y));
            if (gridIndex != -1)
                node.neighboors.Add(Grid[gridIndex]);

            // for a up
            gridIndex = Grid.FindIndex(x => x.position == new Vector2(node.position.x, node.position.y + 1));
            if (gridIndex != -1)
                node.neighboors.Add(Grid[gridIndex]);

            // for a down
            gridIndex = Grid.FindIndex(x => x.position == new Vector2(node.position.x, node.position.y - 1));
            if (gridIndex != -1)
                node.neighboors.Add(Grid[gridIndex]);

        }
    }

    void ResetNodesForNewPath()
    {
        for (int i = 0; i < Grid.Count; i++)
        {
            Grid[i].gCost = 0;
            Grid[i].hCost = 0;
            Grid[i].parent = null;
        }
    }

    // Method to create a path from posA to posB using A* pathfinding.
    public bool CreatePath(Vector2 posA, Vector2 posB, ref List<Vector2> path)
    {
        // First we check if we can get posA and posB in the grid, else no sense to continue
        int indexPosA = Grid.FindIndex(x => x.position == posA);
        int indexPosB = Grid.FindIndex(x => x.position == posB);

        if (indexPosA == -1 || indexPosB == -1)
        {
            return false;
        }

        if (!Grid[indexPosA].walkable || !Grid[indexPosB].walkable)
        {
            return false;
        }

        // We restart all of our previous node values
        ResetNodesForNewPath();

        // First we create open and close list
        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        List<Node> finalPath = new List<Node>();

        Node startNode = Grid[indexPosA];
        Node endNode = Grid[indexPosB];

        // We add the start node to the openList
        openList.Add(startNode);

        int SecurityCount = 0;

        // We loop until we got node in the openList or we found a path
        while (openList.Count > 0 && SecurityCount < 10000)
        {
            // Security if we can't found path under 10k try, let's out
            SecurityCount++;

            // We need to get the current node to test its neighboors
            // To do this, we loop trough the openList and get the node with the lowest fcost
            int nodeIndex = 0; // We start to the index 0

            for (int i = 0; i < openList.Count; i++)
            {                
                if (openList[nodeIndex].fCost() > openList[i].fCost())
                    nodeIndex = i;
            }

            Node currentNode = openList[nodeIndex];

            // If currentNode is the endNode we found a path !
            if (currentNode == endNode)
            {
                finalPath.Add(currentNode);

                while (finalPath[finalPath.Count -1] != startNode)
                {
                    finalPath.Add(finalPath[finalPath.Count - 1].parent);
                }

                finalPath.Reverse();

                path = new List<Vector2>();

                // We start with 1 here to not put the start position in because when we create a path, we assume we already on start position...
                for (int i = 1; i < finalPath.Count; i++)
                {
                    path.Add(finalPath[i].position);
                }

                //Debug.Log(SecurityCount); // Delete it

                return true;
            }

            // We directly put our currentNode in the closedList
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            // Now we check current node neighboor to determine the one we go next (with the lower fcost)
            // We need to set their gCost and hCost
            for (int i = 0; i < currentNode.neighboors.Count; i++)
            {
                int closeListIndex = closeList.FindIndex(x => x == currentNode.neighboors[i]);

                // If the node is not walkable or in the closeList we skip it
                if (!currentNode.neighboors[i].walkable || closeListIndex != -1)
                    continue;

                currentNode.neighboors[i].gCost = (Mathf.Abs((int)startNode.position.x - (int)currentNode.neighboors[i].position.x) +
                                                  Mathf.Abs((int)startNode.position.y - (int)currentNode.neighboors[i].position.y)) * 10;

                currentNode.neighboors[i].hCost = (Mathf.Abs((int)endNode.position.x - (int)currentNode.neighboors[i].position.x) +
                                                  Mathf.Abs((int)endNode.position.y - (int)currentNode.neighboors[i].position.y)) * 10;

                // Set parent node
                currentNode.neighboors[i].parent = currentNode;

                // And we add it to the openList
                openList.Add(currentNode.neighboors[i]);
            }
        }

        return false;
    }

    void CreateDestructibleBlocks()
    {
        // We must know the start points to not spawn destructible blocks around to let the player some space at the beggin
        for (int i = 0; i < Grid.Count; i++)
        {
            bool createBlock = true;

            if (Grid[i].hasUndestructibleBlock)
            {
                createBlock = false;

            }
            else
            {
                for (int j = 0; j < GameManager.instance.spawnPosTeamOne.Length; j++)
                {
                    if ((Grid[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y) ||
                        (Grid[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x + 1 &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y) ||
                        (Grid[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y + 1 ||
                        (Grid[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x - 1 &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y) ||
                        (Grid[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y - 1))
                        )
                    {
                        createBlock = false;
                    }
                }

                for (int j = 0; j < GameManager.instance.spawnPosTeamTwo.Length; j++)
                {
                    if ((Grid[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y) ||
                        (Grid[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x + 1 &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y) ||
                        (Grid[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y + 1) ||
                        (Grid[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x - 1 &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y) ||
                        (Grid[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x &&
                        Grid[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y - 1))
                    {
                        createBlock = false;

                    }
                }
            }
            
            if (createBlock)
            {
                // We'll use a variable to represent the percentage of chance of spawn destructible block
                // after a first loop of blocks creation, if we havnt the total of destructible block, we increase the chance variable etc until we
                // reach the number required. 40% -> 60% -> 80% -> 100%

                GameObject _newBlock = Instantiate(destructibleBlockPrefab, Grid[i].position, Quaternion.identity);
                Grid[i].hasDestructibleBlock = true;
                Grid[i].walkable = false;
                _newBlock.transform.parent = GameObject.Find("Destructible_Blocks").transform;
                _newBlock.name = "Destructible block " + i;
            }
        }
    }

    // Method to reset Danger tile then set it again (after an explosion ends). 
    public void ResetDangerTiles()
    {
        for (int i = 0; i < Grid.Count; i++)
        {
            Grid[i].isDanger = false;
        }

        SetDangerTiles();
    }

    // This method will set danger tiles
    // Get every bombs in the game
    // And relative to their length (and if there are obstacles) defines the isDanger variable
    // Used when a bomb is dropped
    public void SetDangerTiles()
    {
        Bomb[] bombs = FindObjectsOfType<Bomb>();
        int index = 0;
        Vector2 posToCheck = Vector2.zero;

        // Loop trough all bombs
        for (int i = 0; i < bombs.Length; i++)
        {
            index = Grid.FindIndex(x => x.position == new Vector2(bombs[i].transform.position.x, bombs[i].transform.position.y));

            if (index == -1) // There is an issue, we go to the next bomb
                break;

            // else we can set the tile on the bomb as danger one
            Grid[index].isDanger = true;

            // Loop trough all axis
            for (int j = 0; j < 4; j++)
            {
                // Finally loop trough explosion length of the bomb
                for (int k = 1; k <= bombs[i].GetBombSpawner().GetExplosionForce(); k++)
                {
                    switch (j)
                    {
                        case 0: // Up
                            posToCheck = new Vector2(bombs[i].transform.position.x, bombs[i].transform.position.y + k);
                            break;
                        case 1: // Down
                            posToCheck = new Vector2(bombs[i].transform.position.x, bombs[i].transform.position.y - k);
                            break;
                        case 2: // Left
                            posToCheck = new Vector2(bombs[i].transform.position.x - k, bombs[i].transform.position.y);
                            break;
                        case 3: // Right
                            posToCheck = new Vector2(bombs[i].transform.position.x + k, bombs[i].transform.position.y);
                            break;
                    }

                    index = Grid.FindIndex(x => x.position == posToCheck);

                    // We test if we go to the next axis to check
                    if (index == -1 || Grid[index].isDanger || Grid[index].hasUndestructibleBlock || Grid[index].hasDestructibleBlock)
                    {
                        break;
                    }
                    else
                    {
                        Grid[index].isDanger = true;
                    }
                }
            }
        }
    }




    //public void OnDrawGizmos()
    //{
    //    foreach (Node node in Grid)
    //    {
    //        Gizmos.color = (node.walkable) ? Color.white : Color.red;

    //        Gizmos.DrawCube(node.position, Vector2.one / 2);

    //    }
    //}
}
