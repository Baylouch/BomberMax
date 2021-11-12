using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth_UI : MonoBehaviour
{
    [SerializeField] Image firstHealth;
    [SerializeField] Image secondHealth;

    [SerializeField] Sprite emptyHeart;
    [SerializeField] Sprite fullHeart;

    public void LostFirstHealth()
    {
        firstHealth.sprite = emptyHeart;
    }

    public void LostSecondHealth()
    {
        secondHealth.sprite = emptyHeart;
    }

    public void GainHealth()
    {
        if (secondHealth.sprite == emptyHeart)
        {
            secondHealth.sprite = fullHeart;

        }
        else if (firstHealth.sprite == emptyHeart)
        {
            firstHealth.sprite = fullHeart;
        }
    }
}
