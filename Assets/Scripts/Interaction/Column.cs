using UnityEngine;

namespace Interaction
{
    /// <summary>
    /// Represents a column that can interact with the player or other game elements.
    /// Implements the IColumInteraction interface to handle interaction events.
    /// </summary>
    public class Column : MonoBehaviour, IColumInteraction
    {
        /// <summary>
        /// The BoxCollider2D component attached to the column, used for collision detection.
        /// </summary>
        [SerializeField] private new BoxCollider2D collider2D;
    
        // Event that is triggered when the column is interacted with (delegated to GameManager)
        public event IColumInteraction.Interaction OnInteraction;

        /// <summary>
        /// Sets the position of the column in the game world.
        /// </summary>
        /// <param name="position">The new position to set for the column.</param>
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    
        /// <summary>
        /// Sets the size of the collider attached to the column.
        /// </summary>
        /// <param name="size">The new size of the collider.</param>
        public void SetColliderSize(Vector2 size)
        {
            collider2D.size = size;
        }

        /// <summary>
        /// Handles mouse interaction with the column (triggered when the column is clicked).
        /// </summary>
        private void OnMouseDown()
        {
            // Trigger the interaction event if there are any listeners
            OnInteraction?.Invoke(this);
        }
    }
}
