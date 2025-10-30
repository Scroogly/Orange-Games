/********************************************
 * File: PlayerMovementController.cs
 * Author: Orange Ninja Team
 * Description: Handles player movement and jumping.
 ********************************************/

using UnityEngine;


/// Handles all player movement logic and physics.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 7f;

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Debug.Assert(_rb != null, "Rigidbody2D missing on Player!");
    }


    /// Processes input and applies motion to Rigidbody2D.
    public void HandleMovementInput()
    {
        float move = Input.GetAxis("Horizontal");
        _rb.linearVelocity = new Vector2(move * _moveSpeed, _rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            Debug.Log("Player jumped!");
        }
    }
}
