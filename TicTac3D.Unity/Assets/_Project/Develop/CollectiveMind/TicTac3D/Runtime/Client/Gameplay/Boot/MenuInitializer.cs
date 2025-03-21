using CollectiveMind.TicTac3D.Runtime.Client.UI;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class MenuInitializer : IInitializable
  {
    private readonly IWindowManager _windowManager;

    public MenuInitializer(IWindowManager windowManager)
    {
      _windowManager = windowManager;
    }
    
    public void Initialize()
    {
      _windowManager.OpenWindow<MenuWindow>();
    }
  }
}