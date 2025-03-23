using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public interface IGameStateMachine
  {
    void RegisterState<TState>(TState state) where TState : IExitableState;
    UniTask SwitchState<TState>() where TState : class, IGameState;
  }
}