namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public class GameStateMachineInitializer
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