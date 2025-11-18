/********************************************
 * File: PlayerInteractionController.cs
 * Author: Orange Ninja Team
 * Description: Handles generic interaction
 *              input (placeholder system).
 ********************************************/

using UnityEngine;

/// Handles simple interaction key input (expandable later).
public class PlayerInteractionController : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 1.5f;

    /// Handles E key press for interaction.
    public void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interaction attempted.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactionRange);
    }
}
