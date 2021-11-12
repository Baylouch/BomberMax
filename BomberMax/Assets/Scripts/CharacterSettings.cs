using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character", order = 1)]
public class CharacterSettings : ScriptableObject
{
    public Sprite characterSprite;

    public AnimatorOverrideController animatorController;
    
}
