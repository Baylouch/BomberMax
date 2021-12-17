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

    public List<GameNode> GameGrid = new List<GameNode>();

    // Match with unity grid from bottom left(0,0) to top right(max,max)
    public int maxHorizontal; // To know how many tile there are horizontaly
    public int maxVertical; // Same for vertical tile

    // On the scene screen we display :
    // - Grey dots = Required undestructible pos
    // - Blue dots = undestructible pos
    // - Yellow dots = Required destructible pos
    // - Green dots = destructible pos

    [Header("Undestructible blocks settings")]

    [SerializeField] GameObject undestructibleBlockPrefab;
    [Tooltip("Every position will get a block")]
    [SerializeField] Transform requiredUndestructiblePos;
    [Tooltip("Every position has a chance to spawn a block")]
    [SerializeField] Transform undestructiblePos;

    [Header("Destructible blocks settings")]

    [SerializeField] GameObject destructibleBlockPrefab;
    [SerializeField] Transform requiredDestructiblePos;
    [SerializeField] Transform destructiblePos;
    // Use pos from "requiredDestructiblePos" or "destructiblePos".
    [SerializeField] List<Transform> bonusPos; // To define where a destructible block must spawn a bonus (rest has a chance percentage)
    [SerializeField] float bonusChance = 15f;

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

        CreateUndestructibleBlocks();
        CreateDestructibleBlocks();

        // We set PathFinding nodes here (after created and set stage nodes)
        if (PathFinding.instance)
        {
            PathFinding.instance.CreatePathGrid(GameGrid);
        }
    }

    void CreateGrid()
    {
        // We create the grid
        for (int i = 0; i < maxVertical; i++)
        {
            for (int j = 0; j < maxHorizontal; j++)
            {
                GameGrid.Add(new GameNode(new Vector2(j, i)));
            }
        }
    }

    void CreateUndestructibleBlocks()
    {
        float _undestructibleSpawnPercentage = 50f;
        int _undestructibleBlocksCount = 0;

        GameObject parentGO = new GameObject("Undestructible blocks");
        parentGO.transform.position = Vector3.zero;

        // Create the required blocks
        for (int i = 0; i < requiredUndestructiblePos.childCount; i++)
        {
            int gridIndex = GameGrid.FindIndex(x => x.position.x == requiredUndestructiblePos.GetChild(i).transform.position.x &&
                                                x.position.y == requiredUndestructiblePos.GetChild(i).transform.position.y);

            if (gridIndex == -1)
            {
                continue;
            }
            else
            {
                // Spawn it            
                GameObject undestBlock = Instantiate(undestructibleBlockPrefab, GameGrid[gridIndex].position, Quaternion.identity);
                undestBlock.transform.parent = parentGO.transform;

                GameGrid[gridIndex].hasUndestructibleBlock = true;
            }
        }

        // Spawn the rest
        for (int i = 0; i < undestructiblePos.childCount; i++)
        {
            int gridIndex = GameGrid.FindIndex(x => x.position.x == undestructiblePos.GetChild(i).transform.position.x &&
                                                x.position.y == undestructiblePos.GetChild(i).transform.position.y);

            if (gridIndex == -1)
            {
                continue;
            }
            else
            {
                // Define a chance to spawn the block
                float _random = Random.Range(0f, 100f);

                // Spawn it
                if (_undestructibleSpawnPercentage > _random)
                {
                    GameObject undestBlock = Instantiate(undestructibleBlockPrefab, GameGrid[gridIndex].position, Quaternion.identity);
                    undestBlock.transform.parent = parentGO.transform;

                    GameGrid[gridIndex].hasUndestructibleBlock = true;

                    _undestructibleSpawnPercentage = 35f;
                    _undestructibleBlocksCount++;
                }
                else
                {
                    _undestructibleSpawnPercentage += 3f;
                }
            }
        }

        Destroy(undestructiblePos.gameObject);
        Destroy(requiredUndestructiblePos.gameObject);
    }

    // Create destructible blocks.
    // Because we can define a position where destructible block must get a bonus
    void CreateDestructibleBlocks()
    {
        float _destructibleSpawnPercentage = 50f;
        int _destructibleBlocksCount = 0;

        GameObject parentGO = new GameObject("Destructible blocks");
        parentGO.transform.position = Vector3.zero;

        // Create the required blocks
        for (int i = 0; i < requiredDestructiblePos.childCount; i++)
        {
            int gridIndex = GameGrid.FindIndex(x => x.position.x == requiredDestructiblePos.GetChild(i).transform.position.x &&
                                                x.position.y == requiredDestructiblePos.GetChild(i).transform.position.y);

            if (gridIndex == -1)
            {
                continue;
            }
            else
            {
                // Spawn it            
                GameObject destBlock = Instantiate(destructibleBlockPrefab, GameGrid[gridIndex].position, Quaternion.identity);
                destBlock.transform.parent = parentGO.transform;

                GameGrid[gridIndex].hasDestructibleBlock = true;
            }
        }

        // Spawn the rest
        for (int i = 0; i < destructiblePos.childCount; i++)
        {
            int gridIndex = GameGrid.FindIndex(x => x.position.x == destructiblePos.GetChild(i).transform.position.x &&
                                                x.position.y == destructiblePos.GetChild(i).transform.position.y);

            if (gridIndex == -1)
            {
                continue;
            }
            else
            {
                // Define a chance to spawn the block
                float _random = Random.Range(0f, 100f);

                // Spawn it
                if (_destructibleSpawnPercentage > _random)
                {
                    GameObject undestBlock = Instantiate(destructibleBlockPrefab, GameGrid[gridIndex].position, Quaternion.identity);
                    undestBlock.transform.parent = parentGO.transform;

                    GameGrid[gridIndex].hasDestructibleBlock = true;

                    _destructibleSpawnPercentage = 35f;
                    _destructibleBlocksCount++;
                }
                else
                {
                    _destructibleSpawnPercentage += 3f;
                }
            }
        }

        // Then we define blocks with bonus
        for (int i = 0; i < parentGO.transform.childCount; i++)
        {
            int bonusIndex = bonusPos.FindIndex(x => x.position == parentGO.transform.GetChild(i).position);

            if (bonusIndex != -1)
            {
                // Destructible block will loot a bonus
                parentGO.transform.GetChild(i).GetComponent<DestructibleBlock>().SetupBlockBonus();

            }
            else
            {
                // We make a chance it'll drop a bonus
                float randomy = Random.Range(0f, 100f);

                if (bonusChance > randomy)
                {
                    parentGO.transform.GetChild(i).GetComponent<DestructibleBlock>().SetupBlockBonus();
                }
            }
        }

        Destroy(destructiblePos.gameObject);
        Destroy(requiredDestructiblePos.gameObject);
    }

    // Method to set hasDestructibleBlock to false and set path node to walkable
    public void UpdateDestructibleBlock(Vector2 _pos)
    {
        int index = GameGrid.FindIndex(x => x.position == _pos);

        if (index == -1)
        {
            Debug.LogError("Can't update destructible block");
            return;
        }

        GameGrid[index].hasDestructibleBlock = false;

        if (PathFinding.instance)
        {
            int pathIndex = PathFinding.instance.PathGrid.FindIndex(x => x.position == _pos);

            if (pathIndex == -1)
            {
                Debug.LogError("Can't update path node...");
                return;
            }

            PathFinding.instance.PathGrid[pathIndex].walkable = true;
        }
    }

    public void SetBonusNode(Vector2 _pos, bool _value)
    {
        int index = GameGrid.FindIndex(x => x.position == _pos);

        if (index == -1)
        {
            Debug.LogError("Can't update bonus node");
            return;
        }

        GameGrid[index].hasBonus = _value;
    }


    // Method to reset Danger tile then set it again (after an explosion ends). 
    public void ResetDangerTiles()
    {
        for (int i = 0; i < GameGrid.Count; i++)
        {
            GameGrid[i].isDanger = false;
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
            index = GameGrid.FindIndex(x => x.position == new Vector2(bombs[i].transform.position.x, bombs[i].transform.position.y));

            if (index == -1) // There is an issue, we go to the next bomb
                break;

            // else we can set the tile on the bomb as danger one
            GameGrid[index].isDanger = true;

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

                    index = GameGrid.FindIndex(x => x.position == posToCheck);

                    // We test if we go to the next axis to check
                    if (index == -1 || GameGrid[index].isDanger || GameGrid[index].hasUndestructibleBlock || GameGrid[index].hasDestructibleBlock)
                    {
                        break;
                    }
                    else
                    {
                        GameGrid[index].isDanger = true;
                    }
                }
            }
        }
    }
}
