using System.Collections;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// Represents a disc in the game, which is a visual element on the board that can move and change its appearance.
    /// The class extends <see cref="Space"/> and includes functionality for animation, visibility, and color changes.
    /// </summary>
    public class Disc : Space
    {
        private float _startYPosition;
        private float _lastYPosition;

        /// <summary>
        /// Sets the starting Y position for the disc animation.
        /// </summary>
        /// <param name="startYPosition">The Y position where the animation should begin.</param>
        public void SetStartPositionForAnimation(float startYPosition)
        {
            _startYPosition = startYPosition;
        }
        
        /// <summary>
        /// Sets the final Y position for the disc animation.
        /// </summary>
        /// <param name="lastYPosition">The Y position where the animation should end.</param>
        public void SetLastPositionForAnimation(float lastYPosition)
        {
            _lastYPosition = lastYPosition;
        }
        
        /// <summary>
        /// Sets the visibility of the disc. If true, the disc is visible; if false, the disc is hidden.
        /// </summary>
        /// <param name="visibility">True to make the disc visible, false to hide it.</param>
        public void SetVisibility(bool visibility)
        {
            spriteRenderer.enabled = visibility;
        }

        /// <summary>
        /// Sets the color of the disc using a specified <see cref="Color"/>.
        /// </summary>
        /// <param name="color">The color to apply to the disc.</param>
        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

        /// <summary>
        /// Starts the animation for the disc's movement.
        /// </summary>
        public void StartAnimation()
        {
            StartCoroutine(Animation());
        }

        /// <summary>
        /// Coroutine that handles the disc animation, smoothly moving it from its starting Y position to the final Y position.
        /// </summary>
        /// <returns>An enumerator that yields at each frame until the animation is complete.</returns>
        private IEnumerator Animation()
        {
            float elapsedTime = 0;
            const float waitTime = 0.2f;
            
            // While the animation time is less than the total wait time, update the disc's position
            while (elapsedTime < waitTime)
            {
                // Smoothly interpolate (lerp) the Y position from start to end
                transform.position = 
                    new Vector2(
                            transform.position.x, 
                            Mathf.Lerp(_startYPosition, _lastYPosition, elapsedTime / waitTime)
                        );
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the final position is set precisely at the target position
            transform.position = new Vector2(transform.position.x, _lastYPosition);
            yield return null;
        }
    }
}
