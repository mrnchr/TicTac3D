﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public class GameStateMachine : IGameStateMachine
  {
    private readonly Dictionary<Type, IExitableState> _registeredStates = new Dictionary<Type, IExitableState>();
    private IExitableState _currentState;

    public ReadOnlyReactiveProperty<IExitableState> CurrentState { get; }

    public GameStateMachine()
    {
      CurrentState = Observable.EveryValueChanged(this, x => x._currentState).ToReadOnlyReactiveProperty();
    }

    public void RegisterState<TState>(TState state) where TState : IExitableState
    {
      Type stateType = typeof(TState);

      _registeredStates.TryAdd(stateType, state);
    }

    public async UniTask SwitchState<TState>() where TState : class, IGameState
    {
      var nextState = await GetNextStateWithSetCurrentState<TState>();
      await nextState.Enter();
    }
    
    public async UniTask SwitchState<TState, TPayload>(TPayload payload) where TState : class, IPaylodedState<TPayload>
    {
      var nextState = await GetNextStateWithSetCurrentState<TState>();
      await nextState.Enter(payload);
    }

    private async UniTask<TState> GetNextStateWithSetCurrentState<TState>() where TState : class, IExitableState
    {
      var nextState = GetState<TState>();

      if (_currentState != null)
        await _currentState.Exit();

      _currentState = nextState;

      return nextState;
    }

    private TState GetState<TState>() where TState : class, IExitableState
    {
      Type stateType = typeof(TState);

      if (_registeredStates.ContainsKey(stateType) == false)
        throw new Exception($"The condition with type {stateType} is not registered");

      return _registeredStates[stateType] as TState;
    }
  }
}