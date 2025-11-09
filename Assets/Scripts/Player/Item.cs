/********************************************
 * File: Item.cs
 * Author: Orange Ninja Team
 * Description: Defines a basic pickup item.
 ********************************************/

using UnityEngine;

/// Represents a simple item that can be picked up by the player.
public class Item : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private string _effect;

    public string GetName() => _name;
    public string GetEffect() => _effect;
}
