using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Space : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        public float Radius => GetRadius();

        /// Key = Column
        /// Value = Row 
        private KeyValuePair<int, int> _gamePosition;
        public int Column => _gamePosition.Key;
        public int Row => _gamePosition.Value;
    
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    
        public void SetGamePosition(KeyValuePair<int, int> position)
        {
            _gamePosition = position;
        }
    
        private float GetRadius()
        {
            Vector3 halfSize = spriteRenderer.sprite.bounds.extents;
            return halfSize.x > halfSize.y ? halfSize.x : halfSize.y;
        }
    }
}