/********************************************
 * File: PlayerCombatController.cs
 * Author: Orange Ninja Team
 * Description: Manages attack and combat actions.
 ********************************************/

using UnityEngine;

/// Handles attack input, hit detection, and damage.
public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private float _attackRange = 1.2f;
    [SerializeField] private int _attackDamage = 25;
    [SerializeField] private LayerMask _enemyLayer;

    /// Handles attack input and triggers combat actions.
    public void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PerformAttack();
        }
    }


    /// Detects enemies within range and applies damage.
    private void PerformAttack()
    {
        Debug.Log("Player performs attack!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _enemyLayer);
        foreach (Collider2D hit in hits)
        {
            Debug.Log("Enemy hit: " + hit.name);
            // hit.GetComponent<EnemyController>()?.ApplyDamage(_attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
