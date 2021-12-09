using UnityEngine;

public class DangerTilesReseter : MonoBehaviour
{
    private void OnDestroy()
    {
        StageManager.instance.ResetDangerTiles();
    }
}
