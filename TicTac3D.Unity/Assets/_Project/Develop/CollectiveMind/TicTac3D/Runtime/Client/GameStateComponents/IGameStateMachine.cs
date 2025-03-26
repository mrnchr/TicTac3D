using Cysharp.Threading.Tasks;
using R3;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public interface IGameStateMachine
  {
    ReadOnlyReactiveProperty<IExitableState> CurrentState { get; }
    void RegisterState<TState>(TState state) where TState : IExitableState;
    UniTask SwitchState<TState>() where TState : class, IGameState;
    UniTask SwitchState<TState, TPayload>(TPayload payload) where TState : class, IPaylodedState<TPayload>;
  }
}