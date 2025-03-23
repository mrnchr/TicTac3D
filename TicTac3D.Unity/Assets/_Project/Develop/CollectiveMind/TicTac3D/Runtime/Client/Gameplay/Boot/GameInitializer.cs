using CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class GameInitializer : IInitializable
  {
    private readonly IGameplayTickableManager _gameplayTickableManager;
    private readonly IGameStateMachine _gameStateMachine;

    public GameInitializer(IGameplayTickableManager gameplayTickableManager, IGameStateMachine gameStateMachine)
    {
      _gameplayTickableManager = gameplayTickableManager;
      _gameStateMachine = gameStateMachine;
    }
    
    public void Initialize()
    {
      _gameplayTickableManager.IsPaused = true;
      _gameStateMachine.SwitchState<MenuGameState>().Forget();
    }
  }
}