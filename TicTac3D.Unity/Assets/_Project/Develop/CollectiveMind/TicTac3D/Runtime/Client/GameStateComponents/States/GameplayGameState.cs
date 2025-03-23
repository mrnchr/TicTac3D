using CollectiveMind.TicTac3D.Runtime.Client.UI;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public class GameplayGameState : IGameState
  {
    private readonly IWindowManager _windowManager;

    public GameplayGameState(IWindowManager windowManager)
    {
      _windowManager = windowManager;
    }
    
    public async UniTask Enter()
    {
      await _windowManager.OpenWindow<HudWindow>();
    }

    public UniTask Exit()
    {
      return UniTask.CompletedTask;
    }
  }
}