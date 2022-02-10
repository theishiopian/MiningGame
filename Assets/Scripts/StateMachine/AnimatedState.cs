using System;
using UnityEngine;

namespace StateMachine
{
    [Serializable]
    public abstract class AnimatedState<T> : IGenericState<T> where T : AbstractCharacter
    {
        public Sprite[] sprites;
        public float frameTime = 0.2f;
        public bool loop = true;

        private float t = 0;
        private int frame = 0;
        private bool playing = true;
        public virtual void Enter(T owner)
        {
            playing = true;
            frame = 0;
            t = 0;
            owner.Renderer.sprite = sprites[0];
        }

        public virtual void Exit(T owner)
        {
            frame = 0;
            t = 0;
        }

        public virtual void Update(T owner)
        {
            if (!playing) return;
        
            t += Time.deltaTime;

            if (t > frameTime)
            {
                t = 0;
                frame++;
                if (frame >= sprites.Length)
                {
                    if (loop)
                    {
                        frame = 0;
                    }
                    else
                    {
                        frame = sprites.Length - 1;
                        playing = false;
                        return;
                    }
                }
            }

            owner.Renderer.sprite = sprites[frame];
        }

        public virtual void FixedUpdate(T owner)
        {

        }

        public int GetFrame()
        {
            return frame;
        }

        public bool IsOnLastFrame()
        {
            return frame == sprites.Length - 1;
        }
    }
}