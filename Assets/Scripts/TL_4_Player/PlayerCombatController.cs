/********************************************
 * File: PlayerCombatController.cs
 * Author: Safal Shrestha
 * Description: Handles melee attack input,
 *              hit detection, and applying
 *              damage to enemies.
 ********************************************/

using System;
using UnityEngine;

/// Handles attack detection and applying damage to enemies.
public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private int _attackDamage = 10;
    [SerializeField] private LayerMask _enemyLayer;

    public event Action<EnemyController> OnEnemyDefeated;

    /// Handles input for player attacks.
    public void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PerformAttack();
        }
    }

    /// Performs a melee attack by detecting enemies within range.
    private void PerformAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _enemyLayer);

        foreach (Collider2D hit in hits)
        {
            var enemy = hit.GetComponentInParent<EnemyController>();
            var health = hit.GetComponentInParent<IDamageable>();

            if (enemy == null || health == null || !health.IsAlive)
            {
                continue;
            }

            health.ApplyDamage(_attackDamage);
            Debug.Log($"Hit enemy {enemy.name} for {_attackDamage} damage.");

            if (!health.IsAlive)
            {
                OnEnemyDefeated?.Invoke(enemy);
                Debug.Log($"Enemy {enemy.name} defeated.");
                Destroy(enemy.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
