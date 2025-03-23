using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.UI.SetShape;
using CollectiveMind.TicTac3D.Runtime.Client.UI.Settings;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using Cysharp.Threading.Tasks;
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
    private IGameplayTickableManager _gameplayTickableManager;
    private ConfirmationPopup _confirmationPopup;

    [Inject]
    public void Construct(IWindowManager windowManager, IGameplayTickableManager gameplayTickableManager)
    {
      _windowManager = windowManager;
      _gameplayTickableManager = gameplayTickableManager;
      _settingsButton.AddListener(OpenPauseWindow);

      _confirmationPopup = GetComponentInChildren<ConfirmationPopup>(true);
    }

    protected override UniTask OnInvisible()
    {
      if(_confirmationPopup.gameObject.activeSelf)
        _confirmationPopup.Deny(false);
      
      return base.OnInvisible();
    }

    private void OpenPauseWindow()
    {
      _gameplayTickableManager.IsPaused = true;
      _windowManager.OpenWindow<SettingsWindow>();
    }

    private void OnDestroy()
    {
      _settingsButton.RemoveListener(OpenPauseWindow);
    }
  }
}