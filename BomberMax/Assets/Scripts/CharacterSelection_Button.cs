using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelection_Button : MonoBehaviour, IPointerDownHandler
{
    public CharacterSettings characterSettings;

    public void SetCharacterSelectionSettings()
    {
        if (CharacterSelection.instance == null)
        {
            Debug.LogError("No CharacterSelection instance found...");
        }
        else
        {
            CharacterSelection.instance.selectedCharacter = characterSettings;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetCharacterSelectionSettings();
    }
}
