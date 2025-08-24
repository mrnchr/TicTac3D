namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public interface IGameStateFactory
  {
    TState Create<TState>() where TState : IGameState;
  }
}