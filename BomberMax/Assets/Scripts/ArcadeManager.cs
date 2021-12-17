/* ArcadeManager.cs
 * 
 * Spawn player and bot in Start(), set the TeamMember component on it. In Arcande mode, player always be team 1 and bot(s) team 2.
 * Set position randomly relative to team number.
 * 
 * 
 * */

using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeManager : MonoBehaviour
{
    [SerializeField] int stageLevel = 1; // To know what's the next stage to load

    [SerializeField] GameObject playerPrefab;

    [SerializeField] GameObject[] botPrefabs;

    [SerializeField] Transform[] playerSpawnPos;
    [SerializeField] Transform[] botSpawnPos; // Must always be greater or equal than botPrefabs

    [SerializeField] GameObject loosePanel;
    [SerializeField] GameObject winPanel;

    // We know there is only one player but multiple bot. So we need to set variables in each stage to know the number,
    // then we can decrement each time an entity is dead and know when the game is ended
    int botsNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Spawn player and bots
        GameObject _player = Instantiate(playerPrefab, playerSpawnPos[Random.Range(0, playerSpawnPos.Length)].position, Quaternion.identity);
        _player.GetComponent<TeamMember>().TeamNumber = 1;

        for (int i = 0; i < botPrefabs.Length; i++)
        {
            GameObject _bot = Instantiate(botPrefabs[i], botSpawnPos[i].position, Quaternion.identity);
            _bot.GetComponent<TeamMember>().TeamNumber = 2;
            botsNumber++;
        }
    }

    public void BotDeathNotification()
    {
        botsNumber--;

        if (botsNumber <= 0)
        {
            // Player win
            Debug.Log("Player win !");
            winPanel.SetActive(true);
        }
    }

    public void PlayerDeathNotification()
    {
        // Player loose
        Debug.Log("Player loose !");
        loosePanel.SetActive(true);
    }

    // When player win, to go to the next arcade stage
    public void GoToNextArcadeStage()
    {
        // We should have an array contain each build index stage by stage level
        // then we can randomly choose one
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(1);
    }
}
