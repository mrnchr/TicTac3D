using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public interface IGameStateMachine
  {
    IExitableState CurrentState { get; }
    void RegisterState<TState>(TState state) where TState : IExitableState;
    UniTask SwitchState<TState>() where TState : class, IGameState;
  }
}