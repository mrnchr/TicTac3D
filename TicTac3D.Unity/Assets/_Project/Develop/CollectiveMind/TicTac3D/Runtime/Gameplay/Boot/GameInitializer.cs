using CollectiveMind.TicTac3D.Runtime.Boot;
using CollectiveMind.TicTac3D.Runtime.GameStateComponents;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class GameInitializer : IInitializable
  {
    private readonly IWindowManager _windowManager;
    private readonly GameStateMachineInitializer _gameStateMachineInitializer;
    private readonly IGameplayTickableManager _gameplayTickableManager;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly ProjectInitializer _projectInitializer;

    public GameInitializer(IWindowManager windowManager,
      GameStateMachineInitializer gameStateMachineInitializer,
      IGameplayTickableManager gameplayTickableManager,
      IGameStateMachine gameStateMachine,
      ProjectInitializer projectInitializer)
    {
      _windowManager = windowManager;
      _gameStateMachineInitializer = gameStateMachineInitializer;
      _gameplayTickableManager = gameplayTickableManager;
      _gameStateMachine = gameStateMachine;
      _projectInitializer = projectInitializer;
    }

    public async void Initialize()
    {
      await UniTask.WaitUntil(() => _projectInitializer.IsInitialized);
      
      BaseWindow[] windows =
        Object.FindObjectsByType<BaseWindow>(FindObjectsInactive.Include, FindObjectsSortMode.None);
      foreach (BaseWindow window in windows)
        _windowManager.AddWindow(window);

      _gameStateMachineInitializer.Initialize();
      
      _gameplayTickableManager.IsPaused = true;
      _gameStateMachine.SwitchState<MenuGameState>().Forget();
    }
  }
}