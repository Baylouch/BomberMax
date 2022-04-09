using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAction_Bomb : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameObject bombDisableGo;

    BombSpawner bombSpawner;

    void Update()
    {
        if (bombSpawner.CanDropBomb() && bombDisableGo.activeSelf)
        {
            bombDisableGo.SetActive(false);
        }
        else if (!bombSpawner.CanDropBomb() && !bombDisableGo.activeSelf)
        {
            bombDisableGo.SetActive(true);
        }
    }

    public void SetupBombSpawner(BombSpawner _spawner)
    {
        bombSpawner = _spawner;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!bombSpawner.CanDropBomb())
            return;

        bombSpawner.DropBomb();
    }
}
