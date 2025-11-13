/********************************************
 * File: PlayerMovementController.cs
 * Author: Orange Ninja Team
 * Description: Handles player movement and jumping
 * for both PC (keyboard/gamepad) and Android (tilt + touch).
 ********************************************/

using UnityEngine;
using UnityEngine.InputSystem; // Required for new Input System

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 7f;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    [Header("Input Actions")]
    public InputAction moveAction; // Assign Player → Move
    public InputAction jumpAction; // 👈 Add this in Inspector (Player → Jump)

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        Debug.Assert(_rb != null, "Rigidbody2D missing on Player!");
    }

    void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    public void HandleMovementInput()
    {
        // Read movement input (keyboard/gamepad)
        _moveInput = moveAction.ReadValue<Vector2>();

        // On Android, use tilt controls instead
        if (Application.platform == RuntimePlatform.Android)
        {
            Vector3 a = Input.acceleration;

            // Adjust mapping based on phone orientation
            float x;
            switch (Screen.orientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    x = -a.y; // common mapping for landscape-left
                    break;
                case ScreenOrientation.LandscapeRight:
                    x = a.y;  // common mapping for landscape-right
                    break;
                default:
                    x = a.x;
                    break;
            }

            const bool invertTiltX = false;
            const float tiltSensitivity = 1.5f;
            const float tiltDeadzone = 0.05f;

            if (Mathf.Abs(x) < tiltDeadzone) x = 0f;
            if (invertTiltX) x = -x;
            x *= tiltSensitivity;

            _moveInput = new Vector2(x, 0f);
        }

        // Apply movement
        _rb.linearVelocity = new Vector2(_moveInput.x * _moveSpeed, _rb.linearVelocity.y);

        // Handle jump (keyboard, gamepad, or touch)
        bool jumpPressed = jumpAction != null && jumpAction.triggered;

        // Fallback for Android touch
        if (!jumpPressed && Application.platform == RuntimePlatform.Android && Touchscreen.current != null)
        {
            jumpPressed = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        }

        if (jumpPressed)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            Debug.Log("Jump triggered!");
        }
    }
}
