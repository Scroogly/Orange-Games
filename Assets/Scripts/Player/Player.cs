using System.Collections.Generic;

/// Represents the core Player data and basic actions.
public class Player
{
    private int _health;
    private (float x, float y) _position;
    private Weapon _weapon;
    private List<Item> _inventory;

    private const int MAX_HEALTH = 100;

    public Player(int initialHealth = MAX_HEALTH)
    {
        _health = initialHealth;
        _inventory = new List<Item>();
    }


    public void Move(float newX, float newY)
    {
        _position = (newX, newY);
    }


    public void TakeDamage(int amount)
    {
        if (amount < 0) return;
        _health -= amount;
        if (_health < 0) _health = 0;
    }

    public void Heal(int amount)
    {
        _health = System.Math.Min(_health + amount, MAX_HEALTH);
    }


    public bool IsAlive() => _health > 0;

    public int GetHealth() => _health;

    // public void Pickup(Item item)
    // {
    //     _inventory.Add(item);
    // }

    // public List<Item> GetInventory() => _inventory;

    // public void EquipWeapon(Weapon weapon)
    // {
    //     _weapon = weapon;
    // }

    // public Weapon GetWeapon() => _weapon;
}
