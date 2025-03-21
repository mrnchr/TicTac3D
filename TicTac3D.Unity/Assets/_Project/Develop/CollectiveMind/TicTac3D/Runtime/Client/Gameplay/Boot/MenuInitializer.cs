using CollectiveMind.TicTac3D.Runtime.Client.UI;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class MenuInitializer : IInitializable
  {
    private readonly IWindowManager _windowManager;
    private readonly MainMenu _mainMenu;

    public MenuInitializer(IWindowManager windowManager)
    {
      _windowManager = windowManager;
      _mainMenu = Object.FindAnyObjectByType<MainMenu>(FindObjectsInactive.Include);
    }
    
    public void Initialize()
    {
      OpenMenu();
    }

    public void OpenMenu()
    {
      _mainMenu.gameObject.SetActive(true);
      _windowManager.OpenWindow<MenuWindow>();
    }
  }
}