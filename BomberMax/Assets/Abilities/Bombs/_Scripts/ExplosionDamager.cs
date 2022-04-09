/* ExplosionDamager.cs
 * 
 * This script is attach on each explosion's parts to know how much damage will be done to the CharacterHealth.
 * 
 * Damage property is set by BombExplosionSettings.
 * 
 * */

using UnityEngine;

public class ExplosionDamager : MonoBehaviour
{
    public float Damage { get; set; }

}
