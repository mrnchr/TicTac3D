using System;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class RuleButton : MonoBehaviour
  {
    public GameRuleType RuleType;

    [SerializeField]
    [ShowIf(nameof(IsDesiredShape))]
    private ShapeType _shape;
    
    [SerializeField]
    [ShowIf(nameof(IsBotMoveCount))]
    private int _botMoveCount;
    
    [SerializeField]
    [ShowIf(nameof(IsMoveTime))]
    private float _time;
    
    [SerializeField]
    [ShowIf(nameof(IsShapeFading))]
    private ShapeFadingType _shapeFading;
    
    [SerializeField]
    [ShowIf(nameof(IsBotFadingMoveCount))]
    private int _botFadingMoveCount;
    
    [SerializeField]
    [ShowIf(nameof(IsPlayerFadingMoveCount))]
    private int _playerFadingMoveCount;

    private bool IsDesiredShape => RuleType == GameRuleType.DesiredShape;
    private bool IsBotMoveCount => RuleType == GameRuleType.BotMoveCount;
    private bool IsMoveTime => RuleType == GameRuleType.MoveTime;
    private bool IsShapeFading => RuleType == GameRuleType.ShapeFading;
    private bool IsBotFadingMoveCount => RuleType == GameRuleType.BotFadingMoveCount;
    private bool IsPlayerFadingMoveCount => RuleType == GameRuleType.PlayerFadingMoveCount;
    
    private LobbySettingsWindow _lobbySettingsWindow;
    private Image _image;
    private Button _button;

    [Inject]
    public void Construct()
    {
      _lobbySettingsWindow = GetComponentInParent<LobbySettingsWindow>(true);

      _image = GetComponent<Image>();
      _button = GetComponent<Button>();
      _button.AddListener(OnButtonClick);
    }

    public bool IsEqualRule(GameRulesData rules)
    {
      return RuleType switch
      {
        GameRuleType.DesiredShape => _shape == rules.DesiredShape,
        GameRuleType.BotMoveCount => _botMoveCount == rules.BotMoveCount,
        GameRuleType.MoveTime => _time == rules.MoveTime,
        GameRuleType.ShapeFading => _shapeFading == rules.ShapeFading,
        GameRuleType.BotFadingMoveCount => _botFadingMoveCount == rules.BotFadingMoveCount,
        GameRuleType.PlayerFadingMoveCount => _playerFadingMoveCount == rules.PlayerFadingMoveCount,
        _ => false
      };
    }

    public void OnUpdateRule(GameRulesData rules)
    {
      if (!rules.ShapeFading.IsPlayersOrRandom() && RuleType == GameRuleType.BotMoveCount && _botMoveCount is >= 0 and < 2)
        _button.interactable = false;
      else if (!_button.interactable)
        _button.interactable = true;
      
      _image.sprite = IsEqualRule(rules) ? _lobbySettingsWindow.ActiveButtonSprite : _lobbySettingsWindow.InactiveButtonSprite;
    }
    
    private void OnButtonClick()
    {
      switch (RuleType)
      {
        case GameRuleType.DesiredShape:
          SetRule(_shape);
          break;
        case GameRuleType.BotMoveCount:
          SetRule(_botMoveCount);
          break;
        case GameRuleType.MoveTime:
          SetRule(_time);
          break;
        case GameRuleType.ShapeFading:
          SetRule(_shapeFading);
          break;
        case GameRuleType.BotFadingMoveCount:
          SetRule(_botFadingMoveCount);
          break;
        case GameRuleType.PlayerFadingMoveCount:
          SetRule(_playerFadingMoveCount);
          break;
        default:
          throw new ArgumentException($"Invalid rule type {RuleType}");
      }
    }

    private void SetRule<T>(T value)
    {
      _lobbySettingsWindow.SetRule(this, value);
    }

    private void OnDestroy()
    {
      _button.RemoveListener(OnButtonClick);
    }
  }
}