using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTransitionPickup : MonoBehaviour
{
    [Tooltip("The name of the scene to load when picked up.")]
    [SerializeField] private string sceneToLoad;

#if UNITY_EDITOR
    [Tooltip("Drag a Scene asset here to automatically set the scene name.")]
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneToLoad = sceneAsset.name;
        }
    }
#endif

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding is the player
        // We check for the "Player" tag or a specific component like PlayerController
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Pickup collected! Loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("SceneTransitionPickup: No scene name specified!");
        }
    }

    // Allow external scripts (like the Enemy) to configure the destination
    public void SetSceneName(string sceneName)
    {
        sceneToLoad = sceneName;
    }
}
