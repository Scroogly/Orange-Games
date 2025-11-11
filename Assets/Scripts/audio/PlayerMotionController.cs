/********************************************
 * File: PlayerMotionController.cs
 * Author: Orange Ninja Team
 * Description: Handles player locomotion and jumping.
 ********************************************/
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotionController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Audio IDs")]
    [SerializeField] private string _jumpSfxId = "jump";

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        _rb.linearVelocity = new Vector2(x * _moveSpeed, _rb.linearVelocity.y);

        if (x != 0f)
            transform.localScale = new Vector3(Mathf.Sign(x), 1f, 1f);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);

            if (AudioManager.Instance != null)
            {
                float pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                AudioManager.Instance.PlaySFX(_jumpSfxId, pitch);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.1f, _groundLayer);
    }
}
