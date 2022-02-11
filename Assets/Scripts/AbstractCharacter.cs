using System;
using UnityEngine;

public abstract class AbstractCharacter : MonoBehaviour
{
    public SpriteRenderer Renderer { get; private set; }
    public Collider2D Collider { get; private set; }
    public Rigidbody2D Body { get; private set; }

    protected virtual void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();
        Body = GetComponent<Rigidbody2D>();
    }
}