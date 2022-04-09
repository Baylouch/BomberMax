/* MovementButton.cs
 * 
 * This script is attach on the movement pad to simulate a button to move the character.
 * 
 * */

using UnityEngine;
using UnityEngine.EventSystems;

public class MovementButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] MovementDirection direction;

    CharacterMovement movement;

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!movement)
            return;

        movement.StopMovement();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!movement)
            return;

        movement.PerformMovement(direction);
    }

    public void SetupMovement(CharacterMovement _movement)
    {
        movement = _movement;
    }


}
