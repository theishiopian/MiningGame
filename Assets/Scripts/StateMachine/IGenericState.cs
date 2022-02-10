using UnityEngine;

namespace StateMachine
{
    public interface IGenericState<T> where T : MonoBehaviour
    {
        void Enter(T owner);
        void Exit(T owner);
        void Update(T owner);
        void FixedUpdate(T owner);
    }
}