using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMainManager : MonoBehaviour
{
    public void GoToCharacterSelectionLevel()
    {
        SceneManager.LoadScene(3);
    }

    public void GoToOptionsLevel()
    {
        SceneManager.LoadScene(2);
    }
}
