/********************************************
 * File: PlayerActionSFX.cs
 * Author: Orange Games Team
 * Description:
 * Handles player-triggered sounds and game state:
 * - Jump (Space)
 * - Pickup (Trigger)
 * - Pause/Unpause (Escape)
 * Includes full game freeze via Time.timeScale and
 * routes all audio through AudioManager.
 ********************************************/

using UnityEngine;

public class PlayerActionSFX : MonoBehaviour
{
    [Header("SFX IDs (must match SoundLibrary)")]
    [SerializeField] private string _jumpSfxId = "jump";
    [SerializeField] private string _pickupSfxId = "pickup";
    [SerializeField] private string _pauseSfxId = "pause";
    [SerializeField] private string _resumeSfxId = "resume";

    private bool _isPaused = false;

    void Start()
    //Dynamic Binding: Unity decides at run time which start() to call, because many objects can have start() in different scripts
    {
        // Validation — Orange Games coding standards
        Debug.Assert(AudioManager.Instance != null,
            "AudioManager.Instance missing — ensure prefab added.");

        Debug.Assert(!string.IsNullOrWhiteSpace(_jumpSfxId), "Jump SFX ID missing.");
        Debug.Assert(!string.IsNullOrWhiteSpace(_pickupSfxId), "Pickup SFX ID missing.");
        Debug.Assert(!string.IsNullOrWhiteSpace(_pauseSfxId), "Pause SFX ID missing.");
        Debug.Assert(!string.IsNullOrWhiteSpace(_resumeSfxId), "Resume SFX ID missing.");
    }

    void Update()
    //Dynamic Binding: Unity calls this method based on the collider that triggered it
    {
        HandleJumpInput();
        HandlePauseInput();
    }

    // ----------------------------------------------------
    // JUMP (Space key)
    // ----------------------------------------------------
    private void HandleJumpInput()
    // Static Binding: This is a normal private function.The compiler knows ehere itis
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.Instance.PlaySFX(_jumpSfxId);
            Debug.Log("[Audio] Jump SFX played.");
        }
    }

    // ----------------------------------------------------
    // PAUSE & UNPAUSE (Escape key)
    // ----------------------------------------------------
    private void HandlePauseInput()
    // Internal method, no override, no vortual - compile- time binding
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    // ----------------------------------------------------
    // GAME PAUSE
    // ----------------------------------------------------
    private void PauseGame()
    {
        Time.timeScale = 0f;  // freeze gameplay
        AudioManager.Instance.PauseAll();   // pause audio
        AudioManager.Instance.PlaySFX(_pauseSfxId); // pause sound

        Debug.Log("[Game] Paused + Pause SFX.");
    }

    // ----------------------------------------------------
    // GAME RESUME
    // ----------------------------------------------------
    private void ResumeGame()
    {
        Time.timeScale = 1f;  // unfreeze gameplay
        AudioManager.Instance.ResumeAll();  // resume audio
        AudioManager.Instance.PlaySFX(_resumeSfxId); // resume sound

        Debug.Log("[Game] Resumed + Resume SFX.");
    }

    // ----------------------------------------------------
    // PICKUP SFX (Trigger)
    // ----------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            AudioManager.Instance.PlaySFX(_pickupSfxId);
            Debug.Log("[Audio] Pickup SFX played.");
        }
    }
}
