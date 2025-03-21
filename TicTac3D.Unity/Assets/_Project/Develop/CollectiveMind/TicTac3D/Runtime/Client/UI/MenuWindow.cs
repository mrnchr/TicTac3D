using CollectiveMind.TicTac3D.Runtime.Client.UI.LobbySettings;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
{
  public class MenuWindow : BaseWindow
  {
    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private Button _settingsButton;

    private IWindowManager _windowManager;

    [Inject]
    public void Construct(IWindowManager windowManager)
    {
      _windowManager = windowManager;
      
      _playButton.AddListener(OpenLobbySettings);
      _settingsButton.AddListener(OpenSettingsWindow);
    }

    private void OpenLobbySettings()
    {
      _windowManager.OpenWindow<LobbySettingsWindow>().Forget();
    }

    private void OpenSettingsWindow()
    {
      _windowManager.OpenWindow<SettingsWindow>().Forget();
    }

    private void OnDestroy()
    {
      _playButton.RemoveListener(OpenLobbySettings);
      _settingsButton.RemoveListener(OpenSettingsWindow);
    }
  }
}