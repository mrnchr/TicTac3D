using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.WindowManagement
{
  public class WindowInitializer : IInitializable
  {
    private readonly IWindowManager _windowManager;
    private readonly BaseWindow[] _windows;

    public WindowInitializer(IWindowManager windowManager)
    {
      _windowManager = windowManager;
      _windows = Object.FindObjectsByType<BaseWindow>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }
    
    public void Initialize()
    {
      foreach (BaseWindow window in _windows)
      {
        _windowManager.AddWindow(window);
      }
    }
  }
}