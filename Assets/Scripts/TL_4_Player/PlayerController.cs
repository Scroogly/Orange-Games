/********************************************
 * File: PlayerController.cs
 * Author: Orange Ninja Team
 * Description: Coordinates movement, combat,
 *              interaction, powers, and
 *              win/lose conditions.
 ********************************************/

using UnityEngine;

/// Central manager for all player subsystems.
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCombatController))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    [Header("Win Condition Settings")]
    [SerializeField] private bool _requireWinLocation = false;
    [SerializeField] private Transform _winLocation;
    [SerializeField] private float _winRadius = 1.5f;

    private PlayerMovementController _movement;
    private PlayerCombatController _combat;
    private PlayerHealth _health;
    private PlayerPowerBase[] _powers;

    private bool _hasWon;
    private int _totalEnemies;
    private int _defeatedEnemies;

    /// Initializes all player components
    void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _combat = GetComponent<PlayerCombatController>();
        _health = GetComponent<PlayerHealth>();

        Debug.Assert(_movement != null, "Movement controller missing!");
        Debug.Assert(_combat != null, "Combat controller missing!");
        Debug.Assert(_health != null, "PlayerHealth missing!");

        _powers = GetComponentsInChildren<PlayerPowerBase>();

        // Count all enemies at start for win condition
        _totalEnemies = FindObjectsOfType<EnemyController>().Length;
        Debug.Log($"Total enemies to defeat: {_totalEnemies}");

        _combat.OnEnemyDefeated += HandleEnemyDefeated;
    }

    /// Handles all player input and checks win condition
    void Update()
    {
        if (!_health.IsAlive || _hasWon) {
            return;
        }

        _movement.HandleMovementInput();
        _combat.HandleAttackInput();
        CheckWinCondition();
    }

    /// Called whenever the combat controller defeats an enemy
    private void HandleEnemyDefeated(EnemyController enemy)
    {
        _defeatedEnemies++;
        Debug.Log($"Enemies defeated: {_defeatedEnemies}/{_totalEnemies}");

        // Check win condition after each enemy defeat
        CheckWinCondition();
    }

    /// Checks if the player has reached the win condition
    private void CheckWinCondition()
    {
        // First check: all enemies must be defeated
        if (_defeatedEnemies < _totalEnemies) {
            return;
        }

        // If win location is required, check distance
        if (_requireWinLocation) {
            if (_winLocation == null) {
                Debug.LogWarning("Win location required but not assigned!");
                return;
            }

            float distance = Vector3.Distance(transform.position, _winLocation.position);
            if (distance > _winRadius) {
                return;
            }

            // Player reached win location after defeating all enemies
            TriggerWin();
        } else {
            // Win immediately after defeating all enemies
            TriggerWin();
        }
    }

    /// Triggers the win state and disables player controls
    private void TriggerWin()
    {
        if (_hasWon) {
            return;
        }

        _hasWon = true;
        _movement.enabled = false;
        _combat.enabled = false;

        Debug.Log($"All {_totalEnemies} enemies defeated!");
        Debug.Log("=== YOU WON THE GAME! ===");
    }

    /// Draws gizmos for win location in editor
    void OnDrawGizmosSelected()
    {
        if (_winLocation == null || !_requireWinLocation) {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_winLocation.position, _winRadius);
    }
}