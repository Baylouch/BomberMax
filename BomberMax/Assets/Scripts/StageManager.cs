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

public class TileInfo
{
    public Vector2 position;
    public bool hasUndestructibleBlock;
    public bool hasDestructibleBlock;
    public bool hasBomb;
}

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    public List<TileInfo> TilesInfo = new List<TileInfo>();

    [SerializeField] int endHorizontalTile; // To know how many tile there are horizontaly
    [SerializeField] int endVerticalTile; // Same for vertical tile

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
        GameObject[] UndestructibleBlock = GameObject.FindGameObjectsWithTag("UndestructibleBlock");

        int _index = 0;

        for (int i = 0; i < endVerticalTile; i++)
        {
            for (int j = 0; j < endHorizontalTile; j++)
            {
                TilesInfo.Add( new TileInfo { position = new Vector2(j, i), hasUndestructibleBlock = false, hasDestructibleBlock = false, hasBomb = false });

                for (int k = 0; k < UndestructibleBlock.Length; k++)
                {
                    if (UndestructibleBlock[k].transform.position.x == TilesInfo[_index].position.x &&
                        UndestructibleBlock[k].transform.position.y == TilesInfo[_index].position.y)
                    {
                        TilesInfo[_index].hasUndestructibleBlock = true;                      
                    }
                }


                _index++;
            }
        }

        //CreateDestructibleBlocks();

    }

    void CreateDestructibleBlocks()
    {
        // We must know the start points to not spawn destructible blocks around to let the player some space at the beggin
        for (int i = 0; i < TilesInfo.Count; i++)
        {
            bool createBlock = true;

            if (TilesInfo[i].hasUndestructibleBlock)
            {
                createBlock = false;

            }
            else
            {
                for (int j = 0; j < GameManager.instance.spawnPosTeamOne.Length; j++)
                {
                    if ((TilesInfo[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y) ||
                        (TilesInfo[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x + 1 &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y) ||
                        (TilesInfo[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y + 1 ||
                        (TilesInfo[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x - 1 &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y) ||
                        (TilesInfo[i].position.x == GameManager.instance.spawnPosTeamOne[j].position.x &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamOne[j].position.y - 1))
                        )
                    {
                        createBlock = false;
                    }
                }

                for (int j = 0; j < GameManager.instance.spawnPosTeamTwo.Length; j++)
                {
                    if ((TilesInfo[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y) ||
                        (TilesInfo[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x + 1 &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y) ||
                        (TilesInfo[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y + 1) ||
                        (TilesInfo[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x - 1 &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y) ||
                        (TilesInfo[i].position.x == GameManager.instance.spawnPosTeamTwo[j].position.x &&
                        TilesInfo[i].position.y == GameManager.instance.spawnPosTeamTwo[j].position.y - 1))
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

                GameObject _newBlock = Instantiate(destructibleBlockPrefab, TilesInfo[i].position, Quaternion.identity);
                TilesInfo[i].hasDestructibleBlock = true;
                _newBlock.transform.parent = GameObject.Find("Destructible_Blocks").transform;
                _newBlock.name = "Destructible block " + i;
            }


        }
    }
}
