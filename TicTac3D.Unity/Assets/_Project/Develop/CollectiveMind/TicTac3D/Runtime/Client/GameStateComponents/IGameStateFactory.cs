namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public interface IGameStateFactory
  {
    TState Create<TState>() where TState : IGameState;
  }
}