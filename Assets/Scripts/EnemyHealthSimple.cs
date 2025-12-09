 /********************************************
  * File: EnemyHealthSimple.cs
  * Author: Orange Ninja Team
  * Description: Minimal health component for enemies.
  ********************************************/
 
 using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// Basic health component for enemies so they can respond to player attacks.
public class EnemyHealthSimple : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth = 50;

    [Header("Drops")]
    [Tooltip("Prefab to instantiate when the enemy dies.")]
    [SerializeField] private GameObject dropPrefab;
    [Tooltip("If the drop is a SceneTransitionPickup, this scene will be loaded.")]
    [SerializeField] private string sceneToLoadOnDrop;

#if UNITY_EDITOR
    [Tooltip("Drag a Scene asset here to automatically set the scene name.")]
    [SerializeField] private SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneToLoadOnDrop = sceneAsset.name;
        }
    }
#endif

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0 || !IsAlive) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Drop logic is now handled by PlayerController for level completion
        /*
        if (dropPrefab != null)
        {
            GameObject droppedItem = Instantiate(dropPrefab, transform.position, Quaternion.identity);
            
            // If the dropped item has the SceneTransitionPickup script, configure it
            SceneTransitionPickup pickup = droppedItem.GetComponent<SceneTransitionPickup>();
            if (pickup != null && !string.IsNullOrEmpty(sceneToLoadOnDrop))
            {
                pickup.SetSceneName(sceneToLoadOnDrop);
            }
        }
        */

        Destroy(gameObject);
    }
}
