using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class Portal : MonoBehaviour
{
    [Header("Teleport Settings")]
    public bool loadNewScene = false;      // Check this to load a scene
    public string sceneName;                // Name of the scene to load (if checked)
    public bool teleportToPosition = true;  // Check this to teleport to a position
    public Vector2 teleportPosition = Vector2.zero; // Position in the same scene

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (loadNewScene && !string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("Loading scene: " + sceneName);
        }
        else if (teleportToPosition)
        {
            other.transform.position = new Vector3(
                teleportPosition.x,
                teleportPosition.y,
                other.transform.position.z
            );

            // Optional: reset velocity
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            Debug.Log("Player teleported to: " + teleportPosition);
        }
    }
}
