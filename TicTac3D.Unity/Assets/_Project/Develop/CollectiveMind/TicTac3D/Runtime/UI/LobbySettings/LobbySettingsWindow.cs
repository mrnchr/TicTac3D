using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.LobbyManagement;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class LobbySettingsWindow : BaseWindow
  {
    [SerializeField]
    private Toggle _isPrivateGameToggle;

    [SerializeField]
    private TMP_InputField _joinCodeField;

    [SerializeField]
    private Button _searchGameButton;

    [SerializeField]
    private Button _backButton;

    public Sprite ActiveButtonSprite;
    public Sprite InactiveButtonSprite;

    private GameRulesProvider _rulesProvider;
    private IWindowManager _windowManager;
    private IConfigLoader _configLoader;
    private GameConfig _gameConfig;
    private List<RuleDropdown> _ruleButtons;
    private LobbyManager _lobbyManager;
    private List<FadingCountHolder> _holders;

    private GameRules Rules => _rulesProvider.Rules;

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
      _gameConfig = configLoader.LoadConfig<GameConfig>();

      _ruleButtons = GetComponentsInChildren<RuleDropdown>(true).ToList();
      _holders = GetComponentsInChildren<FadingCountHolder>(true).ToList();

      _isPrivateGameToggle.onValueChanged.AddListener(SwitchPrivateGame);
      _isPrivateGameToggle.onValueChanged.AddListener(ChangeSearchButtonText);
      _joinCodeField.onValueChanged.AddListener(ChangeSearchButtonText);
      _searchGameButton.AddListener(SearchGame);
      _backButton.AddListener(CloseWindow);
    }

    private void Start()
    {
      Rules.Data = _gameConfig.DefaultRules;

      foreach (RuleDropdown button in _ruleButtons)
        button.OnUpdateRule(Rules.Data);

      foreach (FadingCountHolder holder in _holders)
        holder.OnUpdateRule(Rules.Data);

      SwitchPrivateGame(_isPrivateGameToggle.isOn);
      ChangeSearchButtonText();
    }

    private void SwitchPrivateGame(bool value)
    {
      _joinCodeField.gameObject.SetActive(value);
    }

    private void ChangeSearchButtonText(bool _)
    {
      ChangeSearchButtonText();
    }

    private void ChangeSearchButtonText(string _)
    {
      ChangeSearchButtonText();
    }

    private void ChangeSearchButtonText()
    {
      // _searchGameButton.GetComponentInChildren<LocalizeStringEvent>().StringReference =
      //   _isPrivateGameToggle.isOn
      //     ? string.IsNullOrWhiteSpace(_joinCodeField.text) ? _config.CreateLobbyText : _config.JoinLobbyText
      //     : _config.SearchText;
    }

    public void SetRule<T>(RuleDropdown sender, T value)
    {
      Rules.SetRule(sender.RuleType, value);

      foreach (RuleDropdown button in _ruleButtons)
        button.OnUpdateRule(Rules.Data);

      foreach (FadingCountHolder holder in _holders)
        holder.OnUpdateRule(Rules.Data);
    }

    private async void SearchGame()
    {
      if (_isPrivateGameToggle.isOn)
      {
        string joinCode = _joinCodeField.text;
        if (string.IsNullOrWhiteSpace(joinCode))
          _lobbyManager.CreateLobby().Forget();
        else
          _lobbyManager.JoinLobby(joinCode).Forget();
      }
      else
      {
        _lobbyManager.SearchFreeLobby().Forget();
      }

      await _windowManager.OpenWindow<SearchGameWindow>();
    }

    private void CloseWindow()
    {
      _windowManager.CloseWindow<LobbySettingsWindow>().Forget();
    }

    private void OnDestroy()
    {
      _isPrivateGameToggle.onValueChanged.RemoveListener(SwitchPrivateGame);
      _isPrivateGameToggle.onValueChanged.RemoveListener(ChangeSearchButtonText);
      _joinCodeField.onValueChanged.RemoveListener(ChangeSearchButtonText);
      _searchGameButton.RemoveListener(SearchGame);
      _backButton.RemoveListener(CloseWindow);
      _configLoader.UnloadConfig<GameConfig>();
    }
  }
}