using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using StateMachine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerController : AbstractCharacter
    {
        #region SINGLETON

        //This code ensure only one instance of this class exists, and provides a static component reference to that instance.
        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                Debug.LogError("Duplicate Singleton: " + gameObject.name + "! Removing...");
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        #endregion

        #region INSPECTOR
        [Header("State")] 
        [SerializeField] private Idle idleState;
        [SerializeField] private Walking walkState;
        [SerializeField] private Jumping jumpState;
        [SerializeField] private Mining mineState;

        [Header("Movement")] 
        public float walkSpeed = 3;
        public float airSpeed = 1;
        public float climbSpeed = 2;
        public float jumpForce = 15;

        [Header("Mining")] 
        public float digRange = 1;
        public int digPower = 1;
        public LayerMask mineMask;

        [Header("UI")] 
        public BucketRenderer bucket;
        public Text fortuneText;
        public Text gloryText;
        
        #endregion

        public readonly Dictionary<ResourceTile, int> minerals = new Dictionary<ResourceTile, int>();

        public GenericStateMachine<PlayerController> StateMachine { get; private set; }

        [HideInInspector] public int fortune;
        [HideInInspector] public int glory;

        protected override void Start()
        {
            base.Start();

            StateMachine = new GenericStateMachine<PlayerController>(idleState, this);

            StateMachine.RegisterState("idle", idleState);
            StateMachine.RegisterState("walking", walkState);
            StateMachine.RegisterState("jumping", jumpState);
            StateMachine.RegisterState("mining", mineState);
        }

        private void Update()
        {
            StateMachine.Update();

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                SellMinerals();
            }
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        /**
         * Helper method that lets you just add a resource to the bag without explicitly checking if it is present first.
         * Largely just for convenience.
         */
        public void AddMinerals(ResourceTile mineral)
        {
            if (bucket.fill < 100)
            {
                bucket.fill += 1;
                
                if (minerals.ContainsKey(mineral))
                {
                    minerals[mineral] += 1;
                }
                else
                {
                    minerals.Add(mineral, 1);
                }
            }
            
        }

        /**
         * Helper method used for selling minerals.
         */
        public int SellMinerals()
        {
            var value = minerals.Sum(mineral => mineral.Key.value * mineral.Value);

            foreach (var key in minerals.Keys.ToList())
            {
                fortune += key.value * minerals[key];
                minerals[key] = 0;
            }

            bucket.fill = 0;
            fortuneText.text = fortune.ToString();

            return value;
        }
    }
    
    [Serializable]
    internal class Idle : AnimatedState<PlayerController>
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
    internal class Walking : AnimatedState<PlayerController>
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
    internal class Jumping : AnimatedState<PlayerController>
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

    internal class Building : AnimatedState<PlayerController>
    {
        
    }
    
    internal class Climbing : AnimatedState<PlayerController>
    {
        private Vector2 _movement;

        public override void Enter(PlayerController owner)
        {
            base.Enter(owner);
        }

        public override void Exit(PlayerController owner)
        {
            base.Exit(owner);
        }

        public override void FixedUpdate(PlayerController owner)
        {
            base.FixedUpdate(owner);

            _movement = InputHandler.Instance.InputVector * Time.fixedDeltaTime * owner.climbSpeed;
        }
    }

    [Serializable]
    internal class Mining : AnimatedState<PlayerController>
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

            var hasRoom = owner.bucket.fill < 100;
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
                    owner.bucket.AddMineralLayerIfNeeded(brokenTile);
                }

                _hasSwung = true;
            }
            else
            {
                _hasSwung = false;
            }
        }
    }
}