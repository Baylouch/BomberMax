using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAction_Bomb : MonoBehaviour, IPointerDownHandler
{
    BombSpawner bombSpawner;

    public void SetupBombSpawner(BombSpawner _spawner)
    {
        bombSpawner = _spawner;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        bombSpawner.SpawnBomb();
    }
}
