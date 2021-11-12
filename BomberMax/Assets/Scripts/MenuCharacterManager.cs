using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCharacterManager : MonoBehaviour
{
    public void GoToLevelSelection()
    {
        SceneManager.LoadScene(4);
    }

    public void BackToPreviousLevel()
    {
        int _buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
        SceneManager.LoadScene(_buildIndex);
    }
}
