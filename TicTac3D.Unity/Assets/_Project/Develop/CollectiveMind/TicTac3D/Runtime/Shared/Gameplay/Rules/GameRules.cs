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
      return Data.Match(rules.Data);
    }

    public bool Match(GameRulesData rules)
    {
      return Data.Match(rules);
    }
  }
}