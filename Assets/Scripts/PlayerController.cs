using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player playerData;          // data object
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public int attackDamage = 20;
    public float attackRange = 1f;     // attack reach distance
    public LayerMask enemyLayer;       // layer to detect enemies

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = new Player(100); // starting HP = 100
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Attack (F key)
        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }
    }

    // --- ATTACK ACTION ---
    void Attack()
    {
        playerData.Attack(); // just logs for now

        // If you later want to hit enemies:
        // Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        // foreach (Collider2D hit in hits)
        // {
        //     Debug.Log("Enemy hit!");
        // }

        // Play animation or effect here later
    }

    // --- DAMAGE FROM ENEMY ---
    public void ApplyDamage(int amount)
    {
        playerData.TakeDamage(amount);
        Debug.Log("Player took " + amount + " damage. HP: " + playerData.health);

        if (!playerData.IsAlive())
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        gameObject.SetActive(false);
    }

    // --- Optional debug visual for attack range ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
