using UnityEngine;

namespace Interaction
{
    public class Column : MonoBehaviour, IColumInteraction
    {
        [SerializeField] private new BoxCollider2D collider2D;
    
        // delegate to OnMoveDone GameManager
        public event IColumInteraction.Interaction OnInteraction;

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    
        public void SetColliderSize(Vector2 size)
        {
            collider2D.size = size;
        }

        private void OnMouseDown()
        {
            // Debug.Log("Interact");
            OnInteraction?.Invoke(this);
        }
    }
}