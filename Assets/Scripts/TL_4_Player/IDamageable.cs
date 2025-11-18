/********************************************
 * File: IDamageable.cs
 * Author: Safal Shrestha
 * Description: Interface for any object that can take damage.
 ********************************************/

/// Defines shared behavior for any object that can take damage.
public interface IDamageable
{
    /// Current health value.
    int CurrentHealth { get; }

    /// Maximum health value.
    int MaxHealth { get; }

    /// Returns true when the object is still alive.
    bool IsAlive { get; }

    /// Applies incoming damage (already sanitized for negative values).
    void ApplyDamage(int amount);
}
