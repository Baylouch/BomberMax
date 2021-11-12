using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStageManager : MonoBehaviour
{
    public bool isStageSelected = false;
    public int stageBuildIndexSelected = -1;

    public void LoadStage()
    {
        if (!isStageSelected)
            return;

        if (stageBuildIndexSelected > 0 && stageBuildIndexSelected < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(stageBuildIndexSelected);
    }

    public void BackToPreviousLevel()
    {
        int _buildIndex = SceneManager.GetActiveScene().buildIndex - 1;
        SceneManager.LoadScene(_buildIndex);
    }
}
