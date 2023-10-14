using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Circle : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer spriteRenderer;
        public float Radius { get; private set; }
        public KeyValuePair<int, int> GamePosition { get; private set; }
    
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    
        public void SetGamePosition(KeyValuePair<int, int> position)
        {
            GamePosition = position;
        }
    
        // Start is called before the first frame update
        public static float GetRadius(SpriteRenderer spriteRenderer)
        {
            Vector3 halfSize = spriteRenderer.sprite.bounds.extents;
            return halfSize.x > halfSize.y ? halfSize.x : halfSize.y;
        }
    }
}