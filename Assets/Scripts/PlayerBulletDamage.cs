using UnityEngine;

public class PlayerBulletDamage : MonoBehaviour
{
    public PlayerHealth playerHealth;  // assign in Inspector
    public int damagePerHit = 5;

    void Awake()
    {
        // If not set in Inspector, grab from this object or parent
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = GetComponentInParent<PlayerHealth>();
            }
        }
    }

    // Called when one of the colliders is marked as "Is Trigger"
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            ApplyBulletHit(other.gameObject);
        }
    }

    // Called when colliders are NOT triggers
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("EnemyBullet"))
        {
            ApplyBulletHit(collision.collider.gameObject);
        }
    }

    private void ApplyBulletHit(GameObject bullet)
    {
        if (playerHealth != null)
        {
            playerHealth.ApplyDamage(damagePerHit);   // your existing PlayerHealth
            Debug.Log("[Player] Hit by bullet. Damage: " + damagePerHit);
        }

        Destroy(bullet);
    }
}
