/********************************************
 * File: PlayerMovementController.cs
 * Author: Orange Ninja Team
 * Description: Handles player movement and
 *              jumping through Rigidbody2D.
 ********************************************/

using UnityEngine;

/// Handles all movement and jump logic for the player character.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _speedMultiplier = 1f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Debug.Assert(_rb != null, "Rigidbody2D missing on Player!");
    }

    /// Handles movement and jump input each frame.
    public void HandleMovementInput()
    {
        float move = Input.GetAxis("Horizontal");
        _rb.linearVelocity = new Vector2(move * GetEffectiveMoveSpeed(), _rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            Debug.Log("Player jumped.");
        }
    }

    /// Returns the effective movement speed after multipliers.
    public float GetEffectiveMoveSpeed()
    {
        return _moveSpeed * _speedMultiplier;
    }

    /// Returns multiplier used by power-ups.
    public float CurrentSpeedMultiplier => _speedMultiplier;

    /// Sets the movement multiplier (clamped to non-negative values).
    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = Mathf.Max(0f, multiplier);
    }

    /// Resets movement multiplier to the default.
    public void ResetSpeedMultiplier()
    {
        _speedMultiplier = 1f;
    }
}
