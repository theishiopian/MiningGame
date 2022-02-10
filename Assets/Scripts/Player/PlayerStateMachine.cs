using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public readonly SpriteRenderer renderer;
    public readonly InputHandler input;
    public readonly Player player;
    public readonly Rigidbody2D body;
        
    private PlayerState _state;
    private readonly Dictionary<string, PlayerState> _states = new Dictionary<string, PlayerState>();
    public PlayerStateMachine(PlayerState startingState, Player player, InputHandler input, SpriteRenderer renderer, Rigidbody2D body)
    {
        _state = startingState;
        _state.Enter(this);
            
        this.renderer = renderer;
        this.input = input;
        this.player = player;
        this.body = body;
    }

    public void RegisterState(string key, PlayerState value)
    {
        _states.Add(key, value);
    }

    public void Update()
    {
        _state.Update(this);
    }
        
    public void FixedUpdate()
    {
        _state.FixedUpdate(this);
    }

    public void ChangeState(string newState)
    {
        _state.Exit(this);
        _state = _states[newState];
        _state.Enter(this);
    }
}