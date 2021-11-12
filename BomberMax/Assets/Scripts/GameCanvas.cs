/* GameCanvas.cs
 * 
 * Is used to set the UI of the player, singleton instance and persistent when stage has multiple scenes, appears only on the local player.
 * 
 * Has a reference to every UI component of the player to access them by another scripts
 * 
 * */

using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas instance;

    public PlayerHealth_UI healthUI;

    [SerializeField] Image characterProfil;
    [SerializeField] Text bombBonus;
    [SerializeField] Text explosionBonus;


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

    // For now we just use CharacterSprite from the CharacterSettings to set the image profil
    public void SetCharacterProfil(Sprite _sprite)
    {
        characterProfil.sprite = _sprite;
    }

    public void SetBombBonusText(int _bonus)
    {
        bombBonus.text = _bonus.ToString();
    }

    public void SetExplosionBonusText(int _bonus)
    {
        explosionBonus.text = _bonus.ToString();
    }
}
