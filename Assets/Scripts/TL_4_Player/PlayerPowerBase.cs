/********************************************
 * File: PlayerPowerBase.cs
 * Author: Safal Shrestha
 * Description: Base MonoBehaviour for power-ups.
 *              Uses helper classes to demonstrate
 *              dynamic and static binding patterns.
 ********************************************/

using UnityEngine;

/// Abstract base class for all power-up components.
/// Uses composition with helper effect classes to demonstrate binding.
public abstract class PlayerPowerBase : MonoBehaviour
{
    [SerializeField] private string _powerName = "Power";

    /// Whether the power-up is currently active.
    public bool IsActive { get; protected set; }

    /// Read-only name of the power-up.
    public string PowerName => _powerName;

    /// Activates the power-up with optional override duration.
    /// Implemented in subclasses.
    public abstract void ActivatePower(float? overrideDurationSeconds = null);

    /// Deactivates the power-up and restores original values.
    /// Implemented in subclasses.
    public abstract void DeactivatePower();
}