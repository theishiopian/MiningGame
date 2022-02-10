using System;
using UnityEngine;

[Serializable]
public class PlayerState
{
    [SerializeField] protected Sprite[] frames;
    [SerializeField] protected float frameTime = 0.2f;
    [SerializeField] protected bool loop = true;

    protected float t;
    protected int currentFrame;

    public virtual void Enter(PlayerStateMachine stateMachine)
    {
        t = 0;
        currentFrame = 0;
    }

    public virtual void Exit(PlayerStateMachine stateMachine)
    {
        t = 0;
        currentFrame = 0;
    }

    public virtual void Update(PlayerStateMachine stateMachine)
    {
        t += Time.deltaTime;
        if (t > frameTime)
        {
            t = 0;
            
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                currentFrame = loop ? 0 : frames.Length - 1;
            }
        }
        stateMachine.renderer.sprite = frames[currentFrame];
    }

    public virtual void FixedUpdate(PlayerStateMachine stateMachine)
    {
    }
    
    public bool IsOnLastFrame()
    {
        return currentFrame >= frames.Length - 1;
    }
}