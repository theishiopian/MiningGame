using System;
using UnityEngine;

public abstract class AbstractCharacter : MonoBehaviour
{
    [Header("Ground Check")] 
    public LayerMask groundMask;
    public float castDist = 0.15f;
    public SpriteRenderer Renderer { get; private set; }
    public Collider2D Collider { get; private set; }
    public Rigidbody2D Body { get; private set; }

    protected virtual void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Collider = GetComponent<Collider2D>();
        Body = GetComponent<Rigidbody2D>();
    }
    
    /**
     * Determines whether the character is touching the ground
     */
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position, ((BoxCollider2D)Collider).size, 0, Vector2.down,
            castDist, groundMask);
    }
}