using System;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
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
        GameRuleType.BotFadingMoveCount when Data.BotFadingMoveCount is TRule rule => rule,
        GameRuleType.PlayerFadingMoveCount when Data.PlayerFadingMoveCount is TRule rule => rule,
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
        case GameRuleType.BotFadingMoveCount when value is int intValue:
          Data.BotFadingMoveCount = intValue;
          break;
        case GameRuleType.PlayerFadingMoveCount when value is int intValue:
          Data.PlayerFadingMoveCount = intValue;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
      
      if(!Data.ShapeFading.IsPlayersOrRandom() && Data.BotMoveCount is >= 0 and < 2)
        Data.BotMoveCount = 2;
    }

    public GameRulesData Join(GameRulesData rules)
    {
      int thisBotCount = Data.ShapeFading == ShapeFadingType.None ? -1 : Data.BotFadingMoveCount;
      int otherBotCount = rules.ShapeFading == ShapeFadingType.None ? -1 : rules.BotFadingMoveCount;
      
      int thisPlayerCount = Data.ShapeFading == ShapeFadingType.None ? -1 : Data.PlayerFadingMoveCount;
      int otherPlayerCount = rules.ShapeFading == ShapeFadingType.None ? -1 : rules.PlayerFadingMoveCount;
      
      return new GameRulesData
      {
        DesiredShape = ShapeType.XO,
        BotMoveCount = Mathf.Max(Data.BotMoveCount, rules.BotMoveCount),
        MoveTime = Mathf.Max(Data.MoveTime, rules.MoveTime),
        ShapeFading = (ShapeFadingType)Mathf.Max((int)Data.ShapeFading, (int)rules.ShapeFading),
        BotFadingMoveCount = Mathf.Max(thisBotCount, otherBotCount),
        PlayerFadingMoveCount = Mathf.Max(thisPlayerCount, otherPlayerCount)
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