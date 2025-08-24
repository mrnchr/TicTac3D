using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.UI;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public class GameplayGameState : IGameState
  {
    private readonly IWindowManager _windowManager;
    private readonly IGameplayTickableManager _gameplayTickableManager;

    public GameplayGameState(IWindowManager windowManager,
      IGameplayTickableManager gameplayTickableManager)
    {
      _windowManager = windowManager;
      _gameplayTickableManager = gameplayTickableManager;
    }
    
    public async UniTask Enter()
    {
      await _windowManager.OpenWindowAsRoot<HudWindow>();
      _gameplayTickableManager.IsPaused = false;
    }

    public UniTask Exit()
    {
      return UniTask.CompletedTask;
    }
  }
}