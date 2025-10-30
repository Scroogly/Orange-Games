//Team Lead 1 - Aiden Weaver
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HealthChangedEvent : UnityEvent<int, int> { } // currentHealth, maxHealth

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    public HealthChangedEvent OnHealthChanged; 

    private void Reset()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
    }

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;


    public int Heal(int amount) // Heals the player by int amount, returns amount actually healed.
    {
        if (amount <= 0 || IsDead) return 0;

        int prev = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        int healed = currentHealth - prev;

        if (healed > 0)
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

        return healed;
    }

    
    public int TakeDamage(int amount) // Damage the player, Returns damage actually taken.
    {
        if (amount <= 0 || IsDead) return 0;

        int prev = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        int damage = prev - currentHealth;

        if (damage > 0)
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (IsDead)
        {
    
        }

        return damage;
    }

    public void SetMaxHealth(int newMax, bool keepCurrentPercent = true) //Allows for changing of max Health
    {
        if (newMax <= 0) return;
        float percent = keepCurrentPercent ? (float)currentHealth / maxHealth : 1f;
        maxHealth = newMax;
        currentHealth = Mathf.Clamp(Mathf.RoundToInt(maxHealth * percent), 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
