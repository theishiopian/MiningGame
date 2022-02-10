using System;
using UnityEngine;

public abstract class AbstractCharacter : MonoBehaviour
{
    public SpriteRenderer Renderer { get; private set; }

    protected virtual void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }
}