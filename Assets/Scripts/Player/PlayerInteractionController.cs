/********************************************
 * File: PlayerInteractionController.cs
 * Author: Orange Ninja Team
 * Description: Manages pickups and object interactions.
 ********************************************/

using UnityEngine;

/// Handles item pickups and interactions.
public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private float _pickupRange = 1.5f;
    [SerializeField] private LayerMask _itemLayer;

    /// Handles input for interaction actions.
    public void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }
    }

    
    /// Detects nearby items and triggers pickup logic.

    private void TryPickupItem()
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(transform.position, _pickupRange, _itemLayer);
        foreach (Collider2D item in items)
        {
            Debug.Log("Picked up item: " + item.name);
            // item.GetComponent<Item>()?.Apply(player);
            Destroy(item.gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _pickupRange);
    }
}
