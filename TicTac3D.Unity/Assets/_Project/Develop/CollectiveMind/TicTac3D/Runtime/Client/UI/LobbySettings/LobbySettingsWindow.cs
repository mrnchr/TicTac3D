using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
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
    private IRpcProvider _rpcProvider;
    private IConfigLoader _configLoader;
    private GameConfig _config;
    private List<RuleButton> _ruleButtons;

    private GameRules Rules => _rulesProvider.Rules;

    [Inject]
    public void Construct(GameRulesProvider gameRulesProvider,
      IWindowManager windowManager,
      IRpcProvider rpcProvider,
      IConfigLoader configLoader)
    {
      _rulesProvider = gameRulesProvider;
      _windowManager = windowManager;
      _rpcProvider = rpcProvider;
      _configLoader = configLoader;
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

    private void SearchGame()
    {
      _rpcProvider.SendRequest(new SearchGameRequest { Rules = Rules.Data });
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
      _configLoader.UnloadConfig<GameConfig>();
    }
  }
}