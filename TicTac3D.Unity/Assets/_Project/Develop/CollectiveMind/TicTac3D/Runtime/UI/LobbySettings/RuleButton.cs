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
    [ShowIf(nameof(IsFadingMoveCount))]
    private int _fadingMoveCount;

    private bool IsDesiredShape => RuleType == GameRuleType.DesiredShape;
    private bool IsBotMoveCount => RuleType == GameRuleType.BotMoveCount;
    private bool IsMoveTime => RuleType == GameRuleType.MoveTime;
    private bool IsShapeFading => RuleType == GameRuleType.ShapeFading;
    private bool IsFadingMoveCount => RuleType == GameRuleType.FadingMoveCount;
    
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

    public void SetSprite(Sprite sprite)
    {
      _image.sprite = sprite;
    }

    public bool IsEqualRule(GameRules rules)
    {
      return RuleType switch
      {
        GameRuleType.DesiredShape => _shape == rules.GetRule<ShapeType>(RuleType),
        GameRuleType.BotMoveCount => _botMoveCount == rules.GetRule<int>(RuleType),
        GameRuleType.MoveTime => _time == rules.GetRule<float>(RuleType),
        GameRuleType.ShapeFading => _shapeFading == rules.GetRule<ShapeFadingType>(RuleType),
        GameRuleType.FadingMoveCount => _fadingMoveCount == rules.GetRule<int>(RuleType),
        _ => false
      };
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
        case GameRuleType.FadingMoveCount:
          SetRule(_fadingMoveCount);
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