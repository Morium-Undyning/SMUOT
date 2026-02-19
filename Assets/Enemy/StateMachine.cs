using System;
using System.Collections.Generic;

public class StateMachine
{
    private IState _currentState;
    private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

    public void AddState<T>(T state) where T : IState
    {
        _states[typeof(T)] = state;
    }

    public void ChangeStateDirect(IState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    
    public void ChangeState<T>(params object[] args) where T : class, IState
    {
        if (!_states.ContainsKey(typeof(T)))
        {
            T newState = (T)Activator.CreateInstance(typeof(T), args);
            ChangeStateDirect(newState);
        }
        else
        {
            T state = _states[typeof(T)] as T;
            ChangeStateDirect(state);
        }
    }

    public void Update()
    {
        _currentState?.Update();
    }

    public Type GetCurrentStateType()
    {
        return _currentState?.GetType();
    }
}
