using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;      // how fast player moves
    public float jumpForce = 7f;      // how high player jumps
    private Rigidbody2D rb;
    private bool isGrounded = true;   // check if on ground

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // get the physics body
    }

    void Update()
    {
        // Move left/right
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    // Detect ground collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}