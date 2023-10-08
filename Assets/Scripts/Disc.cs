using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour
{
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    public float Radius { get; private set; }

    public void SetPosition(float xPosition, float yPosition)
    {
        transform.position = new Vector3(xPosition, yPosition, 0);
    }
    
    public void SetPosition(Vector2 position)
    {
        transform.position = position;
    }

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out _transform);
        TryGetComponent(out _spriteRenderer);
        Vector3 halfSize = _spriteRenderer.sprite.bounds.extents;
        Radius = halfSize.x > halfSize.y ? halfSize.x : halfSize.y;
        _transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
