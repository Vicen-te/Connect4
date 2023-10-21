using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Space : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        public float Radius => GetRadius();
        public KeyValuePair<int, int> GamePosition { get; private set; }
    
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    
        public void SetGamePosition(KeyValuePair<int, int> position)
        {
            GamePosition = position;
        }
    
        private float GetRadius()
        {
            Vector3 halfSize = spriteRenderer.sprite.bounds.extents;
            return halfSize.x > halfSize.y ? halfSize.x : halfSize.y;
        }
    }
}