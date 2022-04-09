using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    public GameBonusData data;

    public void SpawnBonus()
    {
        GameObject _bonusGO = Instantiate(data.prefab, transform.position, Quaternion.identity);
        _bonusGO.GetComponent<GameBonus>().SetupBonus(data);
        StageManager.instance.SetBonusNode(new Vector2(transform.position.x, transform.position.y), true);
    }
}
