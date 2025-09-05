using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class RuleDropdown : MonoBehaviour
  {
    public GameRuleType RuleType;

    [SerializeField]
    [ListDrawerSettings(ShowElementLabels = true)]
    private List<LocalizedString> _optionNames;

    [SerializeField]
    [ShowIf(nameof(IsDesiredShape))]
    private List<ShapeType> _shapes;

    [SerializeField]
    [ShowIf(nameof(IsBotMoveCount))]
    private List<int> _botMoveCounts;

    [SerializeField]
    [ShowIf(nameof(IsMoveTime))]
    private List<float> _times;

    [SerializeField]
    [ShowIf(nameof(IsShapeFading))]
    private List<ShapeFadingType> _shapeFadings;

    [SerializeField]
    [ShowIf(nameof(IsBotFadingMoveCount))]
    private List<int> _botFadingMoveCounts;

    [SerializeField]
    [ShowIf(nameof(IsPlayerFadingMoveCount))]
    private List<int> _playerFadingMoveCounts;

    private bool IsDesiredShape => RuleType == GameRuleType.DesiredShape;
    private bool IsBotMoveCount => RuleType == GameRuleType.BotMoveCount;
    private bool IsMoveTime => RuleType == GameRuleType.MoveTime;
    private bool IsShapeFading => RuleType == GameRuleType.ShapeFading;
    private bool IsBotFadingMoveCount => RuleType == GameRuleType.BotFadingMoveCount;
    private bool IsPlayerFadingMoveCount => RuleType == GameRuleType.PlayerFadingMoveCount;

    private LobbySettingsWindow _lobbySettingsWindow;
    private CustomDropdown _dropdown;
    private GameRulesProvider _gameRulesProvider;
    private int Index => _dropdown.value;
    
    private GameRulesData GameRules => _gameRulesProvider.Rules.Data;

    [Inject]
    public void Construct(GameRulesProvider gameRulesProvider)
    {
      _gameRulesProvider = gameRulesProvider;
      _lobbySettingsWindow = GetComponentInParent<LobbySettingsWindow>(true);

      _dropdown = GetComponent<CustomDropdown>();
      _dropdown.onValueChanged.AddListener(OnValueChanged);
      _dropdown.OnDropdownShowed += ToggleBotOptions;
    }

    private void Start()
    {
      _dropdown.options.Clear();
      foreach (LocalizedString lString in _optionNames)
      {
        _dropdown.options.Add(new TMP_Dropdown.OptionData(lString.GetLocalizedString()));
        lString.StringChanged += x => UpdateOption(lString, x);
      }

      _dropdown.RefreshShownValue();
    }

    private void UpdateOption(LocalizedString lString, string value)
    {
      int index = _optionNames.IndexOf(lString);
      _dropdown.options[index].text = value;
      _dropdown.RefreshShownValue();
    }

    private void ToggleBotOptions(RectTransform dropdownList)
    {
      if (RuleType != GameRuleType.BotMoveCount)
        return;

      if (GameRules.ShapeFading.IsPlayersOrRandom())
        return;
      
      var indices = new List<int>
      {
        _botMoveCounts.IndexOf(0),
        _botMoveCounts.IndexOf(1)
      };
      
      foreach (int index in indices)
      {
        string optionName = _dropdown.options[index].text;
        _dropdown.GetDropdownItemToggle(optionName).interactable = false;
      }
    }

    private void OnDestroy()
    {
      _dropdown.onValueChanged.RemoveListener(OnValueChanged);
      _dropdown.OnDropdownShowed -= ToggleBotOptions;
    }

    private void OnValueChanged(int index)
    {
      switch (RuleType)
      {
        case GameRuleType.DesiredShape:
          SetRule(_shapes[index]);
          break;
        case GameRuleType.BotMoveCount:
          SetRule(_botMoveCounts[index]);
          break;
        case GameRuleType.MoveTime:
          SetRule(_times[index]);
          break;
        case GameRuleType.ShapeFading:
          SetRule(_shapeFadings[index]);
          break;
        case GameRuleType.BotFadingMoveCount:
          SetRule(_botFadingMoveCounts[index]);
          break;
        case GameRuleType.PlayerFadingMoveCount:
          SetRule(_playerFadingMoveCounts[index]);
          break;
        default:
          throw new ArgumentException($"Invalid rule type {RuleType}");
      }
    }

    private void SetRule<T>(T value)
    {
      _lobbySettingsWindow.SetRule(this, value);
    }

    public void OnUpdateRule(GameRulesData rules)
    {
      if (!IsEqualRule(rules))
      {
        _dropdown.SetValueWithoutNotify(GetIndex(rules));
        _dropdown.RefreshShownValue();
      }
    }

    private bool IsEqualRule(GameRulesData rules)
    {
      return RuleType switch
      {
        GameRuleType.DesiredShape => _shapes[Index] == rules.DesiredShape,
        GameRuleType.BotMoveCount => _botMoveCounts[Index] == rules.BotMoveCount,
        GameRuleType.MoveTime => _times[Index] == rules.MoveTime,
        GameRuleType.ShapeFading => _shapeFadings[Index] == rules.ShapeFading,
        GameRuleType.BotFadingMoveCount => _botFadingMoveCounts[Index] == rules.BotFadingMoveCount,
        GameRuleType.PlayerFadingMoveCount => _playerFadingMoveCounts[Index] == rules.PlayerFadingMoveCount,
        _ => false
      };
    }

    private int GetIndex(GameRulesData rules)
    {
      return RuleType switch
      {
        GameRuleType.DesiredShape => _shapes.IndexOf(rules.DesiredShape),
        GameRuleType.BotMoveCount => _botMoveCounts.IndexOf(rules.BotMoveCount),
        GameRuleType.MoveTime => _times.IndexOf(rules.MoveTime),
        GameRuleType.ShapeFading => _shapeFadings.IndexOf(rules.ShapeFading),
        GameRuleType.BotFadingMoveCount => _botFadingMoveCounts.IndexOf(rules.BotFadingMoveCount),
        GameRuleType.PlayerFadingMoveCount => _playerFadingMoveCounts.IndexOf(rules.PlayerFadingMoveCount),
        _ => throw new ArgumentOutOfRangeException()
      };
    }
  }
}