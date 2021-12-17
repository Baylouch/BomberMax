using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMainManager : MonoBehaviour
{
    public void GoToOnePlayer()
    {
        SceneManager.LoadScene(2);
    }

    public void GoToMultiplayer()
    {
        SceneManager.LoadScene(3);
    }

    public void GoToOptions()
    {
        SceneManager.LoadScene(4);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
