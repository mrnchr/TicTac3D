using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
{
  public class SettingsWindow : BaseWindow
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
      _windowManager.CloseWindow<SettingsWindow>();
    }

    private void OnDestroy()
    {
      _backButton.RemoveListener(CloseWindow);
    }
  }
}