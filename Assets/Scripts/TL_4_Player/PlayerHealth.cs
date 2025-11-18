/********************************************
 * File: PlayerHealth.cs
 * Author: Safal Shrestha
 * Description: Handles player HP, damage,
 *              healing, max HP changes,
 *              and death behavior.
 ********************************************/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class HealthChangedEvent : UnityEvent<int, int> { }

/// Manages all player health-related logic.
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    public HealthChangedEvent OnHealthChanged;
    [SerializeField] private UnityEvent onDeath;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;

    /// Applies healing and returns the amount actually healed.
    public int Heal(int amount)
    {
        if (amount <= 0 || !IsAlive)
        {
            return 0;
        }

        int previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (previous != currentHealth)
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            Debug.Log($"Player healed for {currentHealth - previous}.");
        }

        return currentHealth - previous;
    }

    /// Deals damage to the player and returns actual damage taken.
    public int TakeDamage(int amount)
    {
        if (amount <= 0 || !IsAlive)
        {
            return 0;
        }

        int previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"Player took {previous - currentHealth} damage.");

        if (!IsAlive)
        {
            HandleDeath();
        }

        return previous - currentHealth;
    }

    public void ApplyDamage(int amount)
    {
        TakeDamage(amount);
    }

    /// Updates max health while preserving current health amount.
    public void SetMaxHealthPreserveCurrent(int newMax)
    {
        if (newMax <= 0)
        {
            return;
        }

        int previous = currentHealth;
        maxHealth = newMax;
        currentHealth = Mathf.Clamp(previous, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Player max HP updated to {newMax}.");
    }

    /// Handles player death and restarting the level.
    private void HandleDeath()
    {
        Debug.LogWarning("Player has died. Restarting level...");
        onDeath?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
