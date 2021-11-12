using UnityEngine;
using UnityEngine.EventSystems;

public class StageSelection_Button : MonoBehaviour, IPointerDownHandler
{
    public int stageBuildIndex = -1;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (FindObjectOfType<MenuStageManager>())
        {
            FindObjectOfType<MenuStageManager>().stageBuildIndexSelected = stageBuildIndex;
            FindObjectOfType<MenuStageManager>().isStageSelected = true;
        }
        else
            Debug.LogError("MenuStageManager not found...");
    }
}
