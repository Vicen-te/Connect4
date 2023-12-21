using UnityEngine;

namespace Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Space : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        public float Radius => GetRadius();
    
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    
        private float GetRadius()
        {
            Vector3 halfSize = spriteRenderer.sprite.bounds.extents;
            return halfSize.x > halfSize.y ? halfSize.x : halfSize.y;
        }
    }
}