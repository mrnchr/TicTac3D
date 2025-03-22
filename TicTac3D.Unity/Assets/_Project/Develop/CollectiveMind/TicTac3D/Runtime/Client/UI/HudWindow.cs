using CollectiveMind.TicTac3D.Runtime.Client.UI.Settings;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
{
  public class HudWindow : BaseWindow
  {
    [SerializeField]
    private Button _settingsButton;

    private IWindowManager _windowManager;

    [Inject]
    public void Construct(IWindowManager windowManager)
    {
      _windowManager = windowManager;
      _settingsButton.AddListener(OpenPauseWindow);
    }

    private void OpenPauseWindow()
    {
      _windowManager.OpenWindow<SettingsWindow>();
    }

    private void OnDestroy()
    {
      _settingsButton.RemoveListener(OpenPauseWindow);
    }
  }
}