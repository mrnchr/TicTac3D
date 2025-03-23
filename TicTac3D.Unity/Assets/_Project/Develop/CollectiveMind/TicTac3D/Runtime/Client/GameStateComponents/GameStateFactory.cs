using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public class GameStateFactory : IGameStateFactory
  {
    private readonly IInstantiator _instantiator;

    public GameStateFactory(IInstantiator instantiator)
    {
      _instantiator = instantiator;
    }

    public TState Create<TState>() where TState : IGameState
    {
      return _instantiator.Instantiate<TState>();
    }
  }
}