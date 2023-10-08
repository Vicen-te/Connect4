using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Column : MonoBehaviour
{
    [SerializeField] private new BoxCollider2D collider2D;
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
        Debug.Log("Hello");
    }
}
