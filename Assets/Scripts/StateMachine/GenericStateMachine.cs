using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class GenericStateMachine<T> where T : MonoBehaviour
    {
        private IGenericState<T> _state;
        private readonly T _owner;
        private readonly Dictionary<string, IGenericState<T>> _states = new Dictionary<string, IGenericState<T>>();
        
        public GenericStateMachine(IGenericState<T> startingState, T owner)
        {
            _owner = owner;
            _state = startingState;
            _state.Enter(owner);
        }

        public void RegisterState(string key, IGenericState<T> value)
        {
            _states.Add(key, value);
        }

        public void Update()
        {
            _state.Update(_owner);
        }
        
        public void FixedUpdate()
        {
            _state.FixedUpdate(_owner);
        }

        public void ChangeState(string newState)
        {
            _state.Exit(_owner);
            _state = _states[newState];
            _state.Enter(_owner);
        }
    }
}