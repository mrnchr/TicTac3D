using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules
{
  [Serializable]
  public class GameRules
  {
    [InlineProperty]
    [HideLabel]
    public GameRulesData Data;

    public TRule GetRule<TRule>(GameRuleType type)
    {
      return type switch
      {
        GameRuleType.DesiredShape when Data.DesiredShape is TRule rule => rule,
        GameRuleType.BotMoveCount when Data.BotMoveCount is TRule rule => rule,
        GameRuleType.MoveTime when Data.MoveTime is TRule rule => rule,
        GameRuleType.ShapeFading when Data.ShapeFading is TRule rule => rule,
        GameRuleType.FadingMoveCount when Data.FadingMoveCount is TRule rule => rule,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };
    }
    
    public void SetRule<TRule>(GameRuleType type, TRule value)
    {
      switch (type)
      {
        case GameRuleType.DesiredShape when value is ShapeType shapeValue:
          Data.DesiredShape = shapeValue;
          break;
        case GameRuleType.BotMoveCount when value is int intValue:
          Data.BotMoveCount = intValue;
          break;
        case GameRuleType.MoveTime when value is float floatValue:
          Data.MoveTime = floatValue;
          break;
        case GameRuleType.ShapeFading when value is ShapeFadingType fadingType:
          Data.ShapeFading = fadingType;
          break;
        case GameRuleType.FadingMoveCount when value is int intValue:
          Data.FadingMoveCount = intValue;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
    }

    public GameRulesData Join(GameRulesData rules)
    {
      int thisCount = Data.ShapeFading == ShapeFadingType.None ? -1 : Data.FadingMoveCount;
      int otherCount = rules.ShapeFading == ShapeFadingType.None ? -1 : rules.FadingMoveCount;
      return new GameRulesData
      {
        DesiredShape = ShapeType.XO,
        BotMoveCount = Mathf.Max(Data.BotMoveCount, rules.BotMoveCount),
        MoveTime = Mathf.Max(Data.MoveTime, rules.MoveTime),
        ShapeFading = (ShapeFadingType)Mathf.Max((int)Data.ShapeFading, (int)rules.ShapeFading),
        FadingMoveCount = Mathf.Max(thisCount, otherCount)
      };
    }

    public bool Match(GameRules rules)
    {
      return Match(rules.Data);
    }

    public bool Match(GameRulesData rules)
    {
      return MatchDesiredShape(rules.DesiredShape)
        && MatchMoveCount(rules.BotMoveCount)
        && MatchMoveTime(rules.MoveTime)
        && MatchShapeFading(rules.ShapeFading)
        && MatchFadingMoveCount(Data.FadingMoveCount, rules.FadingMoveCount, Data.ShapeFading, rules.ShapeFading);
    }

    private bool MatchDesiredShape(ShapeType shape)
    {
      return (Data.DesiredShape, shape) switch
      {
        (ShapeType.XO, not ShapeType.None) => true,
        (not ShapeType.None, ShapeType.XO) => true,
        (ShapeType.X, ShapeType.O) => true,
        (ShapeType.O, ShapeType.X) => true,
        _ => false
      };
    }

    private bool MatchMoveCount(int count)
    {
      return count < 0 || Data.BotMoveCount < 0 || count == Data.BotMoveCount;
    }

    private bool MatchMoveTime(float time)
    {
      return time < 0 || Data.MoveTime < 0 || time == Data.MoveTime;
    }

    private bool MatchShapeFading(ShapeFadingType fading)
    {
      return Data.ShapeFading == ShapeFadingType.None || fading == ShapeFadingType.None || fading == Data.ShapeFading;
    }

    private bool MatchFadingMoveCount(int left, int right, ShapeFadingType leftFading, ShapeFadingType rightFading)
    {
      int l = leftFading == ShapeFadingType.None ? -1 : left;
      int r = rightFading == ShapeFadingType.None ? -1 : right;
      return l < 0 || r < 0 || l == r;
    }
  }
}