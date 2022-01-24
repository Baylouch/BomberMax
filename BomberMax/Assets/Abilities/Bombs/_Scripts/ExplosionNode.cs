/* ExplosionNode.cs
 * 
 * To have access of each node's informations. Set in ExplosionSetup.cs, created with ExplosionActivator.cs
 * 
 * */


using UnityEngine;

public class ExplosionNode
{
    // Need to know its gfx
    public Sprite gfx;

    // Its position
    public Vector2 position;

    // Its rotation on z
    public float zRotation;

    public ExplosionNode(Sprite _gfx, Vector2 _position, float _zRotation)
    {
        gfx = _gfx;
        position = _position;
        zRotation = _zRotation;
    }

}
