 /********************************************
  * File: EnemyHealthSimple.cs
  * Author: Orange Ninja Team
  * Description: Minimal health component for enemies.
  ********************************************/
 
 using UnityEngine;

/// Basic health component for enemies so they can respond to player attacks.
public class EnemyHealthSimple : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth = 50;

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
    }
}
