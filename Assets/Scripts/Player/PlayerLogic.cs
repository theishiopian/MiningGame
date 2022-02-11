using StateMachine;
using System;
using UnityEngine;

namespace Player
{ 
    public abstract class PlayerLogic
    {
        #region STATES

        [Serializable]
        public class Idle : AnimatedState<PlayerController>
        {
            public override void Update(PlayerController owner)
            {
                base.Update(owner);
                if (InputHandler.Instance.HasInput() && InputHandler.Instance.MineHeld)
                {
                    owner.StateMachine.ChangeState("mining");
                    return;
                }

                if (InputHandler.Instance.HasInput())
                {
                    owner.StateMachine.ChangeState("walking");
                    return;
                }

                if (InputHandler.Instance.JumpHeld && owner.IsGrounded())
                {
                    owner.StateMachine.ChangeState("jumping");
                    return;//yes, this is redundant, but it may NOT be in the future
                }
            }
        }

        [Serializable]
        public class Walking : AnimatedState<PlayerController>
        {
            private Vector2 _movement;
            public override void Update(PlayerController owner)
            {
                base.Update(owner);
                if (InputHandler.Instance.MineHeld)
                {
                    owner.StateMachine.ChangeState("mining");
                    return;
                }

                if (!InputHandler.Instance.HasInput())
                {
                    owner.StateMachine.ChangeState("idle");
                    return;
                }

                if (InputHandler.Instance.JumpHeld && owner.IsGrounded())
                {
                    owner.StateMachine.ChangeState("jumping");
                    return;
                }

                owner.Renderer.flipX = Mathf.Sign(InputHandler.Instance.InputVector.x) < 0;
            }

            public override void FixedUpdate(PlayerController owner)
            {
                base.FixedUpdate(owner);
                _movement.x = InputHandler.Instance.InputVector.x * Time.fixedDeltaTime * owner.walkSpeed;
                owner.Body.position += _movement;
            }
        }

        [Serializable]
        public class Jumping : AnimatedState<PlayerController>
        {
            private Vector2 _movement;
            public override void Enter(PlayerController owner)
            {
                base.Enter(owner);
                owner.Body.AddForce(Vector2.up * (owner.Body.mass * owner.jumpForce),
                    ForceMode2D.Impulse);
            }

            public override void Update(PlayerController owner)
            {
                base.Update(owner);

                if (IsOnLastFrame() && owner.IsGrounded())
                {
                    owner.StateMachine.ChangeState(InputHandler.Instance.HasInput() ? "walking" : "idle");
                }
            }

            public override void FixedUpdate(PlayerController owner)
            {
                base.FixedUpdate(owner);

                _movement.x = InputHandler.Instance.InputVector.x * Time.fixedDeltaTime * owner.airSpeed;
                owner.Body.position += _movement;
            }
        }

        [Serializable]
        public class Mining : AnimatedState<PlayerController>
        {
            private bool _hasSwung;

            public override void Enter(PlayerController owner)
            {
                base.Enter(owner);

                CrossHair.Instance.SetVisible(true);
            }

            public override void Exit(PlayerController owner)
            {
                base.Exit(owner);

                CrossHair.Instance.SetVisible(false);
            }

            public override void Update(PlayerController owner)
            {
                base.Update(owner);

                if (!InputHandler.Instance.MineHeld)
                {
                    owner.StateMachine.ChangeState(InputHandler.Instance.HasInput() ? "walking" : "idle");
                    return;
                }

                if (!InputHandler.Instance.HasInput())
                {
                    owner.StateMachine.ChangeState("idle");
                    return;
                }

                owner.Renderer.flipX = Mathf.Sign(InputHandler.Instance.InputVector.x) < 0;

                var hasRoom = BucketRenderer.Instance.fill <= 100;
                var position = owner.transform.position;
                var hit = Physics2D.Raycast(position, InputHandler.Instance.RawInputVector, owner.digRange,
                    owner.mineMask);
                var targetPos = ((Vector2) position) +
                                InputHandler.Instance.RawInputVector.normalized * (owner.digRange - 0.1f);

                if (hit)
                {
                    CrossHair.Instance.transform.position =
                        hit.point + InputHandler.Instance.RawInputVector.normalized * 0.2f;
                    CrossHair.Instance.SetColor("default");
                }
                else
                {
                    CrossHair.Instance.transform.position = targetPos;
                    CrossHair.Instance.SetColor("invalid");
                }

                if (IsOnLastFrame())
                {
                    if (_hasSwung || !hit) return;
                    var pos = hit.point + (InputHandler.Instance.InputVector * 0.1f);

                    if (hasRoom && Map.Instance.TryBreakTile(pos, owner.digPower, out var tile) &&
                        tile is ResourceTile brokenTile)
                    {
                        BucketRenderer.Instance.AddMineralLayerIfNeeded(brokenTile);
                    }

                    _hasSwung = true;
                }
                else
                {
                    _hasSwung = false;
                }
            }
        }

        #endregion
    }
}