/* PathFinding.cs
 * 
 * Used to process pathfinding with bots to create a path from A to B.
 * 
 * 
 * 
 * */

using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public static PathFinding instance;

    public List<PathNode> PathGrid = new List<PathNode>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void ResetNodesForNewPath()
    {
        for (int i = 0; i < PathGrid.Count; i++)
        {
            PathGrid[i].gCost = 0;
            PathGrid[i].hCost = 0;
            PathGrid[i].parent = null;
        }
    }

    // Return false if the list does already exist...
    public bool CreatePathGrid(List<GameNode> gameNodes)
    {
        int index = 0;
        bool returnValue = true;

        // Security if we already has a PathGrid set. It must never happen
        if (PathGrid.Count > 0)
        {
            PathGrid = new List<PathNode>();
            returnValue = false;
        }

        // We create and set nodes.
        for (int i = 0; i < gameNodes.Count; i++)
        {
            // We don't get nodes with undestructible block on it, because for now
            // they're never destroy, no sense to take into pathfind, less operation while creating path.
            if (gameNodes[i].hasUndestructibleBlock)
                continue;

            // By default walkable is set on true
            PathGrid.Add(new PathNode(gameNodes[i].position));

            // If the node has a destructible block we take it and set it as not walkable (updated when block is destroy)
            if (gameNodes[i].hasDestructibleBlock)
            {
                PathGrid[index].walkable = false;
            }

            index++;
        }

        // We set nodes neighboors
        foreach (PathNode node in PathGrid)
        {
            index = -1;
            // we find for a left node to add
            index = PathGrid.FindIndex(x => x.position == new Vector2(node.position.x - 1, node.position.y));
            if (index != -1)
                node.neighboors.Add(PathGrid[index]);

            // for a right
            index = PathGrid.FindIndex(x => x.position == new Vector2(node.position.x + 1, node.position.y));
            if (index != -1)
                node.neighboors.Add(PathGrid[index]);

            // for a up
            index = PathGrid.FindIndex(x => x.position == new Vector2(node.position.x, node.position.y + 1));
            if (index != -1)
                node.neighboors.Add(PathGrid[index]);

            // for a down
            index = PathGrid.FindIndex(x => x.position == new Vector2(node.position.x, node.position.y - 1));
            if (index != -1)
                node.neighboors.Add(PathGrid[index]);
        }

        return returnValue;
    }

    // Method to create a path from posA to posB using A* pathfinding.
    // return true if the path is complete
    // bool getPath default to true is because sometimes (in Bot_Brain -> CheckAround() method)  just needs to know if the path is reachable
    // we don't need to receive the path.
    public bool CreatePath(Vector2 posA, Vector2 posB, ref List<Vector2> path, bool getPath = true)
    {
        // First we check if we can get posA and posB in the grid, else no sense to continue
        int indexPosA = PathGrid.FindIndex(x => x.position == posA);
        int indexPosB = PathGrid.FindIndex(x => x.position == posB);

        if (indexPosA == -1 || indexPosB == -1)
        {
            return false;
        }

        if (!PathGrid[indexPosA].walkable || !PathGrid[indexPosB].walkable)
        {
            return false;
        }

        // We restart all of our previous node values
        ResetNodesForNewPath();

        // First we create open and close list
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();
        List<PathNode> finalPath = new List<PathNode>();

        PathNode startNode = PathGrid[indexPosA];
        PathNode endNode = PathGrid[indexPosB];

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

            PathNode currentNode = openList[nodeIndex];

            // If currentNode is the endNode we found a path !
            if (currentNode == endNode)
            {
                if (!getPath)
                {
                    return true;
                }

                finalPath.Add(currentNode);

                while (finalPath[finalPath.Count - 1] != startNode)
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

    // Pathfinding gizmos.
    //public void OnDrawGizmos()
    //{
    //    foreach (PathNode node in PathGrid)
    //    {
    //        Gizmos.color = (node.walkable) ? Color.white : Color.red;
    //        Gizmos.DrawCube(node.position, Vector2.one / 2);
    //    }
    //}
}
