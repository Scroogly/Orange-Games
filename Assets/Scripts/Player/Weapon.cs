/********************************************
 * File: Weapon.cs
 * Author: Orange Ninja Team
 * Description: Extends Item to include damage value.
 ********************************************/

using UnityEngine;

/// Represents a weapon with a name and damage amount.
public class Weapon : Item
{
    [SerializeField] private int _damage;

    public int GetDamage() => _damage;
}
