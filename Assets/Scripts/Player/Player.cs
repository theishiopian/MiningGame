using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region SINGLETON
    //This code ensure only one instance of this class exists, and provides a static component reference to that instance.
    public static Player Instance
    {
        get;
        private set;
    }
 	
    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Duplicate Singleton: "+gameObject.name+"! Removing...");
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
    [SerializeField]private PlayerLogic.Idle idleState;
    [SerializeField]private PlayerLogic.Walking walkState;
    [SerializeField]private PlayerLogic.Jumping jumpState;
    [SerializeField]private PlayerLogic.Mining mineState;
 
    [Header("Movement")]
    public float walkSpeed = 3;
    public float airSpeed = 1;
    public float jumpForce = 15;
 
    [Header("Ground Check")]
    public LayerMask groundMask;
    public float castDist = 1;
 
    [Header("Mining")] 
    public float digRange = 1;
    public int digPower = 1;
    public LayerMask mineMask;
    #endregion
    
    public readonly Dictionary<ResourceTile, int> minerals = new Dictionary<ResourceTile, int>();
    
    private SpriteRenderer _render;
    private InputHandler _input;
    private CapsuleCollider2D _collider;
    private Rigidbody2D _body;
    private PlayerStateMachine _stateMachine;
 
    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _input = GetComponent<InputHandler>();
        _render = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CapsuleCollider2D>();

        _stateMachine = new PlayerStateMachine(idleState, this, _input, _render, _body);
 		
        _stateMachine.RegisterState("idle", idleState);
        _stateMachine.RegisterState("walking", walkState);
        _stateMachine.RegisterState("jumping", jumpState);
        _stateMachine.RegisterState("mining", mineState);
    }
 	
    /*
     * All player behaviour is handled in PlayerLogic.cs for now.
     * This design pattern may prove difficult to adapt to entities, but it makes things cleaner.
     * In the future, player logic may be moved back to the player class.
     */
    
    private void Update()
    {
        _stateMachine.Update();

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            SellMinerals();
        }
    }
 	
    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }
 
    public bool IsGrounded()
    {
        return Physics2D.CapsuleCast(transform.position, _collider.size, _collider.direction, 0, Vector2.down, castDist, groundMask);
    }
 	
    /**
     * Helper method that lets you just add a resource to the bag without checking if it is present first.
     * Largely just for convenience.
     */
    public void AddMinerals(ResourceTile mineral)
    {
        if (minerals.ContainsKey(mineral))
        {
            minerals[mineral] += 1;
        }
        else
        {
            minerals.Add(mineral, 1);
        }
    }

    public int SellMinerals()
    {
        var value = minerals.Sum(mineral => mineral.Key.value * mineral.Value);

        foreach (var key in minerals.Keys.ToList())
        {
            minerals[key] = 0;
        }

        BucketRenderer.Instance.fill = 0;
        
        return value;
    }
}