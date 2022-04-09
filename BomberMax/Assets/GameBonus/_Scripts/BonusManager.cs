/* This script is attach on the same gameObject of StageManager.
 * 
 * After StageManager has created blocks, this script will set ones who have bonus on, define the bonus and attach the corresponding script on it.
 * 
 * Then when the block is destroy it'll search if it has a bonus script on it and spawn relative bonus.
 * 
 * 
 * 
 * */

using UnityEngine;

public class BonusManager : MonoBehaviour
{
    [SerializeField] GameBonusData[] datas; // Explosion and Additional bomb bonus must be set on index 0 and 1 !

    // TODO : Define the bonus we want to spawn ? Or leave it random
    [SerializeField] Transform[] bonusPos; // Destructible blocks who must spawn a bonus
    [SerializeField] float bonusChance = 15f; // Chance a destructible block spawn bonus

    // Method call in StageManager Start() to defines block with bonus and apply bonus on them.
    // receive every destructible blocks via blocks parameter, compare positions with bonusPos array and make a chance to other blocks.
    public void SetBonusBlocks(GameObject[] blocks)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            bool blockGetBonus = false;
            bool bonusSet = false;

            for (int j = 0; j < bonusPos.Length; j++)
            {
                if (blocks[i].transform.position == bonusPos[j].transform.position)
                {
                    blockGetBonus = true;

                    break;
                }
            }

            if (!blockGetBonus)
            {
                float randomValue = Random.Range(0f, 100f);

                if (bonusChance >= randomValue)
                {
                    blockGetBonus = true;
                }
            }

            if (blockGetBonus)
            {
                BonusSpawner _bonusSpawner = blocks[i].AddComponent<BonusSpawner>();

                for (int k = datas.Length - 1; k >= 0; k--)
                {
                    float randomValue = Random.Range(0f, 100f);

                    if (datas[k].chanceToSpawn >= randomValue)
                    {
                        // Set BonusSpawner
                        _bonusSpawner.data = datas[k];

                        bonusSet = true;

                        break;
                    }
                }

                // We set a 50% chance to get bomb explosion bonus or additionnal bomb bonus on it if for loop above didn't set any bonus
                if (!bonusSet)
                {
                    if (Random.Range(0f, 100f) < 50f)
                    {
                        _bonusSpawner.data = datas[0];
                    }
                    else
                    {
                        _bonusSpawner.data = datas[1];
                    }
                }
            }
        }

        // This script has no more use, we can destroy it
        Destroy(this);
    }
}
