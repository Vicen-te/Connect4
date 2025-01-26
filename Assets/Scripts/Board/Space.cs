using UnityEngine;

namespace Board
{
    /// <summary>
    /// Represents a space on the board, typically used to represent a position where discs can be placed.
    /// This class handles the position and size of the space, using a SpriteRenderer to manage the visual representation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Space : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        
        /// <summary>
        /// Gets the radius of the space, based on the dimensions of the sprite.
        /// The radius is the larger of the sprite's half-width or half-height.
        /// </summary>
        public float Radius => GetRadius();
    
        /// <summary>
        /// Sets the position of the space in the 2D world.
        /// </summary>
        /// <param name="position">The new position for the space.</param>
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    
        /// <summary>
        /// Calculates and returns the radius of the space based on the sprite's bounds.
        /// The radius is determined by the larger of the sprite's half-width or half-height.
        /// </summary>
        /// <returns>The radius of the space.</returns>
        private float GetRadius()
        {
            Vector3 halfSize = spriteRenderer.sprite.bounds.extents;
            return halfSize.x > halfSize.y ? halfSize.x : halfSize.y;
        }
    }
}
