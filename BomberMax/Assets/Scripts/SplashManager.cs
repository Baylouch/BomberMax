using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    public void GoToMenu()
    {
        SceneManager.LoadScene(1);
    }
}
