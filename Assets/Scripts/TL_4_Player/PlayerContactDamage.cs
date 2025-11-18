/********************************************
 * File: PlayerContactDamage.cs
 * Author: Safal Shrestha
 * Description: Applies periodic damage when
 *              the player is touching enemies.
 ********************************************/

using UnityEngine;

/// Applies collision-based damage from enemies to the player.
[RequireComponent(typeof(PlayerHealth))]
public class PlayerContactDamage : MonoBehaviour
{
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private float damageCooldown = 1f;

    private PlayerHealth _playerHealth;
    private float _nextDamageTime;

    private void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        Debug.Assert(_playerHealth != null, "PlayerHealth is missing!");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryApplyDamage(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryApplyDamage(other);
    }

    /// Attempts to apply damage if the cooldown has expired.
    private void TryApplyDamage(Component collider)
    {
        if (Time.time < _nextDamageTime)
        {
            return;
        }

        var enemy = collider.GetComponentInParent<EnemyController>();
        if (enemy == null)
        {
            return;
        }

        _playerHealth.ApplyDamage(contactDamage);
        _nextDamageTime = Time.time + damageCooldown;

        Debug.Log($"Player took {contactDamage} contact damage from {enemy.name}.");
    }
}
