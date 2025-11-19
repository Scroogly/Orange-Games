using UnityEngine;

// This script makes the enemy fire bullets automatically.
// It works together with the EnemyController script.
[RequireComponent(typeof(EnemyController))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting")]
    // This is the bullet prefab the enemy will shoot.
    public GameObject bulletPrefab;
    // This is the Transform representing the gun muzzle where bullets spawn.
    public Transform firePoint;
    // How many seconds between each shot.
    public float fireRate = 2f;

    // A timer that counts up until it's time to shoot again.
    float fireTimer = 0f;

    // Update is called once per frame.
    // This is where I check if it's time for the enemy to fire a bullet.
    public void Update()
    {
        // If we don't have a bullet prefab or firePoint, we can't shoot.
        if (bulletPrefab == null || firePoint == null)
            return;

        // Count up the timer using Time.deltaTime (seconds since last frame).
        fireTimer += Time.deltaTime;

        // When the timer reaches the fire rate, shoot.
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f; // Reset timer after firing
        }
    }

    // This method actually creates the bullet and sets its direction.
    public void Shoot()
    {
        // Determine which way the enemy is facing.
        // If enemy's scale.x >= 0, it means facing right; otherwise, facing left.
        float dir = transform.localScale.x >= 0 ? 1f : -1f;

        // Create a new bullet at the FirePoint's position.
        // Quaternion.identity means no rotation.
        GameObject bulletObj = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        // Try to get the EnemyBullet component so we can set direction.
        EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            // Send the current facing direction to the bullet.
            bullet.SetDirection(dir);
        }
    }
}
