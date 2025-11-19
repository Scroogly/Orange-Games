using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // How fast the bullet moves.
    public float speed = 6f;

    // How long the bullet exists before it disappears.
    public float lifeTime = 3f;

    // Direction the bullet travels.
    // +1 = right, -1 = left (set by the enemy shooter)
    float direction = 1f;

    // This is called by EnemyShooter to make the bullet move left or right.
    public void SetDirection(float dir)
    {
        // Normalize the direction so it's always exactly +1 or -1.
        direction = Mathf.Sign(dir);

        // Flip the bullet visually if it needs to face the other direction.
        Vector3 s = transform.localScale;
        s.x = direction >= 0 ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        transform.localScale = s;
    }

    void Start()
    {
        // Destroy the bullet after 'lifeTime' seconds
        // so the scene doesn't fill up with bullets forever.
        Destroy(gameObject, lifeTime);
    }

    public void Update()
    {
        // Move the bullet in a straight line every frame.
        // Vector2.right is (1, 0), so multiplying by direction flips it left.
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }
}
