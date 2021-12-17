/* ExplosionStyle.cs
 * 
 * This script is used to represent a style of an explosion. Passed to the bomb trough BombExplosionSettings
 * 
 * Normal explosion : 4 directions and stop when meet a block
 * we could then create an explosion who avoid each block to continue on until explosion force achieved,
 * create another who pass trough the blocks...
 * 
 * 
 * */


using System.Collections.Generic;
using UnityEngine;

public class ExplosionStyle : MonoBehaviour
{
    // TODO : Create ExplosionSetup.cs, it'll be spawn by the bomb and deal with everyting relative to its explosion.
    //

    // We want to have a sort of schema of what we wants the explosion to do
    public bool passTroughBlocks = false;
    public bool avoidBlocks = false;

    // We want to access each node position representing the explosion to be able to set the explosion as expected.
    List<Vector2> explosionPos = new List<Vector2>();




    // Method to fill explosionPos list. (Add bomb position as parameter ?)
    public void GetExplosionPos()
    {

    }

    // Method to update position if needed (after a block was destroy...)
    public void UpdateExplosionPos()
    {

    }
}
