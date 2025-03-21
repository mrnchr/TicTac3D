using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
{
  public class SearchGameWindow : BaseWindow
  {
    [SerializeField]
    private Button _backButton;

    private IWindowManager _windowManager;

    [Inject]
    public void Construct(IWindowManager windowManager)
    {
      _windowManager = windowManager;
      
      _backButton.AddListener(CloseWindow);
    }

    private void CloseWindow()
    {
      _windowManager.CloseWindow<SearchGameWindow>();
    }
  }
}