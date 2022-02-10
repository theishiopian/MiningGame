using System;
using UnityEngine;

public abstract class PlayerLogic
{
    #region STATES
    [Serializable] public class Idle : PlayerState
    {
        public override void Update(PlayerStateMachine stateMachine)
        {
            base.Update(stateMachine);
 
            if (stateMachine.input.HasInput() && stateMachine.input.MineHeld)
            {
                stateMachine.ChangeState("mining");
                return;
            }
 			
            if (stateMachine.input.HasInput())
            {
                stateMachine.ChangeState("walking");
                return;
            }
 
            if (stateMachine.input.JumpHeld && stateMachine.player.IsGrounded())
            {
                stateMachine.ChangeState("jumping");
                return;
            }
        }
    }
    [Serializable] public class Walking : PlayerState
    {
        private Vector2 _movement;
 
        public override void Update(PlayerStateMachine stateMachine)
        {
            base.Update(stateMachine);
 			
            if (stateMachine.input.MineHeld)
            {
                stateMachine.ChangeState("mining");
                return;
            }
 			
            if (!stateMachine.input.HasInput())
            {
                stateMachine.ChangeState("idle");
                return;
            }
 			
            if (stateMachine.input.JumpHeld && stateMachine.player.IsGrounded())
            {
                stateMachine.ChangeState("jumping");
                return;
            }
 
            stateMachine.renderer.flipX = Mathf.Sign(stateMachine.input.InputVector.x) < 0;
        }
 
        public override void FixedUpdate(PlayerStateMachine stateMachine)
        {
            base.FixedUpdate(stateMachine);
            _movement.x = stateMachine.input.InputVector.x * Time.fixedDeltaTime * stateMachine.player.walkSpeed;
            stateMachine.body.position += _movement;
        }
    }
    [Serializable] public class Jumping : PlayerState
    {
        private Vector2 _movement;
        public override void Enter(PlayerStateMachine stateMachine)
        {
            stateMachine.body.AddForce(Vector2.up * (stateMachine.body.mass * stateMachine.player.jumpForce), ForceMode2D.Impulse);
        }
 
        public override void Update(PlayerStateMachine stateMachine)
        {
            base.Update(stateMachine);
 
            if (IsOnLastFrame() && stateMachine.player.IsGrounded())
            {
                stateMachine.ChangeState(stateMachine.input.HasInput() ? "walking" : "idle");
            }
        }
 
        public override void FixedUpdate(PlayerStateMachine stateMachine)
        {
            base.FixedUpdate(stateMachine);
 			
            _movement.x = stateMachine.input.InputVector.x * Time.fixedDeltaTime * stateMachine.player.airSpeed;
            stateMachine.body.position += _movement;
        }
    }
    [Serializable] public class Mining : PlayerState
    {
        private bool _hasSwung;
 
        public override void Enter(PlayerStateMachine stateMachine)
        {
            base.Enter(stateMachine);
 
            CrossHair.Instance.SetVisible(true);
        }
 
        public override void Exit(PlayerStateMachine stateMachine)
        {
            base.Exit(stateMachine);
 
            CrossHair.Instance.SetVisible(false);
        }
 
        public override void Update(PlayerStateMachine stateMachine)
        {
            base.Update(stateMachine);
 
            if (!stateMachine.input.MineHeld)
            {
                stateMachine.ChangeState(stateMachine.input.HasInput() ? "walking" : "idle");
                return;
            }
 
            if (!stateMachine.input.HasInput())
            {
                stateMachine.ChangeState("idle");
                return;
            }
 
            stateMachine.renderer.flipX = Mathf.Sign(stateMachine.input.InputVector.x) < 0;
 
            var hasRoom = BucketRenderer.Instance.fill <= 100;
            var position = stateMachine.player.transform.position;
            var hit = Physics2D.Raycast(position, stateMachine.input.RawInputVector, stateMachine.player.digRange, stateMachine.player.mineMask);
            var targetPos = ((Vector2) position) + stateMachine.input.RawInputVector.normalized * (stateMachine.player.digRange - 0.1f);
 
            if (hit)
            {
                CrossHair.Instance.transform.position = hit.point + stateMachine.input.RawInputVector.normalized * 0.2f;
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
                var pos = hit.point + (stateMachine.input.InputVector * 0.1f);
 
                if (hasRoom && Map.Instance.TryBreakTile(pos, stateMachine.player.digPower, out var tile) &&
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