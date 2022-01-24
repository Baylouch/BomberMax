/* ShotGun.cs
 * 
 * 
 * This script shoot a bullet straithforward. If it hit a bomb, it explode. A player, he lost a life.
 * 
 * Can be access via bonus or additionnal player menu in exchange of xp/coins...
 * 
 * TODO Add a special shotgun who push the target 1 case in the back ?
 * 
 * */

using UnityEngine;

public class ShotGun : MonoBehaviour
{
    public const int MaxAmmo = 4;

    [SerializeField] GameObject bulletPrefab;

    LayerMask teamLayer;
    int currentAmmo = 0;

    // TODO Delete
    private void Start()
    {
        teamLayer = gameObject.layer;
        AddAmmo(10);
    }

    // TODO Delete
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (currentAmmo <= 0)
            return;

        // Trigger an animation

        // Create the bullet and set its orientation
        currentAmmo--;

        CharacterMovement _movement = GetComponent<CharacterMovement>();
        float shootOrientation = 0f;

        if (_movement)
        {
            switch (_movement.GetFirePointDirection())
            {
                case MovementDirection.Up:
                    shootOrientation = 0f;
                    break;
                case MovementDirection.Down:
                    shootOrientation = 180f;
                    break;
                case MovementDirection.Right:
                    shootOrientation = -90f;
                    break;
                case MovementDirection.Left:
                    shootOrientation = 90f;
                    break;
            }
        }

        GameObject bulletGO = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bulletGO.layer = teamLayer;
        bulletGO.transform.localEulerAngles = new Vector3(0f, 0f, shootOrientation);

    }

    // TODO Condition to limit with MaxAmmo
    public void AddAmmo(int _ammoNumb)
    {
        currentAmmo += _ammoNumb;
    }
}
