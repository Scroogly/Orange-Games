using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 6f;
    public float lifeTime = 3f;
    public int damage = 5;   // 5 health per hit

    float direction = 1f;
    Rigidbody2D rb;

    public void SetDirection(float dir)
    {
        direction = Mathf.Sign(dir);

        // Flip sprite visually
        Vector3 s = transform.localScale;
        s.x = direction >= 0 ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        transform.localScale = s;

        // Set initial velocity if rb already exists
        if (rb != null)
        {
            rb.linearVelocity = Vector2.right * direction * speed;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Debug.Log("[Bullet] Spawned, layer = " + gameObject.layer);
        // Move bullet using physics
        if (rb != null)
        {
            rb.linearVelocity = Vector2.right * direction * speed;
        }

        Destroy(gameObject, lifeTime);
    }

    // Keep Update public for your tests – nothing fancy here
    public void Update()
    {
        // nothing, physics is moving us
    }

    // Trigger hit (if collider isTrigger = true)
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[Bullet] OnTriggerEnter2D with: " + other.name);
        HandleHit(other);
    }

    // Collision hit (if collider isTrigger = false)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("[Bullet] OnCollisionEnter2D with: " + collision.collider.name);
        HandleHit(collision.collider);
    }

    private void HandleHit(Collider2D other)
    {
        // Try to find something that implements IDamageable (PlayerHealth)
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
        {
            damageable = other.GetComponentInParent<IDamageable>();
        }

        if (damageable != null)
        {
            Debug.Log("[Bullet] Applying " + damage + " damage to " + other.name);
            damageable.ApplyDamage(damage);   // -> PlayerHealth.ApplyDamage
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
