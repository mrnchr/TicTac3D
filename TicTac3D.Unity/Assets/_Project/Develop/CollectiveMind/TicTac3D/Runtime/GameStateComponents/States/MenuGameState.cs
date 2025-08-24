using CollectiveMind.TicTac3D.Runtime.UI;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public class MenuGameState : IGameState
  {
    private readonly IWindowManager _windowManager;
    private readonly MainMenu _mainMenu;

    public MenuGameState(IWindowManager windowManager)
    {
      _windowManager = windowManager;
      _mainMenu = Object.FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);
    }
    
    public async UniTask Enter()
    {
      _mainMenu.gameObject.SetActive(true);
      await _windowManager.OpenWindowAsRoot<MenuWindow>();
    }

    public async UniTask Exit()
    {
      _mainMenu.gameObject.SetActive(false);
      await _windowManager.CloseWindowsBy<MenuWindow>();
    }
  }
}