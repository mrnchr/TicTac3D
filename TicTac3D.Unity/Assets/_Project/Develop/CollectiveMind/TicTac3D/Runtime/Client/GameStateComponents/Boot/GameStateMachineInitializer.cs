using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public class GameStateMachineInitializer : IInitializable
  {
    private readonly IGameStateFactory _gameStateFactory;
    private readonly IGameStateMachine _gameStateMachine;

    public GameStateMachineInitializer(IGameStateFactory gameStateFactory, IGameStateMachine gameStateMachine)
    {
      _gameStateFactory = gameStateFactory;
      _gameStateMachine = gameStateMachine;
    }
    
    public void Initialize()
    {
      _gameStateMachine.RegisterState(_gameStateFactory.Create<MenuGameState>());
      _gameStateMachine.RegisterState(_gameStateFactory.Create<GameplayGameState>());
      _gameStateMachine.RegisterState(_gameStateFactory.Create<EndGameState>());
    }
  }
}