/* CharacterSelection.cs
 * 
 * This script is used to instantiate the player in the gameplay scene.
 * 
 * public function InstantiateCharacter() is called by the GameManager when gameplay scene is created.
 * 
 * */

using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection instance;

    public CharacterSettings selectedCharacter;

    public GameObject characterPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void InstantiateCharacter()
    {
        // TODO Create spawnpoint knows by GameManager and pass into this function by parameter
        GameObject _character = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);

        // TODO Create a script on the character to set it more properly ?
        _character.GetComponent<Animator>().runtimeAnimatorController = selectedCharacter.animatorController;
        _character.GetComponentInChildren<SpriteRenderer>().sprite = selectedCharacter.characterSprite;

        // Once we finish, we can destroy this gameObject because no more use of it.
        Destroy(gameObject);
    }
}
