/* GameManager.cs
 * 
 * 
 * 
 * 
 * 
 * */


using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject loosePanel;

    public Transform[] spawnPosTeamOne;
    public Transform[] spawnPosTeamTwo;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (CharacterSelection.instance)
            CharacterSelection.instance.InstantiateCharacter();

        // Once we spawn chars we get loop trough CharInfo components to set the ID
        int playerID = 0;

        CharacterInfo[] charsInfo = FindObjectsOfType<CharacterInfo>();
        for (int i = 0; i < charsInfo.Length; i++)
        {
            charsInfo[i].CharID = playerID;
            playerID++;
        }
    }

    public void DisplayLoosePanel()
    {
        loosePanel.SetActive(true);
    }

    public void PlayAgain()
    {
        int _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(_currentSceneIndex);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(1);

    }
}
