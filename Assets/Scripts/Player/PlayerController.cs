/********************************************
 * File: PlayerController.cs
 * Author: Orange Ninja Team
 * Description: Coordinates all Player subsystems.
* and handles win/lose conditions.
 ********************************************/

using UnityEngine;

/// Central coordinator for player systems (movement, combat, interaction).
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerCombatController))]
[RequireComponent(typeof(PlayerInteractionController))]
public class PlayerController : MonoBehaviour
{
    private Player _playerData;
    private PlayerMovementController _movement;
    private PlayerCombatController _combat;
    private PlayerInteractionController _interaction;

    private const int START_HEALTH = 100;
    private bool _hasWon = false;


    void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _combat = GetComponent<PlayerCombatController>();
        _interaction = GetComponent<PlayerInteractionController>();

        _playerData = new Player(START_HEALTH);
    }

   
     void Update()
    {
        // Only allow updates if game not over
        if (!_playerData.IsAlive() || _hasWon) return;

        _movement.HandleMovementInput();
        _combat.HandleAttackInput();
        _interaction.HandleInteractionInput();


        CheckLoseCondition();
        CheckWinCondition();
    }

    public Player GetPlayerData() => _playerData;

    /// Checks if player HP is zero and triggers lose state.
    private void CheckLoseCondition()
    {
        if (!_playerData.IsAlive())
        {
            Debug.LogWarning("Player has died. Game Over!");
            // Disable movement/interaction
            _movement.enabled = false;
            _combat.enabled = false;
            _interaction.enabled = false;
        }
    }

    /// Checks if player reached win condition.
    private void CheckWinCondition()
    {
        if (_playerData.GetInventory().Count >= 3 && !_hasWon)
        {
            _hasWon = true;
            Debug.Log("Player has won the game!");
            // Disable controls
            _movement.enabled = false;
            _combat.enabled = false;
            _interaction.enabled = false;
        }
    }
}
