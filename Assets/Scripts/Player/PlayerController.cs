/********************************************
 * File: PlayerController.cs
 * Author: Orange Ninja Team
 * Description: Coordinates all Player subsystems.
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

    void Awake()
    {
        _movement = GetComponent<PlayerMovementController>();
        _combat = GetComponent<PlayerCombatController>();
        _interaction = GetComponent<PlayerInteractionController>();

        _playerData = new Player(START_HEALTH);
    }

    void Update()
    {
        _movement.HandleMovementInput();
        _combat.HandleAttackInput();
        _interaction.HandleInteractionInput();
    }

    public Player GetPlayerData() => _playerData;
}
