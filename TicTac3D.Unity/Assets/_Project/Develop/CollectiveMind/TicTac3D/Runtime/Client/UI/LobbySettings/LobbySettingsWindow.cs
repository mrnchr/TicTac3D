using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.LobbyManagement;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.LobbySettings
{
  public class LobbySettingsWindow : BaseWindow
  {
    [SerializeField]
    private Button _searchGameButton;

    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private Sprite _activeButtonSprite;

    [SerializeField]
    private Sprite _inactiveButtonSprite;

    private GameRulesProvider _rulesProvider;
    private IWindowManager _windowManager;
    private IConfigLoader _configLoader;
    private GameConfig _config;
    private List<RuleButton> _ruleButtons;
    private LobbyManager _lobbyManager;

    public GameRules Rules => _rulesProvider.Rules;

    [Inject]
    public void Construct(GameRulesProvider gameRulesProvider,
      IWindowManager windowManager,
      IConfigLoader configLoader,
      LobbyManager lobbyManager)
    {
      _rulesProvider = gameRulesProvider;
      _windowManager = windowManager;
      _configLoader = configLoader;
      _lobbyManager = lobbyManager;
      _config = configLoader.LoadConfig<GameConfig>();

      _ruleButtons = GetComponentsInChildren<RuleButton>(true).ToList();

      _searchGameButton.AddListener(SearchGame);
      _backButton.AddListener(CloseWindow);
    }

    private void Start()
    {
      Rules.Data = _config.DefaultRules;
      
      foreach (RuleButton button in _ruleButtons)
      {
        button.SetSprite(button.IsEqualRule(Rules) ? _activeButtonSprite : _inactiveButtonSprite);
      }
    }

    public void SetRule<T>(RuleButton sender, T value)
    {
      Rules.SetRule(sender.RuleType, value);

      foreach (RuleButton button in _ruleButtons
        .Where(x => x.RuleType == sender.RuleType))
      {
        button.SetSprite(_inactiveButtonSprite);
      }

      sender.SetSprite(_activeButtonSprite);
    }

    private async void SearchGame()
    {
      await _windowManager.OpenWindow<SearchGameWindow>();
      _lobbyManager.InitializeLobby(Rules.Data).Forget();
    }

    private void CloseWindow()
    {
      _windowManager.CloseWindow<LobbySettingsWindow>().Forget();
    }

    private void OnDestroy()
    {
      _searchGameButton.RemoveListener(SearchGame);
      _backButton.RemoveListener(CloseWindow);
      _configLoader.UnloadConfig<GameConfig>();
    }
  }
}