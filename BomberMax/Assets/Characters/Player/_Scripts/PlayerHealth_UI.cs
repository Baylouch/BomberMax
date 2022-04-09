using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth_UI : MonoBehaviour
{
    [SerializeField] Image[] healthParts; // First element is first healthpoint part left, Second is first healthpoint part right, etc...

    public void UpdateHealth(float _currentHealth)
    {
        // We first disable each parts, then we unable until we reach the current health point
        for (int i = 0; i < healthParts.Length; i++)
        {
            healthParts[i].gameObject.SetActive(false);
        }

        float toEnable = _currentHealth * 2; // 0.5f represent half of a heart so we multiply by 2 to got 1f to enable 1 part, ect...

        for (int i = 0; i < toEnable; i++)
        {
            healthParts[i].gameObject.SetActive(true);
        }
    }
}
