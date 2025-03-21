using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
{
  public class LobbySettingsWindow : BaseWindow
  {
    [SerializeField]
    private Button _searchGameButton;

    [SerializeField]
    private Button _backButton;

    private GameRulesProvider _rulesProvider;
    private IWindowManager _windowManager;

    public GameRules Rules => _rulesProvider.Rules;

    [Inject]
    public void Construct(GameRulesProvider gameRulesProvider, IWindowManager windowManager)
    {
      _rulesProvider = gameRulesProvider;
      _windowManager = windowManager;
      
      _searchGameButton.AddListener(SearchGame);
      _backButton.AddListener(CloseWindow);
    }

    private void SearchGame()
    {
      _windowManager.OpenWindow<SearchGameWindow>();
    }

    private void CloseWindow()
    {
      _windowManager.CloseWindow<LobbySettingsWindow>().Forget();
    }

    private void OnDestroy()
    {
      _searchGameButton.RemoveListener(SearchGame);
      _backButton.RemoveListener(CloseWindow);
    }
  }
}